using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [FriendOf(typeof(AgentProcessComponent))]
    [MessageHandler(SceneType.Agent)]
    public class Admin2Agent_StopProcessHandler : MessageHandler<Scene, Admin2Agent_StopProcessRequest, Admin2Agent_StopProcessResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2Agent_StopProcessRequest request, Admin2Agent_StopProcessResponse response)
        {
            var agentProcess = scene.GetComponent<AgentProcessComponent>();
            if (agentProcess == null)
            {
                response.Error = 1;
                response.Message = "AgentProcessComponent not initialized";
                return;
            }

            if (!agentProcess.Processes.TryGetValue(request.ProcessId, out var process))
            {
                response.Error = 1;
                response.Message = $"Process {request.ProcessId} not managed by this Agent";
                return;
            }

            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                    if (!process.WaitForExit(5000))
                    {
                        response.Error = 1;
                        response.Message = $"Timed out stopping process {request.ProcessId}";
                        return;
                    }
                }
                agentProcess.Processes.Remove(request.ProcessId);
                process.Dispose();
                Log.Info($"Agent stopped process {request.ProcessId}");
            }
            catch (System.Exception ex)
            {
                response.Error = 1;
                response.Message = $"Failed to stop process {request.ProcessId}: {ex.Message}";
            }

            await UniTask.CompletedTask;
        }
    }
}
