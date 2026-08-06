namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public sealed partial class UIFormSurvivorGameOverComponent:
            UGFUIForm<MonoUIFormSurvivorGameOver>,
            IAwake,
            IUGFUIFormOnOpen,
            IUGFUIFormOnClose
    {
        public SurvivorClientComponent Client { get; set; }

        public string RoomCode { get; set; }

        public bool Returning { get; set; }
    }
}
