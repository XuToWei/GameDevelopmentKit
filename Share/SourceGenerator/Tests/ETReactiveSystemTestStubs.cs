using System;
using System.Collections.Generic;

namespace ET
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CodeAttribute: BaseAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class StaticFieldAttribute: Attribute
    {
    }

    public interface ISingletonAwake
    {
        void Awake();
    }

    public abstract class ASingleton: IDisposable
    {
        internal abstract void Register();

        public abstract void Dispose();
    }

    public abstract class Singleton<T>: ASingleton where T: Singleton<T>
    {
        public static T? Instance { get; private set; }

        internal override void Register()
        {
            Instance = (T)this;
        }

        public override void Dispose()
        {
            if (ReferenceEquals(Instance, this))
            {
                Instance = null;
            }
        }
    }

    public sealed class World: IDisposable
    {
        private static World? instance;

        private readonly Dictionary<Type, ASingleton> singletons = new();

        public static World Instance => instance ??= new World();

        public T AddSingleton<T>() where T: ASingleton, ISingletonAwake, new()
        {
            T singleton = new();
            singleton.Awake();
            this.AddSingleton(singleton);
            return singleton;
        }

        public void AddSingleton(ASingleton singleton)
        {
            Type type = singleton.GetType();
            if (this.singletons.TryGetValue(type, out ASingleton? previous))
            {
                previous.Dispose();
            }

            this.singletons[type] = singleton;
            singleton.Register();
        }

        public void Dispose()
        {
            foreach (ASingleton singleton in this.singletons.Values)
            {
                singleton.Dispose();
            }

            this.singletons.Clear();
            CodeTypes.Instance.Clear();
            instance = null;
        }
    }

    public sealed class CodeTypes
    {
        private readonly HashSet<Type> types = new();

        public static CodeTypes Instance { get; } = new();

        public void SetTypes(IEnumerable<Type> codeTypes)
        {
            this.types.Clear();
            this.types.UnionWith(codeTypes);
        }

        public HashSet<Type> GetTypes(Type attributeType)
        {
            HashSet<Type> result = new();
            foreach (Type type in this.types)
            {
                if (!type.IsAbstract && type.GetCustomAttributes(attributeType, true).Length > 0)
                {
                    result.Add(type);
                }
            }

            return result;
        }

        public void Clear()
        {
            this.types.Clear();
        }
    }

    public class Entity
    {
        private static long nextInstanceId;

        protected Entity()
        {
            this.InstanceId = ++nextInstanceId;
        }

        public long InstanceId { get; protected set; }
    }
}

namespace MemoryPack
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MemoryPackIgnoreAttribute: Attribute
    {
    }
}

namespace MongoDB.Bson.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class BsonIgnoreAttribute: Attribute
    {
    }
}
