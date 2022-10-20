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
            GameObject languageSourceObj = await GameEntry.Resource.LoadAssetAsync<GameObject>(AssetUtility.GetLocalizationAsset());
            GameObject.Instantiate(languageSourceObj).name = "LocalizationSource";
        }
    }
}
