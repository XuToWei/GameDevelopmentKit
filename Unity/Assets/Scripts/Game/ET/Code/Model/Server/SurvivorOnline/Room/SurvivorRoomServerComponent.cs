using System;
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(SurvivorRoom))]
    public sealed class SurvivorRoomServerComponent: Entity, IAwake, IUpdate, IDestroy
    {
        public SurvivorRoomServerRuntime Runtime { get; set; }

        /// <summary>房间与 World 在本组件生命周期内是同一实例，按经验文档第 5 章在 Awake 缓存。</summary>
        public SurvivorWorldComponent World { get; set; }

        public SurvivorRoomPhase Phase => this.World.Data.Phase;
    }

    [EnableClass]
    public sealed class SurvivorRoomServerRuntime: IDisposable
    {
        public HashSet<long> PlayerIds { get; } = new();

        public Dictionary<long, Queue<SurvivorQueuedPlayerInput>> PlayerInputQueues { get; } = new();

        public Dictionary<long, long> LastQueuedInputSequences { get; } = new();

        public SurvivorQueuedPlayerInput QueuedInput { get; set; }

        public long Sequence { get; set; }

        public long NextSimulationTime { get; set; }

        public void Dispose()
        {
            this.PlayerInputQueues.Clear();
            this.LastQueuedInputSequences.Clear();
        }
    }

    /// <summary>
    /// 广播结果的逻辑结构。协议对象只停留在 BroadcastStateFrame 内部，不跨方法传递。
    /// </summary>
    public readonly struct SurvivorStateFrameInfo
    {
        public readonly long Sequence;

        public readonly long ServerTick;

        public readonly byte[] Payload;

        public SurvivorStateFrameInfo(long sequence, long serverTick, byte[] payload)
        {
            this.Sequence = sequence;
            this.ServerTick = serverTick;
            this.Payload = payload;
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
