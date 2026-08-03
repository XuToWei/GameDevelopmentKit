namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public sealed partial class UIFormSurvivorLobbyComponent:
            UGFUIForm<MonoUIFormSurvivorLobby>,
            IAwake,
            IUGFUIFormOnOpen,
            IUGFUIFormOnUpdate,
            IUGFUIFormOnClose,
            IETReactiveHost
    {
        private EntityRef<SurvivorClientComponent> client;

        public SurvivorClientComponent Client
        {
            get
            {
                return this.client;
            }
            set
            {
                this.client = value;
            }
        }
    }

    [ComponentOf(typeof(UIComponent))]
    public sealed partial class UIFormSurvivorHudComponent:
            UGFUIForm<MonoUIFormSurvivorHud>,
            IAwake,
            IUGFUIFormOnOpen,
            IUGFUIFormOnUpdate,
            IUGFUIFormOnClose,
            IETReactiveHost
    {
        private EntityRef<SurvivorClientComponent> client;

        public SurvivorClientComponent Client
        {
            get
            {
                return this.client;
            }
            set
            {
                this.client = value;
            }
        }
    }
}
