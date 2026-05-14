using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputSummaryInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input System Summary Information</b>");
                GUILayout.BeginVertical("box");
                {
                    DrawItem("Input System Version", InputSystem.version.ToString());
                    DrawItem("Update Mode", InputSystem.settings.updateMode.ToString());
                    DrawItem("Compensate For Screen Orientation", InputSystem.settings.compensateForScreenOrientation.ToString());
                    DrawItem("Default Dead Zone Min", InputSystem.settings.defaultDeadzoneMin.ToString());
                    DrawItem("Default Dead Zone Max", InputSystem.settings.defaultDeadzoneMax.ToString());
                    DrawItem("Default Button Press Point", InputSystem.settings.defaultButtonPressPoint.ToString());
                    DrawItem("Default Tap Time", InputSystem.settings.defaultTapTime.ToString());
                    DrawItem("Tap Radius", InputSystem.settings.tapRadius.ToString());
                    DrawItem("Multi Tap Delay Time", InputSystem.settings.multiTapDelayTime.ToString());
                    DrawItem("Background Behavior", InputSystem.settings.backgroundBehavior.ToString());
                    DrawItem("Editor Input Behavior In Play Mode", InputSystem.settings.editorInputBehaviorInPlayMode.ToString());
                    DrawItem("Device Count", InputSystem.devices.Count.ToString());
                    DrawItem("Disconnected Device Count", InputSystem.disconnectedDevices.Count.ToString());
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Metrics</b>");
                GUILayout.BeginVertical("box");
                {
                    var metrics = InputSystem.metrics;
                    DrawItem("Current Num Devices", metrics.currentNumDevices.ToString());
                    DrawItem("Max Num Devices", metrics.maxNumDevices.ToString());
                    DrawItem("Current State Size In Bytes", metrics.currentStateSizeInBytes.ToString());
                    DrawItem("Max State Size In Bytes", metrics.maxStateSizeInBytes.ToString());
                    DrawItem("Total Event Count", metrics.totalEventCount.ToString());
                    DrawItem("Total Event Bytes", metrics.totalEventBytes.ToString());
                    DrawItem("Total Update Count", metrics.totalUpdateCount.ToString());
                    DrawItem("Total Event Processing Time", metrics.totalEventProcessingTime.ToString());
                    DrawItem("Total Event Lag Time", metrics.totalEventLagTime.ToString());
                    DrawItem("Average Event Bytes Per Frame", metrics.averageEventBytesPerFrame.ToString());
                    DrawItem("Average Processing Time Per Event", metrics.averageProcessingTimePerEvent.ToString());
                    DrawItem("Average Lag Time Per Event", metrics.averageLagTimePerEvent.ToString());
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Mouse</b>");
                GUILayout.BeginVertical("box");
                {
                    if (Mouse.current != null)
                    {
                        DrawItem("Mouse Present", "True");
                        DrawItem("Mouse Position", Mouse.current.position.ReadValue().ToString());
                        DrawItem("Mouse Delta", Mouse.current.delta.ReadValue().ToString());
                        DrawItem("Mouse Scroll", Mouse.current.scroll.ReadValue().ToString());
                        DrawItem("Left Button Pressed", Mouse.current.leftButton.isPressed.ToString());
                        DrawItem("Right Button Pressed", Mouse.current.rightButton.isPressed.ToString());
                        DrawItem("Middle Button Pressed", Mouse.current.middleButton.isPressed.ToString());
                    }
                    else
                    {
                        DrawItem("Mouse Present", "False");
                    }
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Keyboard</b>");
                GUILayout.BeginVertical("box");
                {
                    if (Keyboard.current != null)
                    {
                        DrawItem("Keyboard Present", "True");
                        DrawItem("Any Key Pressed", Keyboard.current.anyKey.isPressed.ToString());
                    }
                    else
                    {
                        DrawItem("Keyboard Present", "False");
                    }
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Gamepad</b>");
                GUILayout.BeginVertical("box");
                {
                    if (Gamepad.current != null)
                    {
                        DrawItem("Gamepad Present", "True");
                        DrawItem("Gamepad Name", Gamepad.current.name);
                        DrawItem("Left Stick", Gamepad.current.leftStick.ReadValue().ToString());
                        DrawItem("Right Stick", Gamepad.current.rightStick.ReadValue().ToString());
                    }
                    else
                    {
                        DrawItem("Gamepad Present", "False");
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
