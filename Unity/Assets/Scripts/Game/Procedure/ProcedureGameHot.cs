using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedureGameHot : ProcedureBase
    {
#if UNITY_GAMEHOT
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.CodeRunner.StartRun("Game.Hot.Init");
            Log.Info("Start run Game.Hot!");
        }
        
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.CodeRunner.Shutdown();
            base.OnLeave(procedureOwner, isShutdown);
        }
#endif
    }
}
