using System;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace ET
{
    [ChildOf]
    public sealed class UGFEntity : Entity, IAwake<long, ETMonoEntity>, IDestroy
    {
        [BsonIgnore]
        public UnityGameFramework.Runtime.Entity Entity { get; private set; }
        public long EntityEventTypeLongHashCode { get; private set; }
        [BsonIgnore]
        public Transform Transform { get; private set; }
        public bool IsShow => ETMonoEntity.IsShow;
        [BsonIgnore]
        public ETMonoEntity ETMonoEntity { get; private set; }
        
        internal void OnAwake(long entityEventTypeLongHashCode, ETMonoEntity etMonoEntity)
        {
            ETMonoEntity = etMonoEntity;
            EntityEventTypeLongHashCode = entityEventTypeLongHashCode;
            Transform = etMonoEntity.CachedTransform;
            Entity = etMonoEntity.Entity;
        }
        
        internal void OnDestroy()
        {
            ETMonoEntity = default;
            EntityEventTypeLongHashCode = default;
            Transform = default;
            Entity = default;
        }
    }

    [EntitySystemOf(typeof(UGFEntity))]
    [FriendOf(typeof(UGFEntity))]
    public static partial class UGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this UGFEntity self, long entityEventTypLongHashCode, ETMonoEntity etMonoEntity)
        {
            self.OnAwake(entityEventTypLongHashCode, etMonoEntity);
        }

        [EntitySystem]
        private static void Destroy(this UGFEntity self)
        {
            self.OnDestroy();
        }
    }
}