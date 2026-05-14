using GameFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputTouchInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Touch Information</b>");
                GUILayout.BeginVertical("box");
                {
                    DrawItem("Touchscreen Present", (Touchscreen.current != null).ToString());

                    if (Touchscreen.current != null)
                    {
                        DrawItem("Touchscreen Name", Touchscreen.current.name);
                        DrawItem("Touchscreen Display Name", Touchscreen.current.displayName);
                    }
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Enhanced Touch</b>");
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                        {
                            EnhancedTouchSupport.Enable();
                        }
                        if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                        {
                            EnhancedTouchSupport.Disable();
                        }
                    }
                    GUILayout.EndHorizontal();

                    DrawItem("Enhanced Touch Enabled", EnhancedTouchSupport.enabled.ToString());
                    if (EnhancedTouchSupport.enabled)
                    {
                        var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
                        DrawItem("Active Touch Count", activeTouches.Count.ToString());
                        for (int i = 0; i < activeTouches.Count; i++)
                        {
                            var touch = activeTouches[i];
                            DrawItem(Utility.Text.Format("Touch [{0}]", i), GetTouchString(touch));
                        }
                    }
                }
                GUILayout.EndVertical();
            }

            private string GetTouchString(UnityEngine.InputSystem.EnhancedTouch.Touch touch)
            {
                return Utility.Text.Format("{0}, {1}, {2}, {3}, {4}", touch.screenPosition, touch.delta, touch.pressure, touch.phase, touch.touchId);
            }
        }
    }
}
