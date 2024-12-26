using UnityEngine;

namespace ET.Client
{
    [UGFUIWidgetEvent]
    public class UGFUIWidgetTestEvent : AUGFUIWidgetEvent
    {
        public override void OnInit(UGFUIWidget uiWidget, object userData)
        {
            base.OnInit(uiWidget, userData);
            uiWidget.AddComponent<UGFUIWidgetTest, Transform>(uiWidget.Transform);
        }

        public override void OnOpen(UGFUIWidget uiWidget, object userData)
        {
            base.OnOpen(uiWidget, userData);
            UGFUIWidgetTest uiWidgetTest = uiWidget.GetComponent<UGFUIWidgetTest>();
            uiWidgetTest.TestTextUXText.text = "UIWidget测试成功!";
        }

        public override void OnReload(UGFUIWidget uiWidget)
        {
            base.OnReload(uiWidget);
            UGFUIWidgetTest uiWidgetTest = uiWidget.GetComponent<UGFUIWidgetTest>();
            uiWidgetTest.TestTextUXText.text = "UIWidget Reload测试成功!";
        }
    }
}