using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public sealed class C2G_SurvivorRequestFullSnapshotHandler: MessageSessionHandler<C2G_SurvivorRequestFullSnapshot>
    {
        protected override UniTask Run(Session session, C2G_SurvivorRequestFullSnapshot message)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            SurvivorPlayerRoomComponent playerRoom = player.GetComponent<SurvivorPlayerRoomComponent>();
            if (playerRoom == null)
            {
                return UniTask.CompletedTask;
            }

            G2SurvivorRoom_RequestFullSnapshot fullSnapshotRequest = G2SurvivorRoom_RequestFullSnapshot.Create(true);
            fullSnapshotRequest.PlayerId = player.Id;
            session.Root().GetComponent<MessageSender>().Send(playerRoom.RoomActorId, fullSnapshotRequest);
            return UniTask.CompletedTask;
        }
    }
}
