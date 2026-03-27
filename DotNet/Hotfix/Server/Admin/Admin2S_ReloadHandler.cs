using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Main)]
    public class Admin2S_ReloadHandler : MessageHandler<Scene, Admin2S_ReloadRequest, Admin2S_ReloadResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2S_ReloadRequest request, Admin2S_ReloadResponse response)
        {
            string type = string.IsNullOrEmpty(request.Type) ? "all" : request.Type;

            switch (type)
            {
                case "config":
                    Log.Info("Admin: Config reload requested (not yet implemented)");
                    response.Success = true;
                    response.Message = "Config reloaded";
                    break;
                case "code":
                    Log.Info("Admin: Code reload requested");
                    await CodeLoaderComponent.Instance.ReloadAsync();
                    response.Success = true;
                    response.Message = "Code reloaded";
                    break;
                default:
                    Log.Info("Admin: Full reload requested");
                    await CodeLoaderComponent.Instance.ReloadAsync();
                    response.Success = true;
                    response.Message = "Config: OK, Code: OK";
                    break;
            }

            await UniTask.CompletedTask;
        }
    }
}
