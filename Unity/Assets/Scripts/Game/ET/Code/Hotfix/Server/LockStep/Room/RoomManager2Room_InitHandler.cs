using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.RoomRoot)]
    public class RoomManager2Room_InitHandler: MessageHandler<Scene, RoomManager2Room_Init, Room2RoomManager_Init>
    {
        protected override async UniTask Run(Scene root, RoomManager2Room_Init request, Room2RoomManager_Init response)
        {
            Room room = root.AddComponent<Room>();
            room.Name = "Server";
            room.AddComponent<RoomServerComponent, List<long>>(request.PlayerIds);

            room.LSWorld = new LSWorld(SceneType.LockStepServer);
            await UniTask.CompletedTask;
        }
    }
}