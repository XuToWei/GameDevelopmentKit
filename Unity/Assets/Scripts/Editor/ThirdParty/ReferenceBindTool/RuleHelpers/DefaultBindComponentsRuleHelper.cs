using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ReferenceBindTool.Runtime;
using UnityEngine;
using BindObjectData = ReferenceBindTool.Runtime.ReferenceBindComponent.BindObjectData;

namespace ReferenceBindTool.Editor
{
    internal interface IBindRule
    {
        /// <summary>
        /// 前缀名
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// 获取绑定数据
        /// </summary>
        /// <param name="ruleHelper"></param>
        /// <param name="target"></param>
        /// <param name="bindList"></param>
        void GetBindData(DefaultBindComponentsRuleHelper ruleHelper, Transform target,
            List<(string, Component)> bindList);
    }

    public class DefaultBindRule : IBindRule
    {
        public string Prefix => "BD_";

        private List<Type> m_BindTypes = new List<Type>()
        {
            // typeof(Transform),
            // typeof(RectTransform),
            // typeof(Animation),
            // typeof(Canvas),
            // typeof(CanvasGroup),
            // typeof(VerticalLayoutGroup),
            // typeof(HorizontalLayoutGroup),
            // typeof(GridLayoutGroup),
            // typeof(ToggleGroup),
            // typeof(Button),
            // typeof(Image),
            // typeof(RawImage),
            // typeof(Text),
            // typeof(TMP_Text),
            // typeof(InputField),
            // typeof(TMP_InputField),
            // typeof(Slider),
            // typeof(Mask),
            // typeof(RectMask2D),
            // typeof(Toggle),
            // typeof(Scrollbar),
            // typeof(ScrollRect),
            // typeof(Dropdown),
            // typeof(TMP_Dropdown),
            // typeof(Camera),
            // typeof(EventTrigger),
        };

        public List<Type> BindTypes => m_BindTypes;

        public DefaultBindRule()
        {
            m_BindTypes = m_BindTypes.Distinct().ToList();
        }

        public void GetBindData(DefaultBindComponentsRuleHelper ruleHelper, Transform target,
            List<(string, Component)> bindList)
        {
            if (m_BindTypes.Count == 0)
            {
                throw new Exception($"{nameof(DefaultBindRule)} 前缀{Prefix}的可绑定类型列表为空。请先添加绑定类型。");
            }

            if (target == null || string.IsNullOrEmpty(target.name) || !target.name.StartsWith(Prefix))
            {
                return;
            }

            for (int i = 0; i < BindTypes.Count; i++)
            {
                Component component = target.GetComponent(BindTypes[i]);
                if (component != null)
                {
                    bindList.Add((ruleHelper.GetDefaultFieldName(component), component));
                }
            }
        }
    }

    public class DefaultBindComponentsRuleHelper : IBindComponentsRuleHelper
    {
        private List<IBindRule> m_BindRules = new List<IBindRule>()
        {
            new DefaultBindRule(),
        };

        public string GetDefaultFieldName(Component component)
        {
            string gameObjectName = component.gameObject.name;
            for (int i = 0; i < m_BindRules.Count; i++)
            {
                if (!gameObjectName.StartsWith(m_BindRules[i].Prefix)) continue;
                gameObjectName = gameObjectName.Substring(m_BindRules[i].Prefix.Length);
                break;
            }

            return $"{component.GetType().Name}_{gameObjectName}".Replace(' ', '_');
        }

        public bool CheckFieldNameIsInvalid(string fieldName)
        {
            string regex = "^[a-zA-Z_][a-zA-Z0-9_]*$";
            return !Regex.IsMatch(fieldName, regex);
        }

        public void BindComponents(GameObject gameObject, List<BindObjectData> bindComponents,
            Action<List<(string, Component)>> bindAction)
        {
            if (m_BindRules == null || m_BindRules.Count == 0)
            {
                throw new Exception($"{nameof(DefaultBindComponentsRuleHelper)} 没有前缀规则 请添加后在进行绑定。");
            }

            Transform[] children = gameObject.transform.GetComponentsInChildren<Transform>(true);
            List<(string fieldName, Component bindComponent)> bindList = new List<(string, Component)>();
            List<(string fieldName, Component bindComponent)> tempBindList = new List<(string, Component)>();
            foreach (Transform child in children)
            {
                tempBindList.Clear();
                foreach (IBindRule bindRule in m_BindRules)
                {
                    if (!child.name.StartsWith(bindRule.Prefix))
                    {
                        continue;
                    }

                    bindRule.GetBindData(this, child, tempBindList);
                    for (int i = 0; i < tempBindList.Count; i++)
                    {
                        var bindData = bindComponents.Find(_ => _.BindObject == tempBindList[i].bindComponent);
                        string fieldName = bindData == null ? tempBindList[i].fieldName : bindData.FieldName;
                        bindList.Add((fieldName, tempBindList[i].bindComponent));
                    }
                }
            }

            bindAction.Invoke(bindList);
        }
    }
}