using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class Admin2S_KickPlayerGateHandler : MessageHandler<Scene, Admin2S_KickPlayerRequest, Admin2S_KickPlayerResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2S_KickPlayerRequest request, Admin2S_KickPlayerResponse response)
        {
            var playerComponent = scene.GetComponent<PlayerComponent>();
            var player = playerComponent.GetChild<Player>(request.PlayerId);
            if (player == null)
            {
                response.Error = 1;
                response.Message = $"Player {request.PlayerId} is not online on Gate {scene.Name}";
                return;
            }

            var playerSessionComponent = player.GetComponent<PlayerSessionComponent>();
            if (playerSessionComponent == null)
            {
                response.Error = 1;
                response.Message = $"Player {request.PlayerId} has no session on Gate {scene.Name}";
                return;
            }

            var session = playerSessionComponent.Session;

            if (session == null || session.IsDisposed)
            {
                response.Error = 1;
                response.Message = $"Player {request.PlayerId} is not online on Gate {scene.Name}";
                return;
            }

            session.Dispose();
            response.Message = $"Player {request.PlayerId} disconnected";
            await UniTask.CompletedTask;
        }
    }
}
