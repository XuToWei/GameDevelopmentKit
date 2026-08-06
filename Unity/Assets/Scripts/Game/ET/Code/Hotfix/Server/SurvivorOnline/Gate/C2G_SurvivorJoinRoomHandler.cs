using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public sealed class C2G_SurvivorJoinRoomHandler: MessageSessionHandler<C2G_SurvivorJoinRoom, G2C_SurvivorJoinRoom>
    {
        protected override async UniTask Run(Session session, C2G_SurvivorJoinRoom request, G2C_SurvivorJoinRoom response)
        {
            string roomCode = SurvivorRoomCodeUtility.Normalize(request.RoomCode);
            if (!SurvivorRoomCodeUtility.IsValid(roomCode))
            {
                response.Error = ErrorCode.ERR_SurvivorInvalidRoomCode;
                response.Message = "房间号只能是 4-12 位大写字母或数字";
                return;
            }

            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            SurvivorPlayerRoomComponent existingRoom = player.GetComponent<SurvivorPlayerRoomComponent>();
            if (existingRoom != null && existingRoom.RoomCode != roomCode)
            {
                response.Error = ErrorCode.ERR_SurvivorAlreadyInRoom;
                response.Message = $"玩家已经加入房间 {existingRoom.RoomCode}";
                return;
            }

            Scene root = session.Root();
            using G2SurvivorRoomManager_JoinRoom joinRequest = G2SurvivorRoomManager_JoinRoom.Create(true);
            joinRequest.RoomCode = roomCode;
            joinRequest.PlayerId = player.Id;
            joinRequest.DisplayName = player.Account;
            using SurvivorRoomManager2G_JoinRoom joinResponse = (SurvivorRoomManager2G_JoinRoom)await root.GetComponent<MessageSender>().Call(Tables.Instance.DTStartSceneConfig.SurvivorRoomManager.ActorId, joinRequest);
            if (joinResponse.Error != ErrorCode.ERR_Success)
            {
                response.Error = joinResponse.Error;
                response.Message = joinResponse.Message;
                return;
            }

            if (existingRoom == null)
            {
                existingRoom = player.AddComponent<SurvivorPlayerRoomComponent>();
                existingRoom.RoomCode = roomCode;
            }

            existingRoom.RoomActorId = joinResponse.RoomActorId;
            response.RoomCode = roomCode;
            response.PlayerId = player.Id;
            response.IsHost = joinResponse.IsHost;
            response.Sequence = joinResponse.Sequence;
            response.ServerTick = joinResponse.ServerTick;
            response.FullSnapshot = joinResponse.FullSnapshot;
        }
    }
}
