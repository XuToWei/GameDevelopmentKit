using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        /// <summary>
        /// 加载场景（可等待）
        /// </summary>
        public static UniTask LoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName, int priority = 0 , object userData = null)
        {
            sceneComponent.LoadScene(sceneAssetName, priority, userData);
            bool IsFinished()
            {
                return !sceneComponent.SceneIsLoading(sceneAssetName);
            }
            return UniTask.WaitUntil(IsFinished, PlayerLoopTiming.Update);
        }
        
        /// <summary>
        /// 卸载场景（可等待）
        /// </summary>
        public static UniTask UnLoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName, object userData = null)
        {
            sceneComponent.UnloadScene(sceneAssetName, userData);
            bool IsFinished()
            {
                return !sceneComponent.SceneIsUnloading(sceneAssetName);
            }
            return UniTask.WaitUntil(IsFinished, PlayerLoopTiming.Update);
        }
    }
}
