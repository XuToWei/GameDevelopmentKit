using CodeBind;
using UnityEngine;

namespace Game
{
    [MonoBehaviourBinding]
    public sealed partial class RuntimeInspectorForm : AUIForm
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
                (orientation == ScreenOrientation.AutoRotation && Screen.width >= Screen.height))
            {
                RootLayoutStateGroup.CurrentStateName = RootLayoutStateName.Horizontal;
            }
        }
    }
}
