namespace ET.Server
{
    [FriendOf(typeof(Player))]
    public static partial class PlayerSystem
    {
        [EntitySystem]
        private class PlayerAwakeSystem : AwakeSystem<Player, string>
        {
            protected override void Awake(Player self, string a)
            {
                self.Account = a;
            }
        }
    }
}