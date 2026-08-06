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
                self.ClearViewEntries(self.GetParent<SurvivorClientComponent>());
                self.Runtime.AppliedWorldGeneration = self.WorldGeneration;
            }

            self.ReconcileViewEntities();
        }

        private static void ReconcileViewEntities(this SurvivorViewEntityManagerComponent self)
        {
            SurvivorClientComponent client = self.GetParent<SurvivorClientComponent>();
            SurvivorWorldComponent world = client.World;
            if (world?.Data?.Players == null ||
                world.Data.Monsters == null ||
                world.Data.Projectiles == null ||
                world.Data.Pickups == null)
            {
                self.ClearViewEntries(client);
                return;
            }

            self.Runtime.SeenStateIds.Clear();
            self.Runtime.PlayerStates.Clear();
            self.Runtime.MonsterStates.Clear();
            self.Runtime.ProjectileStates.Clear();
            self.Runtime.PickupStates.Clear();

            self.Runtime.PlayerEnumerator = world.Data.Players.GetEnumerator();
            while (self.Runtime.PlayerEnumerator.MoveNext())
            {
                self.Runtime.StateId = self.Runtime.PlayerEnumerator.Current.Value.StateId;
                self.Runtime.SeenStateIds.Add(self.Runtime.StateId);
                self.Runtime.PlayerStates[self.Runtime.StateId] =
                        self.Runtime.PlayerEnumerator.Current.Value;
                if (client.GetChild<SurvivorPlayerEntry>(self.Runtime.StateId) == null)
                {
                    SurvivorPlayerEntry entry =
                            client.AddChildWithId<SurvivorPlayerEntry>(self.Runtime.StateId);
                    ShowPlayerViewAsync(entry).Forget();
                }
            }

            self.Runtime.PlayerEnumerator.Dispose();
            self.Runtime.PlayerEnumerator = null;

            self.Runtime.MonsterEnumerator = world.Data.Monsters.GetEnumerator();
            while (self.Runtime.MonsterEnumerator.MoveNext())
            {
                self.Runtime.StateId = self.Runtime.MonsterEnumerator.Current.Value.StateId;
                self.Runtime.SeenStateIds.Add(self.Runtime.StateId);
                self.Runtime.MonsterStates[self.Runtime.StateId] =
                        self.Runtime.MonsterEnumerator.Current.Value;
                if (client.GetChild<SurvivorMonsterEntry>(self.Runtime.StateId) == null)
                {
                    SurvivorMonsterEntry entry =
                            client.AddChildWithId<SurvivorMonsterEntry>(self.Runtime.StateId);
                    ShowMonsterViewAsync(entry).Forget();
                }
            }

            self.Runtime.MonsterEnumerator.Dispose();
            self.Runtime.MonsterEnumerator = null;

            self.Runtime.ProjectileEnumerator = world.Data.Projectiles.GetEnumerator();
            while (self.Runtime.ProjectileEnumerator.MoveNext())
            {
                self.Runtime.StateId = self.Runtime.ProjectileEnumerator.Current.Value.StateId;
                self.Runtime.SeenStateIds.Add(self.Runtime.StateId);
                self.Runtime.ProjectileStates[self.Runtime.StateId] =
                        self.Runtime.ProjectileEnumerator.Current.Value;
                if (client.GetChild<SurvivorProjectileEntry>(self.Runtime.StateId) == null)
                {
                    SurvivorProjectileEntry entry =
                            client.AddChildWithId<SurvivorProjectileEntry>(self.Runtime.StateId);
                    ShowProjectileViewAsync(entry).Forget();
                }
            }

            self.Runtime.ProjectileEnumerator.Dispose();
            self.Runtime.ProjectileEnumerator = null;

            self.Runtime.PickupEnumerator = world.Data.Pickups.GetEnumerator();
            while (self.Runtime.PickupEnumerator.MoveNext())
            {
                self.Runtime.StateId = self.Runtime.PickupEnumerator.Current.Value.StateId;
                self.Runtime.SeenStateIds.Add(self.Runtime.StateId);
                self.Runtime.PickupStates[self.Runtime.StateId] =
                        self.Runtime.PickupEnumerator.Current.Value;
                if (client.GetChild<SurvivorPickupEntry>(self.Runtime.StateId) == null)
                {
                    SurvivorPickupEntry entry =
                            client.AddChildWithId<SurvivorPickupEntry>(self.Runtime.StateId);
                    ShowPickupViewAsync(entry).Forget();
                }
            }

            self.Runtime.PickupEnumerator.Dispose();
            self.Runtime.PickupEnumerator = null;
            self.RemoveMissingViewEntries(client);
        }

        private static void RemoveMissingViewEntries(
            this SurvivorViewEntityManagerComponent self,
            SurvivorClientComponent client)
        {
            self.Runtime.RemovalStateIds.Clear();
            self.Runtime.EntryEnumerator = client.Children.Values.GetEnumerator();
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
                client.RemoveChild(self.Runtime.RemovalStateIds[self.Runtime.Index]);
                self.Runtime.Index++;
            }
        }

        private static void ClearViewEntries(
            this SurvivorViewEntityManagerComponent self,
            SurvivorClientComponent client)
        {
            self.Runtime.PlayerStates.Clear();
            self.Runtime.MonsterStates.Clear();
            self.Runtime.ProjectileStates.Clear();
            self.Runtime.PickupStates.Clear();
            self.Runtime.SeenStateIds.Clear();
            self.RemoveMissingViewEntries(client);
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

                SurvivorHealthBarUGFEntity healthBar =
                        entry.AddComponent<SurvivorHealthBarUGFEntity, bool>(true);
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

                SurvivorHealthBarUGFEntity healthBar =
                        entry.AddComponent<SurvivorHealthBarUGFEntity, bool>(false);
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
                SurvivorProjectileUGFEntity projectileView =
                        entry.AddComponent<SurvivorProjectileUGFEntity>();
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

        public static void CreateDamageNumber(
            this SurvivorViewEntityManagerComponent self,
            int damage,
            float positionX,
            float positionY)
        {
            SurvivorCombatFeedbackComponent feedback = self.GetParent<SurvivorClientComponent>()
                    .GetComponent<SurvivorCombatFeedbackComponent>();
            SurvivorDamageNumberEntry entry =
                    feedback.AddChild<SurvivorDamageNumberEntry, int, float, float>(
                        damage,
                        positionX,
                        positionY);
            ShowDamageNumberViewAsync(entry).Forget();
        }

        private static async UniTask ShowDamageNumberViewAsync(SurvivorDamageNumberEntry entry)
        {
            try
            {
                SurvivorDamageNumberUGFEntity damageView =
                        entry.AddComponent<SurvivorDamageNumberUGFEntity>();
                await damageView.ShowEntityAsync(UGFEntityId.SurvivorPickup);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
