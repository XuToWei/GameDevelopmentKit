

namespace ET
{
    public class ProcessManagerService
    {
        private readonly LogService _logService;
        private readonly AdminActorService _adminActorService;
        private readonly AgentActorService _agentActorService;
        private readonly ILogger<ProcessManagerService> _logger;

        public event Action OnProcessStatusChanged;
        public bool MaintenanceMode { get; private set; }

        public ProcessManagerService(LogService logService, AdminActorService adminActorService, AgentActorService agentActorService, ILogger<ProcessManagerService> logger)
        {
            _logService = logService;
            _adminActorService = adminActorService;
            _agentActorService = agentActorService;
            _logger = logger;
        }

        public List<ServerInfo> GetAllServers()
        {
            var startConfig = Options.Instance.StartConfig;
            var servers = new List<ServerInfo>();

            foreach (var processConfig in Tables.Instance.DTStartProcessConfig.DataList)
            {
                if (!string.Equals(processConfig.StartConfig, startConfig))
                    continue;

                var processId = processConfig.Id;

                // Skip Admin and Agent processes
                if (processId == Options.Instance.Process)
                    continue;

                var processScenes = Tables.Instance.DTStartSceneConfig.GetByProcess(processId);
                if (processScenes.Count > 0 && processScenes[0].Type is SceneType.Admin or SceneType.Agent)
                    continue;

                var status = _adminActorService.IsProcessAlive(processId) ? ServerStatus.Running : ServerStatus.Stopped;

                var sceneName = string.Join(", ", processScenes.ConvertAll(s => s.Name));
                var outerPort = processScenes.Count == 1 ? processScenes[0].Port : 0;

                servers.Add(new ServerInfo
                {
                    ProcessId = processId,
                    Name = sceneName,
                    Status = status,
                    InnerIP = processConfig.InnerIP,
                    InnerPort = processConfig.Port,
                    OuterIP = processConfig.OuterIP,
                    OuterPort = outerPort,
                });
            }

            return servers;
        }

        public async Task<bool> StartServerAsync(int processId)
        {
            if (MaintenanceMode)
                return false;

            var agent = Tables.Instance.DTStartSceneConfig.GetAgentForProcess(processId);
            if (agent == null)
            {
                _logger.LogError("No Agent found for process {ProcessId}", processId);
                return false;
            }

            var success = await _agentActorService.StartProcessAsync(agent.Process, processId);
            if (success)
            {
                _logService.AddLog("INFO", $"Process-{processId}", "服务器启动命令已发送");
                OnProcessStatusChanged?.Invoke();
            }
            return success;
        }

        public async Task<bool> StopServerAsync(int processId)
        {
            var agent = Tables.Instance.DTStartSceneConfig.GetAgentForProcess(processId);
            if (agent == null)
            {
                _logger.LogError("No Agent found for process {ProcessId}", processId);
                return false;
            }

            var success = await _agentActorService.StopProcessAsync(agent.Process, processId);
            if (success)
            {
                _logService.AddLog("INFO", $"Process-{processId}", "服务器停止命令已发送");
                OnProcessStatusChanged?.Invoke();
            }
            return success;
        }

        public async Task<bool> RestartServerAsync(int processId)
        {
            await StopServerAsync(processId);
            await Task.Delay(1000);
            return await StartServerAsync(processId);
        }

        public async Task<int> StartAllServersAsync()
        {
            var servers = GetAllServers();
            var tasks = servers.Select(s => StartServerAsync(s.ProcessId));
            var results = await Task.WhenAll(tasks);
            return results.Count(r => r);
        }

        public async Task<int> StopAllServersAsync()
        {
            var servers = GetAllServers().Where(s => s.Status == ServerStatus.Running).ToList();
            var tasks = servers.Select(s => StopServerAsync(s.ProcessId));
            var results = await Task.WhenAll(tasks);
            return results.Count(r => r);
        }

        public void SetMaintenanceMode(bool enabled)
        {
            MaintenanceMode = enabled;
            OnProcessStatusChanged?.Invoke();
        }
    }
}
