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
            if (!self.Runtime.PlayerInputQueues.ContainsKey(playerId))
            {
                self.Runtime.PlayerInputQueues.Add(playerId, new Queue<SurvivorQueuedPlayerInput>());
            }

            if (!self.Runtime.LastQueuedInputSequences.ContainsKey(playerId))
            {
                self.Runtime.LastQueuedInputSequences.Add(playerId, 0);
            }
        }

        public static void ResetForLobby(this SurvivorRoomServerComponent self)
        {
            self.Runtime.PlayerInputQueues.Clear();
            self.Runtime.LastQueuedInputSequences.Clear();
            self.Runtime.QueuedInput = null;
            self.Runtime.NextSimulationTime = TimeInfo.Instance.ServerFrameTime();
            self.Runtime.PlayerIdEnumerator = self.Runtime.PlayerIds.GetEnumerator();
            while (self.Runtime.PlayerIdEnumerator.MoveNext())
            {
                self.RegisterPlayerInputQueue(self.Runtime.PlayerIdEnumerator.Current);
            }

            self.Runtime.PlayerIdEnumerator.Dispose();
            self.Runtime.PlayerIdEnumerator = null;
        }

        public static void QueuePlayerInput(this SurvivorRoomServerComponent self, long playerId, long inputSequence, int moveX, int moveY)
        {
            if (!self.World.Data.Players.ContainsKey(playerId))
            {
                return;
            }

            self.RegisterPlayerInputQueue(playerId);
            if (inputSequence <= self.World.Data.Players[playerId].LastInputSequence ||
                inputSequence <= self.Runtime.LastQueuedInputSequences[playerId] ||
                self.Runtime.PlayerInputQueues[playerId].Count >= MaxQueuedInputsPerPlayer)
            {
                return;
            }

            self.Runtime.QueuedInput = new SurvivorQueuedPlayerInput
            {
                Sequence = inputSequence,
                MoveX = SurvivorMath.Clamp(moveX, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale),
                MoveY = SurvivorMath.Clamp(moveY, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale),
            };
            self.Runtime.PlayerInputQueues[playerId].Enqueue(self.Runtime.QueuedInput);
            self.Runtime.LastQueuedInputSequences[playerId] = inputSequence;
            self.Runtime.QueuedInput = null;
        }

        private static void ConsumePlayerInputs(this SurvivorRoomServerComponent self)
        {
            self.Runtime.PlayerStateEnumerator = self.World.Data.Players.GetEnumerator();
            while (self.Runtime.PlayerStateEnumerator.MoveNext())
            {
                long playerId = self.Runtime.PlayerStateEnumerator.Current.Key;
                SurvivorPlayerState player = self.Runtime.PlayerStateEnumerator.Current.Value;
                if (!self.Runtime.PlayerInputQueues.ContainsKey(playerId) || self.Runtime.PlayerInputQueues[playerId].Count == 0)
                {
                    player.MoveX = 0;
                    player.MoveY = 0;
                    continue;
                }

                self.Runtime.QueuedInput = self.Runtime.PlayerInputQueues[playerId].Dequeue();
                self.World.SetPlayerInput(playerId, self.Runtime.QueuedInput.Sequence, self.Runtime.QueuedInput.MoveX, self.Runtime.QueuedInput.MoveY);
                self.Runtime.QueuedInput = null;
            }

            self.Runtime.PlayerStateEnumerator.Dispose();
            self.Runtime.PlayerStateEnumerator = null;
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
            self.Runtime.PlayerIdEnumerator = self.Runtime.PlayerIds.GetEnumerator();
            while (self.Runtime.PlayerIdEnumerator.MoveNext())
            {
                SurvivorRoom2C_StateFrame frame = SurvivorRoom2C_StateFrame.Create(true);
                frame.Sequence = self.Runtime.Sequence;
                frame.ServerTick = serverTick;
                frame.IsFull = isFull;
                frame.Payload = payload;
                sender.Get(LocationType.GateSession).Send(self.Runtime.PlayerIdEnumerator.Current, frame);
            }

            self.Runtime.PlayerIdEnumerator.Dispose();
            self.Runtime.PlayerIdEnumerator = null;
            return new SurvivorStateFrameInfo(self.Runtime.Sequence, serverTick, payload);
        }
    }
}
