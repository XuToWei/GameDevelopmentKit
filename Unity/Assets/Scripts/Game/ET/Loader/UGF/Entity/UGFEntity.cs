using System;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace ET
{
    [ChildOf]
    public sealed class UGFEntity : Entity, IAwake<Type, ETMonoEntity>, IDestroy
    {
        [BsonIgnore]
        public UnityGameFramework.Runtime.Entity Entity { get; private set; }
        public Type EntityEventType { get; private set; }
        [BsonIgnore]
        public Transform Transform { get; private set; }
        public bool IsShow => ETMonoEntity.IsShow;
        [BsonIgnore]
        public ETMonoEntity ETMonoEntity { get; private set; }
        
        internal void OnAwake(Type entityEventType, ETMonoEntity etMonoEntity)
        {
            ETMonoEntity = etMonoEntity;
            EntityEventType = entityEventType;
            Transform = etMonoEntity.CachedTransform;
            Entity = etMonoEntity.Entity;
        }
        
        internal void OnDestroy()
        {
            ETMonoEntity = default;
            EntityEventType = default;
            Transform = default;
            Entity = default;
        }
    }

    [EntitySystemOf(typeof(UGFEntity))]
    [FriendOf(typeof(UGFEntity))]
    public static partial class UGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this UGFEntity self, Type entityEventTyp, ETMonoEntity etMonoEntity)
        {
            self.OnAwake(entityEventTyp, etMonoEntity);
        }

        [EntitySystem]
        private static void Destroy(this UGFEntity self)
        {
            self.OnDestroy();
        }
    }
}