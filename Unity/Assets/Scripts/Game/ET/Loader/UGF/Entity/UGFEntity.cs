using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;
using GameEntry = Game.GameEntry;

namespace ET
{
    public abstract class UGFEntity<T> : UGFEntity where T : AETMonoUGFEntity
    {
        [BsonIgnore]
        public T View { get; private set; }

        [BsonIgnore]
        internal override ETMonoUGFEntity UGFMono
        {
            get => base.UGFMono;
            set
            {
                base.UGFMono = value;
                this.View = value.GetComponent<T>();
            }
        }
    }

    [EnableMethod]
    public abstract class UGFEntity : Entity
    {
        [BsonIgnore]
        private UnityGameFramework.Runtime.Entity ugfEntity;
        [BsonIgnore]
        private CancellationTokenSourcePlus cts;
        [BsonIgnore]
        internal virtual ETMonoUGFEntity UGFMono { get; set; }
        [BsonIgnore]
        public Transform CachedTransform { get; internal set; }

        public bool Available => this.ugfEntity != null && !this.ugfEntity.Logic.Available;
        public bool Visible
        {
            get
            {
                return this.ugfEntity != null && this.ugfEntity.Logic.Visible;
            }
            set
            {
                if (this.ugfEntity == null)
                {
                    Log.Warning("Entity is not shown.");
                    return;
                }
                this.ugfEntity.Logic.Visible = value;
            }
        }

        public override void Dispose()
        {
            if (!this.IsDisposed)
            {
                if (this.cts != null)
                {
                    this.cts.Cancel();
                    ObjectPool.Instance.Recycle(this.cts);
                    this.cts = null;
                }
                if (this.ugfEntity != null)
                {
                    GameEntry.Entity.HideEntity(this.ugfEntity);
                    this.ugfEntity = null;
                }
            }
            base.Dispose();
        }

        public async UniTask ShowEntityAsync(int entityTypeId)
        {
            if (this.cts == null)
            {
                this.cts = ObjectPool.Instance.Fetch<CancellationTokenSourcePlus>();
            }
            this.ugfEntity = await GameEntry.Entity.ShowEntityAsync<ETMonoUGFEntity>(entityTypeId, ETMonoUGFEntityData.Create(this), cancellationToken: this.cts.MallocToken());
            this.cts.FreeToken();
            if(this.ugfEntity == null)
            {
                throw new System.Exception($"UGFEntity ShowEntityAsync failed! entityTypeId:'{entityTypeId}'.");
            }
        }

        public async UniTask ShowUIEntityAsync(int entityTypeId)
        {
            if (this.cts == null)
            {
                this.cts = ObjectPool.Instance.Fetch<CancellationTokenSourcePlus>();
            }
            this.ugfEntity = await GameEntry.Entity.ShowUIEntityAsync<ETMonoUGFEntity>(entityTypeId, ETMonoUGFEntityData.Create(this), cancellationToken: this.cts.MallocToken());
            this.cts.FreeToken();
            if(this.ugfEntity == null)
            {
                throw new System.Exception($"UGFEntity ShowUIEntityAsync failed! entityTypeId:'{entityTypeId}'.");
            }
        }

        public void SetEntityVisible(bool visible)
        {
            if (this.ugfEntity != null)
            {
                this.ugfEntity.Logic.Visible = visible;
            }
        }

        public void AttachToParent(UGFEntity parentEntity)
        {
            if (this.ugfEntity != null && parentEntity.ugfEntity != null)
            {
                GameEntry.Entity.AttachEntity(this.ugfEntity, parentEntity.ugfEntity);
            }
        }

        public void DetachFromParent()
        {
            if (this.ugfEntity != null)
            {
                GameEntry.Entity.DetachEntity(this.ugfEntity);
            }
        }
    }
}

