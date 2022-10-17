using System;
using System.Linq;
using ReferenceBindTool.Runtime;
using UnityEngine;
using BindObjectData = ReferenceBindTool.Runtime.ReferenceBindComponent.BindObjectData;

namespace ReferenceBindTool.Editor
{
    public static class NotAddComponentDataExtensions
    {
        /// <summary>
        /// 添加绑定数据
        /// </summary>
        public static void AddBindComponent(this NotAddComponentData self, string fieldName, Component bindComponent)
        {
            bool isRepeat = false;
            for (int j = 0; j < self.BindComponents.Count; j++)
            {
                if (self.BindComponents[j].FieldName == fieldName)
                {
                    isRepeat = true;
                    break;
                }
            }

            self.BindComponents.Add(new ReferenceBindComponent.BindObjectData(isRepeat, fieldName, bindComponent)
            {
                FieldNameIsInvalid = self.BindComponentsRuleHelper.CheckFieldNameIsInvalid(fieldName)
            });
        }
        
        /// <summary>
        /// 规则绑定所有组件
        /// </summary>
        public static void RuleBindComponents(this NotAddComponentData self)
        {
            self.BindComponentsRuleHelper.BindComponents(self.GameObject, self.BindComponents,bindList =>
            {
                self.BindComponents.Clear();
                foreach ((string fieldName, Component bindComponent) item in bindList)
                {
                    self.AddBindComponent(item.fieldName,item.bindComponent);
                }
            });
        }
        
        /// <summary>
        /// 设置脚本名称
        /// </summary>
        /// <param name="self"></param>
        /// <param name="className"></param>
        public static void SetClassName(this NotAddComponentData self, string className)
        {
            if (self.GeneratorCodeName == className)
            {
                return;
            }

            self.GeneratorCodeName = className;
            
        }

        /// <summary>
        /// 设置生成规则帮助类
        /// </summary>
        /// <param name="self"></param>
        /// <param name="ruleHelperName"></param>
        public static void SetBindComponentsRuleHelperTypeName(this NotAddComponentData self, string ruleHelperName)
        {
            if (self.BindComponentsRuleHelperTypeName == ruleHelperName && self.BindComponentsRuleHelper != null)
            {
                return;
            }

            self.BindComponentsRuleHelperTypeName = ruleHelperName;
            IBindComponentsRuleHelper helper = (IBindComponentsRuleHelper) RuleHelperUtility.CreateHelperInstance<IBindComponentsRuleHelper>(self.BindComponentsRuleHelperTypeName);
            self.BindComponentsRuleHelper = helper;
            
        }
        /// <summary>
        /// 设置代码生成规则帮助类
        /// </summary>
        /// <param name="self"></param>
        /// <param name="ruleHelperName"></param>
        public static void SetCodeGeneratorRuleHelperTypeName(this NotAddComponentData self, string ruleHelperName)
        {
            if (self.CodeGeneratorRuleHelperTypeName == ruleHelperName && self.CodeGeneratorRuleHelper != null)
            {
                return;
            }

            self.CodeGeneratorRuleHelperTypeName = ruleHelperName;
            ICodeGeneratorRuleHelper helper = RuleHelperUtility.CreateHelperInstance<ICodeGeneratorRuleHelper>(self.CodeGeneratorRuleHelperTypeName);
            self.CodeGeneratorRuleHelper = helper;
        }
        
        /// <summary>
        /// 设置生成代码配置
        /// </summary>
        /// <param name="self"></param>
        /// <param name="data"></param>
        public static void SetSettingData(this NotAddComponentData self, string name)
        {
            self.CodeGeneratorSettingData = ReferenceBindUtility.GetAutoBindSetting(name);
        }
        /// <summary>
        /// 设置生成代码配置
        /// </summary>
        /// <param name="self"></param>
        /// <param name="data"></param>
        public static void SetSettingData(this NotAddComponentData self, CodeGeneratorSettingData data)
        {
            if (self.CodeGeneratorSettingData == data)
            {
                return;
            }

            self.CodeGeneratorSettingData = data;
        }
        
        /// <summary>
        /// 排序
        /// </summary>
        public static void Sort(this NotAddComponentData self)
        {
            self.BindComponents.Sort((x, y) =>
                string.Compare(x.FieldName, y.FieldName, StringComparison.Ordinal));
        }
        

        /// <summary>
        /// 删除Missing Or Null
        /// </summary>
        public static void RemoveNull(this NotAddComponentData self)
        {
            for (int i = self.BindComponents.Count - 1; i >= 0; i--)
            {
                var bindData = self.BindComponents[i];

                if (bindData.BindObject == null)
                {
                    self.BindComponents.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 全部删除
        /// </summary>
        public static void RemoveAll(this NotAddComponentData self)
        {
            self.BindComponents.Clear();
        }
        
           /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="self"></param>
        public static void Refresh(this NotAddComponentData self)
        {
            var tempList = self.BindComponents.ToList();
            self.BindComponents.Clear();
            foreach (var tempData in tempList)
            {
                self.AddBindComponent(tempData.FieldName, (Component)tempData.BindObject);
            }
        }

        /// <summary>
        /// 重置所有字段名
        /// </summary>
        /// <param name="self"></param>
        public static void ResetAllFieldName(this NotAddComponentData self)
        {
           
            var tempList = self.BindComponents.ToList();
            self.BindComponents.Clear();
            foreach (var tempData in tempList)
            {
                self.AddBindComponent(self.BindComponentsRuleHelper.GetDefaultFieldName((Component)tempData.BindObject),
                    (Component)tempData.BindObject);
            }
        }

    }
}