namespace ET.Client
{
    [FriendOf(typeof (UIMainComponent))]
    public static class UIMainSystem
    {
        public class UIMainAwakeSystem : AwakeSystem<UIMainComponent, UIMainView>
        {
             protected override void Awake(UIMainComponent self, UIMainView uiView)
             {
                self.View = uiView;
             }
        }
        
        public class UIMainDestroySystem : DestroySystem<UIMainComponent>
        {
            protected override void Destroy(UIMainComponent self)
            {
                self.View = null;
            }
        }
    }
}
