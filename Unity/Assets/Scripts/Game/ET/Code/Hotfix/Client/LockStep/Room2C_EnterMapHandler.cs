using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [MessageHandler(SceneType.LockStep)]
    public class Room2C_EnterMapHandler: MessageHandler<Scene, Room2C_Start>
    {
        protected override async UniTask Run(Scene root, Room2C_Start message)
        {
            root.GetComponent<ObjectWait>().Notify(new WaitType.Wait_Room2C_Start() {Message = message});
            await UniTask.CompletedTask;
        }
    }
}