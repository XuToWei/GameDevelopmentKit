using GameFramework;
using UnityGameFramework.Runtime;

namespace Game
{
    public static partial class SceneExtension
    {
        public static bool SceneIsLoading(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.Tables.DTScene.GetOrDefault(sceneId);
            if (drScene == null)
            {
                return false;
            }
            string assetName = AssetUtility.GetSceneAsset(drScene.AssetName);
            return sceneComponent.SceneIsLoading(assetName);
        }

        public static bool SceneIsLoaded(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.Tables.DTScene.GetOrDefault(sceneId);
            if (drScene == null)
            {
                return false;
            }
            string assetName = AssetUtility.GetSceneAsset(drScene.AssetName);
            return sceneComponent.SceneIsLoaded(assetName);
        }

        public static bool CanLoadScene(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.Tables.DTScene.GetOrDefault(sceneId);
            if (drScene == null)
            {
                return false;
            }
            string assetName = AssetUtility.GetSceneAsset(drScene.AssetName);
            return !sceneComponent.SceneIsLoading(assetName) && sceneComponent.SceneIsLoaded(assetName);
        }

        public static void LoadScene(this SceneComponent sceneComponent, int sceneId, object userData = null)
        {
            DRScene drScene = GameEntry.Tables.DTScene.GetOrDefault(sceneId);
            if (drScene == null)
            {
                string error = Utility.Text.Format("Can not load Scene '{0}' from data table.", sceneId.ToString());
                throw new GameFrameworkException(error);
            }
            string assetName = AssetUtility.GetSceneAsset(drScene.AssetName);
            sceneComponent.LoadScene(assetName, Constant.AssetPriority.SceneAsset, userData);
        }

        public static void UnloadScene(this SceneComponent sceneComponent, int sceneId, object userData = null)
        {
            DRScene drScene = GameEntry.Tables.DTScene.GetOrDefault(sceneId);
            if (drScene == null)
            {
                string error = Utility.Text.Format("Can not unload Scene '{0}' from data table.", sceneId.ToString());
                throw new GameFrameworkException(error);
            }
            string assetName = AssetUtility.GetSceneAsset(drScene.AssetName);
            sceneComponent.UnloadScene(assetName, userData);
        }
    }
}
