using System.Threading;
using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [EntitySystemOf(typeof(TestComponent))]
    [FriendOf(typeof(TestComponent))]
    public static partial class TestComponentSystem
    {
        [EntitySystem]
        private static void Awake(this TestComponent self)
        {
            RunTestCancel(self).Forget();
        }
        
        [EntitySystem]
        private static void Update(this TestComponent self)
        {
            //Log.Debug("Test:TestReload");
        }

        private static async UniTaskVoid RunTestCancel(TestComponent self)
        {
            await UniTask.CompletedTask;
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(self.Fiber(), 500).Forget();
            Log.Info($"test time:{TimeInfo.Instance.ClientNow()}");
            async UniTaskVoid Run1()
            {
                Log.Info($"test time1-1:{TimeInfo.Instance.ClientNow()}");
                await self.Fiber().Root.GetComponent<TimerComponent>().WaitAsync(1000, cts.Token);
                Log.Info($"test time1-2:{TimeInfo.Instance.ClientNow()}");
                Log.Info("test state1");
            }
            async UniTaskVoid Run2()
            {
                Log.Info($"test time2-1:{TimeInfo.Instance.ClientNow()}");
                bool canceled = await self.Fiber().Root.GetComponent<TimerComponent>().WaitAsync(1000, cts.Token).SuppressCancellationThrow();
                Log.Info($"test time2-2:{TimeInfo.Instance.ClientNow()}");
                Log.Info($"test state2-{canceled}");
            }
            Run1().Forget();
            Run2().Forget();
        }
    }
}
