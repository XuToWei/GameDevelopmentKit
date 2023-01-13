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

            UnityEngine.GameObject.Find("ET").SetActive(true);
            
            Log.Debug("ET load successfully!");
            //ChangeState<ProcedureXXX>(procedureOwner);
        }
    }
}