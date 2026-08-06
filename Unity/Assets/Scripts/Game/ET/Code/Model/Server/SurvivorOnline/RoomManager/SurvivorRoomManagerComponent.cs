using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public sealed class SurvivorRoomManagerComponent: Entity, IAwake
    {
        public SurvivorRoomManagerRuntime Runtime { get; set; }
    }

    [EnableClass]
    public sealed class SurvivorRoomManagerRuntime
    {
        public Dictionary<string, ActorId> Rooms { get; } = new();
    }
}
