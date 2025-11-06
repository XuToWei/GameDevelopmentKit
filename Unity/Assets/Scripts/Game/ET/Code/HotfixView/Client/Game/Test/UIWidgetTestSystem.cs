namespace ET.Client
{
    [FriendOf(typeof(UIWidgetTest))]
    [EntitySystemOf(typeof(UIWidgetTest))]
    public static partial class UIWidgetTestSystem
    {
        [UGFUIWidgetSystem]
        private static void UGFUIWidgetOnOpen(this UIWidgetTest self)
        {
            self.View.TestTextUXText.text = "UIWidget测试成功!";
        }
    }
}