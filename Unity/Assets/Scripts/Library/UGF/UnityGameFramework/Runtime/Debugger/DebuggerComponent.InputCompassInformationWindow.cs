using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputCompassInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Compass Information</b>");
                GUILayout.BeginVertical("box");
                {
                    var magneticField = MagneticFieldSensor.current;
                    DrawItem("Magnetic Field Sensor Present", (magneticField != null).ToString());

                    if (magneticField != null)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                            {
                                InputSystem.EnableDevice(magneticField);
                            }
                            if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                            {
                                InputSystem.DisableDevice(magneticField);
                            }
                        }
                        GUILayout.EndHorizontal();

                        DrawItem("Enabled", magneticField.enabled.ToString());
                        if (magneticField.enabled)
                        {
                            DrawItem("Magnetic Field", magneticField.magneticField.ReadValue().ToString());
                            DrawItem("Sampling Frequency", magneticField.samplingFrequency.ToString());
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
