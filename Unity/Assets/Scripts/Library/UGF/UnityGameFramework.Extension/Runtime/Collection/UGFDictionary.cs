using System;
using System.Collections.Generic;
using GameFramework;

namespace UnityGameFramework.Extension
{
    public sealed class UGFDictionary<K, V> : Dictionary<K, V>, IDisposable, IReference
    {
        public static UGFDictionary<K, V> Create()
        {
            return ReferencePool.Acquire<UGFDictionary<K, V>>();
        }
        
        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}
