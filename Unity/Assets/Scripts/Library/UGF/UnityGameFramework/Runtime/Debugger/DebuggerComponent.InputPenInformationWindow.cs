using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputPenInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Pen Information</b>");
                GUILayout.BeginVertical("box");
                {
                    var pen = Pen.current;
                    DrawItem("Pen Present", (pen != null).ToString());

                    if (pen != null)
                    {
                        DrawItem("Name", pen.name);
                        DrawItem("Display Name", pen.displayName);
                        DrawItem("Enabled", pen.enabled.ToString());
                        DrawItem("Position", pen.position.ReadValue().ToString());
                        DrawItem("Delta", pen.delta.ReadValue().ToString());
                        DrawItem("Pressure", pen.pressure.ReadValue().ToString());
                        DrawItem("Twist", pen.twist.ReadValue().ToString());
                        DrawItem("Tilt", pen.tilt.ReadValue().ToString());
                        DrawItem("Tip Pressed", pen.tip.isPressed.ToString());
                        DrawItem("Eraser Pressed", pen.eraser.isPressed.ToString());
                        DrawItem("First Barrel Button Pressed", pen.firstBarrelButton.isPressed.ToString());
                        DrawItem("Second Barrel Button Pressed", pen.secondBarrelButton.isPressed.ToString());
                        DrawItem("In Range", pen.inRange.isPressed.ToString());
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
