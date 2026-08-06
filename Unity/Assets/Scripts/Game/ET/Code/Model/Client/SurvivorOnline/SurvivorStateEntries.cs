namespace ET.Client
{
    /// <summary>
    /// 每个被同步的世界对象在 View 侧的所有权节点，Id 即 StateId。
    /// State 由 SurvivorViewEntityManagerComponentSystem 在 Reconcile 时写入并刷新，
    /// 表现组件始终通过 Entry 实时读取，避免快照重建实例后拿到过期引用。
    /// </summary>
    [ChildOf(typeof(SurvivorClientComponent))]
    public sealed class SurvivorPlayerEntry: Entity, IAwake<SurvivorPlayerState>, IDestroy
    {
        public SurvivorPlayerState State { get; set; }
    }

    [ChildOf(typeof(SurvivorClientComponent))]
    public sealed class SurvivorMonsterEntry: Entity, IAwake<SurvivorMonsterState>, IDestroy
    {
        public SurvivorMonsterState State { get; set; }
    }

    [ChildOf(typeof(SurvivorClientComponent))]
    public sealed class SurvivorProjectileEntry: Entity, IAwake<SurvivorProjectileState>, IDestroy
    {
        public SurvivorProjectileState State { get; set; }
    }

    [ChildOf(typeof(SurvivorClientComponent))]
    public sealed class SurvivorPickupEntry: Entity, IAwake<SurvivorPickupState>, IDestroy
    {
        public SurvivorPickupState State { get; set; }
    }
}
