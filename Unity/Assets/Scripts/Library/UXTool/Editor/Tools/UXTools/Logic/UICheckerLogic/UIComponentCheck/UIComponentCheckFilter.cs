using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ThunderFireUITool
{
    public enum ComponentFilterType
    {
        ComponentExist, //检查Component是否挂载
        ComponentValue       //检查Component中变量的值
    }
    public enum FilterComponentExistOption
    {
        Exist,  //已经添加Component
        NotExist //没有添加Component
    }
    public enum FilterComponentValueOption
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan
    }

    public abstract class UIComponentCheckFilterBase
    {
        public static int FilterMinusBtnWidth = 30;
        public static int FilterLabelWidth = 100;
        public static int FiletrFirstColoumWidth = 250;
        public static int FiletrColoumWidth = 220;

        
        private UICommonScriptCheckWindow m_window;

        protected static string[] filterNumbericValueOptions = new string[] { "=", "!=", ">", "<" };
        protected static string[] filterUnNumbericValueOptions = new string[] { "=", "!="};
        protected static string[] filterExistOptions = new string[] { "Added", "Not Added" };

        public MonoScript m_selectMonoScript;
        public string m_selectMonoScriptGuid;
        public Type m_selectMonoScriptType;

        public ComponentFilterType m_FilterType;

        public Dictionary<string, FieldInfo> m_fieldDic;
        public int m_selectFieldIndex;
        public FieldInfo m_selectFieldInfo;

        public int m_selectOptionIndex;
        public string m_compareValue;

        public UIComponentCheckFilterBase(UICommonScriptCheckWindow window)
        {
            m_window = window;
            
        }

        public abstract bool Filt(GameObject go);

        public abstract void DrawFilterUI();


        protected void RemoveFilter()
        {
            m_window.RemoveFilter(this);
        }
        protected void UpdateComponentFieldList(MonoScript mono)
        {
            GetComponentFieldList(m_selectMonoScriptType);
        }
        protected void GetComponentFieldList(Type monoScrpit)
        {
            if (m_fieldDic == null)
            {
                m_fieldDic = new Dictionary<string, FieldInfo>();
            }
            else
            {
                m_fieldDic.Clear();
            }
            Type currentType = monoScrpit;
            while (currentType != null && currentType != typeof(object))
            {
                FieldInfo[] infos = currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                foreach (FieldInfo field in infos)
                {
                    if (!field.IsPublic)
                    {
                        object[] attr = field.GetCustomAttributes(typeof(SerializeField), true);
                        if (attr == null || attr.Length == 0)
                            continue;
                    }

                    string nickName = ObjectNames.NicifyVariableName(field.Name);
                    if(!m_fieldDic.ContainsKey(nickName))
                    {
                        m_fieldDic.Add(nickName, field);
                    }
                    
                }
                currentType = currentType.BaseType;
            }
            
            
        }


    }

    public class UIComponentExistCheckFilter : UIComponentCheckFilterBase
    {
        public UIComponentExistCheckFilter(UICommonScriptCheckWindow window) : base(window)
        {
            m_FilterType = ComponentFilterType.ComponentExist;
        }

        public override void DrawFilterUI()
        {
            EditorGUIUtility.labelWidth = FilterLabelWidth;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(FilterMinusBtnWidth)))
            {
                RemoveFilter();
            }

            EditorGUI.BeginChangeCheck();
            m_selectMonoScript = EditorGUILayout.ObjectField(UICommonScriptCheckWindow.CheckComponentString, m_selectMonoScript, typeof(MonoScript), true, GUILayout.Width(FiletrFirstColoumWidth)) as MonoScript;
            if (EditorGUI.EndChangeCheck())
            {
                if (m_selectMonoScript != null && (m_selectMonoScript.GetClass() == null || !m_selectMonoScript.GetClass().IsSubclassOf(typeof(MonoBehaviour))))
                {
                    Debug.LogWarning(UICommonScriptCheckWindow.FilterTipsString);
                    m_selectMonoScript = null;
                }
                if (m_selectMonoScript != null)
                {
                    long fid;
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_selectMonoScript, out m_selectMonoScriptGuid, out fid);
                    m_selectMonoScriptType = m_selectMonoScript.GetClass();
                }
            }
            if (m_selectMonoScript != null)
            {
                m_selectOptionIndex = EditorGUILayout.Popup(UICommonScriptCheckWindow.CompareOptionString, m_selectOptionIndex, filterExistOptions, GUILayout.Width(FiletrColoumWidth));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0;
        }

        public override bool Filt(GameObject go)
        {
            var scriptObject = go.GetComponent(m_selectMonoScriptType);

            bool result = false;
            if (scriptObject != null && (FilterComponentExistOption)m_selectOptionIndex == FilterComponentExistOption.Exist)
            {
                result = true;
            }
            else if (scriptObject == null && (FilterComponentExistOption)m_selectOptionIndex == FilterComponentExistOption.NotExist)
            {
                result = true;
            }
            return result;
        }
    }


    public class UIComponentValueCheckFilter : UIComponentCheckFilterBase
    {
        public UIComponentValueCheckFilter(UICommonScriptCheckWindow window) : base(window)
        {
            m_FilterType = ComponentFilterType.ComponentValue;
        }

        public override void DrawFilterUI()
        {
            EditorGUIUtility.labelWidth = FilterLabelWidth;
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("-", GUILayout.Width(FilterMinusBtnWidth)))
            {
                RemoveFilter();
            }

            EditorGUI.BeginChangeCheck();
            m_selectMonoScript = EditorGUILayout.ObjectField(UICommonScriptCheckWindow.CheckComponentString, m_selectMonoScript, typeof(MonoScript), true, GUILayout.Width(FiletrFirstColoumWidth)) as MonoScript;
            if (EditorGUI.EndChangeCheck())
            {
                if (m_selectMonoScript != null && (m_selectMonoScript.GetClass() == null || !m_selectMonoScript.GetClass().IsSubclassOf(typeof(MonoBehaviour))))
                {
                    Debug.LogWarning(UICommonScriptCheckWindow.FilterTipsString);
                    m_selectMonoScript = null;
                }
                if (m_selectMonoScript != null)
                {
                    long fid;
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_selectMonoScript, out m_selectMonoScriptGuid, out fid);
                    m_selectMonoScriptType = m_selectMonoScript.GetClass();
                    UpdateComponentFieldList(m_selectMonoScript);
                    m_selectFieldIndex = 0;
                }
            }
            if (m_selectMonoScript != null && m_fieldDic.Count > 0)
            {
                m_selectFieldIndex = EditorGUILayout.Popup(UICommonScriptCheckWindow.CheckFieldString, m_selectFieldIndex, m_fieldDic.Keys.ToArray(), GUILayout.Width(FiletrColoumWidth));
                m_selectFieldInfo = m_fieldDic[m_fieldDic.Keys.ToArray()[m_selectFieldIndex]];
                EditorGUIUtility.wideMode = true;
                m_selectOptionIndex = EditorGUILayout.Popup(UICommonScriptCheckWindow.CompareOptionString, m_selectOptionIndex, IsNumbericType(m_selectFieldInfo.FieldType) ? filterNumbericValueOptions : filterUnNumbericValueOptions, GUILayout.Width(FiletrColoumWidth));
                m_compareValue = UIComponentCheckFilterCompareValueDrawer.DrawFieldValueInput(m_compareValue, m_selectFieldInfo);
            }
            else
            {
                EditorGUILayout.LabelField(UICommonScriptCheckWindow.FilterTipsString);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0;
        }

        public override bool Filt(GameObject go)
        {
            var scriptObject = go.GetComponent(m_selectMonoScriptType);
            bool compareResult = false;
            if(string.IsNullOrEmpty(m_compareValue))
            {
                return compareResult;
            }

            if (scriptObject !=  null)
            {
                Transform transform = scriptObject.GetComponent<Transform>();

                var value = m_selectFieldInfo.GetValue(scriptObject);
                Type fieldType = m_selectFieldInfo.FieldType;

                if(IsNumbericType(fieldType))
                {
                    compareResult = CompareNumberic(value, m_compareValue, (FilterComponentValueOption)m_selectOptionIndex);
                }
                else
                {
                    compareResult = CompareUnNumeric(value, m_compareValue, fieldType, (FilterComponentValueOption)m_selectOptionIndex);
                }
                
            }
            return compareResult;
        }


        /// <summary>
        /// 非数值类型只能判断是否相等
        /// </summary>
        /// <returns></returns>
        public bool CompareUnNumeric(object fieldValue, object compareValue, Type fieldType, FilterComponentValueOption option)
        {
            bool result = false;
            string serializedValue = UIComponentCheckFilterCompareValueDrawer.SerializeFieldValue(fieldValue, fieldType);

            switch (option)
            {
                case FilterComponentValueOption.Equal:
                    result = serializedValue.Equals(compareValue.ToString());
                    break;
                case FilterComponentValueOption.NotEqual:
                    result = !serializedValue.Equals(compareValue.ToString());
                    break;
                default:
                    break;
            }
            return result;
        }


        /// <summary>
        /// 数值类型可以判断大小
        /// </summary>
        /// <returns></returns>
        /// 
        private bool CompareNumberic(object fieldValue, object compareValue, FilterComponentValueOption option)
        {
            if (fieldValue is int num1 && int.TryParse(compareValue.ToString(), out int num2))
            {
                return CompareValues(num1, num2, option);
            }
            if (fieldValue is long num1L && long.TryParse(compareValue.ToString(), out long num2L))
            {
                return CompareValues(num1L, num2L, option);
            }
            if (fieldValue is float num1F && float.TryParse(compareValue.ToString(), out float num2F))
            {
                return CompareValues(num1F, num2F, option);
            }
            if (fieldValue is double num1D && double.TryParse(compareValue.ToString(), out double num2D))
            {
                return CompareValues(num1D, num2D, option);
            }
            return false;
        }        

        private bool CompareValues<T>(T num1, T num2, FilterComponentValueOption option) where T : IComparable<T>
        {
            bool result = false;
            switch (option)
            {
                case FilterComponentValueOption.Equal:
                    result = num1.CompareTo(num2) == 0;
                    break;
                case FilterComponentValueOption.NotEqual:
                    result = num1.CompareTo(num2) != 0;
                    break;
                case FilterComponentValueOption.GreaterThan:
                    result = num1.CompareTo(num2) > 0;
                    break;
                case FilterComponentValueOption.LessThan:
                    result = num1.CompareTo(num2) < 0;
                    break;
                default:
                    break;
            }
            return result;
        }

        private bool IsNumbericType(Type type)
        {
            return type.IsPrimitive && type != typeof(bool) && type != typeof(char);
        }
    }
}
