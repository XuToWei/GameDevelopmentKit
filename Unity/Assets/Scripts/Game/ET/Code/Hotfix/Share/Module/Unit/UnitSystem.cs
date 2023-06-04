namespace ET
{
    public static partial class UnitSystem
    {
        [EntitySystem]
        private class UnitAwakeSystem : AwakeSystem<Unit, int>
        {
            protected override void Awake(Unit self, int configId)
            {
                self.ConfigId = configId;
            }
        }
    }
}