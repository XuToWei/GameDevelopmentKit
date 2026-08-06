namespace ET.Client
{
    /// <summary>
    /// Survivor 表现层的 UI 栈编排宿主。Reactive 宿主放在 View 侧读取 Client 侧状态，
    /// 因此不需要从逻辑层 Publish 事件来跨层通知；开关界面的对象也不再是界面自己。
    /// </summary>
    [ComponentOf(typeof(SurvivorClientComponent))]
    public sealed partial class SurvivorViewComponent: Entity, IAwake, IUpdate, IDestroy, IETReactive
    {
        public SurvivorClientComponent Client { get; set; }

        /// <summary>UI 切换是异步的，用单飞标记避免多次 Bind 触发出现交错的 Add/Remove。</summary>
        public bool Switching { get; set; }

        [ETReactiveSource]
        public SurvivorRoomPhase Phase => this.Client.Phase;

        [ETReactiveSource]
        public bool SkillChoiceAvailable => this.Client.SkillChoiceAvailable;
    }
}
