using System;
using System.Reflection;
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

            GameEntry.ETRunner.StartRun();

            Log.Debug("Start run ET!");
            
            GameEntry.UI.CloseUIForm(1, null);
        }
    }
}