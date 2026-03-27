using System.Collections.Concurrent;


namespace ET
{
    public class ServerMonitorBackgroundService : BackgroundService
    {
        private readonly ServerMonitorNotifier _notifier;
        private readonly ServerMonitorService _monitorService;
        private readonly ProcessManagerService _processManager;
        private readonly ILogger<ServerMonitorBackgroundService> _logger;
        private readonly int _pollingIntervalSeconds;

        // Track consecutive failures per process for backoff
        private readonly ConcurrentDictionary<int, int> _failureCounts = new();
        private readonly ConcurrentDictionary<int, byte> _pendingQueries = new();
        private const int MaxBackoffCycles = 6;

        public ServerMonitorBackgroundService(
            ServerMonitorNotifier notifier,
            ServerMonitorService monitorService,
            ProcessManagerService processManager,
            AdminActorService actorService,
            IConfiguration configuration,
            ILogger<ServerMonitorBackgroundService> logger)
        {
            _notifier = notifier;
            _monitorService = monitorService;
            _processManager = processManager;
            _logger = logger;
            _pollingIntervalSeconds = configuration.GetValue("Monitoring:PollingIntervalSeconds", 30);

            // Subscribe to push events from ET server processes
            actorService.OnServerStatusChanged += (processId, status) =>
            {
                _logger.LogDebug("Push: process {ProcessId} status -> {Status}", processId, status);
                var servers = _monitorService.GetServerStatus();
                _notifier.UpdateServers(servers);
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Server monitor polling started (interval: {Interval}s)", _pollingIntervalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PollAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error during server monitoring poll");
                }

                await Task.Delay(TimeSpan.FromSeconds(_pollingIntervalSeconds), stoppingToken);
            }
        }

        private int _pollCycle;
        private bool _initialProbeCompleted;

        private async Task PollAsync(CancellationToken ct)
        {
            _pollCycle++;

            // On first poll, wait for ET runtime to be ready, then probe all configured processes
            if (!_initialProbeCompleted)
            {
                if (ActorRpcBridge.Instance == null)
                {
                    _logger.LogDebug("Waiting for ET runtime to initialize...");
                    return;
                }

                _initialProbeCompleted = true;
                _logger.LogInformation("Initial probe: discovering running servers via ET network...");
                var allServers = _monitorService.GetServerStatus();
                var probeTasks = allServers.Select(s => PollServerAsync(s.ProcessId, ct));
                await Task.WhenAll(probeTasks);
            }

            var servers = _monitorService.GetServerStatus();
            _notifier.UpdateServers(servers);

            var runningServers = servers.Where(s => s.Status == ServerStatus.Running).ToList();
            var stoppedProcessIds = servers
                .Where(s => s.Status != ServerStatus.Running)
                .Select(s => s.ProcessId)
                .ToList();

            foreach (var processId in stoppedProcessIds)
            {
                _notifier.RemoveProcessData(processId);
                _failureCounts.TryRemove(processId, out _);
            }

            var tasks = runningServers
                .Where(server => ShouldPoll(server.ProcessId))
                .Select(server => PollServerAsync(server.ProcessId, ct));
            await Task.WhenAll(tasks);
        }

        private bool ShouldPoll(int processId)
        {
            if (!_failureCounts.TryGetValue(processId, out var failures) || failures == 0)
                return true;

            var skipCycles = Math.Min(failures, MaxBackoffCycles);
            return _pollCycle % (skipCycles + 1) == 0;
        }

        private async Task PollServerAsync(int processId, CancellationToken ct)
        {
            if (!_pendingQueries.TryAdd(processId, 0))
            {
                return; // Skip if a query is already in flight for this process
            }

            try
            {
                var fibersTask = _monitorService.GetFibersAsync(processId);
                var scenesTask = _monitorService.GetScenesAsync(processId);
                await Task.WhenAll(fibersTask, scenesTask);

                var fibers = await fibersTask;
                var scenes = await scenesTask;

                if (fibers.Count == 0 && scenes.Count == 0)
                {
                    _failureCounts.AddOrUpdate(processId, 1, (_, count) => count + 1);
                    return;
                }

                _failureCounts.TryRemove(processId, out _);
                _notifier.UpdateFibers(processId, fibers);
                _notifier.UpdateScenes(processId, scenes);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _failureCounts.AddOrUpdate(processId, 1, (_, count) => count + 1);
                _logger.LogDebug(ex, "Failed to poll process {ProcessId}", processId);
            }
            finally
            {
                _pendingQueries.TryRemove(processId, out _);
            }
        }
    }
}
