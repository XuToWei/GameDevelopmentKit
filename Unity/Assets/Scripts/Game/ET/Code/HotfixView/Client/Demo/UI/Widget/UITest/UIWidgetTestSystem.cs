namespace ET.Client
{
    [EntitySystemOf(typeof(UIWidgetTest))]
    [FriendOf(typeof(UIWidgetTest))]
    public static partial class UIWidgetTestSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIWidgetTest self)
        {
            self.Mono = (MonoUIWidgetTest)self.ETMono;
            self.Mono.TestTextUXText.text = "UIWidget测试成功!";
        }
    }
}