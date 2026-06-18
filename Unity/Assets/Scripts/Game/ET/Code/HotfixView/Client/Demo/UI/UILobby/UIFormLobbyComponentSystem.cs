using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormLobbyComponent))]
    [FriendOf(typeof(UIFormLobbyComponent))]
    public static partial class UIFormLobbyComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormLobbyComponent self)
        {
            self.View.EnterMapButton.SetAsync(self.EnterMap);
        }

        private static async UniTask EnterMap(this UIFormLobbyComponent self)
        {
            Scene root = self.Root();
            await EnterMapHelper.EnterMapAsync(root);
            self.Dispose();
        }
    }
}
