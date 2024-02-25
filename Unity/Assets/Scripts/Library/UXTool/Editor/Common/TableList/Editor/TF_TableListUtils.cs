#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor.Expressions;
using UnityEngine;

namespace TF_TableList
{
    public class RowExtraInfo
    {
        public int origIndex;
        public bool selected;
        public bool filterSuccess;

        public RowExtraInfo(int origIndex, bool selected = false, bool filterSuccess = false)
        {
            this.origIndex = origIndex;
            this.selected = selected;
            this.filterSuccess = filterSuccess;
        }
    }

    public class ScriptModifyHelper
    {
        private object target;
        public string MemberErrorMessage;
        private Delegate modify;
        private Type targetType;
        public bool IsValid { get; private set; }
        public ScriptModifyHelper(Type targetType, string expression)
        {
            this.targetType = targetType;
            var emitContext = new EmitContext()
            {
                IsStatic = true,
                ReturnType = targetType,
                Type = typeof(ScriptModifyHelper),
                Parameters = new Type[] { targetType },
                ParameterNames = new string[] { "v" }
            };
            try
            {
                modify = ExpressionUtility.ParseExpression(expression, emitContext, out this.MemberErrorMessage);
            }
            catch (Exception e)
            {
                MemberErrorMessage = e.ToString();
            }
            IsValid = (MemberErrorMessage == null);
        }

        public object Process(object obj)
        {
            if (IsValid)
            {
                var ret = modify.DynamicInvoke(obj);
                if (ret.GetType() == targetType)
                    return ret;
            }

            if (targetType.IsValueType)
                return Activator.CreateInstance(targetType);
            else
                return null;
        }
    }

    public class StringFilterEdit
    {
        public string FilterStr;
        private Action<string> confirm;
        private Action cancel;

        public StringFilterEdit(string filterStr, Action<string> confirm, Action cancel)
        {
            this.FilterStr = filterStr;
            this.confirm = confirm;
            this.cancel = cancel;
        }

        [Button("确定", ButtonSizes.Medium), HorizontalGroup(0.5f)]
        public void OnConfirm()
        {
            confirm(FilterStr);
        }

        [Button("取消", ButtonSizes.Medium), HorizontalGroup]
        public void OnCancel()
        {
            cancel();
        }
    }

    public class ScriptFilterEdit
    {
        [DetailedInfoBox("使用 $v 表示值,输入表达式,点击查看详情", "使用 $v 表示值, 输入表达式\ne.g. $v > 10\n   $v.StartsWith(\"abc\")")]
        public string FilterStr;
        private Action<string> confirm;
        private Action cancel;

        public ScriptFilterEdit(string filterStr, Action<string> confirm, Action cancel)
        {
            this.FilterStr = filterStr;
            this.confirm = confirm;
            this.cancel = cancel;
        }

        [Button("确定", ButtonSizes.Medium), HorizontalGroup(0.5f)]
        public void OnConfirm()
        {
            confirm(FilterStr);
        }

        [Button("取消", ButtonSizes.Medium), HorizontalGroup]
        public void OnCancel()
        {
            cancel();
        }
    }

    public class ScriptModifyEdit
    {
        [Serializable]
        public class LivePreviewInfo
        {
            public string orig;
            public string modified;
        }

        [ShowInInspector]
        [BoxGroup("Value", GroupName = "通过值修改")]
        public object valueTarget;

        [DetailedInfoBox("使用 $v 表示值, 需要返回$v相同类型", "使用 $v 表示值 \ne.g. $v + 10\n   $v+\"_suffix\"")]
        [OnValueChanged("TryLivePreview")]
        [Multiline]
        [BoxGroup("Script", GroupName = "通过脚本修改")]
        public string ModifyStr;

        [LabelText("实时预览")]
        [BoxGroup("Script")]
        public bool EnableLivePreview = true;
        private Action<string, bool> confirm;
        private Action<object, bool> confirmValue;
        private Action cancel;
        [ShowIf("@!string.IsNullOrEmpty(this.LivePreviewErrorStr)")]
        [GUIColor(1, 0, 0)]
        [Multiline]
        [BoxGroup("Script")]
        public string LivePreviewErrorStr;
        [TableList(IsReadOnly = true, AlwaysExpanded = true, HideToolbar = true)]
        [ShowIf("@this.EnableLivePreview == true && livePreviewList.Count>0")]
        [BoxGroup("Script")]
        public List<LivePreviewInfo> livePreviewList;
        private List<object> livePreviewObjects;
        [LabelText("只应用到被选中的行")]
        public bool OnlyApplyToSelection = true;

        public ScriptModifyEdit(string modifyStr, Action<string, bool> confirm, Action cancel, Action<object, bool> confirmValue, List<object> livePreviewObjects = null)
        {

            this.ModifyStr = modifyStr;
            this.confirmValue = confirmValue;
            this.confirm = confirm;
            this.cancel = cancel;
            this.livePreviewObjects = livePreviewObjects;
            this.livePreviewList = new List<LivePreviewInfo>();
            if (livePreviewObjects != null)
            {
                this.valueTarget = livePreviewObjects[0];
                foreach (var livePreviewObject in livePreviewObjects)
                {
                    var str = livePreviewObject.ToString();
                    var type = livePreviewObject.GetType();
                    if (!type.IsValueType && type != typeof(string))
                    {
                        EnableLivePreview = false;
                    }
                    livePreviewList.Add(new LivePreviewInfo
                    {
                        orig = str,
                        modified = str
                    });
                }
            }
        }

        [Button("值修改", ButtonSizes.Medium), HorizontalGroup]
        public void OnConfirmValue()
        {
            confirmValue(valueTarget, OnlyApplyToSelection);
        }

        [Button("脚本修改", ButtonSizes.Medium), HorizontalGroup]
        [EnableIf("@!string.IsNullOrEmpty(ModifyStr) && LivePreviewErrorStr == null")]
        public void OnConfirm()
        {
            confirm(ModifyStr, OnlyApplyToSelection);
        }

        [Button("取消", ButtonSizes.Medium), HorizontalGroup]
        public void OnCancel()
        {
            cancel();
        }

        private void TryLivePreview()
        {
            if (livePreviewObjects == null || livePreviewObjects.Count == 0)
                return;
            LivePreviewErrorStr = null;
            var type = livePreviewObjects[0].GetType();
            var modifyHelper = new ScriptModifyHelper(type, ModifyStr);
            if (!modifyHelper.IsValid)
            {
                foreach (var livePreviewInfo in livePreviewList)
                {
                    livePreviewInfo.modified = livePreviewInfo.orig;
                }
                LivePreviewErrorStr = modifyHelper.MemberErrorMessage;
                return;
            }

            if (!EnableLivePreview)
                return;
            try
            {
                for (var index = 0; index < livePreviewObjects.Count; index++)
                {
                    var livePreviewObject = livePreviewObjects[index];
                    var obj = modifyHelper.Process(livePreviewObject);
                    livePreviewList[index].modified = obj.ToString();
                }
            }
            catch (Exception e)
            {
                foreach (var livePreviewInfo in livePreviewList)
                {
                    livePreviewInfo.modified = livePreviewInfo.orig;
                }
                LivePreviewErrorStr = e.ToString();
            }
        }
    }
}
#endif