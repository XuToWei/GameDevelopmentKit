public class UXSingleton<T> where T : class, new()
{
    protected static T _Instance;

    private static readonly object sysLock = new object();

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                lock (sysLock)
                {
                    if (_Instance == null)
                    {
                        _Instance = new T();
                    }
                }
            }
            return _Instance;
        }
    }

    public static bool HasInstance
    {
        get
        {
            return _Instance != null;
        }
    }

    public virtual void Init()
    {

    }

    public virtual void Close()
    {

    }

    protected void Release()
    {
        _Instance = null;
    }
}
