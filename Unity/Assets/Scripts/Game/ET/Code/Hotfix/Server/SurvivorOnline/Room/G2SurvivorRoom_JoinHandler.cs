using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoomRoot)]
    public sealed class G2SurvivorRoom_JoinHandler: MessageHandler<Scene, G2SurvivorRoom_Join, SurvivorRoom2G_Join>
    {
        protected override UniTask Run(Scene root, G2SurvivorRoom_Join request, SurvivorRoom2G_Join response)
        {
            SurvivorRoom room = root.GetComponent<SurvivorRoom>();
            SurvivorWorldComponent world = room.GetComponent<SurvivorWorldComponent>();
            SurvivorRoomServerComponent server = room.GetComponent<SurvivorRoomServerComponent>();
            bool isExistingMember = world.Data.Players.ContainsKey(request.PlayerId);
            if (world.Data.Phase == SurvivorRoomPhase.Running && !isExistingMember)
            {
                response.Error = ErrorCode.ERR_SurvivorGameAlreadyStarted;
                response.Message = "游戏已开始，禁止中途加入";
                return UniTask.CompletedTask;
            }

            if (world.Data.Phase == SurvivorRoomPhase.Ended)
            {
                if (!isExistingMember)
                {
                    response.Error = ErrorCode.ERR_SurvivorGameAlreadyStarted;
                    response.Message = "本局已结束，仅原房间成员可以返回";
                    return UniTask.CompletedTask;
                }

                world.ResetForLobby();
                server.ResetForLobby();
            }

            if (!isExistingMember && world.Data.Players.Count >= SurvivorDefaults.MaxPlayers)
            {
                response.Error = ErrorCode.ERR_SurvivorRoomFull;
                response.Message = "房间已满";
                return UniTask.CompletedTask;
            }

            if (!isExistingMember)
            {
                world.AddPlayer(request.PlayerId, request.DisplayName);
                server.Runtime.PlayerIds.Add(request.PlayerId);
            }

            server.RegisterPlayerInputQueue(request.PlayerId);
            SurvivorStateFrameInfo frame = server.BroadcastStateFrame(true);
            response.IsHost = world.Data.HostPlayerId == request.PlayerId;
            response.Sequence = frame.Sequence;
            response.ServerTick = frame.ServerTick;
            response.FullSnapshot = frame.Payload;
            return UniTask.CompletedTask;
        }
    }
}
