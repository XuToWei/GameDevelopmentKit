using Cysharp.Threading.Tasks;

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
        
        private static async UniTask StartAsync()
        {
            WinPeriod.Init();
            
            MongoHelper.Init();
            ProtobufHelper.Init();
            
            Game.AddSingleton<NetServices>();
            Game.AddSingleton<Root>();
            Game.AddSingleton<Tables>();
            await Game.AddSingleton<ConfigComponent>().LoadAllAsync();

            await EventSystem.Instance.PublishAsync(Root.Instance.Scene, new EventType.EntryEvent1());
            await EventSystem.Instance.PublishAsync(Root.Instance.Scene, new EventType.EntryEvent2());
            await EventSystem.Instance.PublishAsync(Root.Instance.Scene, new EventType.EntryEvent3());
        }
    }
}