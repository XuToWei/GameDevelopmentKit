using UnityEngine;

namespace Game.Hot
{
    public abstract class HotComponent : MonoBehaviour
    {
        /// <summary>
        /// 运行优先级，越大运行越靠前
        /// </summary>
        protected internal virtual int Priority => 0;

        protected internal virtual void OnInitialize()
        {
            
        }

        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        protected internal virtual void OnShutdown()
        {
            
        }

        protected virtual void Awake()
        {
            HotComponentEntry.RegisterComponent(this);
        }
    }
}
