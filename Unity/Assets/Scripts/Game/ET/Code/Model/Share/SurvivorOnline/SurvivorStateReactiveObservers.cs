namespace ET
{
    /// <summary>
    /// Reactive 观察者不实现 IUpdate。它的 ObserveChanges 由 SurvivorWorldComponent 的权威 tick
    /// 在 SurvivorSimulationSystem.ObserveStateReactions 里显式调用，
    /// 保证"施加伤害"与"结算死亡/升级"处于同一个 tick。
    /// 注意：生成器对 Source 按成员名 ordinal 排序求值，声明顺序不代表观察顺序，
    /// 因此这两个 Source 的结算不允许互相依赖顺序——Experience 结算后按比例换算出的 Hp
    /// 恒定满足 0 &lt; Hp &lt;= MaxHp，无论先后都不会让 Hp 结算做出多余动作。
    /// </summary>
    [ChildOf(typeof(SurvivorWorldComponent))]
    public sealed partial class SurvivorPlayerStateReactiveObserver: Entity, IAwake<SurvivorPlayerState>, IDestroy, IETReactive
    {
        public SurvivorPlayerState State { get; set; }

        [ETReactiveSource]
        public int Experience => this.State.Experience;

        [ETReactiveSource]
        public int Hp => this.State.Hp;
    }

    [ChildOf(typeof(SurvivorWorldComponent))]
    public sealed partial class SurvivorMonsterStateReactiveObserver: Entity, IAwake<SurvivorMonsterState>, IDestroy, IETReactive
    {
        public SurvivorMonsterState State { get; set; }

        [ETReactiveSource]
        public int Hp => this.State.Hp;
    }
}
