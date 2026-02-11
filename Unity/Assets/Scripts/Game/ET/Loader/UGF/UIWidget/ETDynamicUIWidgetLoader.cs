using Game;
using GameFramework;

namespace ET
{
    public sealed class ETDynamicUIWidgetLoader : ADynamicUIWidgetLoader
    {
        protected override void OnUIWidgetLoaded(AUIWidget uiWidget)
        {
            AETMonoUGFUIWidget monoUIWidget = uiWidget as AETMonoUGFUIWidget;
            if (monoUIWidget == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("'{0}' is not AETMonoUGFUIWidget.", uiWidget.GetType().FullName));
            }
            AETMonoUGFUIWidget parentUIWidget = uiWidget.GetComponentInParent<AETMonoUGFUIWidget>();
            if (parentUIWidget != null)
            {
                parentUIWidget.UGFUIForm.AddChildUIWidget(monoUIWidget);
                if (monoUIWidget.enabled)
                {
                    parentUIWidget.DynamicOpenUIWidget(monoUIWidget);
                }
                return;
            }
            AETMonoUGFUIForm parentUIForm = uiWidget.GetComponentInParent<AETMonoUGFUIForm>();
            if (parentUIForm != null)
            {
                parentUIForm.UGFUIForm.AddChildUIWidget(monoUIWidget);
                if (monoUIWidget.enabled)
                {
                    parentUIForm.DynamicOpenUIWidget(monoUIWidget);
                }
                return;
            }
        }
    }
}
