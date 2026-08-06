using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorClientComponent))]
    public static partial class SurvivorClientComponentSystem
    {
        private const int MaxPredictionTicksPerFrame = 5;
        private const float MaxPredictionDeltaSeconds = MaxPredictionTicksPerFrame * SurvivorLocalPlayerPrediction.InputIntervalSeconds;

        [EntitySystem]
        private static void Awake(this SurvivorClientComponent self)
        {
            self.ClientSender = self.Root().GetComponent<ClientSenderComponent>();
            self.LocalPrediction = new SurvivorLocalPlayerPrediction();
            self.AddComponent<SurvivorViewEntityManagerComponent>();
            self.AddComponent<SurvivorViewComponent>();
            self.Root().SceneType |= SceneType.SurvivorView;
        }

        [EntitySystem]
        private static void Destroy(this SurvivorClientComponent self)
        {
            self.World = default;
            self.Room = default;
            self.LocalPrediction = null;
        }

        public static async UniTask<SurvivorJoinRoomResult> JoinRoom(this SurvivorClientComponent self, string roomCode)
        {
            self.PrepareSnapshotConsumer(roomCode);
            using C2G_SurvivorJoinRoom request = C2G_SurvivorJoinRoom.Create(true);
            request.RoomCode = roomCode;
            using G2C_SurvivorJoinRoom response = (G2C_SurvivorJoinRoom)await self.ClientSender.Call(request, false);
            if (response.Error != ErrorCode.ERR_Success)
            {
                return new SurvivorJoinRoomResult(response.Error, response.Message, 0, false);
            }

            self.PlayerId = response.PlayerId;
            self.IsHost = response.IsHost;
            self.ApplyStateFrame(response.Sequence, true, response.FullSnapshot);
            return new SurvivorJoinRoomResult(response.Error, response.Message, response.PlayerId, response.IsHost);
        }

        public static async UniTask<SurvivorRequestResult> StartGame(this SurvivorClientComponent self)
        {
            using C2G_SurvivorStartGame request = C2G_SurvivorStartGame.Create(true);
            using G2C_SurvivorStartGame response = (G2C_SurvivorStartGame)await self.ClientSender.Call(request, false);
            return new SurvivorRequestResult(response.Error, response.Message);
        }

        public static async UniTask<SurvivorRequestResult> ChooseSkill(this SurvivorClientComponent self, SurvivorSkillType skillType, long choiceRevision)
        {
            using C2G_SurvivorChooseSkill request = C2G_SurvivorChooseSkill.Create(true);
            request.SkillType = (int)skillType;
            request.ChoiceRevision = choiceRevision;
            using G2C_SurvivorChooseSkill response = (G2C_SurvivorChooseSkill)await self.ClientSender.Call(request, false);
            return new SurvivorRequestResult(response.Error, response.Message);
        }

        public static void UpdateLocalInput(this SurvivorClientComponent self, int moveX, int moveY, float deltaTime)
        {
            SurvivorPlayerState player = self.LocalPlayer;
            if (player == null || self.Phase != SurvivorRoomPhase.Running || !player.Alive)
            {
                self.LocalPrediction.CurrentMoveX = 0;
                self.LocalPrediction.CurrentMoveY = 0;
                return;
            }

            self.EnsureLocalPredictionInitialized();
            self.LocalPrediction.CurrentMoveX = SurvivorMath.Clamp(moveX, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale);
            self.LocalPrediction.CurrentMoveY = SurvivorMath.Clamp(moveY, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale);
            if (deltaTime < 0f)
            {
                deltaTime = 0f;
            }
            else if (deltaTime > MaxPredictionDeltaSeconds)
            {
                deltaTime = MaxPredictionDeltaSeconds;
            }

            self.LocalPrediction.AdvancePresentation(deltaTime, player.MovePerTick());
            self.LocalPrediction.InputAccumulator += deltaTime;
            int predictedTicks = 0;
            while (self.LocalPrediction.InputAccumulator >= SurvivorLocalPlayerPrediction.InputIntervalSeconds && predictedTicks < MaxPredictionTicksPerFrame)
            {
                self.InputSequence++;
                self.LocalPrediction.RecordInput(self.InputSequence, self.LocalPrediction.CurrentMoveX, self.LocalPrediction.CurrentMoveY, player.MovePerTick());
                self.SendInputFrame(self.InputSequence, self.LocalPrediction.CurrentMoveX, self.LocalPrediction.CurrentMoveY);
                self.LocalPrediction.InputAccumulator -= SurvivorLocalPlayerPrediction.InputIntervalSeconds;
                predictedTicks++;
            }
        }

        private static void SendInputFrame(this SurvivorClientComponent self, long inputSequence, int moveX, int moveY)
        {
            C2G_SurvivorInput inputMessage = C2G_SurvivorInput.Create(true);
            inputMessage.InputSequence = inputSequence;
            inputMessage.MoveX = moveX;
            inputMessage.MoveY = moveY;
            self.ClientSender.Send(inputMessage);
        }

        public static void EnsureLocalPredictionInitialized(this SurvivorClientComponent self)
        {
            SurvivorPlayerState player = self.LocalPlayer;
            if (player == null || self.LocalPrediction.IsInitialized)
            {
                return;
            }

            self.LocalPrediction.Initialize(player.PositionX, player.PositionY);
            if (self.InputSequence < player.LastInputSequence)
            {
                self.InputSequence = player.LastInputSequence;
            }
        }

        public static void ReconcileLocalPrediction(this SurvivorClientComponent self)
        {
            SurvivorPlayerState player = self.LocalPlayer;
            if (player == null)
            {
                return;
            }

            if (!self.LocalPrediction.IsInitialized)
            {
                self.LocalPrediction.Initialize(player.PositionX, player.PositionY);
                if (self.InputSequence < player.LastInputSequence)
                {
                    self.InputSequence = player.LastInputSequence;
                }

                return;
            }

            self.LocalPrediction.Reconcile(player.PositionX, player.PositionY, player.LastInputSequence, player.MovePerTick());
        }

        public static void ApplyStateFrame(this SurvivorClientComponent self, long sequence, bool isFull, byte[] payload)
        {
            if (sequence <= self.LastSequence)
            {
                return;
            }

            if (!self.HasBaseline && !isFull)
            {
                self.ClientSender.Send(C2G_SurvivorRequestFullSnapshot.Create(true));
                return;
            }

            if (self.HasBaseline && !isFull && sequence != self.LastSequence + 1)
            {
                self.HasBaseline = false;
                self.ClientSender.Send(C2G_SurvivorRequestFullSnapshot.Create(true));
                return;
            }

            self.WorldComponent.ApplySnapshot(payload);
            self.LastSequence = sequence;
            self.HasBaseline = true;
            self.ReconcileLocalPrediction();
        }

        public static void PrepareSnapshotConsumer(this SurvivorClientComponent self, string roomCode)
        {
            if (self.Root().GetComponent<SurvivorRoom>() != null)
            {
                self.Root().RemoveComponent<SurvivorRoom>();
            }

            SurvivorRoom room = self.Root().AddComponent<SurvivorRoom, SceneType, string>(SceneType.SurvivorClient, roomCode);
            self.Room = room;
            self.World = room.AddComponent<SurvivorWorldComponent, SurvivorWorldRole, string>(SurvivorWorldRole.SnapshotConsumer, roomCode);
            self.LastSequence = 0;
            self.InputSequence = 0;
            self.HasBaseline = false;
            self.LocalPrediction.Reset();
            self.GetComponent<SurvivorViewEntityManagerComponent>().WorldGeneration++;
        }
    }
}
