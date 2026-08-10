using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoomRoot)]
    public sealed class G2SurvivorRoom_InputHandler: MessageHandler<Scene, G2SurvivorRoom_Input>
    {
        protected override UniTask Run(Scene root, G2SurvivorRoom_Input message)
        {
            root.GetComponent<SurvivorRoom>().GetComponent<SurvivorRoomServerComponent>().QueuePlayerInput(message.PlayerId, message.InputSequence, message.MoveX, message.MoveY);
            return UniTask.CompletedTask;
        }
    }
}
