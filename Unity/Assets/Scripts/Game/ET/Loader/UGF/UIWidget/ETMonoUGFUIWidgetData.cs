using GameFramework;

namespace ET
{
    public sealed class ETMonoUGFUIWidgetData : IReference
    {
        public EntityRef<UGFUIForm> UGFUIForm
        {
            get;
            private set;
        }

        public EntityRef<UGFUIWidget> UGFUIWidget
        {
            get;
            private set;
        }

        public void Clear()
        {
            UGFUIForm = null;
            UGFUIWidget = null;
        }

        public static ETMonoUGFUIWidgetData Create(UGFUIForm ugfUIForm, UGFUIWidget ugfUIWidget)
        {
            ETMonoUGFUIWidgetData formData = ReferencePool.Acquire<ETMonoUGFUIWidgetData>();
            formData.UGFUIForm = ugfUIForm;
            formData.UGFUIWidget = ugfUIWidget;
            return formData;
        }
    }
}