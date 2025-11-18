using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public abstract class AEntity : EntityLogic
    {
        public int Id => Entity.Id;
        private Transform m_CachedParentTransform;

#if UNITY_EDITOR
        private string m_GameObjectName;
#endif

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_CachedParentTransform = CachedTransform.parent;
#if UNITY_EDITOR
            m_GameObjectName = CachedTransform.name;
#endif
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
#if UNITY_EDITOR
            Name = Utility.Text.Format("[Entity {0} {1}]", Id, m_GameObjectName);
#endif
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            if (m_CachedParentTransform != CachedTransform.parent)
            {
                CachedTransform.SetParent(m_CachedParentTransform, true);
            }
#if UNITY_EDITOR
            Name = m_GameObjectName;
#endif
            base.OnHide(isShutdown, userData);
        }
    }
}