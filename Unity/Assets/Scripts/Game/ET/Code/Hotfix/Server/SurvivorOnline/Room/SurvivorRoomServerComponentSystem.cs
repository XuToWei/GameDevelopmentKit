using System.Collections.Generic;

namespace ET.Server
{
    [EntitySystemOf(typeof(SurvivorRoomServerComponent))]
    public static partial class SurvivorRoomServerComponentSystem
    {
        private const int MaxQueuedInputsPerPlayer = 64;

        [EntitySystem]
        private static void Awake(this SurvivorRoomServerComponent self)
        {
            self.World = self.GetParent<SurvivorRoom>().GetComponent<SurvivorWorldComponent>();
            self.Runtime = new SurvivorRoomServerRuntime();
            self.Runtime.NextSimulationTime = TimeInfo.Instance.ServerFrameTime();
        }

        [EntitySystem]
        private static void Update(this SurvivorRoomServerComponent self)
        {
            if (self.Phase != SurvivorRoomPhase.Running)
            {
                self.Runtime.NextSimulationTime = TimeInfo.Instance.ServerFrameTime();
                return;
            }

            if (TimeInfo.Instance.ServerFrameTime() < self.Runtime.NextSimulationTime)
            {
                return;
            }

            self.Runtime.NextSimulationTime += 1000 / SurvivorDefaults.SimulationTicksPerSecond;
            self.ConsumePlayerInputs();
            self.World.TickAuthority();
            if (self.Phase != SurvivorRoomPhase.Running)
            {
                self.BroadcastStateFrame(false);
                return;
            }

            if (self.World.Data.ServerTick % SurvivorDefaults.FullSnapshotInterval == 0)
            {
                self.BroadcastStateFrame(true);
                return;
            }

            if (self.World.Data.ServerTick % SurvivorDefaults.SnapshotTicks == 0)
            {
                self.BroadcastStateFrame(false);
            }
        }

        [EntitySystem]
        private static void Destroy(this SurvivorRoomServerComponent self)
        {
            self.Runtime.Dispose();
            self.Runtime = null;
            self.World = null;
        }

        public static void RegisterPlayerInputQueue(this SurvivorRoomServerComponent self, long playerId)
        {
            self.Runtime.PlayerInputQueues.TryAdd(playerId, new Queue<SurvivorQueuedPlayerInput>());
            self.Runtime.LastQueuedInputSequences.TryAdd(playerId, 0);
        }

        public static void ResetForLobby(this SurvivorRoomServerComponent self)
        {
            self.Runtime.PlayerInputQueues.Clear();
            self.Runtime.LastQueuedInputSequences.Clear();
            self.Runtime.NextSimulationTime = TimeInfo.Instance.ServerFrameTime();
            using var playerIdEnumerator = self.Runtime.PlayerIds.GetEnumerator();
            while (playerIdEnumerator.MoveNext())
            {
                self.RegisterPlayerInputQueue(playerIdEnumerator.Current);
            }
        }

        public static void QueuePlayerInput(this SurvivorRoomServerComponent self, long playerId, long inputSequence, int moveX, int moveY)
        {
            if (!self.World.Data.Players.TryGetValue(playerId, out SurvivorPlayerState player))
            {
                return;
            }

            self.RegisterPlayerInputQueue(playerId);
            if (inputSequence <= player.LastInputSequence ||
                inputSequence <= self.Runtime.LastQueuedInputSequences[playerId] ||
                self.Runtime.PlayerInputQueues[playerId].Count >= MaxQueuedInputsPerPlayer)
            {
                return;
            }

            SurvivorQueuedPlayerInput queuedInput = new SurvivorQueuedPlayerInput
            {
                Sequence = inputSequence,
                MoveX = SurvivorMath.Clamp(moveX, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale),
                MoveY = SurvivorMath.Clamp(moveY, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale),
            };
            self.Runtime.PlayerInputQueues[playerId].Enqueue(queuedInput);
            self.Runtime.LastQueuedInputSequences[playerId] = inputSequence;
        }

        private static void ConsumePlayerInputs(this SurvivorRoomServerComponent self)
        {
            using var playerStateEnumerator = self.World.Data.Players.GetEnumerator();
            while (playerStateEnumerator.MoveNext())
            {
                long playerId = playerStateEnumerator.Current.Key;
                SurvivorPlayerState player = playerStateEnumerator.Current.Value;
                if (!self.Runtime.PlayerInputQueues.TryGetValue(playerId, out Queue<SurvivorQueuedPlayerInput> inputQueue) ||
                    inputQueue.Count == 0)
                {
                    player.MoveX = 0;
                    player.MoveY = 0;
                    continue;
                }

                SurvivorQueuedPlayerInput queuedInput = inputQueue.Dequeue();
                self.World.SetPlayerInput(playerId, queuedInput.Sequence, queuedInput.MoveX, queuedInput.MoveY);
            }

        }

        /// <summary>
        /// 每个收件人各持有一条池化消息：Send 是所有权转移，多个收件人共用同一实例会导致重复回收。
        /// </summary>
        public static SurvivorStateFrameInfo BroadcastStateFrame(this SurvivorRoomServerComponent self, bool isFull)
        {
            self.Runtime.Sequence++;
            long serverTick = self.World.Data.ServerTick;
            byte[] payload = isFull ? self.World.CaptureFull() : self.World.CaptureDelta();
            MessageLocationSenderComponent sender = self.Root().GetComponent<MessageLocationSenderComponent>();
            using var playerIdEnumerator = self.Runtime.PlayerIds.GetEnumerator();
            while (playerIdEnumerator.MoveNext())
            {
                SurvivorRoom2C_StateFrame frame = SurvivorRoom2C_StateFrame.Create(true);
                frame.Sequence = self.Runtime.Sequence;
                frame.ServerTick = serverTick;
                frame.IsFull = isFull;
                frame.Payload = payload;
                sender.Get(LocationType.GateSession).Send(playerIdEnumerator.Current, frame);
            }

            return new SurvivorStateFrameInfo(self.Runtime.Sequence, serverTick, payload);
        }
    }
}
