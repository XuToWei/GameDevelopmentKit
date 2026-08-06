using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoomRoot)]
    public sealed class G2SurvivorRoom_StartHandler: MessageHandler<Scene, G2SurvivorRoom_Start, SurvivorRoom2G_Start>
    {
        protected override async UniTask Run(Scene root, G2SurvivorRoom_Start request, SurvivorRoom2G_Start response)
        {
            SurvivorRoom room = root.GetComponent<SurvivorRoom>();
            SurvivorWorldComponent world = room.GetComponent<SurvivorWorldComponent>();
            if (world.Data.HostPlayerId != request.PlayerId)
            {
                response.Error = ErrorCode.ERR_SurvivorOnlyHostCanStart;
                response.Message = "只有房主可以开始游戏";
                return;
            }

            if (world.Data.Phase != SurvivorRoomPhase.Lobby)
            {
                response.Error = ErrorCode.ERR_SurvivorGameAlreadyStarted;
                response.Message = "游戏已经开始";
                return;
            }

            if (world.Data.Players.Count == 0)
            {
                response.Error = ErrorCode.ERR_SurvivorRoomNotReady;
                response.Message = "房间内没有玩家";
                return;
            }

            SurvivorRoomServerComponent server = room.GetComponent<SurvivorRoomServerComponent>();
            world.Data.Phase = SurvivorRoomPhase.Running;
            server.Runtime.NextSimulationTime = TimeInfo.Instance.ServerFrameTime();
            server.BroadcastStateFrame(true);
            await UniTask.CompletedTask;
        }
    }
}
