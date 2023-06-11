using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedureET : ProcedureBase
    {
#if UNITY_ET
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.CodeRunner.StartRun("ET.Init");
            Log.Info("Start run ET!");
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.CodeRunner.Shutdown();
            base.OnLeave(procedureOwner, isShutdown);
        }
#endif
    }
}