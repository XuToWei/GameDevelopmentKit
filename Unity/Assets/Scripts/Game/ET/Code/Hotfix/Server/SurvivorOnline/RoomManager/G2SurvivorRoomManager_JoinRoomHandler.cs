using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoomManager)]
    public sealed class G2SurvivorRoomManager_JoinRoomHandler: MessageHandler<Scene, G2SurvivorRoomManager_JoinRoom, SurvivorRoomManager2G_JoinRoom>
    {
        protected override async UniTask Run(Scene root, G2SurvivorRoomManager_JoinRoom request, SurvivorRoomManager2G_JoinRoom response)
        {
            SurvivorRoomManagerComponent manager = root.GetComponent<SurvivorRoomManagerComponent>();
            using CoroutineLock _ = await root.GetComponent<CoroutineLockComponent>().Wait(SurvivorCoroutineLockType.RoomDirectory, request.RoomCode.GetHashCode());
            if (!manager.Rooms.TryGetValue(request.RoomCode, out ActorId roomActorId))
            {
                int fiberId = await FiberManager.Instance.Create(SchedulerType.ThreadPool, root.Fiber().Zone, SceneType.SurvivorRoomRoot, $"SurvivorRoom-{request.RoomCode}");
                roomActorId = new ActorId(root.Fiber().Process, fiberId);

                using G2SurvivorRoom_Init initRequest = G2SurvivorRoom_Init.Create(true);
                initRequest.RoomCode = request.RoomCode;
                using SurvivorRoom2G_Init initResponse = (SurvivorRoom2G_Init)await root.GetComponent<MessageSender>().Call(roomActorId, initRequest);
                if (initResponse.Error != ErrorCode.ERR_Success)
                {
                    response.Error = initResponse.Error;
                    response.Message = initResponse.Message;
                    return;
                }

                manager.Rooms.Add(request.RoomCode, roomActorId);
            }

            using G2SurvivorRoom_Join joinRequest = G2SurvivorRoom_Join.Create(true);
            joinRequest.PlayerId = request.PlayerId;
            joinRequest.DisplayName = request.DisplayName;
            using SurvivorRoom2G_Join joinResponse = (SurvivorRoom2G_Join)await root.GetComponent<MessageSender>().Call(roomActorId, joinRequest);
            if (joinResponse.Error != ErrorCode.ERR_Success)
            {
                response.Error = joinResponse.Error;
                response.Message = joinResponse.Message;
                return;
            }

            response.RoomActorId = roomActorId;
            response.IsHost = joinResponse.IsHost;
            response.Sequence = joinResponse.Sequence;
            response.ServerTick = joinResponse.ServerTick;
            response.FullSnapshot = joinResponse.FullSnapshot;
        }
    }
}
