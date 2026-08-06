namespace ET.Client
{
    [ChildOf(typeof(SurvivorCombatFeedbackComponent))]
    public sealed class SurvivorDamageNumberEntry: Entity, IAwake<int, float, float>
    {
        public int Damage { get; set; }

        public float PositionX { get; set; }

        public float PositionY { get; set; }
    }
}
