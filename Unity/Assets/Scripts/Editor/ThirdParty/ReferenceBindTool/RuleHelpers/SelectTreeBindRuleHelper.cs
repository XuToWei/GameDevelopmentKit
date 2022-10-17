using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    public class SelectTreeBindComponentsRuleHelper : IBindComponentsRuleHelper
    {
        public string GetDefaultFieldName(Component component)
        {
            string filedName = $"{component.GetType().Name}_{component.name}".Replace(' ', '_');
            return filedName;
        }

        public bool CheckFieldNameIsInvalid(string fieldName)
        {
            string regex = "^[a-zA-Z_][a-zA-Z0-9_]*$";
            return !Regex.IsMatch(fieldName, regex);
        }

        public void BindComponents(GameObject gameObject, List<ReferenceBindComponent.BindObjectData> bindComponents, Action<List<(string, Component)>> bindAction)
        {
            var select = bindComponents.Where(_=>_.BindObject != null).ToDictionary(_ => _.BindObject.GetInstanceID(), _ => true);
            Component GetComponent (int instanceID)
            {
                return (Component)EditorUtility.InstanceIDToObject(instanceID);
            }
            void OnComplete()
            {
                List<(string fieldName,Component bindComponent)> bindList = new List<(string,Component)>();
                foreach (var selectItem in select)
                {
                    if (selectItem.Value)
                    {
                        var bindData = bindComponents.Find(_ => _.BindObject.GetInstanceID() == selectItem.Key);
                        Component component = GetComponent(selectItem.Key);
                        string fieldName = bindData == null ? GetDefaultFieldName(component) : bindData.FieldName;
                        bindList.Add((fieldName,component));
                    }
                }
                bindAction.Invoke(bindList);
            }

            m_PopWindow.Show(gameObject.transform, select, OnComplete);
        }

        private SelectComponentTreePopWindow m_PopWindow;
        public SelectTreeBindComponentsRuleHelper()
        {
            m_PopWindow = new SelectComponentTreePopWindow();
        }

       
    }
}