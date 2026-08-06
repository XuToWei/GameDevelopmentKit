using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormSurvivorGameOverComponent))]
    public static partial class UIFormSurvivorGameOverComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIFormSurvivorGameOverComponent self)
        {
            self.Client = self.Root().GetComponent<SurvivorClientComponent>();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormSurvivorGameOverComponent self)
        {
            SurvivorWorldComponent world = self.Client.World;
            self.RoomCode = world.Data.RoomCode;
            self.Returning = false;
            self.View.ReturnToRoomButton.SetAsync(self.ReturnToRoom);
            self.View.StatusUXText.text = self.RoomCode.Length > 0
                    ? $"返回房间 {self.RoomCode}，可由房主再次开始"
                    : "没有可返回的房间";
            self.View.ReturnToRoomButton.interactable = self.RoomCode.Length > 0;
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorGameOverComponent self, bool isShutdown)
        {
            self.RoomCode = string.Empty;
            self.Returning = false;
        }

        public static async UniTask ReturnToRoom(this UIFormSurvivorGameOverComponent self)
        {
            if (self.Returning || self.RoomCode.Length == 0)
            {
                return;
            }

            self.Returning = true;
            self.View.ReturnToRoomButton.interactable = false;
            self.View.StatusUXText.text = $"正在返回房间 {self.RoomCode}...";
            G2C_SurvivorJoinRoom response = await self.Client.JoinRoom(self.RoomCode);
            if (response.Error != ErrorCode.ERR_Success)
            {
                self.Returning = false;
                self.View.ReturnToRoomButton.interactable = true;
                self.View.StatusUXText.text = response.Message;
                return;
            }

            UIComponent uiComponent = self.Root().GetComponent<UIComponent>();
            if (uiComponent.GetComponent<UIFormSurvivorLobbyComponent>() == null)
            {
                await uiComponent.AddUIFormComponentAsync<UIFormSurvivorLobbyComponent>(
                    UGFUIFormId.SurvivorLobby);
            }

            if (uiComponent.GetComponent<UIFormSurvivorGameOverComponent>() != null)
            {
                uiComponent.RemoveComponent<UIFormSurvivorGameOverComponent>();
            }
        }
    }
}
