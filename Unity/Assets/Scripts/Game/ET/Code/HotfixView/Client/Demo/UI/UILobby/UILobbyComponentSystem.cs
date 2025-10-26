using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UILobbyComponent))]
    [FriendOf(typeof(UILobbyComponent))]
    public static partial class UILobbyComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UILobbyComponent self)
        {
            self.View = (ETUILobby)self.ETMono;
            self.View.EnterMapButton.SetAsync(self.EnterMap);
        }

        private static async UniTask EnterMap(this UILobbyComponent self)
        {
            Scene root = self.Root();
            await EnterMapHelper.EnterMapAsync(root);
            self.Dispose();
        }
    }
}
