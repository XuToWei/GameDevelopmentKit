using System.Collections.Generic;

namespace ET.Server
{
    [EntitySystemOf(typeof(SurvivorRoomManagerComponent))]
    public static partial class SurvivorRoomManagerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorRoomManagerComponent self)
        {
            self.Rooms = new Dictionary<string, ActorId>();
        }
    }
}
