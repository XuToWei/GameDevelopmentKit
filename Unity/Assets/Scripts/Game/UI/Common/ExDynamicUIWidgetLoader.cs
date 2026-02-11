using ET;
using GameFramework;

namespace Game
{
    public sealed class ExDynamicUIWidgetLoader : ADynamicUIWidgetLoader
    {
        protected override void OnUIWidgetLoaded(AUIWidget uiWidget)
        {
            AExUIWidget exUIWidget = uiWidget as AExUIWidget;
            if (exUIWidget == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("'{0}' is not AExUIWidget.", uiWidget.GetType().FullName));
            }
            AExUIWidget parentUIWidget = uiWidget.GetComponentInParent<AExUIWidget>();
            if (parentUIWidget != null)
            {
                parentUIWidget.AddUIWidget(exUIWidget);
                if (uiWidget.enabled)
                {
                    parentUIWidget.DynamicOpenUIWidget(exUIWidget);
                }
                return;
            }
            AExUIForm parentUIForm = uiWidget.GetComponentInParent<AExUIForm>();
            if (parentUIForm != null)
            {
                parentUIForm.AddUIWidget(exUIWidget);
                if (uiWidget.enabled)
                {
                    parentUIForm.DynamicOpenUIWidget(exUIWidget);
                }
                return;
            }
        }
    }
}
