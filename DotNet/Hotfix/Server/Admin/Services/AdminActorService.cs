namespace ET
{
    /// <summary>
    /// Bridges ASP.NET Core services with ET Actor messages for querying ET Server processes.
    /// </summary>
    public class AdminActorService : ActorBridgeService
    {
        public event Action<int, ServerStatus> OnServerStatusChanged = delegate { };

        public AdminActorService(ILogger<AdminActorService> logger) : base(logger)
        {
        }

        public async Task<List<FiberInfo>> GetFibersAsync(int processId)
        {
            var response = await CallAsync<Admin2S_GetFibersRequest, Admin2S_GetFibersResponse>(processId, ConstFiberId.Main, Admin2S_GetFibersRequest.Create());
            if (response == null)
                return new List<FiberInfo>();

            return response.Fibers.Select(f => new FiberInfo
            {
                Id = f.Id,
                Zone = f.Zone,
                SceneType = f.SceneType,
                Name = f.Name,
                SchedulerType = f.SchedulerType,
                EntityCount = f.EntityCount,
                ProcessId = f.ProcessId,
            }).ToList();
        }

        public async Task<List<SceneInfo>> GetScenesAsync(int processId)
        {
            var response = await CallAsync<Admin2S_GetScenesRequest, Admin2S_GetScenesResponse>(processId, ConstFiberId.Main, Admin2S_GetScenesRequest.Create());
            if (response == null)
                return new List<SceneInfo>();

            return response.Scenes.Select(s => new SceneInfo
            {
                Id = s.Id,
                SceneType = s.SceneType,
                Name = s.Name,
                Zone = s.Zone,
                InnerAddress = s.InnerAddress,
                OuterAddress = s.OuterAddress,
                PlayerCount = s.PlayerCount,
                FiberId = s.FiberId,
                ProcessId = s.ProcessId,
            }).ToList();
        }

        public async Task<bool> KickPlayerAsync(int processId, long playerId, string reason)
        {
            var request = Admin2S_KickPlayerRequest.Create();
            request.PlayerId = playerId;
            request.Reason = reason;

            var response = await CallAsync<Admin2S_KickPlayerRequest, Admin2S_KickPlayerResponse>(processId, ConstFiberId.Main, request);
            return response is { Error: 0 };
        }

        public async Task<bool> ReloadAsync(int processId, string type)
        {
            var request = Admin2S_ReloadRequest.Create();
            request.Type = type;

            var response = await CallAsync<Admin2S_ReloadRequest, Admin2S_ReloadResponse>(processId, ConstFiberId.Main, request, timeoutSeconds: 30);
            return response is { Error: 0, Success: true };
        }

        /// <summary>
        /// Called by the ET fiber when a S2Admin_ProcessStatusReport is received.
        /// </summary>
        internal void HandleStatusReport(int processId, int status, long memoryUsage, int fiberCount)
        {
            var serverStatus = status switch
            {
                0 => ServerStatus.Running,
                1 => ServerStatus.Stopping,
                2 => ServerStatus.Error,
                _ => ServerStatus.Stopped,
            };

            SetProcessAlive(processId, serverStatus == ServerStatus.Running);
            OnServerStatusChanged(processId, serverStatus);
        }
    }
}
