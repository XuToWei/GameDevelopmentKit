using CodeBind;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFUILobbyComponent))]
    [FriendOf(typeof(UGFUILobbyComponent))]
    public static partial class UGFUILobbyComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUILobbyComponent self, Transform uiTransform)
        {
            self.InitBind(uiTransform);
        }

        [EntitySystem]
        private static void Destroy(this UGFUILobbyComponent self)
        {
            self.ClearBind();
        }

        private static async UniTask EnterMap(this UGFUILobbyComponent self)
        {
            Scene root = self.Root();
            await EnterMapHelper.EnterMapAsync(root);
            root.GetComponent<UGFUIComponent>().CloseUIForm(UGFUIFormId.UILobby);
        }
    }
}
