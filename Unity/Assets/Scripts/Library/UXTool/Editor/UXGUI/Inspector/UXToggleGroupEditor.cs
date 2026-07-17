using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UXToggleGroup))]
    [CanEditMultipleObjects]
    public sealed class UXToggleGroupEditor : Editor
    {
        private const string UndoName = "Assign Child Toggles To UXToggleGroup";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            if (GUILayout.Button("Assign All Child Toggles"))
            {
                Undo.IncrementCurrentGroup();
                int undoGroup = Undo.GetCurrentGroup();
                Undo.SetCurrentGroupName(UndoName);

                foreach (Object currentTarget in targets)
                {
                    AssignChildToggles((UXToggleGroup)currentTarget);
                }

                Undo.CollapseUndoOperations(undoGroup);
            }
        }

        internal static int AssignChildToggles(UXToggleGroup group)
        {
            List<Toggle> changedToggles = new List<Toggle>();
            group.GetComponentsInChildren(true, changedToggles);
            for (int i = changedToggles.Count - 1; i >= 0; i--)
            {
                if (changedToggles[i].group == group)
                {
                    changedToggles.RemoveAt(i);
                }
            }

            if (changedToggles.Count == 0)
            {
                return 0;
            }

            HashSet<Toggle> affectedToggles = new HashSet<Toggle>(changedToggles);
            foreach (Toggle activeToggle in group.ActiveToggles())
            {
                affectedToggles.Add(activeToggle);
            }

            foreach (Toggle toggle in affectedToggles)
            {
                Undo.RegisterCompleteObjectUndo(toggle, UndoName);
                if (toggle.graphic != null)
                {
                    Undo.RegisterCompleteObjectUndo(toggle.graphic.canvasRenderer, UndoName);
                }
            }

            foreach (Toggle toggle in changedToggles)
            {
                toggle.group = group;
                PrefabUtility.RecordPrefabInstancePropertyModifications(toggle);
                EditorUtility.SetDirty(toggle);
            }

            return changedToggles.Count;
        }
    }
}
