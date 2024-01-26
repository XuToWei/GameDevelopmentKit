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
            UIWidgets?.Dispose();
            UIWidgets = null;
            if (etMonoUIForm != default && etMonoUIForm.IsOpen)
            {
                GameEntry.UI.CloseUIForm(etMonoUIForm.UIForm);
            }
        }

        internal void AddUIWidget<T>(Transform transform) where T : IUGFUIWidgetEvent
        {
            UGFUIWidget ugfUIWidget = AddChild<UGFUIWidget, Transform, Type>(transform, typeof(T),true);
            if (UIWidgets == null)
            {
                UIWidgets = ListComponent<EntityRef<UGFUIWidget>>.Create();
            }
            UIWidgets.Add(ugfUIWidget);
        }

        internal void RemoveUIWidget(UGFUIWidget ugfUIWidget)
        {
            UIWidgets.Remove(ugfUIWidget);
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

        public static void AddUIWidget<T>(this UGFUIForm self, Transform transform) where T : IUGFUIWidgetEvent
        {
            self.AddUIWidget<T>(transform);
        }

        public static void RemoveUIWidget(this UGFUIForm self, UGFUIWidget ugfUIWidget)
        {
            self.RemoveUIWidget(ugfUIWidget);
        }
    }
}