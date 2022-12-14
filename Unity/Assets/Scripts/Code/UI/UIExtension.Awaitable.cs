using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class UIExtension
    {
        [ItemCanBeNull]
        public static async Task<UIForm> OpenUIFormAsync(this UIComponent uiComponent, int uiFormId, object userData = null)
        {
            DRUIForm drUIForm = GameEntry.Tables.DTUIForm.GetOrDefault(uiFormId);
            if (drUIForm == null)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
                return null;
            }

            string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
            if (!drUIForm.AllowMultiInstance)
            {
                if (uiComponent.IsLoadingUIForm(assetName))
                {
                    return null;
                }

                if (uiComponent.HasUIForm(assetName))
                {
                    return null;
                }
            }

            return await uiComponent.OpenUIFormAsync(assetName, drUIForm.UIGroupName, Constant.AssetPriority.UIFormAsset, drUIForm.PauseCoveredUIForm, userData);
        }
    }
}
