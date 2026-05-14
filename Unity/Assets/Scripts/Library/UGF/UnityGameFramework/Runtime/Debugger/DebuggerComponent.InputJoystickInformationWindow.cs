using GameFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputJoystickInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Joystick Information</b>");
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
