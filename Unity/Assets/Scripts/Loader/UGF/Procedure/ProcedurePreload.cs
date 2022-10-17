using GameFramework.Fsm;
using GameFramework.Procedure;

namespace UGF
{
    public class ProcedurePreload : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            UnityGameFramework.Runtime.Log.Debug("Enter preload procedure!");
            
            ChangeState<ProcedurePreloadET>(procedureOwner);
        }
    }
}
