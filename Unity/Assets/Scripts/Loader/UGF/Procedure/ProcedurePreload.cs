using ET;
using GameFramework.Fsm;
using GameFramework.Procedure;
using I2.Loc;
using UnityEngine;

namespace UGF
{
    public class ProcedurePreload : ProcedureBase
    {
        protected override async void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            UnityGameFramework.Runtime.Log.Debug("Enter preload procedure!");

            await this.LoadLocalization();
            
            ChangeState<ProcedurePreloadET>(procedureOwner);
        }

        private async ETTask LoadLocalization()
        {
            LanguageSourceAsset languageSourceAsset = await GameEntry.Resource.LoadAssetAsync<LanguageSourceAsset>(AssetUtility.GetLocalizationAsset());
            LocalizationManager.UpdateByOneSource(languageSourceAsset);
        }
    }
}
