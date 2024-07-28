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
        public object m_compareValue;

        public UIComponentCheckFilterCompareValueDrawer checkValueField = new UIComponentCheckFilterCompareValueDrawer();

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
        public void UpdateComponentFieldList(MonoScript mono)
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
                m_compareValue = checkValueField.DrawFieldValueInput(m_compareValue, m_selectFieldInfo);
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

            if (scriptObject !=  null)
            {
                Type fieldType = m_selectFieldInfo.FieldType;
                var value = m_selectFieldInfo.GetValue(scriptObject);
                if(IsNumbericType(fieldType))
                {
                    compareResult = CompareNumberic(value, m_compareValue, fieldType, (FilterComponentValueOption)m_selectOptionIndex);
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
        public bool CompareUnNumeric(object fieldValue, object compareObject, Type fieldType, FilterComponentValueOption option)
        {
            bool result = false;
            var compareValue = Convert.ChangeType(compareObject, fieldType);
            switch (option)
            {
                case FilterComponentValueOption.Equal:    
                    if(fieldValue == null) 
                        result = compareValue == null;
                    else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        result = CheckList(fieldValue, compareValue, fieldType);
                    }
                    else if (fieldType == typeof(Color))
                    {
                        Color32 fieldColor32 = ((Color)fieldValue);
                        Color32 compareColor32 = ((Color)compareValue);
                        result = fieldColor32.Equals(compareColor32);
                    }
                    else
                    {
                        result = (fieldValue.Equals(compareValue));
                    }
                    
                    break;

                case FilterComponentValueOption.NotEqual:
                    if(fieldValue == null) 
                        result = !(compareValue == null);
                    else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        result = !(CheckList(fieldValue, compareValue, fieldType));
                    }
                    else if (fieldType == typeof(Color))
                    {
                        Color32 fieldColor32 = ((Color)fieldValue);
                        Color32 compareColor32 = ((Color)compareValue);
                        result = !(fieldColor32.Equals(compareColor32));
                    }
                    else
                        result = !(fieldValue.Equals(compareValue));                    
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
        private bool CompareNumberic(object fieldValue, object compareObject, Type fieldType, FilterComponentValueOption option)
        {
            if(compareObject == null)
            {
                return false;
            }
            var compareValue = Convert.ChangeType(compareObject, fieldType);
            if (fieldValue is int num1 && compareValue is int num2)
            {
                return CompareValues(num1, num2, option);
            }
            if (fieldValue is long num1L && compareValue is long num2L)
            {
                return CompareValues(num1L, num2L, option);
            }
            if (fieldValue is float num1F && compareValue is float num2F)
            {
                return CompareValues(num1F, num2F, option);
            }
            if (fieldValue is double num1D && compareValue is double num2D)
            {
                return CompareValues(num1D, num2D, option);
            }
            if (fieldValue is decimal num1M && compareValue is decimal num2M)
            {
                return CompareValues(num1M, num2M, option);
            }
            if (fieldValue is byte num1B && compareValue is byte num2B)
            {
                return CompareValues(num1B, num2B, option);
            }
            if (fieldValue is uint num1U && compareValue is uint num2U)
            {
                return CompareValues(num1U, num2U, option);
            }
            if (fieldValue is ulong num1UL && compareValue is ulong num2UL)
            {
                return CompareValues(num1UL, num2UL, option);
            }
            if (fieldValue is ushort num1US && compareValue is ushort num2US)
            {
                return CompareValues(num1US, num2US, option);
            }
            if (fieldValue is sbyte num1SB && compareValue is sbyte num2SB)
            {
                return CompareValues(num1SB, num2SB, option);
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

        private bool CheckList(object fieldValue, object compareValue, Type fieldType)
        {
            var listType = fieldType.GetGenericArguments()[0];
            if (listType == typeof(int))
            {
                return CompareLists<int>(fieldValue, compareValue);
            }
            if (listType == typeof(float))
            {
                return CompareLists<float>(fieldValue, compareValue);
            }
            if (listType == typeof(Color))
            {
                string field = string.Join(",", ((List<Color>)fieldValue).Select(v => ColorUtility.ToHtmlStringRGBA(v)));
                string compare =string.Join(",", ((List<Color>)compareValue).Select(v => ColorUtility.ToHtmlStringRGBA(v)));
                return(field.Equals(compare));
            }
            return false;
        }

        private bool CompareLists<T>(object l1, object l2)
        {
            if(l1 is List<T> list1 && l2 is List<T> list2)
            {
                if(list1 == null && list2 == null) return true;
                if(list1 == null || list2 == null) return false;
                if(list1.Count != list2.Count) return false;
                for(int i = 0; i < list1.Count; i++)
                {
                    if(!list1[i].Equals(list2[i])) return false;
                }
                return true;
            }
            return false;
            
        }

        private bool IsNumbericType(Type type)
        {
            return type.IsPrimitive && type != typeof(bool) && type != typeof(char);
        }

    }
}
