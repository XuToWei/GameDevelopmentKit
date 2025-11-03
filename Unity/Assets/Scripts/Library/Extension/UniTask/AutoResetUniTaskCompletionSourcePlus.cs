using System;
using System.Diagnostics;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// 自动重置的UniTask完成源
    /// 扩展了完成、取消、异常的回调
    /// </summary>
    public class AutoResetUniTaskCompletionSourcePlus : IUniTaskSource, ITaskPoolNode<AutoResetUniTaskCompletionSourcePlus>, IPromise
    {
        static TaskPool<AutoResetUniTaskCompletionSourcePlus> pool;
        AutoResetUniTaskCompletionSourcePlus nextNode;
        event Action onExceptionAction;
        event Action onCancelAction;
        event Action onResultAction;
        public ref AutoResetUniTaskCompletionSourcePlus NextNode => ref nextNode;

        static AutoResetUniTaskCompletionSourcePlus()
        {
            TaskPool.RegisterSizeGetter(typeof(AutoResetUniTaskCompletionSourcePlus), () => pool.Size);
        }

        UniTaskCompletionSourceCore<AsyncUnit> core;
        short version;

        AutoResetUniTaskCompletionSourcePlus()
        {
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus Create()
        {
            if (!pool.TryPop(out var result))
            {
                result = new AutoResetUniTaskCompletionSourcePlus();
            }
            result.version = result.core.Version;
            TaskTracker.TrackActiveTask(result, 2);
            return result;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus CreateFromCanceled(CancellationToken cancellationToken, out short token)
        {
            var source = Create();
            source.TrySetCanceled(cancellationToken);
            token = source.core.Version;
            return source;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus CreateFromException(Exception exception, out short token)
        {
            var source = Create();
            source.TrySetException(exception);
            token = source.core.Version;
            return source;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus CreateCompleted(out short token)
        {
            var source = Create();
            source.TrySetResult();
            token = source.core.Version;
            return source;
        }

        public void AddOnCancelAction(Action action)
        {
            onCancelAction += action;
        }

        public void AddOnExceptionAction(Action action)
        {
            onExceptionAction += action;
        }

        public void AddOnResultAction(Action action)
        {
            onResultAction += action;
        }

        public void RemoveOnCancelAction(Action action)
        {
            onCancelAction -= action;
        }

        public void RemoveOnExceptionAction(Action action)
        {
            onExceptionAction -= action;
        }

        public void RemoveOnResultAction(Action action)
        {
            onResultAction -= action;
        }

        public UniTask Task
        {
            [DebuggerHidden]
            get
            {
                return new UniTask(this, core.Version);
            }
        }

        [DebuggerHidden]
        public bool TrySetResult()
        {
            if (version == core.Version && core.TrySetResult(AsyncUnit.Default))
            {
                if (onResultAction != null)
                {
                    onResultAction.Invoke();
                    onResultAction = null;
                }
                return true;
            }
            return false;
        }

        [DebuggerHidden]
        public bool TrySetCanceled(CancellationToken cancellationToken = default)
        {
            if (version == core.Version && core.TrySetCanceled(cancellationToken))
            {
                if (onCancelAction != null)
                {
                    onCancelAction.Invoke();
                    onCancelAction = null;
                }
                return true;
            }
            return false;
        }

        [DebuggerHidden]
        public bool TrySetException(Exception exception)
        {
            if (version == core.Version && core.TrySetException(exception))
            {
                if (onExceptionAction != null)
                {
                    onExceptionAction.Invoke();
                    onExceptionAction = null;
                }
                return true;
            }
            return false;
        }

        [DebuggerHidden]
        public void GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        [DebuggerHidden]
        public UniTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        [DebuggerHidden]
        public UniTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }

        [DebuggerHidden]
        bool TryReturn()
        {
            onExceptionAction = null;
            onCancelAction = null;
            onResultAction = null;
            TaskTracker.RemoveTracking(this);
            core.Reset();
            return pool.TryPush(this);
        }
    }
    
    public class AutoResetUniTaskCompletionSourcePlus<T> : IUniTaskSource<T>, ITaskPoolNode<AutoResetUniTaskCompletionSourcePlus<T>>, IPromise<T>
    {
        static TaskPool<AutoResetUniTaskCompletionSourcePlus<T>> pool;
        AutoResetUniTaskCompletionSourcePlus<T> nextNode;
        event Action onExceptionAction;
        event Action onCancelAction;
        event Action onResultAction;
        public ref AutoResetUniTaskCompletionSourcePlus<T> NextNode => ref nextNode;

        static AutoResetUniTaskCompletionSourcePlus()
        {
            TaskPool.RegisterSizeGetter(typeof(AutoResetUniTaskCompletionSourcePlus<T>), () => pool.Size);
        }

        UniTaskCompletionSourceCore<T> core;
        short version;

        AutoResetUniTaskCompletionSourcePlus()
        {
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus<T> Create()
        {
            if (!pool.TryPop(out var result))
            {
                result = new AutoResetUniTaskCompletionSourcePlus<T>();
            }
            result.version = result.core.Version;
            TaskTracker.TrackActiveTask(result, 2);
            return result;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus<T> CreateFromCanceled(CancellationToken cancellationToken, out short token)
        {
            var source = Create();
            source.TrySetCanceled(cancellationToken);
            token = source.core.Version;
            return source;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus<T> CreateFromException(Exception exception, out short token)
        {
            var source = Create();
            source.TrySetException(exception);
            token = source.core.Version;
            return source;
        }

        [DebuggerHidden]
        public static AutoResetUniTaskCompletionSourcePlus<T> CreateFromResult(T result, out short token)
        {
            var source = Create();
            source.TrySetResult(result);
            token = source.core.Version;
            return source;
        }

        public void AddOnCancelAction(Action action)
        {
            onCancelAction += action;
        }

        public void AddOnExceptionAction(Action action)
        {
            onExceptionAction += action;
        }

        public void AddOnResultAction(Action action)
        {
            onResultAction += action;
        }
        
        public void RemoveOnCancelAction(Action action)
        {
            onCancelAction -= action;
        }

        public void RemoveOnExceptionAction(Action action)
        {
            onExceptionAction -= action;
        }

        public void RemoveOnResultAction(Action action)
        {
            onResultAction -= action;
        }

        public UniTask<T> Task
        {
            [DebuggerHidden]
            get
            {
                return new UniTask<T>(this, core.Version);
            }
        }

        [DebuggerHidden]
        public bool TrySetResult(T result)
        {
            if (version == core.Version && core.TrySetResult(result))
            {
                if (onResultAction != null)
                {
                    onResultAction.Invoke();
                    onResultAction = null;
                }
                return true;
            }
            return false;
        }

        [DebuggerHidden]
        public bool TrySetCanceled(CancellationToken cancellationToken = default)
        {
            if (version == core.Version && core.TrySetCanceled(cancellationToken))
            {
                if (onCancelAction != null)
                {
                    onCancelAction.Invoke();
                    onCancelAction = null;
                }
                return true;
            }
            return false;
        }

        [DebuggerHidden]
        public bool TrySetException(Exception exception)
        {
            if (version == core.Version && core.TrySetException(exception))
            {
                if (onExceptionAction != null)
                {
                    onExceptionAction.Invoke();
                    onExceptionAction = null;
                }
                return true;
            }
            return false;
        }

        [DebuggerHidden]
        public T GetResult(short token)
        {
            try
            {
                return core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        [DebuggerHidden]
        void IUniTaskSource.GetResult(short token)
        {
            GetResult(token);
        }

        [DebuggerHidden]
        public UniTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        [DebuggerHidden]
        public UniTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        [DebuggerHidden]
        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }

        [DebuggerHidden]
        bool TryReturn()
        {
            onExceptionAction = null;
            onCancelAction = null;
            onResultAction = null;
            TaskTracker.RemoveTracking(this);
            core.Reset();
            return pool.TryPush(this);
        }
    }
}