using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputSensorInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Accelerometer</b>");
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

                GUILayout.Label("<b>Gyroscope</b>");
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

                GUILayout.Label("<b>Magnetic Field Sensor</b>");
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
