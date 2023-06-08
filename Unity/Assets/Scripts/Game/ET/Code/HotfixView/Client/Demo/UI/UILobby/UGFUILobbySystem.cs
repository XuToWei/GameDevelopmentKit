using CodeBind;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUILobby))]
    public static partial class UGFUILobbySystem
    {
        [EntitySystem]
        private class UGFUILobbyAwakeSystem : AwakeSystem<UGFUILobby, Transform>
        {
             protected override void Awake(UGFUILobby self, Transform uiTransform)
             {
                self.InitBind(uiTransform);
             }
        }
        
        [EntitySystem]
        private class UGFUILobbyDestroySystem : DestroySystem<UGFUILobby>
        {
            protected override void Destroy(UGFUILobby self)
            {
                self.ClearBind();
            }
        }
        
        public static async UniTask EnterMap(this UGFUILobby self)
        {
            await EnterMapHelper.EnterMapAsync(self.ClientScene());
            self.DomainScene().GetComponent<UIComponent>().CloseUIForm(UGFUIFormId.UILobby);
        }
    }
}
