using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoom)]
    public sealed class G2SurvivorRoom_StartHandler:
            MessageHandler<Scene, G2SurvivorRoom_Start, SurvivorRoom2G_Start>
    {
        protected override async UniTask Run(
            Scene root,
            G2SurvivorRoom_Start request,
            SurvivorRoom2G_Start response)
        {
            if (root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .HostPlayerId != request.PlayerId)
            {
                response.Error = ErrorCode.ERR_SurvivorOnlyHostCanStart;
                response.Message = "只有房主可以开始游戏";
                return;
            }

            if (root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Phase != SurvivorRoomPhase.Lobby)
            {
                response.Error = ErrorCode.ERR_SurvivorGameAlreadyStarted;
                response.Message = "游戏已经开始";
                return;
            }

            if (root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Players
                    .Count == 0)
            {
                response.Error = ErrorCode.ERR_SurvivorRoomNotReady;
                response.Message = "房间内没有玩家";
                return;
            }

            root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Phase = SurvivorRoomPhase.Running;
            root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorRoomServerComponent>()
                    .Runtime
                    .NextSimulationTime = TimeInfo.Instance.ServerFrameTime();
            root.GetComponent<SurvivorRoom>()
                    .GetComponent<SurvivorRoomServerComponent>()
                    .BroadcastStateFrame(true);
            await UniTask.CompletedTask;
        }
    }
}
