using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class LSSceneInitFinish_Finish: AEvent<Scene, EventType.LSSceneInitFinish>
    {
        protected override async UniTask Run(Scene clientScene, EventType.LSSceneInitFinish args)
        {
            Room room = clientScene.GetComponent<Room>();
            
            room.AddComponent<LSUnitViewComponent>();
            
            room.AddComponent<LSCameraComponent>();

            if (!room.IsReplay)
            {
                room.AddComponent<LSOperaComponent>();
            }

            clientScene.GetComponent<UIComponent>().CloseUIForm(UGFUIFormId.UILSLobby);
            await UniTask.CompletedTask;
        }
    }
}