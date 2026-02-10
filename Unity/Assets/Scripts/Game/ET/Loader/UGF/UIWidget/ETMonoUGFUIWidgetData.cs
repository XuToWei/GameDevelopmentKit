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

        public EntityRef<UGFUIWidget> ParentUGFUIWidget
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
            ParentUGFUIWidget = null;
            UGFUIWidget = null;
        }

        public static ETMonoUGFUIWidgetData Create(UGFUIForm ugfUIForm, UGFUIWidget parentUGFUIWidget, UGFUIWidget ugfUIWidget)
        {
            ETMonoUGFUIWidgetData formData = ReferencePool.Acquire<ETMonoUGFUIWidgetData>();
            formData.UGFUIForm = ugfUIForm;
            formData.ParentUGFUIWidget = parentUGFUIWidget;
            formData.UGFUIWidget = ugfUIWidget;
            return formData;
        }
    }
}
