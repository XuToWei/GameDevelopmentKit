using Cysharp.Threading.Tasks;
using UnityEngine.UI;
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
            await GameEntry.Tables.LoadAllAsync();
            Log.Info("Finish load Game Tables!");
            
            void PlayButtonClickSound()
            {
                GameEntry.Sound.PlayUISound(1000);
            }
            //按钮添加点击音效
            ExButton.AllButtonOnPointerDownEvent += PlayButtonClickSound;
            
#if UNITY_HOTFIX && ENABLE_IL2CPP
            await HybridCLRHelper.LoadAsync();
#endif

#if UNITY_ET
            ChangeState<ProcedureET>(procedureOwner);
#elif UNITY_GAMEHOT
            ChangeState<ProcedureHot>(procedureOwner);
#else
            ChangeState<ProcedureGame>(procedureOwner);
#endif
        }
    }
}
