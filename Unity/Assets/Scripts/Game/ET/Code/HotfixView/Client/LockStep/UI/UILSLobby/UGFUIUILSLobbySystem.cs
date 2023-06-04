using CodeBind;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUILSLobby))]
    public static partial class UGFUIUILSLobbySystem
    {
        [EntitySystem]
        private class UGFUIUILSLobbyAwakeSystem : AwakeSystem<UGFUILSLobby, Transform>
        {
             protected override void Awake(UGFUILSLobby self, Transform uiTransform)
             {
                self.InitBind(uiTransform);
             }
        }
        
        [EntitySystem]
        private class UGFUIUILSLobbyDestroySystem : DestroySystem<UGFUILSLobby>
        {
            protected override void Destroy(UGFUILSLobby self)
            {
                self.ClearBind();
            }
        }
        
        public static async UniTask EnterMap(this UGFUILSLobby self)
        {
            await EnterMapHelper.Match(self.ClientScene());
        }
        
        public static void Replay(this UGFUILSLobby self)
        {
            byte[] bytes = File.ReadAllBytes(self.replayPathInputField.text);
            
            Replay replay = MemoryPackHelper.Deserialize(typeof (Replay), bytes, 0, bytes.Length) as Replay;
            Log.Debug($"start replay: {replay.Snapshots.Count} {replay.FrameInputs.Count} {replay.UnitInfos.Count}");
            LSSceneChangeHelper.SceneChangeToReplay(self.ClientScene(), replay).Forget();
        }
    }
}
