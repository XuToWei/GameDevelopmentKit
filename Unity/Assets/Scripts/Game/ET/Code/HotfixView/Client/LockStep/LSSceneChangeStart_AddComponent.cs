using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class LSSceneChangeStart_AddComponent: AEvent<Scene, LSSceneChangeStart>
    {
        protected override async UniTask Run(Scene clientScene, LSSceneChangeStart args)
        {
            Room room = clientScene.GetComponent<Room>();
            room.AddComponent<UIComponent>();
            
            // 创建loading界面
            
            
            // 创建房间UI
            await room.GetComponent<UIComponent>().AddUIFormAsync<UGFUILSRoomComponent>(UGFUIFormId.UILSRoom);
            
            // 加载场景资源
            await UGFComponent.Instance.LoadSceneAsync(AssetUtility.GetSceneAsset(room.Name));
        }
    }
}