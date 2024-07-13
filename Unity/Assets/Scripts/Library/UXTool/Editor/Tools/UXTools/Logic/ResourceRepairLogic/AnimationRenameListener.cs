#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.SceneManagement;
#if !UNITY_2021_2_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif

namespace ThunderFireUITool
{
    [InitializeOnLoad]
    public class AnimationRenameListener
    {
        private const string NAME_PROPERTY = "m_Name";
        private const int MAX_SAVEDNAMES = 20;
        private static PrefabStage lastPrefabStage;
        public static Dictionary<string, List<string>> nameToPreviousNames;
        static AnimationRenameListener()
        {
            //EditorApplication.hierarchyChanged += OnHierarchyChange;
            Undo.postprocessModifications += CheckUndoEntries;
            nameToPreviousNames = new Dictionary<string, List<string>>();
        }

        private static UndoPropertyModification[] CheckUndoEntries(UndoPropertyModification[] modifications)
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                return modifications;
            }
            else if (prefabStage != lastPrefabStage)
            {
                lastPrefabStage = prefabStage;
                nameToPreviousNames.Clear();
            }
            for (int i = 0; i < modifications.Length; i++)
            {
                var modification = modifications[i];
                if (modification.currentValue.propertyPath == NAME_PROPERTY)
                {
                    //Debug.Log($"GameObject {modification.previousValue.value} was renamed to {modification.currentValue.value}");
                    AddToDict(modification.previousValue.value, modification.currentValue.value);
                }
            }
            return modifications;
        }
        private static void AddToDict(string previousValue, string newValue)
        {
            if (nameToPreviousNames.ContainsKey(previousValue))
            {
                var values = nameToPreviousNames[previousValue];
                values.Add(previousValue);
                if (values.Count > MAX_SAVEDNAMES)
                {
                    values.RemoveAt(0);
                }
                nameToPreviousNames.Add(newValue, values);
                nameToPreviousNames.Remove(previousValue);
            }
            else
            {
                List<string> values = new List<string>();
                values.Add(previousValue);
                if (nameToPreviousNames.ContainsKey(newValue))
                    nameToPreviousNames.Remove(newValue);
                nameToPreviousNames.Add(newValue, values);
            }
        }
        public static List<string> GetPreviousNames(string curName)
        {
            if (nameToPreviousNames.TryGetValue(curName, out var previousNames))
                return previousNames;
            else
                return new List<string>();
        }
    }
}
#endif