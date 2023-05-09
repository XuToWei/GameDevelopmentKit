
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;

namespace UnityGameFramework.Extension
{
    /// <summary>
    /// Await包装类
    /// </summary>
    public class AwaitDataWrap : IReference
    {
        /// <summary>
        /// 自定义数据
        /// </summary>
        public object UserData { get; private set; }
        /// <summary>
        /// TaskCompletionSource
        /// </summary>
        public AutoResetUniTaskCompletionSource TaskCompletionSource { get; private set; }

        public static AwaitDataWrap Create(object userData, AutoResetUniTaskCompletionSource taskCompletionSource)
        {
            AwaitDataWrap awaitDataWrap = ReferencePool.Acquire<AwaitDataWrap>();
            awaitDataWrap.UserData = userData;
            awaitDataWrap.TaskCompletionSource = taskCompletionSource;
            return awaitDataWrap;
        }

        public void Clear()
        {
            UserData = default;
            TaskCompletionSource = default;
        }
    }
    
    
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
        public AutoResetUniTaskCompletionSource<T> TaskCompletionSource { get; private set; }
        
        public CancellationTokenRegistration? CancellationTokenRegistration { get; private set; }

        public static AwaitDataWrap<T> Create(object userData, AutoResetUniTaskCompletionSource<T> taskCompletionSource, CancellationTokenRegistration? cancellationTokenRegistration)
        {
            AwaitDataWrap<T> awaitDataWrap = ReferencePool.Acquire<AwaitDataWrap<T>>();
            awaitDataWrap.UserData = userData;
            awaitDataWrap.TaskCompletionSource = taskCompletionSource;
            awaitDataWrap.CancellationTokenRegistration = cancellationTokenRegistration;
            return awaitDataWrap;
        }

        public void Clear()
        {
            UserData = null;
            TaskCompletionSource = null;
            CancellationTokenRegistration = null;
        }
    }
    
    /// <summary>
    /// Await包装类
    /// </summary>
    public class AwaitTaskWrap : IReference
    {
        /// <summary>
        /// TaskCompletionSource
        /// </summary>
        public AutoResetUniTaskCompletionSource TaskCompletionSource { get; private set; }
        
        public CancellationTokenRegistration? CancellationTokenRegistration { get; private set; }

        public static AwaitTaskWrap Create(AutoResetUniTaskCompletionSource taskCompletionSource, CancellationTokenRegistration? cancellationTokenRegistration)
        {
            AwaitTaskWrap awaitDataWrap = ReferencePool.Acquire<AwaitTaskWrap>();
            awaitDataWrap.TaskCompletionSource = taskCompletionSource;
            awaitDataWrap.CancellationTokenRegistration = cancellationTokenRegistration;
            return awaitDataWrap;
        }

        public void Clear()
        {
            TaskCompletionSource = null;
            CancellationTokenRegistration = null;
        }
    }
    
    /// <summary>
    /// Await包装类
    /// </summary>
    public class AwaitTaskWrap<T> : IReference
    {
        /// <summary>
        /// TaskCompletionSource
        /// </summary>
        public AutoResetUniTaskCompletionSource<T> TaskCompletionSource { get; private set; }
        
        public CancellationTokenRegistration? CancellationTokenRegistration { get; private set; }

        public static AwaitTaskWrap<T> Create(AutoResetUniTaskCompletionSource<T> taskCompletionSource, CancellationTokenRegistration? cancellationTokenRegistration)
        {
            AwaitTaskWrap<T> awaitDataWrap = ReferencePool.Acquire<AwaitTaskWrap<T>>();
            awaitDataWrap.TaskCompletionSource = taskCompletionSource;
            awaitDataWrap.CancellationTokenRegistration = cancellationTokenRegistration;
            return awaitDataWrap;
        }

        public void Clear()
        {
            TaskCompletionSource = null;
            CancellationTokenRegistration = null;
        }
    }
}