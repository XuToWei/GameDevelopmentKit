namespace Game.Hot
{
    public abstract class GameHotModule
    {
        public virtual int Priority => 0;
        public abstract void Initialize();
        public abstract void Update(float elapseSeconds, float realElapseSeconds);
        public abstract void Shutdown();
    }
}
