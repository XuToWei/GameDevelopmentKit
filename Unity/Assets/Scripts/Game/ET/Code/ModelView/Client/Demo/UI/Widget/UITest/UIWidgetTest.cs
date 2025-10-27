namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UILoginComponent))]
    public partial class UIWidgetTest : UGFUIWidget<ETMonoWidgetTest>, IUGFUIWidgetOnOpen
    {
        public ETMonoWidgetTest EtMono;
    }
}