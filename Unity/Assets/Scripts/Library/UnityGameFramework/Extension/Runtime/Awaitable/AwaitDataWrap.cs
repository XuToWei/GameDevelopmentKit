using System.Threading.Tasks;
using GameFramework;

namespace UnityGameFramework.Extension
{
    /// <summary>
    /// Await包装类
    /// </summary>
    public class AwaitDataWrap<T> : IReference
    {
        /// <summary>
        /// 自定义数据
        /// </summary>
        public object UserData { get; private set; }
        /// <summary>
        /// TaskCompletionSource
        /// </summary>
        public TaskCompletionSource<T> Source { get; private set; }

        public static AwaitDataWrap<T> Create(object userData, TaskCompletionSource<T> source)
        {
            AwaitDataWrap<T> awaitDataWrap = ReferencePool.Acquire<AwaitDataWrap<T>>();
            awaitDataWrap.UserData = userData;
            awaitDataWrap.Source = source;
            return awaitDataWrap;
        }

        public void Clear()
        {
            UserData = null;
            Source = null;
        }
    }
}