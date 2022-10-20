namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UILoginComponent: Entity, IAwake<UILoginView>, IDestroy
    {
        public UILoginView View { get; set; }
    }
}
