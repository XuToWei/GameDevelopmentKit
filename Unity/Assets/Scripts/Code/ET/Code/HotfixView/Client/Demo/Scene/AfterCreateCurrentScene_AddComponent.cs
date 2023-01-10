namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterCreateCurrentScene_AddComponent: AEvent<EventType.AfterCreateCurrentScene>
    {
        protected override async ETTask Run(Scene scene, EventType.AfterCreateCurrentScene args)
        {
            
            await ETTask.CompletedTask;
        }
    }
}