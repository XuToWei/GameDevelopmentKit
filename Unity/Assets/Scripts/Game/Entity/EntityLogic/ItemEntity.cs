using UnityEngine;

namespace Game
{
    /// <summary>
    /// 用来动态加载复用的Item
    /// </summary>
    public class ItemEntity : AEntity
    {
        private Transform m_OriginalTransform = null;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_OriginalTransform = CachedTransform.parent;
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            CachedTransform.SetParent(m_OriginalTransform);
        }
    }
}
