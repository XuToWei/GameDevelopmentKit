using System;
using System.Collections.Generic;
using GameFramework;

namespace UnityGameFramework.Extension
{
    public sealed class UGFQueue<T> : Queue<T>, IDisposable, IReference
    {
        public static UGFQueue<T> Create()
        {
            return ReferencePool.Acquire<UGFQueue<T>>();
        }
        
        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}
