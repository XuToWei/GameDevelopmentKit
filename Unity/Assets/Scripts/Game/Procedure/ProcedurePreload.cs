using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
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
            Log.Info("Start load Game Tables!");
            await GameEntry.Tables.LoadAllAsync();
            Log.Info("Finish load Game Tables!");
            
            Log.Info("Start load Localization!");
            await GameEntry.Localization.ReadDataAsync(AssetUtility.GetLocalizationAsset(GameEntry.Localization.Language),
                Constant.AssetPriority.DictionaryAsset);
            Log.Info("Finish load Localization!");

#if UNITY_HOTFIX && ENABLE_IL2CPP
            await HybridCLRHelper.LoadAsync();
#endif
            ChangeState<ProcedurePreset>(procedureOwner);
        }
    }
}
