namespace ET
{
    public class Logger: Singleton<Logger>, ISingletonAwake<ILog>
    {
        public ILog Log { get; private set; }
        
        public void Awake(ILog log)
        {
            this.Log = log;
        }
    }
}