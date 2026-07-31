using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [EntitySystemOf(typeof(SurvivorSessionOperationComponent))]
    public static partial class SurvivorSessionOperationComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorSessionOperationComponent self)
        {
        }

        public static async UniTask Join(
            this SurvivorSessionOperationComponent self,
            C2G_SurvivorJoinRoom request,
            G2C_SurvivorJoinRoom response)
        {
            self.RoomCode = SurvivorRoomCodeUtility.Normalize(request.RoomCode);
            if (!SurvivorRoomCodeUtility.IsValid(self.RoomCode))
            {
                response.Error = ErrorCode.ERR_SurvivorInvalidRoomCode;
                response.Message = "房间号只能是 4-12 位大写字母或数字";
                return;
            }

            if (self.GetParent<Session>().GetComponent<SessionPlayerComponent>()?.Player == null)
            {
                response.Error = ErrorCode.ERR_SurvivorNotInRoom;
                response.Message = "请先登录 Gate";
                return;
            }

            if (self.GetParent<Session>().GetComponent<SessionPlayerComponent>().Player.GetComponent<SurvivorPlayerRoomComponent>() != null)
            {
                response.Error = ErrorCode.ERR_SurvivorAlreadyInRoom;
                response.Message = "玩家已经加入 Survivor 房间";
                return;
            }

            self.Directory =
                    self.Root().GetComponent<SurvivorRoomDirectoryComponent>() ??
                    self.Root().AddComponent<SurvivorRoomDirectoryComponent>();

            using (await self.Root().GetComponent<CoroutineLockComponent>().Wait(
                       SurvivorCoroutineLockType.RoomDirectory,
                       self.RoomCode.GetHashCode()))
            {
                if (!self.Directory.Runtime.Rooms.ContainsKey(self.RoomCode))
                {
                    self.FiberId = await FiberManager.Instance.Create(
                        SchedulerType.ThreadPool,
                        self.Root().Fiber().Zone,
                        SceneType.SurvivorRoom,
                        $"SurvivorRoom-{self.RoomCode}");
                    self.RoomActorId = new ActorId(self.Root().Fiber().Process, self.FiberId);
                    self.InitRequest = G2SurvivorRoom_Init.Create();
                    self.InitRequest.RoomCode = self.RoomCode;
                    self.InitResponse = (SurvivorRoom2G_Init)await self.Root()
                            .GetComponent<MessageSender>()
                            .Call(self.RoomActorId, self.InitRequest);
                    if (self.InitResponse.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = self.InitResponse.Error;
                        response.Message = self.InitResponse.Message;
                        return;
                    }

                    self.Directory.Runtime.Rooms.Add(self.RoomCode, self.RoomActorId);
                }
                else
                {
                    self.RoomActorId = self.Directory.Runtime.Rooms[self.RoomCode];
                }

                self.JoinRequest = G2SurvivorRoom_Join.Create();
                self.JoinRequest.PlayerId =
                        self.GetParent<Session>().GetComponent<SessionPlayerComponent>().Player.Id;
                self.JoinRequest.DisplayName =
                        self.GetParent<Session>().GetComponent<SessionPlayerComponent>().Player.Account;
                self.JoinResponse = (SurvivorRoom2G_Join)await self.Root()
                        .GetComponent<MessageSender>()
                        .Call(self.RoomActorId, self.JoinRequest);
                if (self.JoinResponse.Error != ErrorCode.ERR_Success)
                {
                    response.Error = self.JoinResponse.Error;
                    response.Message = self.JoinResponse.Message;
                    return;
                }

                self.GetParent<Session>()
                        .GetComponent<SessionPlayerComponent>()
                        .Player
                        .AddComponent<SurvivorPlayerRoomComponent>();
                self.GetParent<Session>()
                        .GetComponent<SessionPlayerComponent>()
                        .Player
                        .GetComponent<SurvivorPlayerRoomComponent>()
                        .RoomActorId = self.RoomActorId;
                self.GetParent<Session>()
                        .GetComponent<SessionPlayerComponent>()
                        .Player
                        .GetComponent<SurvivorPlayerRoomComponent>()
                        .RoomCode = self.RoomCode;

                response.RoomCode = self.RoomCode;
                response.PlayerId =
                        self.GetParent<Session>().GetComponent<SessionPlayerComponent>().Player.Id;
                response.IsHost = self.JoinResponse.IsHost;
                response.Sequence = self.JoinResponse.Sequence;
                response.ServerTick = self.JoinResponse.ServerTick;
                response.FullSnapshot = self.JoinResponse.FullSnapshot;
            }
        }

        public static async UniTask Start(
            this SurvivorSessionOperationComponent self,
            C2G_SurvivorStartGame request,
            G2C_SurvivorStartGame response)
        {
            if (self.GetParent<Session>().GetComponent<SessionPlayerComponent>()?.Player
                    ?.GetComponent<SurvivorPlayerRoomComponent>() == null)
            {
                response.Error = ErrorCode.ERR_SurvivorNotInRoom;
                response.Message = "尚未加入 Survivor 房间";
                return;
            }

            self.StartRequest = G2SurvivorRoom_Start.Create();
            self.StartRequest.PlayerId =
                    self.GetParent<Session>().GetComponent<SessionPlayerComponent>().Player.Id;
            self.StartResponse = (SurvivorRoom2G_Start)await self.Root()
                    .GetComponent<MessageSender>()
                    .Call(
                        self.GetParent<Session>()
                                .GetComponent<SessionPlayerComponent>()
                                .Player
                                .GetComponent<SurvivorPlayerRoomComponent>()
                                .RoomActorId,
                        self.StartRequest);
            response.Error = self.StartResponse.Error;
            response.Message = self.StartResponse.Message;
        }

        public static void ForwardInput(
            this SurvivorSessionOperationComponent self,
            C2G_SurvivorInput message)
        {
            if (self.GetParent<Session>().GetComponent<SessionPlayerComponent>()?.Player
                    ?.GetComponent<SurvivorPlayerRoomComponent>() == null)
            {
                return;
            }

            self.InputMessage = G2SurvivorRoom_Input.Create();
            self.InputMessage.PlayerId =
                    self.GetParent<Session>().GetComponent<SessionPlayerComponent>().Player.Id;
            self.InputMessage.InputSequence = message.InputSequence;
            self.InputMessage.MoveX = message.MoveX;
            self.InputMessage.MoveY = message.MoveY;
            self.Root()
                    .GetComponent<MessageSender>()
                    .Send(
                        self.GetParent<Session>()
                                .GetComponent<SessionPlayerComponent>()
                                .Player
                                .GetComponent<SurvivorPlayerRoomComponent>()
                                .RoomActorId,
                        self.InputMessage);
        }

        public static void RequestFullSnapshot(this SurvivorSessionOperationComponent self)
        {
            if (self.GetParent<Session>().GetComponent<SessionPlayerComponent>()?.Player
                    ?.GetComponent<SurvivorPlayerRoomComponent>() == null)
            {
                return;
            }

            self.FullSnapshotRequest = G2SurvivorRoom_RequestFullSnapshot.Create();
            self.FullSnapshotRequest.PlayerId =
                    self.GetParent<Session>().GetComponent<SessionPlayerComponent>().Player.Id;
            self.Root()
                    .GetComponent<MessageSender>()
                    .Send(
                        self.GetParent<Session>()
                                .GetComponent<SessionPlayerComponent>()
                                .Player
                                .GetComponent<SurvivorPlayerRoomComponent>()
                                .RoomActorId,
                        self.FullSnapshotRequest);
        }
    }
}
