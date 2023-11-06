using System;
using Cysharp.Threading.Tasks;

namespace ET
{
    public interface IDynamicEvent
    {
        public Type ArgType { get; }
        public Type EntityType { get; }
    }
    
    public interface IDynamicEvent<in T> : IDynamicEvent where T: struct
    {
        public UniTask Handle(Scene scene, Entity entity, T arg);
    }

    public abstract class ADynamicEvent<A, B> : IDynamicEvent<B> where A : Entity where B : struct
    {
        public Type ArgType => typeof(B);
        public Type EntityType => typeof(A);
        
        protected abstract UniTask Run(Scene scene, A self, B arg);
        
        public async UniTask Handle(Scene scene, Entity self, B arg)
        {
            try
            {
                await Run(scene, (A)self, arg);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}