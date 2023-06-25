namespace Game.Hot
{
    public abstract class GameHotModule
    {
        /// <summary>
        /// 运行优先级，越大运行越靠前
        /// </summary>
        protected internal virtual int Priority => 0;
        protected internal abstract void Initialize();
        protected internal abstract void Update(float elapseSeconds, float realElapseSeconds);
        protected internal abstract void Shutdown();
    }
}
