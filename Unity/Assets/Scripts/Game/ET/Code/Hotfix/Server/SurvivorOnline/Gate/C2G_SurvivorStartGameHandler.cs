using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public sealed class C2G_SurvivorStartGameHandler: MessageSessionHandler<C2G_SurvivorStartGame, G2C_SurvivorStartGame>
    {
        protected override async UniTask Run(Session session, C2G_SurvivorStartGame request, G2C_SurvivorStartGame response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            SurvivorPlayerRoomComponent playerRoom = player.GetComponent<SurvivorPlayerRoomComponent>();
            if (playerRoom == null)
            {
                response.Error = ErrorCode.ERR_SurvivorNotInRoom;
                response.Message = "尚未加入 Survivor 房间";
                return;
            }

            using G2SurvivorRoom_Start startRequest = G2SurvivorRoom_Start.Create(true);
            startRequest.PlayerId = player.Id;
            using SurvivorRoom2G_Start startResponse = (SurvivorRoom2G_Start)await session.Root().GetComponent<MessageSender>().Call(playerRoom.RoomActorId, startRequest);
            response.Error = startResponse.Error;
            response.Message = startResponse.Message;
        }
    }
}
