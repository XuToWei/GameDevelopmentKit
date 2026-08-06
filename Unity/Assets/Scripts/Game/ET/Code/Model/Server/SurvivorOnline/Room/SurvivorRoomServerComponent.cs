using System;
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(SurvivorRoom))]
    public sealed partial class SurvivorRoomServerComponent: Entity, IAwake, IUpdate, IDestroy, IETReactive
    {
        public SurvivorRoomServerRuntime Runtime { get; set; }

        [ETReactiveSource]
        public SurvivorRoomPhase Phase => this.GetParent<SurvivorRoom>().GetComponent<SurvivorWorldComponent>().Data.Phase;
    }

    [EnableClass]
    public sealed class SurvivorRoomServerRuntime: IDisposable
    {
        public HashSet<long> PlayerIds { get; } = new();

        public Dictionary<long, Queue<SurvivorQueuedPlayerInput>> PlayerInputQueues { get; } = new();

        public Dictionary<long, long> LastQueuedInputSequences { get; } = new();

        public IEnumerator<long> PlayerIdEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorPlayerState>> PlayerStateEnumerator { get; set; }

        public SurvivorQueuedPlayerInput QueuedInput { get; set; }

        public long Sequence { get; set; }

        public long NextSimulationTime { get; set; }

        public SurvivorRoom2C_StateFrame Frame { get; set; }

        public void Dispose()
        {
            this.PlayerIdEnumerator?.Dispose();
            this.PlayerStateEnumerator?.Dispose();
            this.PlayerInputQueues.Clear();
            this.LastQueuedInputSequences.Clear();
        }
    }

    [EnableClass]
    public sealed class SurvivorQueuedPlayerInput
    {
        public long Sequence { get; set; }

        public int MoveX { get; set; }

        public int MoveY { get; set; }
    }
}
