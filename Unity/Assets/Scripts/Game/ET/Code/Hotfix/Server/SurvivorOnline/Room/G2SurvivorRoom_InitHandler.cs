using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoom)]
    public sealed class G2SurvivorRoom_InitHandler:
            MessageHandler<Scene, G2SurvivorRoom_Init, SurvivorRoom2G_Init>
    {
        protected override async UniTask Run(
            Scene root,
            G2SurvivorRoom_Init request,
            SurvivorRoom2G_Init response)
        {
            root.AddComponent<SurvivorRoom, SceneType, string>(SceneType.SurvivorRoom, request.RoomCode);
            root.GetComponent<SurvivorRoom>()
                    .AddComponent<SurvivorWorldComponent, SurvivorWorldRole, string>(
                        SurvivorWorldRole.ServerAuthority,
                        request.RoomCode);
            root.GetComponent<SurvivorRoom>().AddComponent<SurvivorRoomServerComponent>();
            await UniTask.CompletedTask;
        }
    }
}
