using System;
using System.Collections.Generic;
using GameFramework;

namespace UnityGameFramework.Extension
{
    public class UGFList<T> : List<T>, IDisposable, IReference
    {
        public static UGFList<T> Create()
        {
            return ReferencePool.Acquire<UGFList<T>>();
        }
        
        public void Dispose()
        {
            this.Clear();
            ReferencePool.Release(this);
        }
    }
}
