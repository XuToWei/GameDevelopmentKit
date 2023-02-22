using System.Collections.Generic;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using ET.Client;

namespace ET.Server
{
    public static class BenchmarkClientComponentSystem
    {
        public class AwakeSystem: AwakeSystem<BenchmarkClientComponent>
        {
            protected override void Awake(BenchmarkClientComponent self)
            {
                for (int i = 0; i < 50; ++i)
                {
                    self.Start().Forget();
                }
            }
        }

        private static async UniTask Start(this BenchmarkClientComponent self)
        {
            await TimerComponent.Instance.WaitAsync(1000);

            Scene scene = await SceneFactory.CreateServerScene(self, IdGenerater.Instance.GenerateId(), IdGenerater.Instance.GenerateInstanceId(),
                self.DomainZone(), "bechmark", SceneType.Benchmark);
            
            NetClientComponent netClientComponent = scene.AddComponent<NetClientComponent, AddressFamily>(AddressFamily.InterNetwork);

            using Session session = netClientComponent.Create(Tables.Instance.DTStartSceneConfig.BenchmarkServer.OuterIPPort);
            List<UniTask<IResponse>> list = new List<UniTask<IResponse>>(100000);
            for (int j = 0; j < 100000000; ++j)
            {
                list.Clear();
                for (int i = 0; i < list.Capacity; ++i)
                {
                    list.Add(session.Call(new C2G_Benchmark()));
                }
                await UniTask.WhenAll(list);
            }
        }
    }
}