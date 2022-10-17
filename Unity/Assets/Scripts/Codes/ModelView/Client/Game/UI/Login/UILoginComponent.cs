namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UILoginComponent: Entity, IAwake<LoginForm>, IDestroy
    {
        public LoginForm Form { get; set; }
    }
}
