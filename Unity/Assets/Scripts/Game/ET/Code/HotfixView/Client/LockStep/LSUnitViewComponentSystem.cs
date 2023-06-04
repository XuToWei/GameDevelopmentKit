using Game;
using UnityEngine;
using UnityGameFramework.Extension;

namespace ET.Client
{
    public static partial class LSUnitViewComponentSystem
    {
        [EntitySystem]
        private class LSUnitViewComponentAwakeSystem : AwakeSystem<LSUnitViewComponent>
        {
            protected override void Awake(LSUnitViewComponent self)
            {
                Room room = self.Room();
                LSUnitComponent lsUnitComponent = room.LSWorld.GetComponent<LSUnitComponent>();
                foreach (long playerId in room.PlayerIds)
                {
                    LSUnit lsUnit = lsUnitComponent.GetChild<LSUnit>(playerId);
                
                    // GameObject prefab = await GameEntry.Resource.LoadAssetAsync<GameObject>(AssetUtility.GetPrefabAsset("Skeleton/Skeleton"));
                    //
                    // GameObject unitGo = UnityEngine.Object.Instantiate(prefab, GlobalComponent.Instance.Unit, true);
                    GameObject unitGo = new GameObject();
                    unitGo.transform.position = lsUnit.Position.ToVector();

                    LSUnitView lsUnitView = self.AddChildWithId<LSUnitView, GameObject>(lsUnit.Id, unitGo);
                    lsUnitView.AddComponent<LSAnimatorComponent>();
                }
            }
        }
    }
}