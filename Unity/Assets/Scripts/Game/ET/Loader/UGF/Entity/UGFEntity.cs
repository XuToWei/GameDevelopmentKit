using System;
using UnityEngine;

namespace ET
{
    [ChildOf]
    public sealed class UGFEntity : Entity, IAwake<string, ETMonoEntity>, IDestroy
    {
        public UnityGameFramework.Runtime.Entity entity { get; private set; }

        public string entityEventTypeName { get; private set; }

        public Transform transform { get; private set; }

        public bool isShow => this.etMonoEntity.isShow;

        public ETMonoEntity etMonoEntity;
        
        internal void OnAwake(string entityEventTypeName, ETMonoEntity etMonoEntity)
        {
            this.etMonoEntity = etMonoEntity;
            this.entityEventTypeName = entityEventTypeName;
            this.transform = etMonoEntity.CachedTransform;
            this.entity = etMonoEntity.Entity;
        }
        
        internal void OnDestroy()
        {
            this.etMonoEntity = default;
            this.entityEventTypeName = default;
            this.transform = default;
            this.entity = default;
        }
    }

    [EntitySystemOf(typeof(UGFEntity))]
    [FriendOf(typeof(UGFEntity))]
    public static partial class UGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this UGFEntity self, string entityEventTypeName, ETMonoEntity etMonoEntity)
        {
            self.OnAwake(entityEventTypeName, etMonoEntity);
        }

        [EntitySystem]
        private static void Destroy(this UGFEntity self)
        {
            self.OnDestroy();
        }
    }
}