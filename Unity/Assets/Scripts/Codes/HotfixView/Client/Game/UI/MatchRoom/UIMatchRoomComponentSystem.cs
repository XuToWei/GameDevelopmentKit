namespace ET.Client
{
    [FriendOf(typeof (UIMatchRoomComponent))]
    public static class UIMatchRoomComponentSystem
    {
        public class UIMatchRoomComponentAwakeSystem : AwakeSystem<UIMatchRoomComponent, MatchRoomForm>
        {
             protected override void Awake(UIMatchRoomComponent self, MatchRoomForm uiForm)
             {
                self.Form = uiForm as MatchRoomForm;
             }
        }
        
        public class UIMatchRoomComponentDestroySystem : DestroySystem<UIMatchRoomComponent>
        {
            protected override void Destroy(UIMatchRoomComponent self)
            {
                self.Form = null;
            }
        }
    }
}
