using GameFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputPeripheralInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Pen</b>");
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

                GUILayout.Label("<b>Joystick</b>");
                GUILayout.BeginVertical("box");
                {
                    var joystick = Joystick.current;
                    DrawItem("Joystick Present", (joystick != null).ToString());

                    if (joystick != null)
                    {
                        DrawItem("Name", joystick.name);
                        DrawItem("Display Name", joystick.displayName);
                        DrawItem("Enabled", joystick.enabled.ToString());
                        DrawItem("Stick", joystick.stick.ReadValue().ToString());
                        DrawItem("Trigger Pressed", joystick.trigger.isPressed.ToString());
                    }
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>All Joysticks</b>");
                GUILayout.BeginVertical("box");
                {
                    var allJoysticks = Joystick.all;
                    DrawItem("Joystick Count", allJoysticks.Count.ToString());

                    for (int i = 0; i < allJoysticks.Count; i++)
                    {
                        var js = allJoysticks[i];
                        DrawItem(Utility.Text.Format("Joystick [{0}]", i), Utility.Text.Format("{0} (ID: {1})", js.displayName, js.deviceId));
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
