using System;

namespace ET
{
    public sealed class UGFEntity : Entity, IAwake<Type, ETMonoEntity>, ILoad
    {
        public ETMonoEntity ETMonoEntity;

        public Type EntityEventType;
    }
}
