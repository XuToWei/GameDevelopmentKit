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
        private sealed class InputGyroscopeInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Gyroscope Information</b>");
                GUILayout.BeginVertical("box");
                {
                    var gyroscope = UnityEngine.InputSystem.Gyroscope.current;
                    DrawItem("Gyroscope Present", (gyroscope != null).ToString());

                    if (gyroscope != null)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                            {
                                InputSystem.EnableDevice(gyroscope);
                            }
                            if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                            {
                                InputSystem.DisableDevice(gyroscope);
                            }
                        }
                        GUILayout.EndHorizontal();

                        DrawItem("Enabled", gyroscope.enabled.ToString());
                        if (gyroscope.enabled)
                        {
                            DrawItem("Angular Velocity", gyroscope.angularVelocity.ReadValue().ToString());
                            DrawItem("Sampling Frequency", gyroscope.samplingFrequency.ToString());
                        }
                    }
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Attitude Sensor</b>");
                GUILayout.BeginVertical("box");
                {
                    var attitude = AttitudeSensor.current;
                    DrawItem("Attitude Sensor Present", (attitude != null).ToString());

                    if (attitude != null)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                            {
                                InputSystem.EnableDevice(attitude);
                            }
                            if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                            {
                                InputSystem.DisableDevice(attitude);
                            }
                        }
                        GUILayout.EndHorizontal();

                        DrawItem("Enabled", attitude.enabled.ToString());
                        if (attitude.enabled)
                        {
                            DrawItem("Attitude", attitude.attitude.ReadValue().eulerAngles.ToString());
                            DrawItem("Sampling Frequency", attitude.samplingFrequency.ToString());
                        }
                    }
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Gravity Sensor</b>");
                GUILayout.BeginVertical("box");
                {
                    var gravity = GravitySensor.current;
                    DrawItem("Gravity Sensor Present", (gravity != null).ToString());

                    if (gravity != null)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                            {
                                InputSystem.EnableDevice(gravity);
                            }
                            if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                            {
                                InputSystem.DisableDevice(gravity);
                            }
                        }
                        GUILayout.EndHorizontal();

                        DrawItem("Enabled", gravity.enabled.ToString());
                        if (gravity.enabled)
                        {
                            DrawItem("Gravity", gravity.gravity.ReadValue().ToString());
                            DrawItem("Sampling Frequency", gravity.samplingFrequency.ToString());
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
