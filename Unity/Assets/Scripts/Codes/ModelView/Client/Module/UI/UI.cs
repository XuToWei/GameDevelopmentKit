using UGF;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ET.Client
{
    [FriendOf(typeof (UI))]
    public static class UISystem
    {
        [ObjectSystem]
        public class UIAwakeSystem: AwakeSystem<UI, UIFormId, UIForm>
        {
            protected override void Awake(UI self, UIFormId uiFormId, UIForm uiForm)
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
    public sealed class UI: Entity, IAwake<UIFormId, UIForm>, IDestroy
    {
        public UIFormId UIFormId { get; set; }
        public UIForm UIForm { get; set; }
    }
}