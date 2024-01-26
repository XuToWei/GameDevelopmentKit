using CodeBind;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace ET
{
    [ChildOf]
    public sealed class UGFUIWidget : Entity, IAwake<Transform>, IDestroy
    {
        [BsonIgnore]
        public Transform transform { get; private set; }

        internal void OnAwake(Transform trans)
        {
            this.transform = trans;
        }

        internal void OnDestroy()
        {
            this.transform = default;
        }
    }
    
    [EntitySystemOf(typeof(UGFUIWidget))]
    [FriendOf(typeof(UGFUIWidget))]
    public static partial class UGFUIWidgetSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUIWidget self, Transform trans)
        {
            self.OnAwake(trans);
        }
    
        [EntitySystem]
        private static void Destroy(this UGFUIWidget self)
        {
            self.OnDestroy();
        }
    }
}