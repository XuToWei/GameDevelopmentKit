using System.Collections.Concurrent;

namespace QFSW.QC.Pooling
{
    public class ConcurrentPool<T> : IPool<T> where T : class, new()
    {
        private readonly ConcurrentBag<T> _objs;

        public ConcurrentPool()
        {
            _objs = new ConcurrentBag<T>();
        }

        public ConcurrentPool(int objCount)
        {
            _objs = new ConcurrentBag<T>();
            for (int i = 0; i < objCount; i++)
            {
                _objs.Add(new T());
            }
        }

        public T GetObject()
        {
            if (_objs.TryTake(out T obj))
            {
                return obj;
            }

            return new T();
        }

        public void Release(T obj)
        {
            _objs.Add(obj);
        }
    }
}