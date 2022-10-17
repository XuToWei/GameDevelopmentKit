namespace ET
{
    [Event(SceneType.Process)]
    public class BattleMapEvent_Connection : AEvent<BattleEventType.BattleMap_Connection>
    {
        protected override async ETTask Run(Scene scene, BattleEventType.BattleMap_Connection args)
        {
            BattleComponent battleComponent = scene.GetChild<BattleComponent>(args.BattleId);
            BattleMapComponent battleMapComponent = battleComponent.GetComponent<BattleMapComponent>();
            
            await ETTask.CompletedTask;
        }
    }
}
