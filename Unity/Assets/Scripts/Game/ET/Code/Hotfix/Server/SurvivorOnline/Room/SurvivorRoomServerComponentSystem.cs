namespace ET.Server
{
    [EntitySystemOf(typeof(SurvivorRoomServerComponent))]
    public static partial class SurvivorRoomServerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorRoomServerComponent self)
        {
            self.Runtime = new SurvivorRoomServerRuntime();
            self.Runtime.NextSimulationTime = TimeInfo.Instance.ServerFrameTime();
        }

        [EntitySystem]
        private static void Update(this SurvivorRoomServerComponent self)
        {
            if (self.GetParent<SurvivorRoom>()
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Phase != SurvivorRoomPhase.Running)
            {
                return;
            }

            if (TimeInfo.Instance.ServerFrameTime() < self.Runtime.NextSimulationTime)
            {
                return;
            }

            self.Runtime.NextSimulationTime += 1000 / SurvivorDefaults.SimulationTicksPerSecond;
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
            self.Runtime.Dispose();
            self.Runtime = null;
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
