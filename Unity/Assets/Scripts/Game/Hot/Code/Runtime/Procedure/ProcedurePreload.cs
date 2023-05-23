using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public sealed class ProcedurePreload : ProcedureBase
    {
        protected override void OnEnter(IFsm<ProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            PreloadAsync(procedureOwner).Forget();
        }

        private async UniTaskVoid PreloadAsync(IFsm<ProcedureManager> procedureOwner)
        {
            await Tables.Instance.LoadAllAsync();
            Log.Info("Game.Hot.Code Load Config!");
            
            ChangeState<ProcedureGame>(procedureOwner);
        }
    }
}
