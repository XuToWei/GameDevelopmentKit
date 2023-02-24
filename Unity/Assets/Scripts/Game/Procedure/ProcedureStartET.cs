using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

namespace Game
{
    public class ProcedureStartET : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
#if UNITY_ET
            GameEntry.ET.StartRun();
            Log.Debug("Start run ET!");
#else
            Log.Error("ET is not open!");
#endif
        }
    }
}