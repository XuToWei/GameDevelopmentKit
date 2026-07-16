using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [FriendOf(typeof(AgentProcessComponent))]
    [MessageHandler(SceneType.Agent)]
    public class Admin2Agent_StartProcessHandler : MessageHandler<Scene, Admin2Agent_StartProcessRequest, Admin2Agent_StartProcessResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2Agent_StartProcessRequest request, Admin2Agent_StartProcessResponse response)
        {
            var agentProcess = scene.GetComponent<AgentProcessComponent>();
            if (agentProcess == null)
            {
                response.Error = 1;
                response.Message = "AgentProcessComponent not initialized";
                return;
            }

            if (agentProcess.Processes.TryGetValue(request.ProcessId, out var existingProcess))
            {
                if (!existingProcess.HasExited)
                {
                    response.Error = 1;
                    response.Message = $"Process {request.ProcessId} already running";
                    return;
                }

                agentProcess.Processes.Remove(request.ProcessId);
                existingProcess.Dispose();
            }

            try
            {
                var process = WatcherHelper.StartProcess(request.ProcessId);
                if (process != null && !process.HasExited)
                {
                    agentProcess.Processes.Add(request.ProcessId, process);
                    Log.Info($"Agent started process {request.ProcessId}");
                }
                else
                {
                    if (process != null)
                    {
                        process.Dispose();
                    }

                    response.Error = 1;
                    response.Message = $"Failed to start process {request.ProcessId}";
                }
            }
            catch (System.Exception ex)
            {
                response.Error = 1;
                response.Message = $"Failed to start process {request.ProcessId}: {ex.Message}";
            }

            await UniTask.CompletedTask;
        }
    }
}
