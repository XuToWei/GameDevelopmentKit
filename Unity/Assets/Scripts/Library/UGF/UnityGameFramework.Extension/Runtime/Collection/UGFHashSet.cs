using System;
using System.Collections.Generic;
using GameFramework;

namespace UnityGameFramework.Extension
{
    public sealed class UGFHashSet<T> : HashSet<T>, IDisposable, IReference
    {
        public static UGFHashSet<T> Create()
        {
            return ReferencePool.Acquire<UGFHashSet<T>>();
        }
        
        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}