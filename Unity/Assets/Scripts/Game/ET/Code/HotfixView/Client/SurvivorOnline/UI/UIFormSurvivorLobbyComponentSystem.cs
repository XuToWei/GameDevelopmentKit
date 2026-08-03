using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormSurvivorLobbyComponent))]
    [ETReactiveSystem]
    public static partial class UIFormSurvivorLobbyComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormSurvivorLobbyComponent self)
        {
            self.Client = self.Root().GetComponent<SurvivorClientComponent>();
            self.View.JoinButton.SetAsync(self.JoinRoom);
            self.View.StartButton.SetAsync(self.StartGame);
            self.View.StartButton.gameObject.SetActive(false);
            self.View.StatusText.text = "输入房间号加入；不存在的房间会自动创建";
            self.ObserveChanges();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnUpdate(
            this UIFormSurvivorLobbyComponent self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.ObserveChanges();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorLobbyComponent self, bool isShutdown)
        {
            self.ClearReactive();
            self.Client = null;
        }

        public static async UniTask JoinRoom(this UIFormSurvivorLobbyComponent self)
        {
            if (self.Client == null)
            {
                self.Client = self.Root().AddComponent<SurvivorClientComponent>();
            }

            G2C_SurvivorJoinRoom response = await self.Client.JoinRoom(self.View.RoomCodeInput.text);
            if (response.Error != ErrorCode.ERR_Success)
            {
                self.View.StatusText.text = response.Message;
                return;
            }

            self.ObserveChanges();
            await self.Root()
                    .GetComponent<UIComponent>()
                    .AddUIFormComponentAsync<UIFormSurvivorHudComponent>(UGFUIFormId.SurvivorHud);
        }

        public static async UniTask StartGame(this UIFormSurvivorLobbyComponent self)
        {
            G2C_SurvivorStartGame response = await self.Client.StartGame();
            if (response.Error != ErrorCode.ERR_Success)
            {
                self.View.StatusText.text = response.Message;
            }
        }

        private static SurvivorWorldData WorldData(this UIFormSurvivorLobbyComponent self)
        {
            return self.Client?.World?.Data;
        }

        [ETReactiveSource]
        private static string RoomCode(this UIFormSurvivorLobbyComponent self)
        {
            return self.WorldData()?.RoomCode ?? string.Empty;
        }

        [ETReactiveSource]
        private static SurvivorRoomPhase Phase(this UIFormSurvivorLobbyComponent self)
        {
            return self.WorldData()?.Phase ?? SurvivorRoomPhase.Lobby;
        }

        [ETReactiveSource]
        private static bool IsHost(this UIFormSurvivorLobbyComponent self)
        {
            return self.Client?.IsHost ?? false;
        }

        [ETReactiveBind(nameof(RoomCode), nameof(Phase), nameof(IsHost))]
        private static void OnRoomChanged(
            this UIFormSurvivorLobbyComponent self,
            string roomCode,
            SurvivorRoomPhase phase,
            bool isHost)
        {
            self.View.StartButton.gameObject.SetActive(
                roomCode.Length > 0 &&
                isHost &&
                phase == SurvivorRoomPhase.Lobby);
            if (roomCode.Length == 0)
            {
                return;
            }

            self.View.StatusText.text = phase == SurvivorRoomPhase.Lobby
                    ? $"已加入房间 {roomCode}"
                    : phase == SurvivorRoomPhase.Running
                            ? "游戏开始"
                            : "游戏结束";
        }
    }
}
