using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    public abstract class MessageObject: ProtoObject, IDisposable
    {
        [BsonIgnore]
        public bool IsFromPool;

        public virtual void Dispose()
        {
        }

        public virtual void Reset()
        {
        }
    }
}