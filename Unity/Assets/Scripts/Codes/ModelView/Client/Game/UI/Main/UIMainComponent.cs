namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIMainComponent: Entity, IAwake<UIMainView>, IDestroy
    {
        public UIMainView View { get; set; }
    }
}
