using System;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = Game.GameEntry;

namespace ET
{
    [ChildOf]
    public sealed class UGFUIForm : Entity, IAwake<int, ETMonoUIForm>, IDestroy
    {
        [BsonIgnore]
        public UIForm UIForm { get; private set; }
        public int UIFormId { get; private set; }
        [BsonIgnore]
        public Transform Transform { get; private set; }
        /// <summary>
        /// 界面是否开启
        /// </summary>
        public bool IsOpen => this.m_ETMonoUIForm.IsOpen;
        [BsonIgnore]
        private ETMonoUIForm m_ETMonoUIForm;
        private HashSetComponent<Transform> m_UIWidgetTransforms;
        public ListComponent<EntityRef<UGFUIWidget>> UIWidgets { get; private set; }

        internal void OnAwake(int uiFormId, ETMonoUIForm ugfETUIForm)
        {
            UIFormId = uiFormId;
            m_ETMonoUIForm = ugfETUIForm;
            UIForm = ugfETUIForm.UIForm;
            Transform = ugfETUIForm.CachedTransform;
        }

        internal void OnDestroy()
        {
            ETMonoUIForm etMonoUIForm = m_ETMonoUIForm;
            UIFormId = default;
            m_ETMonoUIForm = default;
            UIForm = default;
            Transform = default;
            m_UIWidgetTransforms?.Dispose();
            m_UIWidgetTransforms = null;
            UIWidgets?.Dispose();
            UIWidgets = null;
            if (etMonoUIForm != default && etMonoUIForm.IsOpen)
            {
                GameEntry.UI.CloseUIForm(etMonoUIForm.UIForm);
            }
        }

        internal UGFUIWidget AddUIWidget<T>(Transform transform, object userData) where T : IUGFUIWidgetEvent
        {
            if (m_UIWidgetTransforms == null)
            {
                m_UIWidgetTransforms = HashSetComponent<Transform>.Create();
            }
            else if (m_UIWidgetTransforms.Contains(transform))
            {
                throw new Exception($"Add UIWidget fail, {transform.name} is already in the UGFUIForm:'{Transform.name}'!");
            }
            m_UIWidgetTransforms.Add(transform);
            UGFUIWidget ugfUIWidget = AddChild<UGFUIWidget, Transform, long>(transform, typeof(T).FullName.GetLongHashCode(),true);
            if (UIWidgets == null)
            {
                UIWidgets = ListComponent<EntityRef<UGFUIWidget>>.Create();
            }
            UIWidgets.Add(ugfUIWidget);
            UGFEventComponent.Instance.GetUIWidgetEvent(ugfUIWidget.WidgetEventTypeLongHashCode).OnInit(ugfUIWidget, userData);
            return ugfUIWidget;
        }

        internal void RemoveUIWidget(UGFUIWidget ugfUIWidget)
        {
            m_UIWidgetTransforms.Remove(ugfUIWidget.Transform);
            UIWidgets.Remove(ugfUIWidget);
        }

        internal void OpenUIWidget(UGFUIWidget ugfUIWidget, object userData)
        {
            ugfUIWidget.IsOpen = true;
            ugfUIWidget.Visible = true;
            UGFEventComponent.Instance.GetUIWidgetEvent(ugfUIWidget.WidgetEventTypeLongHashCode).OnOpen(ugfUIWidget, userData);
        }

        internal void DynamicOpenUIWidget(UGFUIWidget ugfUIWidget, object userData)
        {
            UGFEventComponent.Instance.GetUIWidgetEvent(ugfUIWidget.WidgetEventTypeLongHashCode).OnOpen(ugfUIWidget, userData);
            UGFUIForm ugfUIForm = ugfUIWidget.GetParent<UGFUIForm>();
            UGFEventComponent.Instance.GetUIWidgetEvent(ugfUIWidget.WidgetEventTypeLongHashCode).OnDepthChanged(ugfUIWidget, ugfUIForm.UIForm.UIGroup.Depth, ugfUIForm.UIForm.DepthInUIGroup);
        }

        internal void CloseUIWidget(UGFUIWidget ugfUIWidget, object userData, bool isShutdown)
        {
            ugfUIWidget.Visible = false;
            ugfUIWidget.IsOpen = false;
            UGFEventComponent.Instance.GetUIWidgetEvent(ugfUIWidget.WidgetEventTypeLongHashCode).OnClose(ugfUIWidget, isShutdown, userData);
        }
    }
    
    [EntitySystemOf(typeof(UGFUIForm))]
    [FriendOf(typeof(UGFUIForm))]
    public static partial class UGFUIFormSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUIForm self, int uiFormId, ETMonoUIForm ugfETUIForm)
        {
            self.OnAwake(uiFormId, ugfETUIForm);
        }

        [EntitySystem]
        private static void Destroy(this UGFUIForm self)
        {
            self.OnDestroy();
        }

        public static UGFUIWidget AddUIWidget<T>(this UGFUIForm self, Transform transform, object userData = default) where T : IUGFUIWidgetEvent
        {
            return self.AddUIWidget<T>(transform, userData);
        }

        public static void RemoveUIWidget(this UGFUIForm self, UGFUIWidget ugfUIWidget)
        {
            self.RemoveUIWidget(ugfUIWidget);
        }

        public static void OpenUIWidget(this UGFUIForm self, UGFUIWidget ugfUIWidget, object userData = default)
        {
            self.OpenUIWidget(ugfUIWidget, userData);
        }

        public static void DynamicOpenUIWidget(this UGFUIForm self, UGFUIWidget ugfUIWidget, object userData = default)
        {
            self.DynamicOpenUIWidget(ugfUIWidget, userData);
        }

        public static void CloseUIWidget(this UGFUIForm self, UGFUIWidget ugfUIWidget, object userData = default, bool isShutdown = false)
        {
            self.CloseUIWidget(ugfUIWidget, userData, isShutdown);
        }
    }
}
