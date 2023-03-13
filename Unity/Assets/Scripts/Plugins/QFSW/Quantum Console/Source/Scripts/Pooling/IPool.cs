namespace QFSW.QC.Pooling
{
    public interface IPool<T> where T : class, new()
    {
        T GetObject();
        void Release(T obj);
    }
}