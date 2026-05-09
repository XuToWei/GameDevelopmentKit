using TrueSync;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ET.Client
{
    [EntitySystemOf(typeof(LSOperaComponent))]
    [FriendOf(typeof(LSClientUpdater))]
    public static partial class LSOperaComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.LSOperaComponent self)
        {

        }
        
        [EntitySystem]
        private static void Update(this LSOperaComponent self)
        {
            TSVector2 v = new();
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                v.y += 1;
            }

            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                v.x -= 1;
            }

            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                v.y -= 1;
            }

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                v.x += 1;
            }

            LSClientUpdater lsClientUpdater = self.GetParent<Room>().GetComponent<LSClientUpdater>();
            lsClientUpdater.Input.V = v.normalized;
        }

    }
}