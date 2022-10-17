using System.Collections.Generic;
using System.Text.RegularExpressions;
using ReferenceBindTool.Runtime;
using UnityEditor;

namespace ReferenceBindTool.Editor
{
    public class RuleHelperInfo<T> where T : IRuleHelper
    {
        private string m_HelperTypeName;
        private string[] m_HelperTypeNames;
        private int m_HelperTypeNameIndex;
        private System.Func<string, string> m_SetRuleHelperFunc;
        private string m_FieldName;
        private string[] m_ExcludeTypeNames;

        public RuleHelperInfo(string name, string[] excludeTypeNames)
        {
            m_FieldName = name;
            m_HelperTypeNames = null;
            m_HelperTypeNameIndex = 0;
            m_ExcludeTypeNames = excludeTypeNames;
        }

        public void Init(string helperName, System.Func<string, string> setRuleHelper)
        {
            m_HelperTypeName = helperName;
            m_SetRuleHelperFunc = setRuleHelper;
        }

        public void Draw()
        {
            if (m_HelperTypeNames.Length == 0)
            {
                EditorGUILayout.HelpBox($"程序集中没有实现{typeof(T)}的类。 请检查。", MessageType.Error);
                return;
            }

            string displayName = FieldNameForDisplay(m_FieldName);
            int selectedIndex = EditorGUILayout.Popup($"{displayName} Helper",
                m_HelperTypeNameIndex, m_HelperTypeNames);
            if (selectedIndex != m_HelperTypeNameIndex)
            {
                m_HelperTypeNameIndex = selectedIndex;
                m_HelperTypeName = m_SetRuleHelperFunc.Invoke(m_HelperTypeNames[selectedIndex]);
            }
        }

        public void Refresh()
        {
            List<string> helperTypeNameList = new List<string>();

            helperTypeNameList.AddRange(RuleHelperUtility.GetHelperNames<T>());
            if (m_ExcludeTypeNames != null)
            {
                foreach (var t in m_ExcludeTypeNames)
                {
                    helperTypeNameList.Remove(t);
                }
            }

            m_HelperTypeNames = helperTypeNameList.ToArray();
            if (m_HelperTypeNames.Length == 0)
            {
                return;
            }

            m_HelperTypeNameIndex = 0;
            if (!string.IsNullOrEmpty(m_HelperTypeName))
            {
                m_HelperTypeNameIndex = helperTypeNameList.IndexOf(m_HelperTypeName);
                if (m_HelperTypeNameIndex < 0)
                {
                    m_HelperTypeNameIndex = 0;
                    m_HelperTypeName = m_SetRuleHelperFunc.Invoke(null);
                }
            }
        }

        private string FieldNameForDisplay(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return string.Empty;
            }

            string str = Regex.Replace(fieldName, @"^m_", string.Empty);
            str = Regex.Replace(str, @"((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", @" $1").TrimStart();
            return str;
        }
    }
}