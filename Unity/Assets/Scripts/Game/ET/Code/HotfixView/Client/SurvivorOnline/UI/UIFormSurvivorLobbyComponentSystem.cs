using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormSurvivorLobbyComponent))]
    public static partial class UIFormSurvivorLobbyComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormSurvivorLobbyComponent self)
        {
            self.View.JoinButton.SetAsync(self.JoinRoom);
            self.View.StartButton.SetAsync(self.StartGame);
            self.View.StartButton.gameObject.SetActive(false);
            self.View.StatusText.text = "输入房间号加入；不存在的房间会自动创建";
            self.Observer = new SurvivorLobbyReactiveObserver(
                self,
                self.Root().GetComponent<SurvivorClientComponent>(),
                new SurvivorLobbyReactionSink());
            self.Observer.ResetChanges();
            self.Root()
                    .GetComponent<SurvivorClientComponent>()
                    .RegisterPresentationObserver(self.Observer);
            self.Observer.ObserveChanges();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorLobbyComponent self, bool isShutdown)
        {
            self.Root()
                    .GetComponent<SurvivorClientComponent>()
                    .UnregisterPresentationObserver(self.Observer);
            self.Observer.ResetChanges();
            self.Observer = null;
        }

        public static async UniTask JoinRoom(this UIFormSurvivorLobbyComponent self)
        {
            if (self.Root().GetComponent<SurvivorClientComponent>() == null)
            {
                self.Root().AddComponent<SurvivorClientComponent>();
            }

            await self.Root()
                    .GetComponent<SurvivorClientComponent>()
                    .JoinRoom(self.View.RoomCodeInput.text);
            if (self.Root().GetComponent<SurvivorClientComponent>().Runtime.JoinResponse.Error !=
                ErrorCode.ERR_Success)
            {
                self.View.StatusText.text =
                        self.Root().GetComponent<SurvivorClientComponent>().Runtime.JoinResponse.Message;
                return;
            }

            if (self.Root().GetComponent<SurvivorClientComponent>().Runtime.JoinResponse.Error ==
                ErrorCode.ERR_Success)
            {
                await self.Root()
                        .GetComponent<UIComponent>()
                        .AddUIFormComponentAsync<UIFormSurvivorHudComponent>(UGFUIFormId.SurvivorHud);
            }
        }

        public static async UniTask StartGame(this UIFormSurvivorLobbyComponent self)
        {
            await self.Root().GetComponent<SurvivorClientComponent>().StartGame();
            if (self.Root().GetComponent<SurvivorClientComponent>().Runtime.StartResponse.Error !=
                ErrorCode.ERR_Success)
            {
                self.View.StatusText.text =
                        self.Root().GetComponent<SurvivorClientComponent>().Runtime.StartResponse.Message;
            }
        }
    }
}
