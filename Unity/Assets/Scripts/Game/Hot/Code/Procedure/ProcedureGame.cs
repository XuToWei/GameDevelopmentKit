using GameFramework.Fsm;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public sealed class ProcedureGame : ProcedureBase
    {
        protected override void OnEnter(IFsm<ProcedureComponent> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Debug("开始 GameHot！");
        }

        protected override void OnUpdate(IFsm<ProcedureComponent> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            procedureOwner.SetData<VarInt32>("NextSceneId", HotEntry.Tables.DTOneConfig.SceneMenu);
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }
    }
}
