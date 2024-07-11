using Cysharp.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class SceneExtension
    {
        public static UniTask LoadSceneAsync(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.Tables.DTScene.GetOrDefault(sceneId);
            if (drScene == null)
            {
                string error = Utility.Text.Format("Can not load Scene '{0}' from data table.", sceneId.ToString());
                return UniTask.FromException(new GameFrameworkException(error));
            }
            string assetName = AssetUtility.GetSceneAsset(drScene.AssetName);
            return sceneComponent.LoadSceneAsync(assetName, Constant.AssetPriority.SceneAsset);
        }

        public static UniTask UnloadSceneAsync(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.Tables.DTScene.GetOrDefault(sceneId);
            if (drScene == null)
            {
                string error = Utility.Text.Format("Can not unload Scene '{0}' from data table.", sceneId.ToString());
                return UniTask.FromException(new GameFrameworkException(error));
            }
            string assetName = AssetUtility.GetSceneAsset(drScene.AssetName);
            return sceneComponent.UnloadSceneAsync(assetName);
        }
    }
}
