using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Main)]
    public class Admin2S_KickPlayerHandler : MessageHandler<Scene, Admin2S_KickPlayerRequest, Admin2S_KickPlayerResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2S_KickPlayerRequest request, Admin2S_KickPlayerResponse response)
        {
            // TODO: Implement actual player disconnect via ET Actor message
            Log.Info($"Admin kick request: PlayerId={request.PlayerId}, Reason={request.Reason}");
            response.Message = "OK";
            await UniTask.CompletedTask;
        }
    }
}
