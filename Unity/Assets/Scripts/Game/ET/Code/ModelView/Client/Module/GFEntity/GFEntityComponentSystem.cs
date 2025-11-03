using System;
using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [FriendOf(typeof(GFEntityComponent))]
    [EntitySystemOf(typeof(GFEntityComponent))]
    public static partial class GFEntityComponentSystem
    {
        [EntitySystem]
        private static void Awake(this GFEntityComponent self)
        {
            
        }

        [EntitySystem]
        private static void Destroy(this GFEntityComponent self)
        {
            
        }

        public static async UniTask<T> AddGFEntityChildAsync<T>(this GFEntityComponent self, int gfEntityTypeId) where T : UGFEntity, IAwake
        {
            T ugfEntity = self.AddChild<T>();
            await ugfEntity.ShowEntityAsync(gfEntityTypeId);
            return ugfEntity;
        }

        public static async UniTask<T> AddGFEntityChildAsync<T, A>(this GFEntityComponent self, int gfEntityTypeId, A a) where T : UGFEntity, IAwake<A>
        {
            T ugfEntity = self.AddChild<T, A>(a);
            await ugfEntity.ShowEntityAsync(gfEntityTypeId);
            return ugfEntity;
        }

        public static async UniTask<T> AddGFEntityChildAsync<T, A, B>(this GFEntityComponent self, int gfEntityTypeId, A a, B b) where T : UGFEntity, IAwake<A, B>
        {
            T ugfEntity = self.AddChild<T, A, B>(a, b);
            await ugfEntity.ShowEntityAsync(gfEntityTypeId);
            return ugfEntity;
        }

        public static async UniTask<T> AddGFEntityChildAsync<T, A, B, C>(this GFEntityComponent self, int gfEntityTypeId, A a, B b, C c) where T : UGFEntity, IAwake<A, B, C>
        {
            T ugfEntity = self.AddChild<T, A, B, C>(a, b, c);
            await ugfEntity.ShowEntityAsync(gfEntityTypeId);
            return ugfEntity;
        }

        public static async UniTask<T> AddGFEntityComponentAsync<T>(this GFEntityComponent self, int gfEntityTypeId) where T : UGFEntity, IAwake, new()
        {
            T ugfEntity = self.AddComponent<T>();
            await ugfEntity.ShowEntityAsync(gfEntityTypeId);
            return ugfEntity;
        }

        public static async UniTask<T> AddGFEntityComponentAsync<T, A>(this GFEntityComponent self, int gfEntityTypeId, A a) where T : UGFEntity, IAwake<A>, new()
        {
            T ugfEntity = self.AddComponent<T, A>(a);
            await ugfEntity.ShowEntityAsync(gfEntityTypeId);
            return ugfEntity;
        }

        public static async UniTask<T> AddGFEntityComponentAsync<T, A, B>(this GFEntityComponent self, int gfEntityTypeId, A a, B b) where T : UGFEntity, IAwake<A, B>, new()
        {
            T ugfEntity = self.AddComponent<T, A, B>(a, b);
            await ugfEntity.ShowEntityAsync(gfEntityTypeId);
            return ugfEntity;
        }

        public static async UniTask<T> AddGFEntityComponentAsync<T, A, B, C>(this GFEntityComponent self, int gfEntityTypeId, A a, B b, C c) where T : UGFEntity, IAwake<A, B, C>, new()
        {
            T ugfEntity = self.AddComponent<T, A, B, C>(a, b, c);
            await ugfEntity.ShowEntityAsync(gfEntityTypeId);
            return ugfEntity;
        }

        public static void HideAllGFEntities(this GFEntityComponent self)
        {
            using var removeChildIds = ListComponent<long>.Create();
            foreach (var child in self.Children.Values)
            {
                if (child is UGFEntity)
                {
                    removeChildIds.Add(child.Id);
                }
            }
            foreach (var childId in removeChildIds)
            {
                self.RemoveChild(childId);
            }

            using var removeComponentTypes = ListComponent<Type>.Create();
            foreach (var component in self.Components.Values)
            {
                if (component is UGFEntity)
                {
                    removeComponentTypes.Add(component.GetType());
                }
            }
            foreach (var componentType in removeComponentTypes)
            {
                self.RemoveComponent(componentType);
            }
        }
    }
}