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

        /// <summary>本界面由 SurvivorViewComponent 在有可用技能点时打开，但世界切换时可能残留一帧观察。</summary>
        [ETReactiveSource]
        public long SkillChoiceRevision()
        {
            SurvivorPlayerState player = this.Client.LocalPlayer;
            return player == null ? 0 : player.SkillChoiceRevision;
        }
    }
}
