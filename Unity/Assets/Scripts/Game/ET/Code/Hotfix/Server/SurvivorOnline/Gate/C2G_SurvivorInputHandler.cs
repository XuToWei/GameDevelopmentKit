using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public sealed class C2G_SurvivorInputHandler: MessageSessionHandler<C2G_SurvivorInput>
    {
        protected override async UniTask Run(Session session, C2G_SurvivorInput message)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            SurvivorPlayerRoomComponent playerRoom = player.GetComponent<SurvivorPlayerRoomComponent>();
            if (playerRoom == null)
            {
                return;
            }

            G2SurvivorRoom_Input inputMessage = G2SurvivorRoom_Input.Create(true);
            inputMessage.PlayerId = player.Id;
            inputMessage.InputSequence = message.InputSequence;
            inputMessage.MoveX = message.MoveX;
            inputMessage.MoveY = message.MoveY;
            session.Root().GetComponent<MessageSender>().Send(playerRoom.RoomActorId, inputMessage);
            await UniTask.CompletedTask;
        }
    }
}
