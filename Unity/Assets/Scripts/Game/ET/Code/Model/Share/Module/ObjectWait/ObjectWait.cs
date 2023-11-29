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

        private class ResultCallback<K>: IDestroyRun where K : struct, IWaitType
        {
            private AutoResetUniTaskCompletionSource<K> tcs;

            public ResultCallback()
            {
                this.tcs = AutoResetUniTaskCompletionSource<K>.Create();
            }

            public bool IsDisposed
            {
                get
                {
                    return this.tcs == null;
                }
            }

            public UniTask<K> Task => this.tcs.Task;

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
        
        public static async UniTask<T> Wait<T>(this ObjectWait self, CancellationTokenSource cts = null) where T : struct, IWaitType
        {
            ResultCallback<T> tcs = new ResultCallback<T>();
            Type type = typeof (T);
            self.tcss.Add(type, tcs);

            void CancelAction()
            {
                self.Notify(new T() { Error = WaitTypeError.Cancel });
            }

            T ret;
            CancellationTokenRegistration? ctr = null;
            try
            {
                ctr = cts?.Token.Register(CancelAction);
                ret = await tcs.Task;
            }
            finally
            {
                ctr?.Dispose();
            }
            return ret;
        }

        public static async UniTask<T> Wait<T>(this ObjectWait self, int timeout, CancellationTokenSource cts = null) where T : struct, IWaitType
        {
            ResultCallback<T> tcs = new ResultCallback<T>();
            async UniTaskVoid WaitTimeout()
            {
                await self.Root().GetComponent<TimerComponent>().WaitAsync(timeout, cts);
                if (cts.IsCancel())
                {
                    return;
                }
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
            
            T ret;
            CancellationTokenRegistration? ctr = null;
            try
            {
                ctr = cts?.Token.Register(CancelAction);
                ret = await tcs.Task;
            }
            finally
            {
                ctr?.Dispose();
            }
            return ret;
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