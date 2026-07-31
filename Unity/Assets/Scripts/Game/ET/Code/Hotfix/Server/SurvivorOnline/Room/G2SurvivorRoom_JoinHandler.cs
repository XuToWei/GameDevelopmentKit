using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoom)]
    public sealed class G2SurvivorRoom_JoinHandler:
            MessageHandler<Scene, G2SurvivorRoom_Join, SurvivorRoom2G_Join>
    {
        protected override async UniTask Run(
            Scene root,
            G2SurvivorRoom_Join request,
            SurvivorRoom2G_Join response)
        {
            if (root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Phase != SurvivorRoomPhase.Lobby)
            {
                response.Error = ErrorCode.ERR_SurvivorGameAlreadyStarted;
                response.Message = "游戏已开始，禁止中途加入";
                return;
            }

            if (root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Players
                    .Count >= SurvivorDefaults.MaxPlayers)
            {
                response.Error = ErrorCode.ERR_SurvivorRoomFull;
                response.Message = "房间已满";
                return;
            }

            if (!root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Players
                    .ContainsKey(request.PlayerId))
            {
                root.GetComponent<SurvivorRoom>()
                        .GetComponent<SurvivorWorldComponent>()
                        .AddPlayer(request.PlayerId, request.DisplayName);
                root.GetComponent<SurvivorRoom>()
                        .GetComponent<SurvivorRoomServerComponent>()
                        .Runtime
                        .PlayerIds
                        .Add(request.PlayerId);
            }

            root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorRoomServerComponent>()
                    .BroadcastStateFrame(true);
            response.IsHost = root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .HostPlayerId == request.PlayerId;
            response.Sequence = root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorRoomServerComponent>()
                    .Runtime
                    .Frame
                    .Sequence;
            response.ServerTick = root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorRoomServerComponent>()
                    .Runtime
                    .Frame
                    .ServerTick;
            response.FullSnapshot = root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorRoomServerComponent>()
                    .Runtime
                    .Frame
                    .Payload;
            await UniTask.CompletedTask;
        }
    }
}
