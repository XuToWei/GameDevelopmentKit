using System.Collections.Generic;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using ET.Client;

namespace ET.Server
{
    public static partial class BenchmarkClientComponentSystem
    {
        [EntitySystem]
        private class BenchmarkClientComponentAwakeSystem : AwakeSystem<BenchmarkClientComponent>
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
            
            Client.NetClientComponent netClientComponent = scene.AddComponent<Client.NetClientComponent, AddressFamily>(AddressFamily.InterNetwork);

            using Session session = netClientComponent.Create(Tables.Instance.DTStartSceneConfig.BenchmarkServer.OuterIPPort);
            List<UniTask> list = new List<UniTask>(100000);

            async UniTask Call(Session s)
            {
                using G2C_Benchmark benchmark = await s.Call(C2G_Benchmark.Create(true)) as G2C_Benchmark;
            }
            
            for (int j = 0; j < 100000000; ++j)
            {
                list.Clear();
                for (int i = 0; i < list.Capacity; ++i)
                {
                    list.Add(Call(session));
                }
                await UniTask.WhenAll(list);
            }
        }
    }
}