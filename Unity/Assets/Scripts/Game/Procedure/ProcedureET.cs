#if UNITY_ET
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedureET : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.ET.StartRun();
            Log.Debug("Start run ET!");
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            if (isShutdown)
            {
                GameEntry.ET.ShutDown();
            }
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}
#endif