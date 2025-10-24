using CodeBind;
using System.IO;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFUILSLobbyComponent))]
    [FriendOf(typeof(UGFUILSLobbyComponent))]
    public static partial class UGFUILSLobbyComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnInit(this UGFUILSLobbyComponent self)
        {
            self.EnterMapButton.SetAsync(self.EnterMap);
            self.ReplayButton.Set(self.Replay);
        }

        public static async UniTask EnterMap(this UGFUILSLobbyComponent self)
        {
            await EnterMapHelper.Match(self.Fiber());
        }

        public static void Replay(this UGFUILSLobbyComponent self)
        {
            byte[] bytes = File.ReadAllBytes(self.ReplayPathInputField.text);
            
            Replay replay = MemoryPackHelper.Deserialize(typeof (Replay), bytes, 0, bytes.Length) as Replay;
            Log.Debug($"start replay: {replay.Snapshots.Count} {replay.FrameInputs.Count} {replay.UnitInfos.Count}");
            LSSceneChangeHelper.SceneChangeToReplay(self.Root(), replay).Forget();
        }
    }
}
