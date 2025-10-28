using System.IO;
using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormLSLobbyComponent))]
    [FriendOf(typeof(UIFormLSLobbyComponent))]
    public static partial class UIFormLSLobbyComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormLSLobbyComponent self)
        {
            self.Mono.EnterMapButton.SetAsync(self.EnterMap);
            self.Mono.ReplayButton.Set(self.Replay);
        }

        public static async UniTask EnterMap(this UIFormLSLobbyComponent self)
        {
            await EnterMapHelper.Match(self.Fiber());
        }

        public static void Replay(this UIFormLSLobbyComponent self)
        {
            byte[] bytes = File.ReadAllBytes(self.Mono.ReplayPathInputField.text);
            
            Replay replay = MemoryPackHelper.Deserialize(typeof (Replay), bytes, 0, bytes.Length) as Replay;
            Log.Debug($"start replay: {replay.Snapshots.Count} {replay.FrameInputs.Count} {replay.UnitInfos.Count}");
            LSSceneChangeHelper.SceneChangeToReplay(self.Root(), replay).Forget();
        }
    }
}
