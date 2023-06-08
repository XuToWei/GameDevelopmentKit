using Cysharp.Threading.Tasks;

namespace ET.Server
{
    // 离开视野
    [Event(SceneType.Map)]
    public class UnitLeaveSightRange_NotifyClient: AEvent<Scene, EventType.UnitLeaveSightRange>
    {
        protected override async UniTask Run(Scene scene, EventType.UnitLeaveSightRange args)
        {
            await UniTask.CompletedTask;
            AOIEntity a = args.A;
            AOIEntity b = args.B;
            if (a.Unit.Type != UnitType.Player)
            {
                return;
            }

            MessageHelper.NoticeUnitRemove(a.GetParent<Unit>(), b.GetParent<Unit>());
        }
    }
}