using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace Game
{
    /// <summary>
    /// 游戏框架平台类，可以用来处收集日志，广告，sdk相关的, 更具项目自行修改
    /// </summary>
    public sealed class PlatformComponent : GameFrameworkComponent
    {
#if UNITY_EDITOR
        private readonly IPlatform m_Platform = new PlatformEditor();
#elif UNITY_ANDROID
        private readonly IPlatform m_Platform = new PlatformAndroid();
#elif UNITY_IOS
        private readonly IPlatform m_Platform = new PlatformIOS();
#else
        private readonly IPlatform m_Platform = new PlatformEditor();
#endif

        public void TrackEvent(string eventName, string key, object value)
        {
            m_Platform.TrackEvent(eventName, new Dictionary<string, object>());
        }
    }
}