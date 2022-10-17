#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using BindObjectData =  ReferenceBindTool.Runtime.ReferenceBindComponent.BindObjectData;

namespace ReferenceBindTool.Runtime
{
    /// <summary>
    /// 自动绑定规则辅助器接口
    /// </summary>
    public interface IBindComponentsRuleHelper : IRuleHelper
    {
        /// <summary>
        /// 获取默认字段名
        /// </summary>
        /// <param name="component">组件</param>
        /// <returns>默认字段名</returns>
        string GetDefaultFieldName(Component component);

        /// <summary>
        /// 检查字段名称是否无效
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>字段名是否无效</returns>
        bool CheckFieldNameIsInvalid(string fieldName);

        /// <summary>
        /// 绑定符合规则的所有组件
        /// </summary>
        /// <param name="gameObject">绑定物体</param>
        /// <param name="bindComponents">现在绑定的组件</param>
        /// <param name="bindAction">绑定操作</param>
        void BindComponents(GameObject gameObject,List<ReferenceBindComponent.BindObjectData> bindComponents,Action<List<(string,Component)>> bindAction);
    }
}
#endif