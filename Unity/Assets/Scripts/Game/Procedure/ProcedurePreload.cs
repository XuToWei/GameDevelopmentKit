using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedurePreload : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            PreloadAsync(procedureOwner).Forget();
        }

        private async UniTaskVoid PreloadAsync(ProcedureOwner procedureOwner)
        {
            Log.Info("Start load Game Tables!");
            await GameEntry.Tables.LoadAsync();
            Log.Info("Finish load Game Tables!");

#if UNITY_HOTFIX && ENABLE_IL2CPP
            await HybridCLRHelper.LoadAsync();
#endif

#if UNITY_ET
            ChangeState<ProcedureStartET>(procedureOwner);
#elif UNITY_GAMEHOT
            ChangeState<ProcedureStartHot>(procedureOwner);
#else
            ChangeState<ProcedureStartGame>(procedureOwner);
#endif
        }
    }
}
