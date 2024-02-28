using UnityEngine;
using UnityGameFramework.Runtime;

namespace ET.Client
{
    [EnableClass]
    [FriendOf(typeof(UGFEntityComponent))]
    public abstract class AUGFEntityEvent : IUGFEntityEvent
    {
        public virtual void OnReload(UGFEntity entity)
        {
        }

        public virtual void OnInit(UGFEntity entity, object userData)
        {
        }

        public virtual void OnShow(UGFEntity entity, object userData)
        {
            entity.GetParent<UGFEntityComponent>().AllShowEntities.Add(entity);
        }

        public virtual void OnHide(UGFEntity entity, bool isShutdown, object userData)
        {
            entity.GetParent<UGFEntityComponent>().AllShowEntities.Remove(entity);
        }

        public virtual void OnAttached(UGFEntity entity, EntityLogic childEntity, Transform parentTransform, object userData)
        {
        }

        public virtual void OnDetached(UGFEntity entity, EntityLogic childEntity, object userData)
        {
        }

        public virtual void OnAttachTo(UGFEntity entity, EntityLogic parentEntity, Transform parentTransform, object userData)
        {
        }

        public virtual void OnDetachFrom(UGFEntity entity, EntityLogic parentEntity, object userData)
        {
        }

        public virtual void OnUpdate(UGFEntity entity, float elapseSeconds, float realElapseSeconds)
        {
        }

        public virtual void OnRecycle(UGFEntity entity)
        {
        }
    }
}
