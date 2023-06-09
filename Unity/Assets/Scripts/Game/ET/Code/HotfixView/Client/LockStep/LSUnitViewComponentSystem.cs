using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = Game.GameEntry;

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
                
                    GameObject unitGo = GameEntry.DataNode.GetData<VarGameObject>("UnitGameObject");
                    
                    unitGo = UnityEngine.Object.Instantiate(unitGo);
                    unitGo.transform.position = lsUnit.Position.ToVector();

                    LSUnitView lsUnitView = self.AddChildWithId<LSUnitView, GameObject>(lsUnit.Id, unitGo);
                    lsUnitView.AddComponent<LSAnimatorComponent>();
                }
            }
        }
    }
}