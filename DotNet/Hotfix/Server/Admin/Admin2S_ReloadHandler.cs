using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Main)]
    public class Admin2S_ReloadHandler : MessageHandler<Scene, Admin2S_ReloadRequest, Admin2S_ReloadResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2S_ReloadRequest request, Admin2S_ReloadResponse response)
        {
            string type = string.IsNullOrWhiteSpace(request.Type)
                    ? "all"
                    : request.Type.Trim().ToLowerInvariant();

            try
            {
                switch (type)
                {
                    case "config":
                        Log.Info("Admin: Config reload requested");
                        await ConfigComponent.Instance.ReloadAllAsync();
                        response.Message = "Config reloaded";
                        break;
                    case "code":
                        Log.Info("Admin: Code reload requested");
                        await CodeLoaderComponent.Instance.ReloadAsync();
                        response.Message = "Code reloaded";
                        break;
                    case "all":
                        Log.Info("Admin: Full reload requested");
                        await ConfigComponent.Instance.ReloadAllAsync();
                        await CodeLoaderComponent.Instance.ReloadAsync();
                        response.Message = "Config and code reloaded";
                        break;
                    default:
                        response.Error = 1;
                        response.Message = $"Unsupported reload type: {request.Type}";
                        return;
                }

                response.Success = true;
            }
            catch (Exception e)
            {
                response.Error = 1;
                response.Success = false;
                response.Message = $"Reload ({type}) failed: {e.Message}";
                Log.Error($"Admin reload ({type}) failed: {e}");
            }
        }
    }
}
