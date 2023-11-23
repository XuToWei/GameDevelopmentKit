using Cysharp.Threading.Tasks;
using UnityGameFramework.Extension;
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
            Log.Info("Start load Resource List!");
            await GameEntry.ResourceList.LoadAsync(AssetUtility.GetConfigAsset("ResourceList.bytes"));
            Log.Info("Finish load Resource List!");
            
            Log.Info("Start load Game Tables!");
            await GameEntry.Tables.LoadAllAsync();
            Log.Info("Finish load Game Tables!");
            
            Log.Info("Start load Localization!");
            await GameEntry.Localization.LoadLanguageAsync(GameEntry.Localization.Language);
            Log.Info("Finish load Localization!");

#if UNITY_HOTFIX && ENABLE_IL2CPP
            await HybridCLRHelper.LoadAsync();
#endif
            ChangeState<ProcedurePreset>(procedureOwner);
        }
    }
}
