using System.Collections.Generic;

namespace QFSW.QC.Pooling
{
    public class Pool<T> : IPool<T> where T : class, new()
    {
        private readonly Stack<T> _objs;

        public Pool()
        {
            _objs = new Stack<T>();
        }

        public Pool(int objCount)
        {
            _objs = new Stack<T>(objCount);
            for (int i = 0; i < objCount; i++)
            {
                _objs.Push(new T());
            }
        }

        public T GetObject()
        {
            if (_objs.Count > 0)
            {
                return _objs.Pop();
            }
            else
            {
                return new T();
            }
        }

        public void Release(T obj)
        {
            _objs.Push(obj);
        }
    }
}