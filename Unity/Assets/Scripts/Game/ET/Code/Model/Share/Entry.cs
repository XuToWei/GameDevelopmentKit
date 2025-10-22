﻿using Cysharp.Threading.Tasks;

namespace ET
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
    
    public static class Entry
    {
        public static void Init()
        {
            
        }
        
        public static void Start()
        {
            StartAsync().Forget();
        }
        
        private static async UniTaskVoid StartAsync()
        {
            WinPeriod.Init();

            // 注册Mongo type
            MongoRegister.Init();
            // 注册Entity序列化器
            EntitySerializeRegister.Init();
            World.Instance.AddSingleton<IdGenerater>();
            World.Instance.AddSingleton<OpcodeType>();
            World.Instance.AddSingleton<ObjectPool>();
            World.Instance.AddSingleton<MessageQueue>();
            World.Instance.AddSingleton<NetServices>();
            World.Instance.AddSingleton<NavmeshComponent>();
            World.Instance.AddSingleton<LogMsg>();
            World.Instance.AddSingleton<UGFComponent>();

            // 创建需要reload的code singleton
            CodeTypes.Instance.CreateCode();

            await ConfigComponent.Instance.LoadAllAsync();

            await FiberManager.Instance.Create(SchedulerType.Main, ConstFiberId.Main, 0, SceneType.Main, "");
        }
    }
}