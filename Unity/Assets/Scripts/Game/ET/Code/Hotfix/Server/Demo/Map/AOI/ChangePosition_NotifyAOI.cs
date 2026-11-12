using Cysharp.Threading.Tasks;
using TrueSync;

namespace ET.Server
{
    [Event(SceneType.Map)]
    public class ChangePosition_NotifyAOI: AEvent<Scene, ChangePosition>
    {
        protected override async UniTask Run(Scene scene, ChangePosition args)
        {
            Unit unit = args.Unit;
            TSVector oldPos = args.OldPos;
            int oldCellX = AOIHelper.GetCellCoordinate(oldPos.x);
            int oldCellY = AOIHelper.GetCellCoordinate(oldPos.z);
            int newCellX = AOIHelper.GetCellCoordinate(unit.Position.x);
            int newCellY = AOIHelper.GetCellCoordinate(unit.Position.z);
            if (oldCellX == newCellX && oldCellY == newCellY)
            {
                return;
            }

            AOIEntity aoiEntity = unit.GetComponent<AOIEntity>();
            if (aoiEntity == null)
            {
                return;
            }

            unit.Scene().GetComponent<AOIManagerComponent>().Move(aoiEntity, newCellX, newCellY);
            await UniTask.CompletedTask;
        }
    }
}
