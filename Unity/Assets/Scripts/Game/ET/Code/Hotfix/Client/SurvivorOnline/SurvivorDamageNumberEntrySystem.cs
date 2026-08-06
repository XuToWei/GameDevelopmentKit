namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorDamageNumberEntry))]
    public static partial class SurvivorDamageNumberEntrySystem
    {
        [EntitySystem]
        private static void Awake(
            this SurvivorDamageNumberEntry self,
            int damage,
            float positionX,
            float positionY)
        {
            self.Damage = damage;
            self.PositionX = positionX;
            self.PositionY = positionY;
        }
    }
}
