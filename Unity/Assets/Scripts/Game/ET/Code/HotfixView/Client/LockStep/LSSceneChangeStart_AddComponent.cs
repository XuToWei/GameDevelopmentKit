using Cysharp.Threading.Tasks;
using Game;
using UnityGameFramework.Extension;

namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class LSSceneChangeStart_AddComponent: AEvent<Scene, LSSceneChangeStart>
    {
        protected override async UniTask Run(Scene clientScene, LSSceneChangeStart args)
        {
            Room room = clientScene.GetComponent<Room>();
            room.AddComponent<UGFUIComponent>();
            room.AddComponent<UGFEntityComponent>();
            
            // 创建loading界面
            
            
            // 创建房间UI
            await room.GetComponent<UGFUIComponent>().OpenUIFormAsync(UGFUIFormId.UILSRoom);
            
            // 加载场景资源
            await GameEntry.Scene.LoadSceneAsync(AssetUtility.GetSceneAsset(room.Name));
        }
    }
}