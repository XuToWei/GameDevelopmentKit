namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public sealed class UIFormSurvivorLoginComponent:
            UGFUIForm<MonoUIFormSurvivorLogin>,
            IAwake,
            IUGFUIFormOnOpen,
            IUGFUIFormOnClose
    {
    }
}
