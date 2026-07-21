using System;
using Cysharp.Threading.Tasks;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        private delegate bool MoveNextFunc<T>(ref UniTaskCompletionSourceCore<T> core);

        private static UniTask<T> NewUniTask<T>(MoveNextFunc<T> moveNext, Action returnAction)
        {
            return new UniTask<T>(UniTaskConfiguredSource<T>.Create(moveNext, PlayerLoopTiming.LastUpdate, returnAction, out short token), token);
        }

        private sealed class UniTaskConfiguredSource<T> : IUniTaskSource<T>, IPlayerLoopItem, ITaskPoolNode<UniTaskConfiguredSource<T>>
        {
            static TaskPool<UniTaskConfiguredSource<T>> pool;
            UniTaskConfiguredSource<T> nextNode;
            public ref UniTaskConfiguredSource<T> NextNode => ref nextNode;

            static UniTaskConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(UniTaskConfiguredSource<T>), () => pool.Size);
            }

            MoveNextFunc<T> moveNext;
            Action returnAction;

            UniTaskCompletionSourceCore<T> core;

            UniTaskConfiguredSource()
            {

            }

            public static IUniTaskSource<T> Create(MoveNextFunc<T> moveNext, PlayerLoopTiming timing, Action returnAction, out short token)
            {
                if (!pool.TryPop(out var result))
                {
                    result = new UniTaskConfiguredSource<T>();
                }

                result.moveNext = moveNext;
                result.returnAction = returnAction;

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                token = result.core.Version;
                return result;
            }

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

            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public bool MoveNext()
            {
                if (moveNext.Invoke(ref core))
                {
                    return true;
                }

                return false;
            }

            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                moveNext = null;
                returnAction?.Invoke();
                returnAction = null;
                return pool.TryPush(this);
            }
        }
    }
}
