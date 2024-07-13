#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace TF_TableList
{
    public class InlineBooleanRowFilter : InlineRowFilter<bool>
    {
        private bool enable;
        private bool value;

        public override bool Success(object target)
        {
            return value.Equals(target);
        }

        public override bool IsValid => enable;

        public override void Reset()
        {
            enable = false;
            value = false;
            Dirty = true;
        }

        public override string GetPersistentValue()
        {
            if (enable)
                return value ? "1" : "0";
            return "-1";
        }

        public override void OnLoadPersistentValue(string valueStr)
        {
            if (valueStr == "0")
            {
                enable = true;
                value = false;
            }
            else if (valueStr == "1")
            {
                enable = true;
                value = true;
            }
            else
            {
                enable = false;
                value = false;
            }
            Dirty = true;
        }

        public override void OnDrawInline(Rect rect)
        {
            if (IsValid)
            {
                EditorGUI.DrawRect(rect, Color.green);
                EditorGUI.DrawRect(rect.Padding(1), SirenixGUIStyles.DarkEditorBackground);
            }

            if (enable)
            {
                var right = rect.AlignRight(16);
                rect.width = rect.width - 16;
                EditorGUI.BeginChangeCheck();
                value = GUI.Toggle(rect, value, "");
                if (EditorGUI.EndChangeCheck())
                {
                    Dirty = true;
                    enable = true;
                }
                if (SirenixEditorGUI.IconButton(right, EditorIcons.X))
                {
                    Reset();
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                value = GUI.Toggle(rect, value, "");
                if (EditorGUI.EndChangeCheck())
                {
                    Dirty = true;
                    enable = true;
                }
            }
        }
    }
}
#endif