namespace ET
{
    [EntitySystemOf(typeof(Unit))]
    public static partial class UnitSystem
    {
        [EntitySystem]
        private static void Awake(this Unit self, int configId)
        {
            self.ConfigId = configId;
        }
        
        [EntitySystem]
        private static void Update(this Unit self)
        {
            Log.Debug("XXX222");
        }

        public static DRUnitConfig Config(this Unit self)
        {
            return Tables.Instance.DTUnitConfig.Get(self.ConfigId);
        }
        
        public static UnitType Type(this Unit self)
        {
            return (UnitType)self.Config().Type;
        }
    }
}