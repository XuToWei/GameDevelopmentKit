using CodeBind;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFUIHelpComponent))]
    [FriendOf(typeof(UGFUIHelpComponent))]
    public static partial class UGFUIHelpComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUIHelpComponent self, Transform uiTransform)
        {
            self.InitBind(uiTransform);
        }

        [EntitySystem]
        private static void Destroy(this UGFUIHelpComponent self)
        {
            self.ClearBind();
        }
    }
}
