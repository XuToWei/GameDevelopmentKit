using CodeBind;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFUIWidgetTest))]
    [FriendOf(typeof(UGFUIWidgetTest))]
    public static partial class UGFUITestSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUIWidgetTest self, Transform uiWidgetTransform)
        {
            self.InitBind(uiWidgetTransform);
        }

        [EntitySystem]
        private static void Destroy(this UGFUIWidgetTest self)
        {
            self.ClearBind();
        }
    }
}