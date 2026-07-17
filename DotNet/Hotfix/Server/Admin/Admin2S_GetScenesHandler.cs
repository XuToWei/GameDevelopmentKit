using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Main)]
    public class Admin2S_GetScenesHandler : MessageHandler<Scene, Admin2S_GetScenesRequest, Admin2S_GetScenesResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2S_GetScenesRequest request, Admin2S_GetScenesResponse response)
        {
            int process = Options.Instance.Process;
            var processScenes = Tables.Instance.DTStartSceneConfig.GetByProcess(process);

            foreach (var startConfig in processScenes)
            {
                var proto = SceneInfoProto.Create();
                proto.Id = startConfig.Id;
                proto.SceneType = startConfig.Type.ToString();
                proto.Name = startConfig.Name;
                proto.Zone = startConfig.Zone;
                proto.InnerAddress = startConfig.InnerIPPort.ToString();
                proto.OuterAddress = startConfig.OuterIPPort.ToString();
                proto.PlayerCount = 0;
                proto.FiberId = startConfig.Id;
                proto.ProcessId = process;
                response.Scenes.Add(proto);
            }

            await UniTask.CompletedTask;
        }
    }
}
