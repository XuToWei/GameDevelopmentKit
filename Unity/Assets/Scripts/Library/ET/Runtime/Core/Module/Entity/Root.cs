namespace ET
{
    // 管理根部的Scene
    public class Root: Singleton<Root>, ISingletonAwake
    {
        public Scene Scene { get; private set; }

        public void Awake()
        {
            this.Scene = EntitySceneFactory.CreateScene(0, SceneType.Process, "Process");
        }

        public override void Dispose()
        {
            base.Dispose();
            this.Scene.Dispose();
        }
    }
}