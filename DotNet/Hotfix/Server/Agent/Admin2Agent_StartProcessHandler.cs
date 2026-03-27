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

            if (agentProcess.Processes.ContainsKey(request.ProcessId))
            {
                response.Error = 1;
                response.Message = $"Process {request.ProcessId} already running";
                return;
            }

            try
            {
                var process = WatcherHelper.StartProcess(request.ProcessId);
                if (process != null)
                {
                    agentProcess.Processes.Add(request.ProcessId, process);
                }
                else
                {
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
