using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [FriendOf(typeof(UIWidgetTest))]
    [EntitySystemOf(typeof(UIWidgetTest))]
    public static partial class UIWidgetTestSystem
    {
        [UGFUIWidgetSystem]
        private static void UGFUIWidgetOnInit(this UIWidgetTest self)
        {
            self.View.TestButton.SetAsync(self.OnClickTestButton);
            Log.Info("UIWidget测试OnInit");
        }
        
        [UGFUIWidgetSystem]
        private static void UGFUIWidgetOnOpen(this UIWidgetTest self)
        {
            self.View.TestUXText.text = "UIWidget测试成功!";
            Log.Info("UIWidget测试OnOpen");
        }
        
        private static async UniTask OnClickTestButton(this UIWidgetTest self)
        {
            Log.Info("点击了测试按钮");
            var scene = self.Root();
            scene.GetComponent<UIComponent>().RemoveComponent<UIFormLoginComponent>();
            Log.Info("测试登录界面关闭");
            await scene.GetComponent<UIComponent>().AddUIFormComponentAsync<UIFormLoginComponent>(UGFUIFormId.UILogin);
            Log.Info("测试登录界面打开");
        }
    }
}