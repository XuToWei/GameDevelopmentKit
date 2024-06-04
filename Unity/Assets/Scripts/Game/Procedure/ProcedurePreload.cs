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
            await GameEntry.Tables.LoadAllAsync();
            Log.Info("Finish load Game Tables!");
            
            Log.Info("Start load Localization!");
            await GameEntry.Localization.LoadLanguageAsync(GameEntry.Localization.Language);
            Log.Info("Finish load Localization!");

            Log.Info("Start init UXTool!");
            await UXTool.InitAsync();
            Log.Info("Finish init UXTool!");
#if UNITY_HOTFIX && ENABLE_IL2CPP
            await HybridCLRHelper.LoadAsync();
#endif
#if UNITY_EDITOR
            Check();
#endif
            ChangeState<ProcedurePreset>(procedureOwner);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            UXTool.Clear();
            base.OnDestroy(procedureOwner);
        }

#if UNITY_EDITOR
        private void Check()
        {
            foreach (var drUIForm in GameEntry.Tables.DTUIForm.DataList)
            {
                GameFramework.UI.IUIGroup uiGroup = GameEntry.UI.GetUIGroup(drUIForm.UIGroupName);
                if (uiGroup == null)
                {
                    Log.Error(GameFramework.Utility.Text.Format("DRUIForm '{0}' - ui group '{1}' is not exist.", drUIForm.AssetName, drUIForm.UIGroupName));
                }
            }
            foreach (var drEntity in GameEntry.Tables.DTEntity.DataList)
            {
                GameFramework.Entity.IEntityGroup entityGroup = GameEntry.Entity.GetEntityGroup(drEntity.EntityGroupName);
                if (entityGroup == null)
                {
                    Log.Error(GameFramework.Utility.Text.Format("DREntity '{0}' - entity group '{1}' is not exist.", drEntity.AssetName, drEntity.EntityGroupName));
                }
            }
        }
#endif
    }
}
