using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ET.Client
{
    [EntitySystemOf(typeof(OperaComponent))]
    [FriendOf(typeof(OperaComponent))]
    public static partial class OperaComponentSystem
    {
        [EntitySystem]
        private static void Awake(this OperaComponent self)
        {
            self.mapMask = LayerMask.GetMask("Map");
        }

        [EntitySystem]
        private static void Update(this OperaComponent self)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Ray ray = GameEntry.Camera.CurrentSceneCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, self.mapMask))
                {
                    C2M_PathfindingResult c2MPathfindingResult = C2M_PathfindingResult.Create();
                    c2MPathfindingResult.Position = hit.point;
                    self.Root().GetComponent<ClientSenderComponent>().Send(c2MPathfindingResult);
                }
            }

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                self.Test1().Forget();
            }

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                self.Test2().Forget();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                CodeLoaderComponent.Instance.ReloadAsync().Forget();
                return;
            }

            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                C2M_TransferMap c2MTransferMap = C2M_TransferMap.Create();
                self.Root().GetComponent<ClientSenderComponent>().Call(c2MTransferMap).Forget();
            }
        }
        
        private static async UniTaskVoid Test1(this OperaComponent self)
        {
            Log.Debug("Coroutine 1 start1 ");
            using (await self.Root().GetComponent<CoroutineLockComponent>().Wait(1, 20000, 3000))
            {
                await self.Root().GetComponent<TimerComponent>().WaitAsync(6000);
            }
            Log.Debug("Coroutine 1 end1");
        }
        
        private static async UniTaskVoid Test2(this OperaComponent self)
        {
            Log.Debug("Coroutine 2 start2");
            using (await self.Root().GetComponent<CoroutineLockComponent>().Wait(1, 20000, 3000))
            {
                await self.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            }
            Log.Debug("Coroutine 2 end2");
        }
    }
}