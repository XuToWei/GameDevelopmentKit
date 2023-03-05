using System;
using System.Collections.Generic;

namespace CodeBind
{
    internal class CSCodeBindPool
    {
        private const int MaxCount = 100;

        private readonly Dictionary<Type, Queue<ICSCodeBind>> m_Pool = new Dictionary<Type, Queue<ICSCodeBind>>();

        public T Fetch<T>(CSCodeBindMono mono) where T : ICSCodeBind, new()
        {
            T obj;
            if (!m_Pool.TryGetValue(typeof(T), out var queue))
            {
                obj = Activator.CreateInstance<T>();
            }
            else
            {
                if (queue.Count == 0)
                {
                    obj = Activator.CreateInstance<T>();
                }
                else
                {
                    obj = (T)queue.Dequeue();
                }
            }
            obj.InitBind(mono);
            return obj;
        }

        public void Recycle(ICSCodeBind obj)
        {
            obj.ClearBind();
            Type type = obj.GetType();
            if (!m_Pool.TryGetValue(type, out var queue))
            {
                queue = new Queue<ICSCodeBind>();
                m_Pool.Add(type, queue);
            }

            if (queue.Count > MaxCount)
            {
                return;
            }

            queue.Enqueue(obj);
        }
    }
}