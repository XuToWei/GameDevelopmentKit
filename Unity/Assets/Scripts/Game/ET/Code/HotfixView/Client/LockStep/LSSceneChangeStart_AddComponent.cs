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
            Room room = clientScene.GetComponent<Room>();
            
            // 创建房间UI
            await clientScene.GetComponent<UIComponent>().OpenUIFormAsync(UGFUIFormId.UILSRoom);
            
            // 切换到map场景
            await GameEntry.Scene.LoadSceneAsync(room.Name);
        }
    }
}