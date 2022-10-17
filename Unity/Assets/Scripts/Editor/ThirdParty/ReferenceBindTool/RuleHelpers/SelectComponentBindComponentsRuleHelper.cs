using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ReferenceBindTool.Runtime;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    public class SelectComponentBindComponentsRuleHelper : IBindComponentsRuleHelper
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
            Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
            List<(string fieldName,Component bindComponent)> bindList = new List<(string,Component)>();
            foreach (Transform child in children)
            {
                BindDataSelect bindDataSelect = child.GetComponent<BindDataSelect>();
                if (bindDataSelect == null)
                {
                    continue;
                }

                foreach (Component component in bindDataSelect.BindComponents)
                {
                    if (component == null)
                    {
                        continue;
                    }
                    var bindData = bindComponents.Find(_ => _.BindObject == component);
                    string fieldName = bindData == null ? GetDefaultFieldName(component) : bindData.FieldName;
                    bindList.Add((fieldName,component));
                }
            }

            bindAction.Invoke(bindList);
        }
    }
}