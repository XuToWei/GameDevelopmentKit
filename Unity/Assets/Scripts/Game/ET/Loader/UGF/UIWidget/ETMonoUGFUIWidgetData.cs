using GameFramework;

namespace ET
{
    public sealed class ETMonoUGFUIWidgetData : IReference
    {
        public EntityRef<UGFUIWidget> UGFUIWidget
        {
            get;
            private set;
        }

        public void Clear()
        {
            UGFUIWidget = null;
        }

        public static ETMonoUGFUIWidgetData Create(UGFUIWidget ugfUIWidget)
        {
            ETMonoUGFUIWidgetData formData = ReferencePool.Acquire<ETMonoUGFUIWidgetData>();
            formData.UGFUIWidget = ugfUIWidget;
            return formData;
        }
    }
}