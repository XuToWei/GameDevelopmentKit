using GameFramework;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 默认的关闭辅助器。
    /// </summary>
    public class DefaultShutdownHelper : IShutdownHelper
    {
        public virtual void Shutdown()
        {
        }
    }
}