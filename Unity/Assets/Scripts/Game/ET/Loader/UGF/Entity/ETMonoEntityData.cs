using System;
using GameFramework;

namespace ET
{
    public sealed class ETMonoEntityData : IReference
    {
        public long EntityEventTypeLongHashCode
        {
            get;
            private set;
        }

        public Entity ParentEntity
        {
            get;
            private set;
        }
        
        public object UserData
        {
            get;
            private set;
        }
        
        public void Clear()
        {
            this.EntityEventTypeLongHashCode = default;
            this.ParentEntity = default;
            this.UserData = default;
        }
        
        public static ETMonoEntityData Acquire(Type entityEventType, Entity parentEntity, object userData)
        {
            ETMonoEntityData entityData = ReferencePool.Acquire<ETMonoEntityData>();
            entityData.EntityEventTypeLongHashCode = entityEventType.FullName.GetLongHashCode();
            entityData.ParentEntity = parentEntity;
            entityData.UserData = userData;
            return entityData;
        }

        public void Release()
        {
            ReferencePool.Release(this);
        }
    }
}
