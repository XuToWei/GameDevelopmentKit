using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class UIExtension
    {
        public static UniTask<UIForm> OpenUIFormAsync(this UIComponent uiComponent, int uiFormTypeId, object userData = null,
            CancellationToken cancellationToken = default, Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null)
        {
            DRUIForm drUIForm = GameEntry.Tables.DTUIForm.GetOrDefault(uiFormTypeId);
            if (drUIForm == null)
            {
                string error = Utility.Text.Format("Can not load UI form '{0}' from data table.", uiFormTypeId.ToString());
                return UniTask.FromException<UIForm>(new GameFrameworkException(error));
            }

            string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
            if (!drUIForm.AllowMultiInstance)
            {
                if (uiComponent.IsLoadingUIForm(assetName))
                {
                    string error = Utility.Text.Format("UI form '{0}' is loading.", assetName);
                    return UniTask.FromException<UIForm>(new GameFrameworkException(error));
                }
                if (uiComponent.HasUIForm(assetName))
                {
                    string error = Utility.Text.Format("UI form '{0}' is already open.", assetName);
                    return UniTask.FromException<UIForm>(new GameFrameworkException(error));
                }
            }

            return uiComponent.OpenUIFormAsync(assetName, drUIForm.UIGroupName, Constant.AssetPriority.UIFormAsset, 
                drUIForm.PauseCoveredUIForm, userData, cancellationToken, updateEvent, dependencyAssetEvent);
        }
    }
}
