using UnityEngine;

namespace Game
{
    /// <summary>
    /// 用来动态加载复用的Item
    /// </summary>
    public sealed class ItemEntity : AEntity
    {
        private Transform m_OriginalTransform = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_OriginalTransform = CachedTransform.parent;
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            ItemEntityData itemEntityData = (ItemEntityData)userData;
            CachedTransform.SetParent(itemEntityData.ParentTransform);
            CachedTransform.localPosition = Vector3.zero;
            CachedTransform.localRotation = Quaternion.identity;
            CachedTransform.localScale = Vector3.one;
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            CachedTransform.SetParent(m_OriginalTransform);
        }
    }
}
