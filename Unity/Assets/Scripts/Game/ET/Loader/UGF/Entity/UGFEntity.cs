using System;
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
                if (value == null)
                {
                    base.UGFMono = null;
                    this.View = null;
                }
                else
                {
                    base.UGFMono = value;
                    this.View = value.GetComponent<T>();
                }
            }
        }
    }

    [EnableMethod]
    public abstract class UGFEntity : Entity
    {
        [BsonIgnore]
        private UnityGameFramework.Runtime.Entity m_UGFEntity;
        [BsonIgnore]
        private CancellationTokenSourcePlus m_Cts;
        [BsonIgnore]
        internal virtual ETMonoUGFEntity UGFMono { get; set; }
        [BsonIgnore]
        public Transform CachedTransform { get; internal set; }
        [BsonIgnore]
        public bool Available => this.m_UGFEntity != null && this.m_UGFEntity.Logic.Available;
        [BsonIgnore]
        public bool Visible
        {
            get
            {
                return this.m_UGFEntity != null && this.m_UGFEntity.Logic.Visible;
            }
            set
            {
                if (this.m_UGFEntity == null)
                {
                    Log.Warning("Entity is not shown.");
                    return;
                }
                this.m_UGFEntity.Logic.Visible = value;
            }
        }

        public override void Dispose()
        {
            bool isDisposed = this.IsDisposed;
            if (!isDisposed)
            {
                if (this.m_Cts != null)
                {
                    this.m_Cts.Cancel();
                    ObjectPool.Instance.Recycle(this.m_Cts);
                    this.m_Cts = null;
                }
                if (this.Available)
                {
                    GameEntry.Entity.HideEntity(this.m_UGFEntity);
                    this.m_UGFEntity = null;
                }
            }
            base.Dispose();
        }

        public async UniTask ShowEntityAsync(int entityTypeId)
        {
            if (this.m_Cts == null)
            {
                this.m_Cts = ObjectPool.Instance.Fetch<CancellationTokenSourcePlus>();
            }
            this.m_UGFEntity = await GameEntry.Entity.ShowEntityAsync<ETMonoUGFEntity>(entityTypeId, ETMonoUGFEntityData.Create(this), cancellationToken: this.m_Cts.MallocToken());
            this.m_Cts.FreeToken();
            if(this.m_UGFEntity == null)
            {
                throw new Exception($"UGFEntity ShowEntityAsync failed! entityTypeId:'{entityTypeId}'.");
            }
        }

        public async UniTask ShowUIEntityAsync(int entityTypeId)
        {
            if (this.m_Cts == null)
            {
                this.m_Cts = ObjectPool.Instance.Fetch<CancellationTokenSourcePlus>();
            }
            this.m_UGFEntity = await GameEntry.Entity.ShowUIEntityAsync<ETMonoUGFEntity>(entityTypeId, ETMonoUGFEntityData.Create(this), cancellationToken: this.m_Cts.MallocToken());
            this.m_Cts.FreeToken();
            if(this.m_UGFEntity == null)
            {
                throw new Exception($"UGFEntity ShowUIEntityAsync failed! entityTypeId:'{entityTypeId}'.");
            }
        }

        public void SetEntityVisible(bool visible)
        {
            if (this.m_UGFEntity != null)
            {
                this.m_UGFEntity.Logic.Visible = visible;
            }
        }

        public void AttachToParent(UGFEntity parentEntity)
        {
            if (this.m_UGFEntity != null && parentEntity.m_UGFEntity != null)
            {
                GameEntry.Entity.AttachEntity(this.m_UGFEntity, parentEntity.m_UGFEntity);
            }
        }

        public void DetachFromParent()
        {
            if (this.m_UGFEntity != null)
            {
                GameEntry.Entity.DetachEntity(this.m_UGFEntity);
            }
        }
    }
}

