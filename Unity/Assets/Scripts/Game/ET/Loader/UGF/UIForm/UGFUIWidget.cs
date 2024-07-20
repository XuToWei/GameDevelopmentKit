using System;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace ET
{
    [ChildOf(typeof(UGFUIForm))]
    public sealed class UGFUIWidget : Entity, IAwake<Transform, long>, IDestroy
    {
        [BsonIgnore]
        public Transform Transform { get; private set; }
        public long WidgetEventTypeLongHashCode { get; private set; }

        public bool IsOpen { get; internal set; }

        private bool m_Visible = false;
        /// <summary>
        /// 获取或设置界面是否可见。
        /// </summary>
        public bool Visible
        {
            get
            {
                return IsOpen && m_Visible;
            }
            internal set
            {
                if (!IsOpen)
                {
                    Log.Warning($"UI widget '{Transform.name}' is not available.");
                    return;
                }
                if (m_Visible == value)
                {
                    return;
                }
                m_Visible = value;
                Transform.gameObject.SetActive(value);
            }
        }

        internal void OnAwake(Transform transform, long widgetEventTypeLongHashCode)
        {
            Transform = transform;
            WidgetEventTypeLongHashCode = widgetEventTypeLongHashCode;
        }

        internal void OnDestroy()
        {
            Transform = default;
            WidgetEventTypeLongHashCode = default;
            IsOpen = false;
            m_Visible = false;
        }
    }
    
    [EntitySystemOf(typeof(UGFUIWidget))]
    [FriendOf(typeof(UGFUIWidget))]
    public static partial class UGFUIWidgetSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUIWidget self, Transform transform, long widgetEventTypeLongHashCode)
        {
            self.OnAwake(transform, widgetEventTypeLongHashCode);
        }
    
        [EntitySystem]
        private static void Destroy(this UGFUIWidget self)
        {
            self.OnDestroy();
        }
    }
}