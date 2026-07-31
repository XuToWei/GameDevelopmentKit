using ReactiveBinding;

namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public sealed class UIFormSurvivorLobbyComponent:
            UGFUIForm<MonoUIFormSurvivorLobby>,
            IAwake,
            IUGFUIFormOnOpen,
            IUGFUIFormOnClose
    {
        public IReactiveObserver Observer { get; set; }
    }

    [ComponentOf(typeof(UIComponent))]
    public sealed class UIFormSurvivorHudComponent:
            UGFUIForm<MonoUIFormSurvivorHud>,
            IAwake,
            IUGFUIFormOnOpen,
            IUGFUIFormOnUpdate,
            IUGFUIFormOnClose
    {
        public IReactiveObserver Observer { get; set; }
    }
}
