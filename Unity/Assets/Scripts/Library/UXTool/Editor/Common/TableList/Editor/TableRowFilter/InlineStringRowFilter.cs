#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace TF_TableList
{
    public sealed class InlineStringRowFilter : InlineRowFilter<string>
    {
        public enum Mode
        {
            Normal,
            NormalMatchCase,
            Full,
            FullMatchCase,
            Regex,
            RegexMatchCase,
        }

        public string FilterStr = "";

        public Mode mode;

        private Regex pattern;

        private List<string> wordList = new List<string>();

        public void Clear()
        {
            FilterStr = "";
            Dirty = true;
        }

        public override bool IsValid => !string.IsNullOrEmpty(FilterStr);

        public override bool Success(object target)
        {
            if (target == null) return false;
            if (CheckString(target.ToString()))
                return true;
            if (target is IEnumerable enumerable)
            {
                foreach (var obj in enumerable)
                {
                    if (obj != null && CheckString(obj.ToString()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckString(string str)
        {
            if (mode == Mode.Normal)
            {
                return wordList.All(x => str.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (mode == Mode.NormalMatchCase)
            {
                return wordList.All(x => str.Contains(x));
            }
            else if (mode == Mode.Full)
            {
                return str == FilterStr;
            }
            else if (mode == Mode.FullMatchCase)
            {
                return str.Equals(FilterStr, StringComparison.OrdinalIgnoreCase);
            }
            else if (mode == Mode.RegexMatchCase)
            {
                return pattern.IsMatch(str);
            }
            else if (mode == Mode.Regex)
            {
                return pattern.IsMatch(str);
            }

            return false;
        }

        public void OnValueChanged()
        {
            Dirty = true;
            if (mode == Mode.Normal || mode == Mode.NormalMatchCase)
                wordList = FilterStr.Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            else if (mode == Mode.Regex)
            {
                pattern = new Regex(FilterStr, RegexOptions.IgnoreCase);
            }
            else if (mode == Mode.RegexMatchCase)
            {
                pattern = new Regex(FilterStr);
            }
        }

        [System.Serializable]
        private class PersistentValueHelper
        {
            public string str;
            public Mode mode;
            public bool enabled;
        }

        public override string GetPersistentValue()
        {
            return JsonUtility.ToJson(new PersistentValueHelper { str = FilterStr, mode = mode, enabled = IsEnable });
        }

        public override void OnLoadPersistentValue(string value)
        {
            var helper = JsonUtility.FromJson<PersistentValueHelper>(value);
            FilterStr = helper.str;
            mode = helper.mode;
            IsEnable = helper.enabled;
            OnValueChanged();
        }

        private static GUIStyle inlineTextStyle = new GUIStyle(EditorStyles.textField)
        {
            alignment = TextAnchor.MiddleLeft,
        };

        public override void OnDrawInline(Rect rect)
        {
            if (IsValid)
            {
                EditorGUI.DrawRect(rect, Color.green);
                EditorGUI.DrawRect(rect.Padding(1), SirenixGUIStyles.DarkEditorBackground);
            }

            var old = FilterStr;
            if (string.IsNullOrEmpty(FilterStr))
            {
                FilterStr = SirenixEditorFields.DelayedTextField(rect.Padding(1), GUIContent.none, FilterStr, inlineTextStyle);
            }
            else
            {
                var right = rect.AlignRight(16);
                rect.width = rect.width - 16;
                FilterStr = SirenixEditorFields.DelayedTextField(rect.Padding(1), GUIContent.none, FilterStr, inlineTextStyle);
                if (SirenixEditorGUI.IconButton(right, EditorIcons.X))
                {
                    Clear();
                }
            }

            if (FilterStr != old)
            {
                OnValueChanged();
            }
        }

        public override void Reset()
        {
            Clear();
        }
    }

    public abstract class InlineStringSearchRowFilter<T> : InlineRowFilter<T>
    {
        public string FilterStr = "";
        public List<string> wordList = new List<string>();

        public abstract bool Success(T target);
        public override bool Success(object target)
        {
            if (typeof(T).IsClass && target == null)
                return false;
            return Success((T)target);
        }

        public void Clear()
        {
            FilterStr = "";
            Dirty = true;
        }

        public override bool IsValid => !string.IsNullOrEmpty(FilterStr);

        public void OnValueChanged()
        {
            Dirty = true;
            wordList = FilterStr.Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        public override string GetPersistentValue()
        {
            return FilterStr;
        }

        public override void OnLoadPersistentValue(string value)
        {
            FilterStr = value;
            OnValueChanged();
        }

        private static GUIStyle inlineTextStyle = new GUIStyle(EditorStyles.textField)
        {
            alignment = TextAnchor.MiddleLeft,
        };

        public override void OnDrawInline(Rect rect)
        {
            if (IsValid)
            {
                EditorGUI.DrawRect(rect, Color.green);
                EditorGUI.DrawRect(rect.Padding(1), SirenixGUIStyles.DarkEditorBackground);
            }

            var old = FilterStr;
            if (string.IsNullOrEmpty(FilterStr))
            {
                FilterStr = SirenixEditorFields.DelayedTextField(rect.Padding(1), GUIContent.none, FilterStr, inlineTextStyle);
            }
            else
            {
                var right = rect.AlignRight(16);
                rect.width = rect.width - 16;
                FilterStr = SirenixEditorFields.DelayedTextField(rect.Padding(1), GUIContent.none, FilterStr, inlineTextStyle);
                if (SirenixEditorGUI.IconButton(right, EditorIcons.X))
                {
                    Clear();
                }
            }

            if (FilterStr != old)
            {
                OnValueChanged();
            }
        }

        public override void Reset()
        {
            Clear();
        }

    }

}
#endif