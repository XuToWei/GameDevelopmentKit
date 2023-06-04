using CodeBind;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (UGFUIHelp))]
    public static partial class UGFUIHelpSystem
    {
        [EntitySystem]
        private class UGFUIHelpAwakeSystem : AwakeSystem<UGFUIHelp, Transform>
        {
             protected override void Awake(UGFUIHelp self, Transform uiTransform)
             {
                 self.InitBind(uiTransform);
             }
        }
        
        [EntitySystem]
        private class UGFUIHelpDestroySystem : DestroySystem<UGFUIHelp>
        {
            protected override void Destroy(UGFUIHelp self)
            {
                self.ClearBind();
            }
        }
    }
}
