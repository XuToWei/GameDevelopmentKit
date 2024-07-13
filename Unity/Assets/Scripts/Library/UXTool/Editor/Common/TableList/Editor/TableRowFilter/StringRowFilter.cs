#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TF_TableList
{
    public sealed class StringRowFilter : RowFilter
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
        [Delayed]
        [OnValueChanged("OnValueChanged")]
        [InlineButton("Clear", "清除")]
        [LabelText("@\"filter by \" + this.PropertyName")]
        [LabelWidth(100)]
        [HorizontalGroup]
        public string FilterStr = "";

        [HideLabel]
        [HorizontalGroup(55)]
        [OnValueChanged("OnValueChanged")]
        public Mode mode;

        private Regex pattern;

        private List<string> wordList = new List<string>();

        public StringRowFilter(string filterStr)
        {
            FilterStr = filterStr;
            OnValueChanged();
        }

        public StringRowFilter()
        {

        }

        public void Clear()
        {
            FilterStr = "";
            Dirty = true;
        }

        public override bool IsValid => !string.IsNullOrEmpty(FilterStr);

        public override bool Success(object target)
        {
            if (target == null) return false;
            var str = target.ToString();
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
    }
}
#endif