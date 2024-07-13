#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor.Expressions;
using UnityEngine;

namespace TF_TableList
{
    public sealed class ScriptRowFilter : RowFilter
    {
        [OnValueChanged("OnValueChanged")]
        [InlineButton("Clear", "清除")]
        [InlineButton("Apply", "应用")]
        [LabelText("@\"script filter by \" + this.PropertyName")]
        [LabelWidth(150)]
        public string FilterStr = "";
        private Type _targetType;
        private Delegate filter;
        [ShowInInspector]
        [ShowIf("@!string.IsNullOrEmpty(this.memberErrorMessage)")]
        [HideLabel]
        [Multiline]
        [GUIColor(1, 0, 0)]
        private string memberErrorMessage;
        private EmitContext _emitContext;

        public override bool IsValid => filter != null && string.IsNullOrEmpty(memberErrorMessage);

        public override bool Success(object target)
        {

            var ret = false;
            try
            {
                ret = (bool)filter.DynamicInvoke(target);
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        }

        public ScriptRowFilter()
        {
        }

        public ScriptRowFilter(Type targetType, string filterStr)
        {
            FilterStr = filterStr;
            _targetType = targetType;
            _emitContext = new EmitContext()
            {
                IsStatic = true,
                ReturnType = typeof(bool),
                Type = typeof(ScriptRowFilter),
                Parameters = new Type[] { targetType },
                ParameterNames = new string[] { "v" }
            };
            try
            {
                filter = ExpressionUtility.ParseExpression(FilterStr, _emitContext, out this.memberErrorMessage);
            }
            catch (Exception e)
            {
                memberErrorMessage = e.ToString();
            }
        }

        private void OnValueChanged()
        {
            memberErrorMessage = null;
            if (string.IsNullOrEmpty(FilterStr))
            {
                filter = null;
                memberErrorMessage = null;
                return;
            }
            else
            {
                try
                {
                    filter = ExpressionUtility.ParseExpression(FilterStr, _emitContext, out this.memberErrorMessage);
                }
                catch (Exception e)
                {
                    memberErrorMessage = e.ToString();
                }
            }
        }

        private void Apply()
        {
            Dirty = true;
        }

        private void Clear()
        {
            FilterStr = "";
            OnValueChanged();
            Dirty = true;
        }
    }
}
#endif