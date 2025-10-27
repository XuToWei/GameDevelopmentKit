using System.IO;
using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UILSLobbyComponent))]
    [FriendOf(typeof(UILSLobbyComponent))]
    public static partial class UILSLobbyComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UILSLobbyComponent self)
        {
            self.Mono.EnterMapButton.SetAsync(self.EnterMap);
            self.Mono.ReplayButton.Set(self.Replay);
        }

        public static async UniTask EnterMap(this UILSLobbyComponent self)
        {
            await EnterMapHelper.Match(self.Fiber());
        }

        public static void Replay(this UILSLobbyComponent self)
        {
            byte[] bytes = File.ReadAllBytes(self.Mono.ReplayPathInputField.text);
            
            Replay replay = MemoryPackHelper.Deserialize(typeof (Replay), bytes, 0, bytes.Length) as Replay;
            Log.Debug($"start replay: {replay.Snapshots.Count} {replay.FrameInputs.Count} {replay.UnitInfos.Count}");
            LSSceneChangeHelper.SceneChangeToReplay(self.Root(), replay).Forget();
        }
    }
}
