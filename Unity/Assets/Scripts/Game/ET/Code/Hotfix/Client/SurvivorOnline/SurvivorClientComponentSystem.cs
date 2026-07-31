using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorClientComponent))]
    public static partial class SurvivorClientComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorClientComponent self)
        {
            self.Runtime = new SurvivorClientRuntime();
            self.Runtime.DataObserver = new SurvivorClientReactiveObserver(
                self,
                new SurvivorClientReactionSink());
            self.Runtime.DataObserver.ResetChanges();
            self.Root().SceneType |= SceneType.SurvivorClient | SceneType.SurvivorView;
        }

        [EntitySystem]
        private static void Destroy(this SurvivorClientComponent self)
        {
            self.Runtime.DataObserver.ResetChanges();
            self.Runtime.DataObserver = null;
            self.Runtime.PresentationObservers.Clear();
            self.Room = null;
            self.Runtime = null;
        }

        public static async UniTask<G2C_SurvivorJoinRoom> JoinRoom(
            this SurvivorClientComponent self,
            string roomCode)
        {
            self.PrepareSnapshotConsumer(roomCode);
            self.Runtime.JoinRequest = C2G_SurvivorJoinRoom.Create();
            self.Runtime.JoinRequest.RoomCode = roomCode;
            self.Runtime.JoinResponse = (G2C_SurvivorJoinRoom)await self.Root()
                    .GetComponent<ClientSenderComponent>()
                    .Call(self.Runtime.JoinRequest, false);
            if (self.Runtime.JoinResponse.Error != ErrorCode.ERR_Success)
            {
                return self.Runtime.JoinResponse;
            }

            self.PlayerId = self.Runtime.JoinResponse.PlayerId;
            self.IsHost = self.Runtime.JoinResponse.IsHost;
            self.ApplyStateFrame(
                self.Runtime.JoinResponse.Sequence,
                true,
                self.Runtime.JoinResponse.FullSnapshot);
            return self.Runtime.JoinResponse;
        }

        public static async UniTask<G2C_SurvivorStartGame> StartGame(this SurvivorClientComponent self)
        {
            self.Runtime.StartRequest = C2G_SurvivorStartGame.Create();
            self.Runtime.StartResponse = (G2C_SurvivorStartGame)await self.Root()
                    .GetComponent<ClientSenderComponent>()
                    .Call(self.Runtime.StartRequest, false);
            return self.Runtime.StartResponse;
        }

        public static void SendInput(this SurvivorClientComponent self, int moveX, int moveY)
        {
            if (!self.HasBaseline)
            {
                return;
            }

            self.InputSequence++;
            self.Runtime.InputMessage = C2G_SurvivorInput.Create();
            self.Runtime.InputMessage.InputSequence = self.InputSequence;
            self.Runtime.InputMessage.MoveX =
                    SurvivorMath.Clamp(moveX, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale);
            self.Runtime.InputMessage.MoveY =
                    SurvivorMath.Clamp(moveY, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale);
            self.Root().GetComponent<ClientSenderComponent>().Send(self.Runtime.InputMessage);
        }

        public static void ApplyStateFrame(
            this SurvivorClientComponent self,
            long sequence,
            bool isFull,
            byte[] payload)
        {
            if (sequence <= self.LastSequence)
            {
                return;
            }

            if (!self.HasBaseline && !isFull)
            {
                self.Root()
                        .GetComponent<ClientSenderComponent>()
                        .Send(C2G_SurvivorRequestFullSnapshot.Create());
                return;
            }

            if (self.HasBaseline && !isFull && sequence != self.LastSequence + 1)
            {
                self.HasBaseline = false;
                self.Root()
                        .GetComponent<ClientSenderComponent>()
                        .Send(C2G_SurvivorRequestFullSnapshot.Create());
                return;
            }

            self.Room.GetComponent<SurvivorWorldComponent>().ApplySnapshot(payload);
            self.LastSequence = sequence;
            self.HasBaseline = true;
            self.Runtime.DataObserver.ObserveChanges();
            self.NotifyPresentationObservers();
        }

        public static void PrepareSnapshotConsumer(this SurvivorClientComponent self, string roomCode)
        {
            if (self.Root().GetComponent<SurvivorRoom>() != null)
            {
                self.Root().RemoveComponent<SurvivorRoom>();
            }

            self.Room = self.Root().AddComponent<SurvivorRoom, SceneType, string>(
                SceneType.SurvivorClient,
                roomCode);
            self.Room.AddComponent<SurvivorWorldComponent, SurvivorWorldRole, string>(
                SurvivorWorldRole.SnapshotConsumer,
                roomCode);
            self.LastSequence = 0;
            self.InputSequence = 0;
            self.HasBaseline = false;
            self.Runtime.DataObserver.ResetChanges();
            self.Runtime.PlayerStates.Clear();
            self.Runtime.MonsterStates.Clear();
            self.Runtime.ProjectileStates.Clear();
            self.Runtime.PickupStates.Clear();
            self.Runtime.SeenStateIds.Clear();
            self.Runtime.RemovalStateIds.Clear();
        }

        public static void RegisterPresentationObserver(
            this SurvivorClientComponent self,
            ReactiveBinding.IReactiveObserver observer)
        {
            if (self.Runtime.PresentationObservers.Contains(observer))
            {
                return;
            }

            self.Runtime.PresentationObservers.Add(observer);
        }

        public static void UnregisterPresentationObserver(
            this SurvivorClientComponent self,
            ReactiveBinding.IReactiveObserver observer)
        {
            if (self.Runtime == null || observer == null)
            {
                return;
            }

            self.Runtime.PresentationObservers.Remove(observer);
        }

        public static void NotifyPresentationObservers(this SurvivorClientComponent self)
        {
            self.Runtime.Index = 0;
            while (self.Runtime.Index < self.Runtime.PresentationObservers.Count)
            {
                self.Runtime.PresentationObservers[self.Runtime.Index].ObserveChanges();
                self.Runtime.Index++;
            }
        }

        public static void ReconcileStateEntries(this SurvivorClientComponent self)
        {
            self.Runtime.SeenStateIds.Clear();
            self.Runtime.PlayerStates.Clear();
            self.Runtime.MonsterStates.Clear();
            self.Runtime.ProjectileStates.Clear();
            self.Runtime.PickupStates.Clear();

            self.Runtime.PlayerEnumerator = self.Room
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Players
                    .GetEnumerator();
            while (self.Runtime.PlayerEnumerator.MoveNext())
            {
                self.Runtime.StateId = self.Runtime.PlayerEnumerator.Current.Value.StateId;
                self.Runtime.SeenStateIds.Add(self.Runtime.StateId);
                self.Runtime.PlayerStates[self.Runtime.StateId] =
                        self.Runtime.PlayerEnumerator.Current.Value;
                if (self.GetChild<SurvivorPlayerEntry>(self.Runtime.StateId) == null)
                {
                    self.AddChildWithId<SurvivorPlayerEntry>(self.Runtime.StateId);
                }
            }

            self.Runtime.PlayerEnumerator.Dispose();
            self.Runtime.PlayerEnumerator = null;

            self.Runtime.MonsterEnumerator = self.Room
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Monsters
                    .GetEnumerator();
            while (self.Runtime.MonsterEnumerator.MoveNext())
            {
                self.Runtime.StateId = self.Runtime.MonsterEnumerator.Current.Value.StateId;
                self.Runtime.SeenStateIds.Add(self.Runtime.StateId);
                self.Runtime.MonsterStates[self.Runtime.StateId] =
                        self.Runtime.MonsterEnumerator.Current.Value;
                if (self.GetChild<SurvivorMonsterEntry>(self.Runtime.StateId) == null)
                {
                    self.AddChildWithId<SurvivorMonsterEntry>(self.Runtime.StateId);
                }
            }

            self.Runtime.MonsterEnumerator.Dispose();
            self.Runtime.MonsterEnumerator = null;

            self.Runtime.ProjectileEnumerator = self.Room
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Projectiles
                    .GetEnumerator();
            while (self.Runtime.ProjectileEnumerator.MoveNext())
            {
                self.Runtime.StateId = self.Runtime.ProjectileEnumerator.Current.Value.StateId;
                self.Runtime.SeenStateIds.Add(self.Runtime.StateId);
                self.Runtime.ProjectileStates[self.Runtime.StateId] =
                        self.Runtime.ProjectileEnumerator.Current.Value;
                if (self.GetChild<SurvivorProjectileEntry>(self.Runtime.StateId) == null)
                {
                    self.AddChildWithId<SurvivorProjectileEntry>(self.Runtime.StateId);
                }
            }

            self.Runtime.ProjectileEnumerator.Dispose();
            self.Runtime.ProjectileEnumerator = null;

            self.Runtime.PickupEnumerator = self.Room
                    .GetComponent<SurvivorWorldComponent>()
                    .Data
                    .Pickups
                    .GetEnumerator();
            while (self.Runtime.PickupEnumerator.MoveNext())
            {
                self.Runtime.StateId = self.Runtime.PickupEnumerator.Current.Value.StateId;
                self.Runtime.SeenStateIds.Add(self.Runtime.StateId);
                self.Runtime.PickupStates[self.Runtime.StateId] =
                        self.Runtime.PickupEnumerator.Current.Value;
                if (self.GetChild<SurvivorPickupEntry>(self.Runtime.StateId) == null)
                {
                    self.AddChildWithId<SurvivorPickupEntry>(self.Runtime.StateId);
                }
            }

            self.Runtime.PickupEnumerator.Dispose();
            self.Runtime.PickupEnumerator = null;

            self.Runtime.RemovalStateIds.Clear();
            self.Runtime.EntryEnumerator = self.Children.Values.GetEnumerator();
            while (self.Runtime.EntryEnumerator.MoveNext())
            {
                if (!self.Runtime.SeenStateIds.Contains(self.Runtime.EntryEnumerator.Current.Id))
                {
                    self.Runtime.RemovalStateIds.Add(self.Runtime.EntryEnumerator.Current.Id);
                }
            }

            self.Runtime.EntryEnumerator.Dispose();
            self.Runtime.EntryEnumerator = null;
            self.Runtime.Index = 0;
            while (self.Runtime.Index < self.Runtime.RemovalStateIds.Count)
            {
                self.RemoveChild(self.Runtime.RemovalStateIds[self.Runtime.Index]);
                self.Runtime.Index++;
            }
        }
    }
}
