#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using ThunderFireUnityEx;
using UnityEditor;
using UnityEngine;

namespace TF_TableList
{
    public class InlineEnumRowFilter<T> : InlineRowFilter<T> where T : Enum
    {
        private object value;
        private string[] names;
        private Array values;
        private int index;
        public override bool Success(object target)
        {
            return value.Equals(target);
        }

        public override bool IsValid => value != null;

        public override void Initialize()
        {
            names = Enum.GetNames(typeof(T)).Add("Disable");
            index = names.Length - 1;
            values = Enum.GetValues(typeof(T));
        }

        public override void OnDrawInline(Rect rect)
        {
            if (IsValid)
            {
                if (IsValid)
                {
                    EditorGUI.DrawRect(rect, Color.green);
                    EditorGUI.DrawRect(rect.Padding(1), SirenixGUIStyles.DarkEditorBackground);
                }
            }
            EditorGUI.BeginChangeCheck();
            index = SirenixEditorFields.Dropdown(rect.AlignCenterY(16), index, names);
            if (EditorGUI.EndChangeCheck())
            {
                Dirty = true;
                if (index != names.Length - 1)
                    value = values.GetValue(index);
                else
                {
                    value = null;
                }
            }
        }

        public override void Reset()
        {
            value = null;
            index = names.Length - 1;
            Dirty = true;
        }

        public override string GetPersistentValue()
        {
            return index.ToString();
        }

        public override void OnLoadPersistentValue(string valueStr)
        {
            if (int.TryParse(valueStr, out var id))
            {
                index = Mathf.Max(Mathf.Min(id, names.Length - 1), 0);
                if (index != names.Length - 1)
                    value = values.GetValue(index);
                Dirty = true;
            }
        }
    }
}
#endif