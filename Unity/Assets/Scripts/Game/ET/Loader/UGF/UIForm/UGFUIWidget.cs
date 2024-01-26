using System;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace ET
{
    [ChildOf]
    public sealed class UGFUIWidget : Entity, IAwake<Transform, Type>, IDestroy
    {
        [BsonIgnore]
        public Transform Transform { get; private set; }
        public Type WidgetEventType { get; private set; }

        internal void OnAwake(Transform transform, Type widgetEventType)
        {
            Transform = transform;
            WidgetEventType = widgetEventType;
        }

        internal void OnDestroy()
        {
            this.Transform = default;
        }
    }
    
    [EntitySystemOf(typeof(UGFUIWidget))]
    [FriendOf(typeof(UGFUIWidget))]
    public static partial class UGFUIWidgetSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUIWidget self, Transform transform, Type widgetEventType)
        {
            self.OnAwake(transform, widgetEventType);
        }
    
        [EntitySystem]
        private static void Destroy(this UGFUIWidget self)
        {
            self.OnDestroy();
        }
    }
}