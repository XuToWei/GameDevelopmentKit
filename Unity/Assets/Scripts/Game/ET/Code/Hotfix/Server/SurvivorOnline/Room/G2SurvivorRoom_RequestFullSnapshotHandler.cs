using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoomRoot)]
    public sealed class G2SurvivorRoom_RequestFullSnapshotHandler: MessageHandler<Scene, G2SurvivorRoom_RequestFullSnapshot>
    {
        protected override async UniTask Run(Scene root, G2SurvivorRoom_RequestFullSnapshot message)
        {
            SurvivorRoomServerComponent server = root.GetComponent<SurvivorRoom>().GetComponent<SurvivorRoomServerComponent>();
            if (server.Runtime.PlayerIds.Contains(message.PlayerId))
            {
                server.BroadcastStateFrame(true);
            }

            await UniTask.CompletedTask;
        }
    }
}
