using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ET
{
    public static class WaitTypeError
    {
        public const int Success = 0;
        public const int Destroy = 1;
        public const int Cancel = 2;
        public const int Timeout = 3;
    }
    
    public interface IWaitType
    {
        int Error
        {
            get;
            set;
        }
    }

    [EntitySystemOf(typeof(ObjectWait))]
    [FriendOf(typeof(ObjectWait))]
    public static partial class ObjectWaitSystem
    {
        [EntitySystem]
        private static void Awake(this ObjectWait self)
        {
            self.tcss.Clear();
        }
        
        [EntitySystem]
        private static void Destroy(this ObjectWait self)
        {
            foreach (object v in self.tcss.Values.ToArray())
            {
                ((IDestroyRun) v).SetResult();
            }
        }

        private interface IDestroyRun
        {
            void SetResult();
        }

        private class ResultCallback<K>: Object, IDestroyRun where K : struct, IWaitType
        {
            private AutoResetUniTaskCompletionSourcePlus<K> tcs;

            public ResultCallback()
            {
                this.tcs = AutoResetUniTaskCompletionSourcePlus<K>.Create();
            }

            public bool IsDisposed
            {
                get
                {
                    return this.tcs == null;
                }
            }

            public AutoResetUniTaskCompletionSourcePlus<K> Task => this.tcs;

            public void SetResult(K k)
            {
                var t = tcs;
                this.tcs = null;
                t.TrySetResult(k);
            }

            public void SetResult()
            {
                var t = tcs;
                this.tcs = null;
                t.TrySetResult(new K() { Error = WaitTypeError.Destroy });
            }
        }
        
        public static UniTask<T> Wait<T>(this ObjectWait self, CancellationToken token = default) where T : struct, IWaitType
        {
            ResultCallback<T> tcs = new ResultCallback<T>();
            Type type = typeof (T);
            self.tcss.Add(type, tcs);
            
            void CancelAction()
            {
                self.Notify(new T() { Error = WaitTypeError.Cancel });
            }
            
            tcs.Task.AddOnCancelAction(CancelAction);
            tcs.Task.AttachCancellation(token);
            return tcs.Task.Task;
        }

        public static UniTask<T> Wait<T>(this ObjectWait self, int timeout, CancellationToken token = default) where T : struct, IWaitType
        {
            if (token.IsCancellationRequested)
            {
                return UniTask.FromCanceled<T>(token);
            }
            
            ResultCallback<T> tcs = new ResultCallback<T>();
            async UniTaskVoid WaitTimeout()
            {
                await self.Root().GetComponent<TimerComponent>().WaitAsync(timeout, token);
                if (tcs.IsDisposed)
                {
                    return;
                }
                self.Notify(new T() { Error = WaitTypeError.Timeout });
            }
            
            WaitTimeout().Forget();
            
            self.tcss.Add(typeof (T), tcs);
            
            void CancelAction()
            {
                self.Notify(new T() { Error = WaitTypeError.Cancel });
            }
            
            tcs.Task.AddOnCancelAction(CancelAction);
            tcs.Task.AttachCancellation(token);
            return tcs.Task.Task;
        }

        public static void Notify<T>(this ObjectWait self, T obj) where T : struct, IWaitType
        {
            Type type = typeof (T);
            if (!self.tcss.TryGetValue(type, out object tcs))
            {
                return;
            }

            self.tcss.Remove(type);
            ((ResultCallback<T>) tcs).SetResult(obj);
        }
    }

    [ComponentOf]
    public class ObjectWait: Entity, IAwake, IDestroy
    {
        public Dictionary<Type, object> tcss = new Dictionary<Type, object>();
    }
}