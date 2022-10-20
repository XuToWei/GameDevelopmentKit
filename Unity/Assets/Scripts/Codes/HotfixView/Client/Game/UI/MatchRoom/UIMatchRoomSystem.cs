namespace ET.Client
{
    [FriendOf(typeof (UIMatchRoomComponent))]
    public static class UIMatchRoomSystem
    {
        public class UIMatchRoomAwakeSystem : AwakeSystem<UIMatchRoomComponent, UIMatchRoomView>
        {
             protected override void Awake(UIMatchRoomComponent self, UIMatchRoomView uiView)
             {
                self.View = uiView;
             }
        }
        
        public class UIMatchRoomDestroySystem : DestroySystem<UIMatchRoomComponent>
        {
            protected override void Destroy(UIMatchRoomComponent self)
            {
                self.View = null;
            }
        }
    }
}
