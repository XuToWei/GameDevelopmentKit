namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public sealed partial class UIFormSurvivorSkillChoiceComponent:
            UGFUIForm<MonoUIFormSurvivorSkillChoice>,
            IAwake,
            IUGFUIFormOnOpen,
            IUGFUIFormOnUpdate,
            IUGFUIFormOnClose,
            IETReactive
    {
        public SurvivorClientComponent Client { get; set; }

        public bool Choosing { get; set; }

        public SurvivorWorldComponent WorldComponent => this.Client.World;

        public SurvivorPlayerState LocalPlayerState => this.WorldComponent.Data.Players[this.Client.PlayerId];

        [ETReactiveSource]
        public long SkillChoiceRevision => this.LocalPlayerState.SkillChoiceRevision;
    }
}
