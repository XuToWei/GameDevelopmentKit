namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIMainComponent: Entity, IAwake<MainForm>, IDestroy
    {
        public MainForm Form { get; set; }
    }
}
