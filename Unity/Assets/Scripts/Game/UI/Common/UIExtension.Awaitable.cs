using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class UIExtension
    {
        public static async UniTask<UIForm> OpenUIFormAsync(this UIComponent uiComponent, int uiFormTypeId, object userData = null,
            CancellationToken cancellationToken = default, Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null)
        {
            DRUIForm drUIForm = GameEntry.Tables.DTUIForm.GetOrDefault(uiFormTypeId);
            if (drUIForm == null)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", uiFormTypeId.ToString());
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

            return await uiComponent.OpenUIFormAsync(assetName, drUIForm.UIGroupName, Constant.AssetPriority.UIFormAsset, 
                drUIForm.PauseCoveredUIForm, userData, cancellationToken, updateEvent, dependencyAssetEvent);
        }
    }
}
