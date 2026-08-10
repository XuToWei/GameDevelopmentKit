using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoomRoot)]
    public sealed class G2SurvivorRoom_InitHandler: MessageHandler<Scene, G2SurvivorRoom_Init, SurvivorRoom2G_Init>
    {
        protected override UniTask Run(Scene root, G2SurvivorRoom_Init request, SurvivorRoom2G_Init response)
        {
            SurvivorRoom room = root.AddComponent<SurvivorRoom, SceneType, string>(SceneType.SurvivorServer, request.RoomCode);
            room.AddComponent<SurvivorWorldComponent, SurvivorWorldRole, string>(SurvivorWorldRole.ServerAuthority, request.RoomCode);
            room.AddComponent<SurvivorRoomServerComponent>();
            return UniTask.CompletedTask;
        }
    }
}
