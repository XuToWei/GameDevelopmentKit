using System;
using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorViewEntityManagerComponent))]
    [ETReactiveSystem]
    public static partial class SurvivorViewEntityManagerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorViewEntityManagerComponent self)
        {
            self.Client = self.GetParent<SurvivorClientComponent>();
            self.Runtime = new SurvivorViewEntityManagerRuntime();
        }

        [EntitySystem]
        private static void Update(this SurvivorViewEntityManagerComponent self)
        {
            self.ObserveChanges();
        }

        [EntitySystem]
        private static void Destroy(this SurvivorViewEntityManagerComponent self)
        {
            self.ClearReactive();
            self.Runtime = null;
            self.Client = null;
            self.WorldGeneration = 0;
        }

        [ETReactiveBind(
            nameof(SurvivorViewEntityManagerComponent.WorldGeneration),
            nameof(SurvivorViewEntityManagerComponent.PlayerSetRevision),
            nameof(SurvivorViewEntityManagerComponent.MonsterSetRevision),
            nameof(SurvivorViewEntityManagerComponent.ProjectileSetRevision),
            nameof(SurvivorViewEntityManagerComponent.PickupSetRevision))]
        private static void OnMembershipChanged(this SurvivorViewEntityManagerComponent self)
        {
            if (self.Runtime.AppliedWorldGeneration != self.WorldGeneration)
            {
                self.ClearViewEntries();
                self.Runtime.AppliedWorldGeneration = self.WorldGeneration;
            }

            self.ReconcileViewEntities();
        }

        private static void ReconcileViewEntities(this SurvivorViewEntityManagerComponent self)
        {
            if (!self.Client.HasBaseline)
            {
                self.ClearViewEntries();
                return;
            }

            SurvivorWorldData world = self.Client.WorldComponent.Data;
            self.Runtime.SeenStateIds.Clear();

            self.Runtime.PlayerEnumerator = world.Players.GetEnumerator();
            while (self.Runtime.PlayerEnumerator.MoveNext())
            {
                self.SyncPlayerEntry(self.Runtime.PlayerEnumerator.Current.Value);
            }

            self.Runtime.PlayerEnumerator.Dispose();
            self.Runtime.PlayerEnumerator = null;

            self.Runtime.MonsterEnumerator = world.Monsters.GetEnumerator();
            while (self.Runtime.MonsterEnumerator.MoveNext())
            {
                self.SyncMonsterEntry(self.Runtime.MonsterEnumerator.Current.Value);
            }

            self.Runtime.MonsterEnumerator.Dispose();
            self.Runtime.MonsterEnumerator = null;

            self.Runtime.ProjectileEnumerator = world.Projectiles.GetEnumerator();
            while (self.Runtime.ProjectileEnumerator.MoveNext())
            {
                self.SyncProjectileEntry(self.Runtime.ProjectileEnumerator.Current.Value);
            }

            self.Runtime.ProjectileEnumerator.Dispose();
            self.Runtime.ProjectileEnumerator = null;

            self.Runtime.PickupEnumerator = world.Pickups.GetEnumerator();
            while (self.Runtime.PickupEnumerator.MoveNext())
            {
                self.SyncPickupEntry(self.Runtime.PickupEnumerator.Current.Value);
            }

            self.Runtime.PickupEnumerator.Dispose();
            self.Runtime.PickupEnumerator = null;
            self.RemoveMissingViewEntries();
        }

        private static void SyncPlayerEntry(this SurvivorViewEntityManagerComponent self, SurvivorPlayerState state)
        {
            self.Runtime.SeenStateIds.Add(state.StateId);
            SurvivorPlayerEntry entry = self.Client.GetChild<SurvivorPlayerEntry>(state.StateId);
            if (entry != null)
            {
                entry.State = state;
                return;
            }

            ShowPlayerViewAsync(self.Client.AddChildWithId<SurvivorPlayerEntry, SurvivorPlayerState>(state.StateId, state)).Forget();
        }

        private static void SyncMonsterEntry(this SurvivorViewEntityManagerComponent self, SurvivorMonsterState state)
        {
            self.Runtime.SeenStateIds.Add(state.StateId);
            SurvivorMonsterEntry entry = self.Client.GetChild<SurvivorMonsterEntry>(state.StateId);
            if (entry != null)
            {
                entry.State = state;
                return;
            }

            ShowMonsterViewAsync(self.Client.AddChildWithId<SurvivorMonsterEntry, SurvivorMonsterState>(state.StateId, state)).Forget();
        }

        private static void SyncProjectileEntry(this SurvivorViewEntityManagerComponent self, SurvivorProjectileState state)
        {
            self.Runtime.SeenStateIds.Add(state.StateId);
            SurvivorProjectileEntry entry = self.Client.GetChild<SurvivorProjectileEntry>(state.StateId);
            if (entry != null)
            {
                entry.State = state;
                return;
            }

            ShowProjectileViewAsync(self.Client.AddChildWithId<SurvivorProjectileEntry, SurvivorProjectileState>(state.StateId, state)).Forget();
        }

        private static void SyncPickupEntry(this SurvivorViewEntityManagerComponent self, SurvivorPickupState state)
        {
            self.Runtime.SeenStateIds.Add(state.StateId);
            SurvivorPickupEntry entry = self.Client.GetChild<SurvivorPickupEntry>(state.StateId);
            if (entry != null)
            {
                entry.State = state;
                return;
            }

            ShowPickupViewAsync(self.Client.AddChildWithId<SurvivorPickupEntry, SurvivorPickupState>(state.StateId, state)).Forget();
        }

        private static void RemoveMissingViewEntries(this SurvivorViewEntityManagerComponent self)
        {
            self.Runtime.RemovalStateIds.Clear();
            self.Runtime.EntryEnumerator = self.Client.Children.Values.GetEnumerator();
            while (self.Runtime.EntryEnumerator.MoveNext())
            {
                Entity entry = self.Runtime.EntryEnumerator.Current;
                if (IsManagedEntry(entry) && !self.Runtime.SeenStateIds.Contains(entry.Id))
                {
                    self.Runtime.RemovalStateIds.Add(entry.Id);
                }
            }

            self.Runtime.EntryEnumerator.Dispose();
            self.Runtime.EntryEnumerator = null;
            self.Runtime.Index = 0;
            while (self.Runtime.Index < self.Runtime.RemovalStateIds.Count)
            {
                self.Client.RemoveChild(self.Runtime.RemovalStateIds[self.Runtime.Index]);
                self.Runtime.Index++;
            }
        }

        private static void ClearViewEntries(this SurvivorViewEntityManagerComponent self)
        {
            self.Runtime.SeenStateIds.Clear();
            self.RemoveMissingViewEntries();
        }

        private static bool IsManagedEntry(Entity entity)
        {
            return entity is SurvivorPlayerEntry ||
                    entity is SurvivorMonsterEntry ||
                    entity is SurvivorProjectileEntry ||
                    entity is SurvivorPickupEntry;
        }

        private static async UniTask ShowPlayerViewAsync(SurvivorPlayerEntry entry)
        {
            EntityRef<SurvivorPlayerEntry> entryRef = entry;
            try
            {
                SurvivorPlayerUGFEntity playerView = entry.AddComponent<SurvivorPlayerUGFEntity>();
                await playerView.ShowEntityAsync(UGFEntityId.SurvivorPlayer);
                entry = entryRef;
                if (entry == null)
                {
                    return;
                }

                SurvivorHealthBarUGFEntity healthBar = entry.AddComponent<SurvivorHealthBarUGFEntity, bool>(true);
                await healthBar.ShowEntityAsync(UGFEntityId.SurvivorPickup);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static async UniTask ShowMonsterViewAsync(SurvivorMonsterEntry entry)
        {
            EntityRef<SurvivorMonsterEntry> entryRef = entry;
            try
            {
                SurvivorMonsterUGFEntity monsterView = entry.AddComponent<SurvivorMonsterUGFEntity>();
                await monsterView.ShowEntityAsync(UGFEntityId.SurvivorMonster);
                entry = entryRef;
                if (entry == null)
                {
                    return;
                }

                SurvivorHealthBarUGFEntity healthBar = entry.AddComponent<SurvivorHealthBarUGFEntity, bool>(false);
                await healthBar.ShowEntityAsync(UGFEntityId.SurvivorPickup);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static async UniTask ShowProjectileViewAsync(SurvivorProjectileEntry entry)
        {
            try
            {
                SurvivorProjectileUGFEntity projectileView = entry.AddComponent<SurvivorProjectileUGFEntity>();
                await projectileView.ShowEntityAsync(UGFEntityId.SurvivorProjectile);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static async UniTask ShowPickupViewAsync(SurvivorPickupEntry entry)
        {
            try
            {
                SurvivorPickupUGFEntity pickupView = entry.AddComponent<SurvivorPickupUGFEntity>();
                await pickupView.ShowEntityAsync(UGFEntityId.SurvivorPickup);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public static void CreateDamageNumber(this SurvivorViewEntityManagerComponent self, int damage, float positionX, float positionY)
        {
            SurvivorDamageNumberEntry entry = self.AddChild<SurvivorDamageNumberEntry, int, float, float>(damage, positionX, positionY);
            ShowDamageNumberViewAsync(entry).Forget();
        }

        private static async UniTask ShowDamageNumberViewAsync(SurvivorDamageNumberEntry entry)
        {
            try
            {
                SurvivorDamageNumberUGFEntity damageView = entry.AddComponent<SurvivorDamageNumberUGFEntity>();
                await damageView.ShowEntityAsync(UGFEntityId.SurvivorPickup);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
