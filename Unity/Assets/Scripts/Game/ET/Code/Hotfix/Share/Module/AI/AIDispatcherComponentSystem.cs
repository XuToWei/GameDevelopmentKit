using System;

namespace ET
{
    [FriendOf(typeof(AIDispatcherComponent))]
    public static partial class AIDispatcherComponentSystem
    {
        [EntitySystem]
        private class AIDispatcherComponentAwakeSystem : AwakeSystem<AIDispatcherComponent>
        {
            protected override void Awake(AIDispatcherComponent self)
            {
                AIDispatcherComponent.Instance = self;
                self.Load();
            }
        }

        [EntitySystem]
        private class AIDispatcherComponentDestroySystem : DestroySystem<AIDispatcherComponent>
        {
            protected override void Destroy(AIDispatcherComponent self)
            {
                self.AIHandlers.Clear();
                AIDispatcherComponent.Instance = null;
            }
        }

        [EntitySystem]
        private class AIDispatcherComponentLoadSystem : LoadSystem<AIDispatcherComponent>
        {
            protected override void Load(AIDispatcherComponent self)
            {
                self.Load();
            }
        }

        private static void Load(this AIDispatcherComponent self)
        {
            self.AIHandlers.Clear();
            
            var types = EventSystem.Instance.GetTypes(typeof (AIHandlerAttribute));
            foreach (Type type in types)
            {
                AAIHandler aaiHandler = Activator.CreateInstance(type) as AAIHandler;
                if (aaiHandler == null)
                {
                    Log.Error($"robot ai is not AAIHandler: {type.Name}");
                    continue;
                }
                self.AIHandlers.Add(type.Name, aaiHandler);
            }
        }
    }
}