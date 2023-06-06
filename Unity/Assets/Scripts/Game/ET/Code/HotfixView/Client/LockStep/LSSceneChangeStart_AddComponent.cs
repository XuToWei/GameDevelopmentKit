using Cysharp.Threading.Tasks;
using Game;
using UnityGameFramework.Extension;

namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class LSSceneChangeStart_AddComponent: AEvent<Scene, EventType.LSSceneChangeStart>
    {
        protected override async UniTask Run(Scene clientScene, EventType.LSSceneChangeStart args)
        {
            Room room = args.Room;
            room.AddComponent<UIComponent>();
            room.AddComponent<EntityComponent>();
            
            // 创建房间UI
            await room.GetComponent<UIComponent>().OpenUIFormAsync(UGFUIFormId.UILSRoom);
            
            // 切换到map场景
            await GameEntry.Scene.LoadSceneAsync(AssetUtility.GetSceneAsset(room.Name));
        }
    }
}