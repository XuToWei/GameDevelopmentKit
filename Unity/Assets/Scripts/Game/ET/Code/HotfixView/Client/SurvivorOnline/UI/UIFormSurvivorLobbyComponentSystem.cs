using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormSurvivorLobbyComponent))]
    [ETReactiveSystem]
    public static partial class UIFormSurvivorLobbyComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIFormSurvivorLobbyComponent self)
        {
            self.Client = self.Root().GetComponent<SurvivorClientComponent>();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormSurvivorLobbyComponent self)
        {
            self.View.JoinButton.SetAsync(self.JoinRoom);
            self.View.StartButton.SetAsync(self.StartGame);
            self.StatusMessage = "输入房间号；不存在的房间将自动创建";
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnUpdate(this UIFormSurvivorLobbyComponent self, float elapseSeconds, float realElapseSeconds)
        {
            self.ObserveChanges();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorLobbyComponent self, bool isShutdown)
        {
            self.ResetReactive();
        }

        public static async UniTask JoinRoom(this UIFormSurvivorLobbyComponent self)
        {
            EntityRef<UIFormSurvivorLobbyComponent> selfRef = self;
            self.StatusMessage = string.Empty;
            SurvivorJoinRoomResult result = await self.Client.JoinRoom(self.View.RoomCodeInputField.text);
            self = selfRef;
            if (self == null || result.Success)
            {
                return;
            }

            self.StatusMessage = result.Message;
        }

        public static async UniTask StartGame(this UIFormSurvivorLobbyComponent self)
        {
            EntityRef<UIFormSurvivorLobbyComponent> selfRef = self;
            self.StatusMessage = string.Empty;
            SurvivorRequestResult result = await self.Client.StartGame();
            self = selfRef;
            if (self == null || result.Success)
            {
                return;
            }

            self.StatusMessage = result.Message;
        }

        [ETReactiveBind(nameof(UIFormSurvivorLobbyComponent.RoomCode))]
        private static void OnRoomCodeChanged(this UIFormSurvivorLobbyComponent self, string roomCode)
        {
            if (roomCode.Length > 0)
            {
                self.View.RoomCodeInputField.text = roomCode;
            }
        }

        [ETReactiveBind(nameof(UIFormSurvivorLobbyComponent.RoomCode), nameof(UIFormSurvivorLobbyComponent.Phase), nameof(UIFormSurvivorLobbyComponent.IsHost))]
        private static void OnStartAvailabilityChanged(this UIFormSurvivorLobbyComponent self, string roomCode, SurvivorRoomPhase phase, bool isHost)
        {
            self.View.StartGameObject.SetActive(roomCode.Length > 0 && isHost && phase == SurvivorRoomPhase.Lobby);
        }

        [ETReactiveBind(nameof(UIFormSurvivorLobbyComponent.RoomCode), nameof(UIFormSurvivorLobbyComponent.Phase), nameof(UIFormSurvivorLobbyComponent.StatusMessage))]
        private static void OnStatusChanged(this UIFormSurvivorLobbyComponent self, string roomCode, SurvivorRoomPhase phase, string statusMessage)
        {
            if (statusMessage.Length > 0)
            {
                self.View.StatusUXText.text = statusMessage;
                return;
            }

            if (roomCode.Length == 0)
            {
                self.View.StatusUXText.text = "输入房间号；不存在的房间将自动创建";
                return;
            }

            switch (phase)
            {
                case SurvivorRoomPhase.Lobby:
                    self.View.StatusUXText.text = $"已加入房间 {roomCode}";
                    break;
                case SurvivorRoomPhase.Running:
                    self.View.StatusUXText.text = "游戏开始";
                    break;
                case SurvivorRoomPhase.Ended:
                    self.View.StatusUXText.text = "游戏结束";
                    break;
            }
        }
    }
}
