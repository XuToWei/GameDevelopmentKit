namespace ET.Client
{
    [ChildOf(typeof(SurvivorClientComponent))]
    public sealed class SurvivorPlayerEntry: Entity, IAwake
    {
    }

    [ChildOf(typeof(SurvivorClientComponent))]
    public sealed class SurvivorMonsterEntry: Entity, IAwake
    {
    }

    [ChildOf(typeof(SurvivorClientComponent))]
    public sealed class SurvivorProjectileEntry: Entity, IAwake
    {
    }

    [ChildOf(typeof(SurvivorClientComponent))]
    public sealed class SurvivorPickupEntry: Entity, IAwake
    {
    }
}
