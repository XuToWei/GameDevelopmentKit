using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public sealed class SurvivorRoomDirectoryComponent: Entity, IAwake
    {
        public SurvivorRoomDirectoryRuntime Runtime { get; set; }
    }

    [EnableClass]
    public sealed class SurvivorRoomDirectoryRuntime
    {
        public Dictionary<string, ActorId> Rooms { get; } = new();
    }
}
