namespace ET
{
    /// <summary>
    /// Bridges ASP.NET Core services with ET Actor messages for calling Agent processes.
    /// </summary>
    public class AgentActorService : ActorBridgeService
    {
        public AgentActorService(ILogger<AgentActorService> logger) : base(logger)
        {
        }

        /// <summary>
        /// Get the Agent's fiber ID from Luban config for the given agent process.
        /// </summary>
        private static int GetAgentFiberId(int agentProcessId)
        {
            var agents = Tables.Instance.DTStartSceneConfig.Agents;
            var agent = agents.FirstOrDefault(a => a.Process == agentProcessId);
            return agent?.Id ?? 0;
        }

        public async Task<bool> StartProcessAsync(int agentProcessId, int targetProcessId)
        {
            var request = Admin2Agent_StartProcessRequest.Create();
            request.ProcessId = targetProcessId;

            var fiberId = GetAgentFiberId(agentProcessId);
            var response = await CallAsync<Admin2Agent_StartProcessRequest, Admin2Agent_StartProcessResponse>(agentProcessId, fiberId, request);
            return response is { Error: 0 };
        }

        public async Task<bool> StopProcessAsync(int agentProcessId, int targetProcessId)
        {
            var request = Admin2Agent_StopProcessRequest.Create();
            request.ProcessId = targetProcessId;

            var fiberId = GetAgentFiberId(agentProcessId);
            var response = await CallAsync<Admin2Agent_StopProcessRequest, Admin2Agent_StopProcessResponse>(agentProcessId, fiberId, request);
            return response is { Error: 0 };
        }

        private const int ChunkSize = 1024 * 1024; // 1 MB per chunk

        public async Task<bool> DeployFileAsync(int agentProcessId, string fileName, byte[] data, string targetPath)
        {
            var fiberId = GetAgentFiberId(agentProcessId);

            if (data.Length <= ChunkSize)
            {
                var request = Admin2Agent_DeployFileRequest.Create();
                request.FileName = fileName;
                request.FileData = data;
                request.TargetPath = targetPath;
                var response = await CallAsync<Admin2Agent_DeployFileRequest, Admin2Agent_DeployFileResponse>(agentProcessId, fiberId, request, timeoutSeconds: 30);
                return response is { Error: 0 };
            }

            // Chunked transfer: append to .tmp file, last chunk finalizes
            // Convention: FileName containing ".chunk" → Agent appends to TargetPath
            //             FileName without ".chunk" → Agent appends + renames .tmp → TargetPath
            var tmpPath = $"{targetPath}.tmp";
            var totalChunks = (data.Length + ChunkSize - 1) / ChunkSize;
            for (var i = 0; i < totalChunks; i++)
            {
                var offset = i * ChunkSize;
                var length = Math.Min(ChunkSize, data.Length - offset);
                var chunk = new byte[length];
                Array.Copy(data, offset, chunk, 0, length);

                var isLast = i == totalChunks - 1;
                var request = Admin2Agent_DeployFileRequest.Create();
                request.FileName = isLast ? fileName : $"{fileName}.chunk{i}";
                request.FileData = chunk;
                request.TargetPath = isLast ? targetPath : tmpPath;

                var response = await CallAsync<Admin2Agent_DeployFileRequest, Admin2Agent_DeployFileResponse>(agentProcessId, fiberId, request, timeoutSeconds: 60);
                if (response is not { Error: 0 })
                    return false;
            }

            return true;
        }

        public async Task<bool> HotReloadAsync(int agentProcessId, string type, List<int> processIds)
        {
            var request = Admin2Agent_HotReloadRequest.Create();
            request.Type = type;
            foreach (var pid in processIds)
                request.ProcessIds.Add(pid);

            var fiberId = GetAgentFiberId(agentProcessId);
            var response = await CallAsync<Admin2Agent_HotReloadRequest, Admin2Agent_HotReloadResponse>(agentProcessId, fiberId, request, timeoutSeconds: 30);
            return response is { Error: 0 };
        }

        /// <summary>
        /// Check if an Agent is alive based on ET connection state.
        /// </summary>
        public bool IsAgentAlive(int agentProcessId) => IsProcessAlive(agentProcessId);

        public void MarkAgentAlive(int agentProcessId)
        {
            SetProcessAlive(agentProcessId, true);
        }
    }
}
