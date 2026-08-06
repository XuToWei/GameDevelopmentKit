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
            self.RoomCode = self.Client.HasBaseline ? self.Client.WorldComponent.Data.RoomCode : string.Empty;
            self.Returning = false;
            self.View.ReturnToRoomButton.SetAsync(self.ReturnToRoom);
            self.View.StatusUXText.text = self.RoomCode.Length > 0 ? $"返回房间 {self.RoomCode}，可由房主再次开始" : "没有可返回的房间";
            self.View.ReturnToRoomButton.interactable = self.RoomCode.Length > 0;
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorGameOverComponent self, bool isShutdown)
        {
            self.RoomCode = string.Empty;
            self.Returning = false;
        }

        /// <summary>
        /// 成功后不再自己打开 Lobby、关闭自己：Phase 回到 Lobby 之后由 SurvivorViewComponent 统一切换。
        /// </summary>
        public static async UniTask ReturnToRoom(this UIFormSurvivorGameOverComponent self)
        {
            if (self.Returning || self.RoomCode.Length == 0)
            {
                return;
            }

            EntityRef<UIFormSurvivorGameOverComponent> selfRef = self;
            string roomCode = self.RoomCode;
            self.Returning = true;
            self.View.ReturnToRoomButton.interactable = false;
            self.View.StatusUXText.text = $"正在返回房间 {roomCode}...";
            SurvivorJoinRoomResult result = await self.Client.JoinRoom(roomCode);
            self = selfRef;
            if (self == null || result.Success)
            {
                return;
            }

            self.Returning = false;
            self.View.ReturnToRoomButton.interactable = true;
            self.View.StatusUXText.text = result.Message;
        }
    }
}
