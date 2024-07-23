using CodeBind;
using UnityEngine;

namespace Game
{
    [MonoCodeBind]
    public sealed partial class RuntimeInspectorForm : AExUGuiForm
    {
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            CloseButton.onClick.Set(Close);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ScreenOrientation orientation = Screen.orientation;
            if (orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight ||
                (orientation == ScreenOrientation.AutoRotation && (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)))
            {
                RootLayoutStateControllerData.SelectedName = RootLayoutStateName.Horizontal;
            }
        }
    }
}
