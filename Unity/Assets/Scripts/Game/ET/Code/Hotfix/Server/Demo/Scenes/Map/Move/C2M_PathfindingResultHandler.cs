using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Map)]
    public class C2M_PathfindingResultHandler : AMActorLocationHandler<Unit, C2M_PathfindingResult>
    {
        protected override async UniTask Run(Unit unit, C2M_PathfindingResult message)
        {
            unit.FindPathMoveToAsync(message.Position).Forget();
            await UniTask.CompletedTask;
        }
    }
}