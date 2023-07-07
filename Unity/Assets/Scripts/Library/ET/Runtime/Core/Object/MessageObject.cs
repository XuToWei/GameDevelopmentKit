using System;
using MemoryPack;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    public abstract class MessageObject: ProtoObject, IDisposable
    {
        [BsonIgnore]
        [MemoryPackIgnore]
        public bool IsFromPool;

        public virtual void Dispose()
        {
        }

        public virtual void Reset()
        {
        }
    }
}