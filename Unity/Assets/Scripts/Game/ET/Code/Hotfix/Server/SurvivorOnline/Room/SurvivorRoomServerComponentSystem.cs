namespace ET.Server
{
    [EntitySystemOf(typeof(SurvivorRoomServerComponent))]
    [ETReactiveSystem]
    public static partial class SurvivorRoomServerComponentSystem
    {
        private const int MaxQueuedInputsPerPlayer = 64;

        [EntitySystem]
        private static void Awake(this SurvivorRoomServerComponent self)
        {
            self.Runtime = new SurvivorRoomServerRuntime();
            self.Runtime.NextSimulationTime = TimeInfo.Instance.ServerFrameTime();
        }

        [EntitySystem]
        private static void Update(this SurvivorRoomServerComponent self)
        {
            self.ObserveChanges();
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
            self.GetParent<SurvivorRoom>().GetComponent<SurvivorWorldComponent>().TickAuthority();
            if (self.GetParent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .ServerTick % SurvivorDefaults.FullSnapshotInterval == 0)
            {
                self.BroadcastStateFrame(true);
                return;
            }

            if (self.GetParent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .ServerTick % SurvivorDefaults.SnapshotTicks == 0)
            {
                self.BroadcastStateFrame(false);
            }
        }

        [EntitySystem]
        private static void Destroy(this SurvivorRoomServerComponent self)
        {
            self.ClearReactive();
            self.Runtime.Dispose();
            self.Runtime = null;
        }

        [ETReactiveBind(nameof(SurvivorRoomServerComponent.Phase))]
        private static void OnPhaseChanged(this SurvivorRoomServerComponent self, SurvivorRoomPhase phase)
        {
            if (phase != SurvivorRoomPhase.Ended)
            {
                return;
            }

            self.BroadcastStateFrame(false);
        }

        public static void RegisterPlayerInputQueue(
            this SurvivorRoomServerComponent self,
            long playerId)
        {
            if (!self.Runtime.PlayerInputQueues.ContainsKey(playerId))
            {
                self.Runtime.PlayerInputQueues.Add(
                    playerId,
                    new System.Collections.Generic.Queue<SurvivorQueuedPlayerInput>());
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

        public static void QueuePlayerInput(
            this SurvivorRoomServerComponent self,
            long playerId,
            long inputSequence,
            int moveX,
            int moveY)
        {
            SurvivorWorldComponent world =
                    self.GetParent<SurvivorRoom>().GetComponent<SurvivorWorldComponent>();
            if (!world.Data.Players.ContainsKey(playerId))
            {
                return;
            }

            self.RegisterPlayerInputQueue(playerId);
            if (inputSequence <= world.Data.Players[playerId].LastInputSequence ||
                inputSequence <= self.Runtime.LastQueuedInputSequences[playerId] ||
                self.Runtime.PlayerInputQueues[playerId].Count >= MaxQueuedInputsPerPlayer)
            {
                return;
            }

            self.Runtime.QueuedInput = new SurvivorQueuedPlayerInput
            {
                Sequence = inputSequence,
                MoveX = SurvivorMath.Clamp(
                    moveX,
                    -SurvivorDefaults.InputScale,
                    SurvivorDefaults.InputScale),
                MoveY = SurvivorMath.Clamp(
                    moveY,
                    -SurvivorDefaults.InputScale,
                    SurvivorDefaults.InputScale),
            };
            self.Runtime.PlayerInputQueues[playerId].Enqueue(self.Runtime.QueuedInput);
            self.Runtime.LastQueuedInputSequences[playerId] = inputSequence;
            self.Runtime.QueuedInput = null;
        }

        private static void ConsumePlayerInputs(this SurvivorRoomServerComponent self)
        {
            SurvivorWorldComponent world =
                    self.GetParent<SurvivorRoom>().GetComponent<SurvivorWorldComponent>();
            self.Runtime.PlayerStateEnumerator = world.Data.Players.GetEnumerator();
            while (self.Runtime.PlayerStateEnumerator.MoveNext())
            {
                long playerId = self.Runtime.PlayerStateEnumerator.Current.Key;
                SurvivorPlayerState player = self.Runtime.PlayerStateEnumerator.Current.Value;
                if (!self.Runtime.PlayerInputQueues.ContainsKey(playerId) ||
                    self.Runtime.PlayerInputQueues[playerId].Count == 0)
                {
                    player.MoveX = 0;
                    player.MoveY = 0;
                    continue;
                }

                self.Runtime.QueuedInput = self.Runtime.PlayerInputQueues[playerId].Dequeue();
                world.SetPlayerInput(
                    playerId,
                    self.Runtime.QueuedInput.Sequence,
                    self.Runtime.QueuedInput.MoveX,
                    self.Runtime.QueuedInput.MoveY);
                self.Runtime.QueuedInput = null;
            }

            self.Runtime.PlayerStateEnumerator.Dispose();
            self.Runtime.PlayerStateEnumerator = null;
        }

        public static void BroadcastStateFrame(this SurvivorRoomServerComponent self, bool isFull)
        {
            self.Runtime.Sequence++;
            self.Runtime.Frame = SurvivorRoom2C_StateFrame.Create();
            self.Runtime.Frame.Sequence = self.Runtime.Sequence;
            self.Runtime.Frame.ServerTick =
                    self.GetParent<SurvivorRoom>().GetComponent<SurvivorWorldComponent>().Data.ServerTick;
            self.Runtime.Frame.IsFull = isFull;
            self.Runtime.Frame.Payload = isFull
                    ? self.GetParent<SurvivorRoom>().GetComponent<SurvivorWorldComponent>().CaptureFull()
                    : self.GetParent<SurvivorRoom>().GetComponent<SurvivorWorldComponent>().CaptureDelta();
            self.Runtime.PlayerIdEnumerator = self.Runtime.PlayerIds.GetEnumerator();
            while (self.Runtime.PlayerIdEnumerator.MoveNext())
            {
                self.Root()
                        .GetComponent<MessageLocationSenderComponent>()
                        .Get(LocationType.GateSession)
                        .Send(self.Runtime.PlayerIdEnumerator.Current, self.Runtime.Frame);
            }

            self.Runtime.PlayerIdEnumerator.Dispose();
            self.Runtime.PlayerIdEnumerator = null;
        }
    }
}
