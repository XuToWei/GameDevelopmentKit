using GameFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputActionInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Action Information</b>");

                var actionMaps = InputSystem.ListEnabledActions();
                DrawItem("Enabled Action Count", actionMaps.Count.ToString());

                for (int i = 0; i < actionMaps.Count; i++)
                {
                    var action = actionMaps[i];
                    GUILayout.Label(Utility.Text.Format("<b>Action [{0}]</b>", i));
                    GUILayout.BeginVertical("box");
                    {
                        DrawItem("Name", action.name);
                        DrawItem("Action Map", action.actionMap != null ? action.actionMap.name : "N/A");
                        DrawItem("Type", action.type.ToString());
                        DrawItem("Phase", action.phase.ToString());
                        DrawItem("Enabled", action.enabled.ToString());
                        DrawItem("Control Type", action.expectedControlType ?? "N/A");

                        var bindings = action.bindings;
                        for (int j = 0; j < bindings.Count; j++)
                        {
                            var binding = bindings[j];
                            DrawItem(Utility.Text.Format("  Binding [{0}]", j), Utility.Text.Format("{0} ({1})", binding.effectivePath, binding.groups ?? "N/A"));
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
        }
    }
}
