using System;
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(SurvivorRoom))]
    public sealed class SurvivorRoomServerComponent: Entity, IAwake, IUpdate, IDestroy
    {
        public SurvivorRoomServerRuntime Runtime { get; set; }
    }

    [EnableClass]
    public sealed class SurvivorRoomServerRuntime: IDisposable
    {
        public HashSet<long> PlayerIds { get; } = new();

        public IEnumerator<long> PlayerIdEnumerator { get; set; }

        public long Sequence { get; set; }

        public long NextSimulationTime { get; set; }

        public SurvivorRoom2C_StateFrame Frame { get; set; }

        public void Dispose()
        {
            this.PlayerIdEnumerator?.Dispose();
        }
    }
}
