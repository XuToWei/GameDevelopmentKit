using GameFramework;

namespace UnityGameFramework.Extension
{
    /// <summary>
    /// Await包装类
    /// </summary>
    internal class AwaitDataWrap : IReference
    {
        /// <summary>
        /// 自定义数据
        /// </summary>
        internal object UserData { get; private set; }

        /// <summary>
        /// 等待是否结束
        /// </summary>
        internal bool IsFinished { get; private set; }

        internal static AwaitDataWrap Create(object userData)
        {
            AwaitDataWrap awaitDataWrap = ReferencePool.Acquire<AwaitDataWrap>();
            awaitDataWrap.UserData = userData;
            awaitDataWrap.IsFinished = false;
            return awaitDataWrap;
        }

        public void Clear()
        {
            UserData = null;
            IsFinished = true;
        }
    }
    
    /// <summary>
    /// Await包装类
    /// </summary>
    internal class AwaitDataWrap<T> : IReference where T : class
    {
        /// <summary>
        /// 自定义数据
        /// </summary>
        internal object UserData { get; private set; }

        /// <summary>
        /// Result
        /// </summary>
        internal T Result { get; private set; }
        
        /// <summary>
        /// 等待是否结束
        /// </summary>
        internal bool IsFinished { get; private set; }

        internal static AwaitDataWrap<T> Create(object userData, T result)
        {
            AwaitDataWrap<T> awaitDataWrap = ReferencePool.Acquire<AwaitDataWrap<T>>();
            awaitDataWrap.UserData = userData;
            awaitDataWrap.Result = result;
            awaitDataWrap.IsFinished = false;
            return awaitDataWrap;
        }

        public void Clear()
        {
            UserData = null;
            Result = default;
            IsFinished = true;
        }
    }
}