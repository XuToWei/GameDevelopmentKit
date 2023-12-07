using UnityEngine;
using UnityGameFramework.Runtime;

namespace ET
{
    public interface IUGFEntityEvent
    {
        void OnReload(UGFEntity entity);

        void OnInit(UGFEntity entity, object userData);

        void OnShow(UGFEntity entity, object userData);

        void OnHide(UGFEntity entity, bool isShutdown, object userData);

        void OnAttached(UGFEntity entity, EntityLogic childEntity, Transform parentTransform, object userData);

        void OnDetached(UGFEntity entity, EntityLogic childEntity, object userData);

        void OnAttachTo(UGFEntity entity, EntityLogic parentEntity, Transform parentTransform, object userData);

        void OnDetachFrom(UGFEntity entity, EntityLogic parentEntity, object userData);

        void OnUpdate(UGFEntity entity, float elapseSeconds, float realElapseSeconds);

        void OnRecycle(UGFEntity entity);
    }
}