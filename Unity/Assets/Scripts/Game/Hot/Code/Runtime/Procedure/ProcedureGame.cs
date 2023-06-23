using GameFramework.Fsm;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public sealed class ProcedureGame : ProcedureBase
    {
        protected override void OnEnter(IFsm<ProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Debug("开始 GameHot！");
        }

        protected override void OnUpdate(IFsm<ProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            procedureOwner.SetData<VarInt32>("NextSceneId", Tables.Instance.DTOneConfig.SceneMenu);
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }
    }
}
