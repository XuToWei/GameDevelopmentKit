using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageLocationHandler(SceneType.Map)]
    public class C2M_StopHandler: MessageLocationHandler<Unit, C2M_Stop>
    {
        protected override async UniTask Run(Unit unit, C2M_Stop message)
        {
            unit.Stop(1);
            await UniTask.CompletedTask;
        }
    }
}