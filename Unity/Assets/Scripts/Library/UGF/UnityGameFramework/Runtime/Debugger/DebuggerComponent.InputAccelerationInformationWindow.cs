//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputAccelerationInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Acceleration Information</b>");
                GUILayout.BeginVertical("box");
                {
                    var accelerometer = Accelerometer.current;
                    DrawItem("Accelerometer Present", (accelerometer != null).ToString());

                    if (accelerometer != null)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                            {
                                InputSystem.EnableDevice(accelerometer);
                            }
                            if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                            {
                                InputSystem.DisableDevice(accelerometer);
                            }
                        }
                        GUILayout.EndHorizontal();

                        DrawItem("Enabled", accelerometer.enabled.ToString());
                        if (accelerometer.enabled)
                        {
                            DrawItem("Acceleration", accelerometer.acceleration.ReadValue().ToString());
                            DrawItem("Sampling Frequency", accelerometer.samplingFrequency.ToString());
                        }
                    }
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Linear Acceleration Sensor</b>");
                GUILayout.BeginVertical("box");
                {
                    var linearAcceleration = LinearAccelerationSensor.current;
                    DrawItem("Linear Acceleration Sensor Present", (linearAcceleration != null).ToString());

                    if (linearAcceleration != null)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                            {
                                InputSystem.EnableDevice(linearAcceleration);
                            }
                            if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                            {
                                InputSystem.DisableDevice(linearAcceleration);
                            }
                        }
                        GUILayout.EndHorizontal();

                        DrawItem("Enabled", linearAcceleration.enabled.ToString());
                        if (linearAcceleration.enabled)
                        {
                            DrawItem("Acceleration", linearAcceleration.acceleration.ReadValue().ToString());
                            DrawItem("Sampling Frequency", linearAcceleration.samplingFrequency.ToString());
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
