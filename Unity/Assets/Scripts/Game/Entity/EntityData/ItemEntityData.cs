using GameFramework;
using UnityEngine;

namespace Game
{
    public sealed class ItemEntityData : IReference
    {
        private Transform m_ParentTransform;

        public Transform ParentTransform => m_ParentTransform;

        public static ItemEntityData Create(Transform parentTransform)
        {
            ItemEntityData itemEntityData = ReferencePool.Acquire<ItemEntityData>();
            itemEntityData.m_ParentTransform = parentTransform;
            return itemEntityData;
        }

        public void Clear()
        {
            m_ParentTransform = null;
        }
    }
}
