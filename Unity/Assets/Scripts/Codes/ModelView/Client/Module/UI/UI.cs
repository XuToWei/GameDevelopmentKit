using UGF;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ET.Client
{
    [FriendOf(typeof (UI))]
    public static class UISystem
    {
        [ObjectSystem]
        public class UIAwakeSystem: AwakeSystem<UI, UIFormId, ETUIForm>
        {
            protected override void Awake(UI self, UIFormId uiFormId, ETUIForm uiForm)
            {
                self.UIFormId = uiFormId;
                self.UIForm = uiForm;
            }
        }

        [ObjectSystem]
        public class UIDestroySystem: DestroySystem<UI>
        {
            protected override void Destroy(UI self)
            {
                self.UIFormId = default;
                self.UIForm = default;
            }
        }
    }

    [ChildOf]
    public sealed class UI: Entity, IAwake<UIFormId, ETUIForm>, IDestroy
    {
        public UIFormId UIFormId { get; set; }
        public ETUIForm UIForm { get; set; }
    }
}