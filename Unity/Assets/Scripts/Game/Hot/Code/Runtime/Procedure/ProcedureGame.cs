using GameFramework.Fsm;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public class ProcedureGame : ProcedureBase
    {
        protected override void OnEnter(IFsm<ProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            Log.Debug("开始 GameHot！");
        }
    }
}
