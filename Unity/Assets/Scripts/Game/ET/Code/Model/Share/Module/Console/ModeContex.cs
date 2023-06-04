namespace ET
{
    [FriendOf(typeof(ModeContex))]
    public static partial class ModeContexSystem
    {
        [EntitySystem]
        private class ModeContexAwakeSystem : AwakeSystem<ModeContex>
        {
            protected override void Awake(ModeContex self)
            {
                self.Mode = "";
            }
        }

        [EntitySystem]
        private class ModeContexDestroySystem : DestroySystem<ModeContex>
        {
            protected override void Destroy(ModeContex self)
            {
                self.Mode = "";
            }
        }
    }
    


    [ComponentOf(typeof(ConsoleComponent))]
    public class ModeContex: Entity, IAwake, IDestroy
    {
        public string Mode = "";
    }
}