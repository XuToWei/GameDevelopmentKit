using ReactiveBinding;

namespace ET
{
    [EnableClass]
    public partial class SurvivorWorldData: IVersionSync
    {
        [VersionField]
        private long __ServerTick;

        [VersionField]
        private long __NextStateId;

        [VersionField]
        private long __HostPlayerId;

        [VersionField]
        private int __RandomState;

        [VersionField]
        private string __RoomCode;

        [VersionField]
        private SurvivorRoomPhase __Phase;

        [VersionField]
        private long __PlayerSetRevision;

        [VersionField]
        private long __MonsterSetRevision;

        [VersionField]
        private long __ProjectileSetRevision;

        [VersionField]
        private long __PickupSetRevision;

        [VersionField]
        private VersionSyncDictionary<long, SurvivorPlayerState> __Players;

        [VersionField]
        private VersionSyncDictionary<long, SurvivorMonsterState> __Monsters;

        [VersionField]
        private VersionSyncDictionary<long, SurvivorProjectileState> __Projectiles;

        [VersionField]
        private VersionSyncDictionary<long, SurvivorPickupState> __Pickups;
    }

    [EnableClass]
    public partial class SurvivorPlayerState: IVersionSync
    {
        [VersionField]
        private long __StateId;

        [VersionField]
        private long __PlayerId;

        [VersionField]
        private long __LastInputSequence;

        [VersionField]
        private string __DisplayName;

        [VersionField]
        private int __PositionX;

        [VersionField]
        private int __PositionY;

        [VersionField]
        private int __MoveX;

        [VersionField]
        private int __MoveY;

        [VersionField]
        private int __Hp;

        [VersionField]
        private int __MaxHp;

        [VersionField]
        private int __Level;

        [VersionField]
        private int __Experience;

        [VersionField]
        private int __AutoFireCooldown;

        [VersionField]
        private int __SwordWaveCooldown;

        [VersionField]
        private long __SwordWaveRevision;

        [VersionField]
        private bool __Alive;

        [VersionField]
        private int __AutoFireLevel;

        [VersionField]
        private int __PowerShotLevel;

        [VersionField]
        private int __SwiftStepLevel;

        [VersionField]
        private int __UnspentSkillPoints;

        [VersionField]
        private SurvivorSkillType __SkillChoice1;

        [VersionField]
        private SurvivorSkillType __SkillChoice2;

        [VersionField]
        private SurvivorSkillType __SkillChoice3;

        [VersionField]
        private long __SkillChoiceRevision;

        public EntityRef<SurvivorPlayerStateReactiveObserver> LogicObserver { get; set; }
    }

    [EnableClass]
    public partial class SurvivorMonsterState: IVersionSync
    {
        [VersionField]
        private long __StateId;

        [VersionField]
        private long __TargetPlayerId;

        [VersionField]
        private int __ConfigId;

        [VersionField]
        private int __PositionX;

        [VersionField]
        private int __PositionY;

        [VersionField]
        private int __Hp;

        [VersionField]
        private int __MaxHp;

        [VersionField]
        private bool __Alive;

        public EntityRef<SurvivorMonsterStateReactiveObserver> LogicObserver { get; set; }
    }

    [EnableClass]
    public partial class SurvivorProjectileState: IVersionSync
    {
        [VersionField]
        private long __StateId;

        [VersionField]
        private long __OwnerPlayerId;

        [VersionField]
        private int __PositionX;

        [VersionField]
        private int __PositionY;

        [VersionField]
        private int __VelocityX;

        [VersionField]
        private int __VelocityY;

        [VersionField]
        private int __Damage;

        [VersionField]
        private int __RemainingTicks;
    }

    [EnableClass]
    public partial class SurvivorPickupState: IVersionSync
    {
        [VersionField]
        private long __StateId;

        [VersionField]
        private int __PositionX;

        [VersionField]
        private int __PositionY;

        [VersionField]
        private int __Experience;
    }
}
