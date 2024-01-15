using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ET
{
    public class NavmeshComponent: Singleton<NavmeshComponent>, ISingletonAwake
    {
        public struct RecastFileLoader
        {
            public string Name { get; set; }
        }

        private readonly Dictionary<string, byte[]> navmeshs = new();
        
        public void Awake()
        {
        }

        public async UniTask LoadAsync(string name)
        {
            byte[] buffer = await EventSystem.Instance.Invoke<NavmeshComponent.RecastFileLoader,
                UniTask<byte[]>>(new NavmeshComponent.RecastFileLoader() { Name = name });
            lock (this)
            {
                this.navmeshs[name] = buffer;
            }
        }

        public byte[] Get(string name)
        {
            lock (this)
            {
                if (this.navmeshs.TryGetValue(name, out byte[] bytes))
                {
                    return bytes;
                }
                
                throw new Exception($"no nav data: {name}");
            }
        }
    }
}