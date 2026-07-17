using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Main)]
    public class Admin2S_GetFibersHandler : MessageHandler<Scene, Admin2S_GetFibersRequest, Admin2S_GetFibersResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2S_GetFibersRequest request, Admin2S_GetFibersResponse response)
        {
            int process = Options.Instance.Process;
            var processScenes = Tables.Instance.DTStartSceneConfig.GetByProcess(process);

            // Return configured scenes as fibers (each configured scene corresponds to a fiber)
            foreach (var startConfig in processScenes)
            {
                var proto = FiberInfoProto.Create();
                proto.Id = startConfig.Id;
                proto.Zone = startConfig.Zone;
                proto.SceneType = startConfig.Type.ToString();
                proto.Name = startConfig.Name;
                proto.SchedulerType = "ThreadPool";
                proto.EntityCount = 0;
                proto.ProcessId = process;
                response.Fibers.Add(proto);
            }

            await UniTask.CompletedTask;
        }
    }
}
