

namespace ET
{
    public class ServerMonitorService
    {
        private readonly ProcessManagerService _processManager;
        private readonly AdminActorService _actorService;
        private readonly ILogger<ServerMonitorService> _logger;

        public ServerMonitorService(
            ProcessManagerService processManager,
            AdminActorService actorService,
            ILogger<ServerMonitorService> logger)
        {
            _processManager = processManager;
            _actorService = actorService;
            _logger = logger;
        }

        public List<ServerInfo> GetServerStatus()
        {
            return _processManager.GetAllServers();
        }

        public ServerInfo GetServerDetails(int processId)
        {
            var servers = _processManager.GetAllServers();
            return servers.FirstOrDefault(s => s.ProcessId == processId);
        }

        public async Task<List<FiberInfo>> GetFibersAsync(int processId)
        {
            return await _actorService.GetFibersAsync(processId);
        }

        public async Task<List<SceneInfo>> GetScenesAsync(int processId)
        {
            return await _actorService.GetScenesAsync(processId);
        }

        public async Task<List<SceneInfo>> GetAllScenesAsync()
        {
            var servers = _processManager.GetAllServers();
            var runningServers = servers.Where(s => s.Status == ServerStatus.Running).ToList();

            var tasks = runningServers.Select(s => GetScenesAsync(s.ProcessId));
            var results = await Task.WhenAll(tasks);

            var allScenes = new List<SceneInfo>();
            foreach (var scenes in results)
            {
                allScenes.AddRange(scenes);
            }
            return allScenes;
        }

        public int GetTotalOnlinePlayers()
        {
            var servers = _processManager.GetAllServers();
            return servers.Sum(s => s.PlayerCount);
        }
    }
}
