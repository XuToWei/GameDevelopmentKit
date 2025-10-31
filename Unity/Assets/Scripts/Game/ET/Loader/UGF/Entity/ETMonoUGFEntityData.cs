using GameFramework;

namespace ET
{
    public sealed class ETMonoUGFEntityData : IReference
    {
        public EntityRef<UGFEntity> UGFEntity
        {
            get;
            private set;
        }

        public void Clear()
        {
            UGFEntity = null;
        }

        public static ETMonoUGFEntityData Create(UGFEntity ugfEntity)
        {
            ETMonoUGFEntityData entityData = ReferencePool.Acquire<ETMonoUGFEntityData>();
            entityData.UGFEntity = ugfEntity;
            return entityData;
        }
    }
}


