using System;

namespace ET.Client
{
    public static partial class GameObjectComponentSystem
    {
        [EntitySystem]
        private class GameObjectComponentDestroySystem : DestroySystem<GameObjectComponent>
        {
            protected override void Destroy(GameObjectComponent self)
            {
                UnityEngine.Object.Destroy(self.GameObject);
            }
        }
    }
}