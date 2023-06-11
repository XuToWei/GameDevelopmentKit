using System.IO;
using Cysharp.Threading.Tasks;
using MemoryPack;

namespace ET
{
    namespace EventType
    {
        public struct EntryEvent1
        {
        }   
        
        public struct EntryEvent2
        {
        } 
        
        public struct EntryEvent3
        {
        } 
    }
    
    public static class Entry
    {
        public static void Init()
        {
            
        }
        
        public static void Start()
        {
            StartAsync().Forget();
        }

        private static async UniTask Test1()
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB, 1, 2000))
            {
                await TimerComponent.Instance.WaitAsync(100000);
            }
        }
        
        private static async UniTask Test2()
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB, 1, 10000))
            {
                await TimerComponent.Instance.WaitAsync(100000);
            }
        }
        
        private static async UniTask StartAsync()
        {
            WinPeriod.Init();
            
            MongoHelper.Register();
            MongoHelper.RegisterStruct<LSInput>();
            
            Game.AddSingleton<EntitySystemSingleton>();
            Game.AddSingleton<LSEntitySystemSington>();

            Game.AddSingleton<NetServices>();
            Game.AddSingleton<Root>();
            
            await ConfigComponent.Instance.LoadAllAsync();
            Root.Instance.Scene.AddComponent<DynamicEventWatcherComponent>();

            await EventSystem.Instance.PublishAsync(Root.Instance.Scene, new EventType.EntryEvent1());
            await EventSystem.Instance.PublishAsync(Root.Instance.Scene, new EventType.EntryEvent2());
            await EventSystem.Instance.PublishAsync(Root.Instance.Scene, new EventType.EntryEvent3());
        }
    }
}