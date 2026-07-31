# 联机幸存者 Demo 整体伪代码

> 状态：架构参考伪代码 v1.0  
> 目标：描述端到端结构与数据流；最终类型和字段以当前实现为准  
> 需求基线：[联机幸存者Demo架构需求.md](./联机幸存者Demo架构需求.md)

> 说明：下文较早的算法草图仍使用 `Data.Scratch` 和顺序列表来直观表达“零局部变量”循环。当前实现没有同步 Scratch，也没有这些顺序列表；对应逻辑使用 `SurvivorWorldRuntime` 临时槽和稳定的字典枚举器。它们仅在完整 Tick 内使用，并在每个 System 使用前重置。

## 1. 核心约定

```text
一个 SurvivorRoom
    = 一个 ET SurvivorWorldComponent
    = 一个 ReactiveBinding 根
    = 一个 SyncContext
    = 一棵 SurvivorWorldData 数据图
```

```text
ET Entity / Component
    负责生命周期、System 入口、运行时组织

ReactiveBinding Data
    负责全部权威玩法状态、快照、恢复、网络同步

ReactiveObserver + ReactiveBind
    负责字段级派生规则、集合结构对账和表现刷新

UGFEntity / UGFUIForm
    负责客户端表现
```

禁止把 ET Entity 或 ET Component 作为 ReactiveBinding 动态子节点。ReactiveBinding 动态创建的对象必须是普通数据结构。

所有 Survivor System 的示例都遵循：

- 不声明局部变量；
- 不使用 `foreach`、LINQ、闭包或局部函数；
- 循环索引和当前 Tick 中间结果放在非版本化 `SurvivorWorldRuntime` 字段中；
- Runtime 槽由每个 System 在使用前覆盖或清空，不能保存跨 Tick 玩法状态；
- 核心 Tick 内不使用 `async/await`；
- 只在完整 Tick 边界捕获和恢复快照。

观察调度约定：

- 玩家 `Hp/Experience`、怪物 `Hp` 使用独立 ReactiveSource；
- 集合结构使用独立 MembershipRevision，避免监听整个 World；
- 客户端 Apply 完成后统一调用一次已注册的 PresentationObserver；
- View 首次创建时 Observe 一次，之后不在 Update 中轮询数据；
- Observer 放 Model/ModelView，ReactiveBind 回调通过无状态 Sink 进入 Hotfix System。

## 2. 模块和 SceneType

### 2.1 SceneType

```csharp
[Flags]
public enum SceneType : long
{
    // 现有值保持不变

    SurvivorLobby  = /* 新的未占用 bit */,
    SurvivorRoom   = /* 新的未占用 bit */,
    SurvivorClient = /* 新的未占用 bit */,
    SurvivorView   = /* 新的未占用 bit */,
}
```

职责：

```text
Gate
    接收客户端 Session 消息

SurvivorLobby
    保存 RoomCode -> RoomActorId
    处理按房间号加入
    创建或查询房间

SurvivorRoom
    运行服务器权威玩法
    捕获并广播快照

SurvivorClient
    保存客户端同步数据
    应用状态帧

SurvivorView
    管理 UGFEntity 和 UGFUIForm
```

### 2.2 独立源文件

```text
Design/Proto/
├── ET-Client/
│   └── SurvivorOnlineOuter.proto
└── ET-ClientServer/
    └── SurvivorOnlineInner.proto

Design/Excel/.../
├── SurvivorOnlineRoom.xlsx
├── SurvivorOnlineMonster.xlsx
├── SurvivorOnlineWeapon.xlsx
├── SurvivorOnlineSkill.xlsx
└── SurvivorOnlineLevel.xlsx
```

Proto 直接复用 `ET-Client/proto.conf` 和 `ET-ClientServer/proto.conf`，不为 SurvivorOnline
新建 Proto 工程目录；通过独立 `.proto` 文件保持业务隔离。

## 3. 协议

### 3.1 加入房间

```proto
// SurvivorOnlineLobby.proto

message C2G_JoinSurvivorRoom
{
    string RoomCode = 1;
}

message G2C_JoinSurvivorRoom
{
    int32 Error = 1;
    string Message = 2;
    string RoomCode = 3;
    int64 PlayerId = 4;
    int64 RoomActorId = 5;
}

message C2Room_ClientReady
{
    int64 PlayerId = 1;
}

message Room2C_StartGame
{
    int64 StartTick = 1;
}
```

### 3.2 玩家输入

```proto
// SurvivorOnlineInput.proto

message C2Room_SurvivorInput
{
    int64 PlayerId = 1;
    int64 InputSequence = 2;
    float MoveX = 3;
    float MoveY = 4;
}

message C2Room_SelectUpgrade
{
    int64 PlayerId = 1;
    int64 UpgradeSequence = 2;
    int32 OptionIndex = 3;
}
```

客户端只提交输入和选择，不提交权威位置、伤害、经验或死亡结果。

### 3.3 状态帧

```proto
// SurvivorOnlineState.proto

message SurvivorStateFrame
{
    int64 RoomActorId = 1;
    int64 Tick = 2;
    int64 Sequence = 3;
    bool IsFull = 4;
    bytes SnapshotPayload = 5;
}

message C2Room_RequestFullSnapshot
{
    int64 LastAppliedSequence = 1;
}
```

## 4. ReactiveBinding 数据图

服务端权威房间和客户端房间副本使用同一个共享 ET 类型，具体职责由 SceneType 区分：

```csharp
[ComponentOf]
public class SurvivorRoom :
    Entity,
    IScene,
    IAwake<SceneType>
{
    public Fiber Fiber { get; set; }
    public SceneType SceneType { get; set; }
    public string Name { get; set; }
}
```

### 4.1 根 ET Component

```csharp
public enum SurvivorWorldRole
{
    ServerAuthority,
    SnapshotConsumer,
}

[ComponentOf(typeof(SurvivorRoom))]
public partial class SurvivorWorldComponent :
    Entity,
    IAwake<SurvivorWorldRole>,
    IVersionSync
{
    [VersionField]
    private SurvivorWorldData __Data;
}
```

`SurvivorWorldComponent` 是 ET Component，同时是 ReactiveBinding 根节点。它只保存一个顶层数据入口。

### 4.2 世界数据

```csharp
public partial class SurvivorWorldData : IVersionSync
{
    [VersionField] private string __RoomCode;
    [VersionField] private SurvivorRoomPhase __Phase;
    [VersionField] private long __Tick;
    [VersionField] private long __NextEntityId;
    [VersionField] private uint __RandomState;
    [VersionField] private int __RoomRuleConfigId;

    [VersionField]
    private VersionSyncDictionary<long, SurvivorPlayerData>
        __Players;

    [VersionField]
    private VersionSyncDictionary<long, SurvivorMonsterData>
        __Monsters;

    [VersionField]
    private VersionSyncDictionary<long, SurvivorProjectileData>
        __Projectiles;

    [VersionField]
    private VersionSyncDictionary<long, SurvivorPickupData>
        __Pickups;

    [VersionField]
    private VersionSyncList<long> __PlayerOrder;

    [VersionField]
    private VersionSyncList<long> __MonsterOrder;

    [VersionField]
    private VersionSyncList<long> __ProjectileOrder;

    [VersionField]
    private VersionSyncList<long> __PickupOrder;

    [VersionField]
    private SurvivorWaveData __Wave;

    [VersionField]
    private SurvivorSimulationScratchData __Scratch;
}
```

### 4.3 玩家数据

```csharp
public partial class SurvivorPlayerData : IVersionSync
{
    [VersionField] private long __EntityId;
    [VersionField] private long __PlayerId;
    [VersionField] private int __CharacterConfigId;

    [VersionField] private float __PositionX;
    [VersionField] private float __PositionY;
    [VersionField] private float __MoveX;
    [VersionField] private float __MoveY;
    [VersionField] private float __MoveSpeed;

    [VersionField] private int __Health;
    [VersionField] private int __MaxHealth;
    [VersionField] private bool __IsDead;

    [VersionField] private int __Level;
    [VersionField] private int __Experience;
    [VersionField] private int __ExperienceToNextLevel;

    [VersionField] private long __LastInputSequence;
    [VersionField] private long __LastUpgradeSequence;
    [VersionField] private long __AttackCooldownEndTick;

    [VersionField] private bool __HasPendingUpgrade;
    [VersionField]
    private VersionSyncList<int> __UpgradeOptions;

    [VersionField]
    private VersionSyncDictionary<int, SurvivorSkillData>
        __Skills;

    [VersionField]
    private VersionSyncDictionary<int, SurvivorBuffData>
        __Buffs;

    // 严格零局部变量下使用的可恢复搜索状态
    [VersionField] private int __TargetScanIndex;
    [VersionField] private long __CandidateEntityId;
    [VersionField] private float __CandidateDistanceSquared;
    [VersionField] private long __TargetEntityId;
    [VersionField] private float __TargetDistanceSquared;
}
```

### 4.4 怪物数据

```csharp
public partial class SurvivorMonsterData : IVersionSync
{
    [VersionField] private long __EntityId;
    [VersionField] private int __MonsterConfigId;

    [VersionField] private float __PositionX;
    [VersionField] private float __PositionY;
    [VersionField] private float __MoveDirectionX;
    [VersionField] private float __MoveDirectionY;
    [VersionField] private float __MoveSpeed;

    [VersionField] private int __Health;
    [VersionField] private int __MaxHealth;
    [VersionField] private bool __IsDead;

    [VersionField] private long __TargetPlayerEntityId;
    [VersionField] private long __AttackCooldownEndTick;

    [VersionField] private int __TargetScanIndex;
    [VersionField] private long __CandidatePlayerEntityId;
    [VersionField] private float __CandidateDistanceSquared;
    [VersionField] private float __TargetDistanceSquared;
}
```

### 4.5 投射物与拾取物

```csharp
public partial class SurvivorProjectileData : IVersionSync
{
    [VersionField] private long __EntityId;
    [VersionField] private long __OwnerPlayerEntityId;
    [VersionField] private int __ProjectileConfigId;

    [VersionField] private float __PositionX;
    [VersionField] private float __PositionY;
    [VersionField] private float __DirectionX;
    [VersionField] private float __DirectionY;
    [VersionField] private float __Speed;

    [VersionField] private int __Damage;
    [VersionField] private float __Radius;
    [VersionField] private long __ExpireTick;
    [VersionField] private bool __Consumed;
}

public partial class SurvivorPickupData : IVersionSync
{
    [VersionField] private long __EntityId;
    [VersionField] private SurvivorPickupType __Type;
    [VersionField] private int __Value;
    [VersionField] private float __PositionX;
    [VersionField] private float __PositionY;
    [VersionField] private bool __Consumed;
}
```

### 4.6 技能、Buff、波次与原始 Scratch 草图

```csharp
public partial class SurvivorSkillData : IVersionSync
{
    [VersionField] private int __SkillConfigId;
    [VersionField] private int __Level;
    [VersionField] private long __CooldownEndTick;
}

public partial class SurvivorBuffData : IVersionSync
{
    [VersionField] private int __BuffConfigId;
    [VersionField] private int __Stack;
    [VersionField] private long __ExpireTick;
}

public partial class SurvivorWaveData : IVersionSync
{
    [VersionField] private int __WaveIndex;
    [VersionField] private long __NextSpawnTick;
    [VersionField] private int __RemainingSpawnCount;
    [VersionField] private int __MonsterConfigId;
}

public partial class SurvivorSimulationScratchData : IVersionSync
{
    [VersionField] private int __PlayerIndex;
    [VersionField] private int __MonsterIndex;
    [VersionField] private int __ProjectileIndex;
    [VersionField] private int __PickupIndex;

    [VersionField] private int __NestedMonsterIndex;
    [VersionField] private int __NestedPlayerIndex;

    [VersionField] private long __CurrentPlayerEntityId;
    [VersionField] private long __CurrentMonsterEntityId;
    [VersionField] private long __CurrentProjectileEntityId;
    [VersionField] private long __CurrentPickupEntityId;

    [VersionField] private float __DistanceSquared;
}
```

这一 `SurvivorSimulationScratchData` 是早期设计草图，当前实现已废弃。正式实现把这些槽放在非版本化 `SurvivorWorldRuntime`，并只允许完整 Tick 边界 Capture/Apply；未来若支持阶段级恢复，必须重新把会跨恢复点的状态纳入同步数据。

## 5. 根数据初始化

```csharp
[EntitySystemOf(typeof(SurvivorWorldComponent))]
public static partial class SurvivorWorldComponentSystem
{
    [EntitySystem]
    private static void Awake(
        this SurvivorWorldComponent self,
        SurvivorWorldRole role)
    {
        if (role != SurvivorWorldRole.ServerAuthority)
        {
            return;
        }

        self.Data = new SurvivorWorldData();
        self.Data.RoomCode = string.Empty;
        self.Data.Phase = SurvivorRoomPhase.Waiting;
        self.Data.Tick = 0;
        self.Data.NextEntityId = 1;
        self.Data.RandomState = SurvivorRandom.InitialState;

        self.Data.Players =
            new VersionSyncDictionary<long, SurvivorPlayerData>();
        self.Data.Monsters =
            new VersionSyncDictionary<long, SurvivorMonsterData>();
        self.Data.Projectiles =
            new VersionSyncDictionary<long, SurvivorProjectileData>();
        self.Data.Pickups =
            new VersionSyncDictionary<long, SurvivorPickupData>();

        self.Data.PlayerOrder = new VersionSyncList<long>();
        self.Data.MonsterOrder = new VersionSyncList<long>();
        self.Data.ProjectileOrder = new VersionSyncList<long>();
        self.Data.PickupOrder = new VersionSyncList<long>();

        self.Data.Wave = new SurvivorWaveData();
        self.Data.Scratch = new SurvivorSimulationScratchData();
    }
}
```

服务端权威实例使用 `ServerAuthority` 初始化完整数据树，再执行 `AttachTo`。客户端和恢复实例使用 `SnapshotConsumer`，Awake 后 `Data` 保持为空，只 Attach 根节点，内部数据由完整快照 Apply 创建。

服务端创建：

```csharp
room.AddComponent<
    SurvivorWorldComponent,
    SurvivorWorldRole>(
        SurvivorWorldRole.ServerAuthority);
room.AddComponent<
    SurvivorSnapshotComponent,
    SurvivorWorldComponent>(
        room.GetComponent<SurvivorWorldComponent>());
```

客户端副本创建：

```csharp
room.AddComponent<
    SurvivorWorldComponent,
    SurvivorWorldRole>(
        SurvivorWorldRole.SnapshotConsumer);
room.AddComponent<
    SurvivorStateFrameApplyComponent,
    SurvivorWorldComponent>(
        room.GetComponent<SurvivorWorldComponent>());
```

## 6. ET 运行时 Entity

### 6.1 Entry Entity

```csharp
[ChildOf(typeof(SurvivorRoom))]
public class SurvivorPlayer : Entity, IAwake<long>
{
    public long StateId;
}

[ChildOf(typeof(SurvivorRoom))]
public class SurvivorMonster : Entity, IAwake<long>
{
    public long StateId;
}

[ChildOf(typeof(SurvivorRoom))]
public class SurvivorProjectile : Entity, IAwake<long>
{
    public long StateId;
}

[ChildOf(typeof(SurvivorRoom))]
public class SurvivorPickup : Entity, IAwake<long>
{
    public long StateId;
}
```

### 6.2 数据入口

```csharp
public static SurvivorWorldComponent World(
    this Entity self)
{
    return self.IScene
        .GetComponent<SurvivorWorldComponent>();
}

public static SurvivorPlayerData State(
    this SurvivorPlayer self)
{
    return self.World().Data.Players[self.StateId];
}

public static SurvivorMonsterData State(
    this SurvivorMonster self)
{
    return self.World().Data.Monsters[self.StateId];
}

public static SurvivorProjectileData State(
    this SurvivorProjectile self)
{
    return self.World().Data.Projectiles[self.StateId];
}

public static SurvivorPickupData State(
    this SurvivorPickup self)
{
    return self.World().Data.Pickups[self.StateId];
}
```

Entry Entity 只保存 `StateId`，玩法字段全部来自 `WorldData`。

## 7. 房间创建与加房

### 7.1 Lobby 目录

```csharp
[ComponentOf(typeof(Scene))]
public class SurvivorRoomDirectoryComponent :
    Entity,
    IAwake
{
    public Dictionary<string, ActorId> Rooms;
}
```

该目录是服务器基础设施，不进入单局玩法快照。

### 7.2 Gate Handler

```csharp
[MessageHandler(SceneType.Gate)]
public sealed class C2G_JoinSurvivorRoomHandler :
    MessageSessionHandler<
        C2G_JoinSurvivorRoom,
        G2C_JoinSurvivorRoom>
{
    protected override async UniTask Run(
        Session session,
        C2G_JoinSurvivorRoom request,
        G2C_JoinSurvivorRoom response)
    {
        response.CopyFrom(
            await session.Root()
                .GetComponent<SurvivorLobbySenderComponent>()
                .JoinByCode(
                    session.GetComponent<SessionPlayerComponent>()
                        .PlayerId,
                    request.RoomCode));
    }
}
```

### 7.3 Lobby 加入规则

```csharp
[EntitySystemOf(typeof(SurvivorRoomDirectoryComponent))]
public static partial class SurvivorRoomDirectoryComponentSystem
{
    public static async UniTask<G2C_JoinSurvivorRoom>
        JoinByCode(
            this SurvivorRoomDirectoryComponent self,
            long playerId,
            string roomCode)
    {
        if (!SurvivorRoomCode.IsValid(roomCode))
        {
            return SurvivorJoinResult.InvalidRoomCode(roomCode);
        }

        if (!self.Rooms.ContainsKey(roomCode))
        {
            return await self.HandleMissingRoom(
                playerId,
                roomCode);
        }

        return await self.Root()
            .GetComponent<MessageSender>()
            .Call<RoomJoinResponse>(
                self.Rooms[roomCode],
                SurvivorLobby2Room_Join.Create(
                    playerId,
                    roomCode));
    }
}
```

`HandleMissingRoom` 的行为待确认：

```text
方案 A：返回 RoomNotFound
方案 B：创建房间，并让第一个玩家加入
```

### 7.4 Room 加入

```csharp
[MessageHandler(SceneType.SurvivorRoom)]
public sealed class SurvivorLobby2Room_JoinHandler :
    MessageHandler<
        SurvivorRoom,
        SurvivorLobby2Room_Join,
        Room2SurvivorLobby_Join>
{
    protected override UniTask Run(
        SurvivorRoom room,
        SurvivorLobby2Room_Join request,
        Room2SurvivorLobby_Join response)
    {
        if (room.GetComponent<SurvivorWorldComponent>()
            .Data.Phase != SurvivorRoomPhase.Waiting)
        {
            response.Error = SurvivorError.GameAlreadyStarted;
            return UniTask.CompletedTask;
        }

        if (room.GetComponent<SurvivorWorldComponent>()
            .Data.PlayerOrder.Count
            >= SurvivorRoomRules.MaxPlayerCount(
                room.GetComponent<SurvivorWorldComponent>()
                    .Data.RoomRuleConfigId))
        {
            response.Error = SurvivorError.RoomFull;
            return UniTask.CompletedTask;
        }

        room.GetComponent<SurvivorPlayerCreateComponent>()
            .CreatePlayer(request.PlayerId);
        room.GetComponent<SurvivorRuntimeReconcileComponent>()
            .ReconcilePlayers();
        response.Error = SurvivorError.None;
        response.RoomActorId = room.GetActorId();
        return UniTask.CompletedTask;
    }
}
```

客户端收到加入成功响应后创建自己的 SurvivorRoom 副本和空同步根，再发送 `C2Room_ClientReady`。Room 收到 Ready 后重新捕获完整快照并广播给房间内所有客户端，不能只发送给新客户端。

```csharp
[MessageHandler(SceneType.SurvivorRoom)]
public sealed class C2Room_ClientReadyHandler :
    MessageHandler<
        SurvivorRoom,
        C2Room_ClientReady>
{
    protected override UniTask Run(
        SurvivorRoom room,
        C2Room_ClientReady request)
    {
        room.GetComponent<SurvivorRoomSessionComponent>()
            .MarkReady(request.PlayerId);
        room.GetComponent<SurvivorSnapshotComponent>()
            .CaptureFullAndBroadcast();
        return UniTask.CompletedTask;
    }
}
```

## 8. 创建玩法数据

### 8.1 创建玩家

```csharp
[ComponentOf(typeof(SurvivorRoom))]
public class SurvivorPlayerCreateComponent :
    Entity,
    IAwake
{
}
```

```csharp
[EntitySystemOf(typeof(SurvivorPlayerCreateComponent))]
public static partial class SurvivorPlayerCreateComponentSystem
{
    public static void CreatePlayer(
        this SurvivorPlayerCreateComponent self,
        long playerId)
    {
        self.World().Data.Scratch.CurrentPlayerEntityId =
            self.World().Data.NextEntityId;
        self.World().Data.NextEntityId += 1;

        self.World().Data.Players.Add(
            self.World().Data.Scratch.CurrentPlayerEntityId,
            new SurvivorPlayerData());

        self.World().Data.PlayerOrder.Add(
            self.World().Data.Scratch.CurrentPlayerEntityId);

        self.World().Data.Players[
            self.World().Data.Scratch.CurrentPlayerEntityId]
            .EntityId =
                self.World().Data.Scratch.CurrentPlayerEntityId;
        self.World().Data.Players[
            self.World().Data.Scratch.CurrentPlayerEntityId]
            .PlayerId = playerId;
        self.World().Data.Players[
            self.World().Data.Scratch.CurrentPlayerEntityId]
            .CharacterConfigId =
                SurvivorDefaults.CharacterConfigId;
        self.World().Data.Players[
            self.World().Data.Scratch.CurrentPlayerEntityId]
            .MaxHealth =
                SurvivorConfigs.Character(
                    SurvivorDefaults.CharacterConfigId)
                    .MaxHealth;
        self.World().Data.Players[
            self.World().Data.Scratch.CurrentPlayerEntityId]
            .Health =
                self.World().Data.Players[
                    self.World().Data.Scratch.CurrentPlayerEntityId]
                    .MaxHealth;
        self.World().Data.Players[
            self.World().Data.Scratch.CurrentPlayerEntityId]
            .MoveSpeed =
                SurvivorConfigs.Character(
                    SurvivorDefaults.CharacterConfigId)
                    .MoveSpeed;
        self.World().Data.Players[
            self.World().Data.Scratch.CurrentPlayerEntityId]
            .UpgradeOptions = new VersionSyncList<int>();
        self.World().Data.Players[
            self.World().Data.Scratch.CurrentPlayerEntityId]
            .Skills =
                new VersionSyncDictionary<int, SurvivorSkillData>();
        self.World().Data.Players[
            self.World().Data.Scratch.CurrentPlayerEntityId]
            .Buffs =
                new VersionSyncDictionary<int, SurvivorBuffData>();
    }
}
```

### 8.2 创建怪物

```csharp
public static void SpawnMonster(
    this SurvivorMonsterSpawnComponent self,
    int monsterConfigId,
    float positionX,
    float positionY)
{
    self.World().Data.Scratch.CurrentMonsterEntityId =
        self.World().Data.NextEntityId;
    self.World().Data.NextEntityId += 1;

    self.World().Data.Monsters.Add(
        self.World().Data.Scratch.CurrentMonsterEntityId,
        new SurvivorMonsterData());
    self.World().Data.MonsterOrder.Add(
        self.World().Data.Scratch.CurrentMonsterEntityId);

    self.World().Data.Monsters[
        self.World().Data.Scratch.CurrentMonsterEntityId]
        .EntityId =
            self.World().Data.Scratch.CurrentMonsterEntityId;
    self.World().Data.Monsters[
        self.World().Data.Scratch.CurrentMonsterEntityId]
        .MonsterConfigId = monsterConfigId;
    self.World().Data.Monsters[
        self.World().Data.Scratch.CurrentMonsterEntityId]
        .PositionX = positionX;
    self.World().Data.Monsters[
        self.World().Data.Scratch.CurrentMonsterEntityId]
        .PositionY = positionY;
    self.World().Data.Monsters[
        self.World().Data.Scratch.CurrentMonsterEntityId]
        .MaxHealth =
            SurvivorConfigs.Monster(monsterConfigId).MaxHealth;
    self.World().Data.Monsters[
        self.World().Data.Scratch.CurrentMonsterEntityId]
        .Health =
            self.World().Data.Monsters[
                self.World().Data.Scratch.CurrentMonsterEntityId]
                .MaxHealth;
    self.World().Data.Monsters[
        self.World().Data.Scratch.CurrentMonsterEntityId]
        .MoveSpeed =
            SurvivorConfigs.Monster(monsterConfigId).MoveSpeed;
}
```

## 9. 输入处理

```csharp
[MessageHandler(SceneType.SurvivorRoom)]
public sealed class C2Room_SurvivorInputHandler :
    MessageHandler<
        SurvivorRoom,
        C2Room_SurvivorInput>
{
    protected override UniTask Run(
        SurvivorRoom room,
        C2Room_SurvivorInput request)
    {
        if (!room.GetComponent<SurvivorWorldComponent>()
            .Data.Players.ContainsKey(
                room.GetComponent<SurvivorPlayerLookupComponent>()
                    .EntityId(request.PlayerId)))
        {
            return UniTask.CompletedTask;
        }

        if (request.InputSequence
            <= room.GetComponent<SurvivorPlayerLookupComponent>()
                .State(request.PlayerId).LastInputSequence)
        {
            return UniTask.CompletedTask;
        }

        room.GetComponent<SurvivorPlayerLookupComponent>()
            .State(request.PlayerId)
            .LastInputSequence = request.InputSequence;
        room.GetComponent<SurvivorPlayerLookupComponent>()
            .State(request.PlayerId)
            .MoveX = SurvivorMath.ClampInput(request.MoveX);
        room.GetComponent<SurvivorPlayerLookupComponent>()
            .State(request.PlayerId)
            .MoveY = SurvivorMath.ClampInput(request.MoveY);

        return UniTask.CompletedTask;
    }
}
```

输入消息在 Room Fiber 中串行处理。最新有效输入写入同步数据，下一逻辑 Tick 使用。

## 10. 服务器 Tick 管线

### 10.1 Tick 入口

```csharp
[ComponentOf(typeof(SurvivorRoom))]
public class SurvivorSimulationComponent :
    Entity,
    IAwake,
    IFixedUpdate
{
}
```

```csharp
[EntitySystemOf(typeof(SurvivorSimulationComponent))]
public static partial class SurvivorSimulationComponentSystem
{
    [EntitySystem]
    private static void FixedUpdate(
        this SurvivorSimulationComponent self)
    {
        if (self.World().Data.Phase
            != SurvivorRoomPhase.Running)
        {
            return;
        }

        self.World().Data.Tick += 1;

        self.GetComponent<SurvivorWaveSystemComponent>()
            .Tick();
        self.GetComponent<SurvivorRuntimeReconcileComponent>()
            .ReconcileAll();
        self.GetComponent<SurvivorPlayerSystemComponent>()
            .TickAll();
        self.GetComponent<SurvivorMonsterSystemComponent>()
            .TickAll();
        self.GetComponent<SurvivorProjectileSystemComponent>()
            .TickAll();
        self.GetComponent<SurvivorPickupSystemComponent>()
            .TickAll();
        self.GetComponent<SurvivorCleanupSystemComponent>()
            .Tick();
        self.GetComponent<SurvivorRuntimeReconcileComponent>()
            .ReconcileAll();
        self.GetComponent<SurvivorRoomEndSystemComponent>()
            .Evaluate();

        self.Room().GetComponent<SurvivorSnapshotComponent>()
            .CaptureAndBroadcast();
    }
}
```

固定执行顺序是架构的一部分，不能依赖 ET Entity 字典的自然枚举顺序。

### 10.2 玩家更新

```csharp
public static void TickAll(
    this SurvivorPlayerSystemComponent self)
{
    self.World().Data.Scratch.PlayerIndex = 0;

    while (self.World().Data.Scratch.PlayerIndex
        < self.World().Data.PlayerOrder.Count)
    {
        self.World().Data.Scratch.CurrentPlayerEntityId =
            self.World().Data.PlayerOrder[
                self.World().Data.Scratch.PlayerIndex];

        self.TickPlayer(
            self.World().Data.Players[
                self.World().Data.Scratch.CurrentPlayerEntityId]);

        self.World().Data.Scratch.PlayerIndex += 1;
    }
}

public static void TickPlayer(
    this SurvivorPlayerSystemComponent self,
    SurvivorPlayerData player)
{
    if (player.IsDead)
    {
        return;
    }

    player.PositionX += player.MoveX
        * player.MoveSpeed
        * SurvivorConstants.FixedDeltaTime;
    player.PositionY += player.MoveY
        * player.MoveSpeed
        * SurvivorConstants.FixedDeltaTime;

    self.FindNearestMonster(player);
    self.TryAutoAttack(player);
}
```

### 10.3 玩家寻找最近怪物

```csharp
public static void FindNearestMonster(
    this SurvivorPlayerSystemComponent self,
    SurvivorPlayerData player)
{
    player.TargetScanIndex = 0;
    player.TargetEntityId = 0;
    player.TargetDistanceSquared = float.MaxValue;

    while (player.TargetScanIndex
        < self.World().Data.MonsterOrder.Count)
    {
        player.CandidateEntityId =
            self.World().Data.MonsterOrder[
                player.TargetScanIndex];
        player.CandidateDistanceSquared =
            SurvivorMath.DistanceSquared(
                player.PositionX,
                player.PositionY,
                self.World().Data.Monsters[
                    player.CandidateEntityId].PositionX,
                self.World().Data.Monsters[
                    player.CandidateEntityId].PositionY);

        if (!self.World().Data.Monsters[
                player.CandidateEntityId].IsDead
            && player.CandidateDistanceSquared
                < player.TargetDistanceSquared)
        {
            player.TargetEntityId =
                player.CandidateEntityId;
            player.TargetDistanceSquared =
                player.CandidateDistanceSquared;
        }

        player.TargetScanIndex += 1;
    }
}
```

### 10.4 自动攻击

```csharp
public static void TryAutoAttack(
    this SurvivorPlayerSystemComponent self,
    SurvivorPlayerData player)
{
    if (player.TargetEntityId == 0)
    {
        return;
    }

    if (self.World().Data.Tick
        < player.AttackCooldownEndTick)
    {
        return;
    }

    if (player.TargetDistanceSquared
        > SurvivorConfigs.Weapon(
            SurvivorDefaults.WeaponConfigId)
            .RangeSquared)
    {
        return;
    }

    self.GetComponent<SurvivorProjectileSystemComponent>()
        .CreateFromPlayer(
            player,
            self.World().Data.Monsters[
                player.TargetEntityId]);

    player.AttackCooldownEndTick =
        self.World().Data.Tick
        + SurvivorConfigs.Weapon(
            SurvivorDefaults.WeaponConfigId)
            .AttackIntervalTicks;
}
```

### 10.5 怪物更新

```csharp
public static void TickAll(
    this SurvivorMonsterSystemComponent self)
{
    self.World().Data.Scratch.MonsterIndex = 0;

    while (self.World().Data.Scratch.MonsterIndex
        < self.World().Data.MonsterOrder.Count)
    {
        self.World().Data.Scratch.CurrentMonsterEntityId =
            self.World().Data.MonsterOrder[
                self.World().Data.Scratch.MonsterIndex];

        self.TickMonster(
            self.World().Data.Monsters[
                self.World().Data.Scratch
                    .CurrentMonsterEntityId]);

        self.World().Data.Scratch.MonsterIndex += 1;
    }
}

public static void TickMonster(
    this SurvivorMonsterSystemComponent self,
    SurvivorMonsterData monster)
{
    if (monster.IsDead)
    {
        return;
    }

    self.FindNearestPlayer(monster);

    if (monster.TargetPlayerEntityId == 0)
    {
        return;
    }

    monster.MoveDirectionX =
        SurvivorMath.DirectionX(
            monster.PositionX,
            monster.PositionY,
            self.World().Data.Players[
                monster.TargetPlayerEntityId].PositionX,
            self.World().Data.Players[
                monster.TargetPlayerEntityId].PositionY);
    monster.MoveDirectionY =
        SurvivorMath.DirectionY(
            monster.PositionX,
            monster.PositionY,
            self.World().Data.Players[
                monster.TargetPlayerEntityId].PositionX,
            self.World().Data.Players[
                monster.TargetPlayerEntityId].PositionY);

    monster.PositionX += monster.MoveDirectionX
        * monster.MoveSpeed
        * SurvivorConstants.FixedDeltaTime;
    monster.PositionY += monster.MoveDirectionY
        * monster.MoveSpeed
        * SurvivorConstants.FixedDeltaTime;

    self.TryAttackPlayer(monster);
}
```

`FindNearestPlayer` 与玩家目标搜索相同，使用 MonsterData 中的扫描字段，不声明局部变量。

### 10.6 投射物更新与命中

```csharp
public static void TickAll(
    this SurvivorProjectileSystemComponent self)
{
    self.World().Data.Scratch.ProjectileIndex = 0;

    while (self.World().Data.Scratch.ProjectileIndex
        < self.World().Data.ProjectileOrder.Count)
    {
        self.World().Data.Scratch.CurrentProjectileEntityId =
            self.World().Data.ProjectileOrder[
                self.World().Data.Scratch.ProjectileIndex];

        self.TickProjectile(
            self.World().Data.Projectiles[
                self.World().Data.Scratch
                    .CurrentProjectileEntityId]);

        self.World().Data.Scratch.ProjectileIndex += 1;
    }
}

public static void TickProjectile(
    this SurvivorProjectileSystemComponent self,
    SurvivorProjectileData projectile)
{
    if (projectile.Consumed)
    {
        return;
    }

    if (self.World().Data.Tick >= projectile.ExpireTick)
    {
        projectile.Consumed = true;
        return;
    }

    projectile.PositionX += projectile.DirectionX
        * projectile.Speed
        * SurvivorConstants.FixedDeltaTime;
    projectile.PositionY += projectile.DirectionY
        * projectile.Speed
        * SurvivorConstants.FixedDeltaTime;

    self.TryHitMonster(projectile);
}

public static void TryHitMonster(
    this SurvivorProjectileSystemComponent self,
    SurvivorProjectileData projectile)
{
    self.World().Data.Scratch.NestedMonsterIndex = 0;

    while (!projectile.Consumed
        && self.World().Data.Scratch.NestedMonsterIndex
            < self.World().Data.MonsterOrder.Count)
    {
        self.World().Data.Scratch.CurrentMonsterEntityId =
            self.World().Data.MonsterOrder[
                self.World().Data.Scratch.NestedMonsterIndex];
        self.World().Data.Scratch.DistanceSquared =
            SurvivorMath.DistanceSquared(
                projectile.PositionX,
                projectile.PositionY,
                self.World().Data.Monsters[
                    self.World().Data.Scratch
                        .CurrentMonsterEntityId].PositionX,
                self.World().Data.Monsters[
                    self.World().Data.Scratch
                        .CurrentMonsterEntityId].PositionY);

        if (!self.World().Data.Monsters[
                self.World().Data.Scratch
                    .CurrentMonsterEntityId].IsDead
            && self.World().Data.Scratch.DistanceSquared
                <= projectile.Radius * projectile.Radius)
        {
            self.World().Data.Monsters[
                self.World().Data.Scratch
                    .CurrentMonsterEntityId].Health
                -= projectile.Damage;
            projectile.Consumed = true;
        }

        self.World().Data.Scratch.NestedMonsterIndex += 1;
    }
}
```

### 10.7 清理、掉落和经验

```csharp
public static void Tick(
    this SurvivorCleanupSystemComponent self)
{
    self.World().Data.Scratch.MonsterIndex = 0;

    while (self.World().Data.Scratch.MonsterIndex
        < self.World().Data.MonsterOrder.Count)
    {
        self.World().Data.Scratch.CurrentMonsterEntityId =
            self.World().Data.MonsterOrder[
                self.World().Data.Scratch.MonsterIndex];

        if (self.World().Data.Monsters[
                self.World().Data.Scratch
                    .CurrentMonsterEntityId].Health <= 0)
        {
            self.CreateExperiencePickup(
                self.World().Data.Monsters[
                    self.World().Data.Scratch
                        .CurrentMonsterEntityId]);
            self.World().Data.Monsters.Remove(
                self.World().Data.Scratch
                    .CurrentMonsterEntityId);
            self.World().Data.MonsterOrder.RemoveAt(
                self.World().Data.Scratch.MonsterIndex);
        }
        else
        {
            self.World().Data.Scratch.MonsterIndex += 1;
        }
    }

    self.RemoveConsumedProjectiles();
    self.RemoveConsumedPickups();
}
```

集合删除通过 ReactiveBinding 集合 API 执行，增量帧会携带集合操作和节点 tombstone。

## 11. 快照捕获

### 11.1 Snapshot Component

```csharp
[ComponentOf(typeof(SurvivorRoom))]
public class SurvivorSnapshotComponent :
    Entity,
    IAwake<SurvivorWorldComponent>
{
    public SurvivorWorldComponent WorldRoot;
    public SyncContext Context;
    public ReusableSyncBuffer Buffer;
    public SurvivorStateFrame OutgoingFrame;
    public long Sequence;
    public int FramesSinceFull;
}
```

### 11.2 初始化

```csharp
[EntitySystemOf(typeof(SurvivorSnapshotComponent))]
public static partial class SurvivorSnapshotComponentSystem
{
    [EntitySystem]
    private static void Awake(
        this SurvivorSnapshotComponent self,
        SurvivorWorldComponent worldRoot)
    {
        self.WorldRoot = worldRoot;
        self.Context = new SyncContext();
        self.Buffer = new ReusableSyncBuffer();
        self.OutgoingFrame = new SurvivorStateFrame();
        self.Sequence = 0;
        self.FramesSinceFull = 0;
        self.WorldRoot.AttachTo(self.Context);
    }
}
```

### 11.3 完整快照

```csharp
public static void CaptureFull(
    this SurvivorSnapshotComponent self)
{
    self.Buffer.ResetForWrite();
    self.Context.CaptureFull(self.Buffer.Writer);

    self.Sequence += 1;
    self.FramesSinceFull = 0;
    self.OutgoingFrame.Tick = self.WorldRoot.Data.Tick;
    self.OutgoingFrame.Sequence = self.Sequence;
    self.OutgoingFrame.IsFull = true;
    self.OutgoingFrame.SnapshotPayload =
        self.Buffer.ToByteArray();
}
```

### 11.4 增量快照

```csharp
public static void CaptureDelta(
    this SurvivorSnapshotComponent self)
{
    self.Buffer.ResetForWrite();
    self.Context.CaptureDelta(self.Buffer.Writer);

    self.Sequence += 1;
    self.FramesSinceFull += 1;
    self.OutgoingFrame.Tick = self.WorldRoot.Data.Tick;
    self.OutgoingFrame.Sequence = self.Sequence;
    self.OutgoingFrame.IsFull = false;
    self.OutgoingFrame.SnapshotPayload =
        self.Buffer.ToByteArray();
}
```

### 11.5 Tick 末广播

```csharp
public static void CaptureAndBroadcast(
    this SurvivorSnapshotComponent self)
{
    if (self.FramesSinceFull
        >= SurvivorConstants.FullSnapshotIntervalFrames)
    {
        self.CaptureFull();
    }
    else
    {
        self.CaptureDelta();
    }

    self.Room()
        .GetComponent<SurvivorRoomSessionComponent>()
        .Broadcast(self.OutgoingFrame);
}

public static void CaptureFullAndBroadcast(
    this SurvivorSnapshotComponent self)
{
    self.CaptureFull();
    self.Room()
        .GetComponent<SurvivorRoomSessionComponent>()
        .Broadcast(self.OutgoingFrame);
}
```

Capture 只发生在完整 Tick 结束后。

使用单一服务器 `SyncContext` 时，任何主动 `CaptureFull` 都必须把该完整帧广播给房间内全部客户端。因为完整捕获会清空 Dirty 集合，只给单个客户端发送会让其他客户端失去尚未收到的变化。

```csharp
[MessageHandler(SceneType.SurvivorRoom)]
public sealed class C2Room_RequestFullSnapshotHandler :
    MessageHandler<
        SurvivorRoom,
        C2Room_RequestFullSnapshot>
{
    protected override UniTask Run(
        SurvivorRoom room,
        C2Room_RequestFullSnapshot request)
    {
        room.GetComponent<SurvivorSnapshotComponent>()
            .CaptureFullAndBroadcast();
        return UniTask.CompletedTask;
    }
}
```

## 12. 客户端 Apply

### 12.1 客户端根

```csharp
[ComponentOf(typeof(SurvivorRoom))]
public class SurvivorStateFrameApplyComponent :
    Entity,
    IAwake<SurvivorWorldComponent>
{
    public SurvivorWorldComponent WorldRoot;
    public SyncContext Context;
    public ReusableSyncBuffer Buffer;
    public long LastSequence;
    public long LastTick;
    public SurvivorStateFrame PendingFrame;
}
```

客户端创建空 `SurvivorWorldComponent`，不初始化 `Data`，只把根节点 Attach 到新 Context。完整 Apply 会创建内部普通数据图。

```csharp
[EntitySystemOf(typeof(SurvivorStateFrameApplyComponent))]
public static partial class SurvivorStateFrameApplyComponentSystem
{
    [EntitySystem]
    private static void Awake(
        this SurvivorStateFrameApplyComponent self,
        SurvivorWorldComponent worldRoot)
    {
        self.WorldRoot = worldRoot;
        self.Context = new SyncContext();
        self.Buffer = new ReusableSyncBuffer();
        self.LastSequence = 0;
        self.LastTick = 0;
        self.WorldRoot.AttachTo(self.Context);
    }
}
```

### 12.2 应用帧

```csharp
[EntitySystemOf(typeof(SurvivorStateFrameApplyComponent))]
public static partial class SurvivorStateFrameApplyComponentSystem
{
    public static void Apply(
        this SurvivorStateFrameApplyComponent self,
        SurvivorStateFrame frame)
    {
        self.PendingFrame = frame;

        if (!self.PendingFrame.IsFull
            && self.PendingFrame.Sequence
                != self.LastSequence + 1)
        {
            self.RequestFullSnapshot();
            self.PendingFrame = null;
            return;
        }

        self.Buffer.ResetForRead(
            self.PendingFrame.SnapshotPayload);
        self.Context.Apply(self.Buffer.Reader);

        self.LastSequence = self.PendingFrame.Sequence;
        self.LastTick = self.PendingFrame.Tick;

        self.Room()
            .GetComponent<SurvivorClientEntityReconcileComponent>()
            .ReconcileAll();
        self.Room()
            .GetComponent<SurvivorViewReconcileComponent>()
            .ReconcileAll();
        self.Room()
            .GetComponent<SurvivorUIRefreshComponent>()
            .RefreshAll();

        self.PendingFrame = null;
    }
}
```

完整 Frame Apply 完成前，不刷新 View。

## 13. ET Entry Entity 对账

```csharp
[ComponentOf(typeof(SurvivorRoom))]
public class SurvivorClientEntityReconcileComponent :
    Entity,
    IAwake
{
    public int PlayerIndex;
    public int MonsterIndex;
    public int ProjectileIndex;
    public int PickupIndex;

    public Dictionary<long, EntityRef<SurvivorPlayer>>
        PlayerEntries;
    public Dictionary<long, EntityRef<SurvivorMonster>>
        MonsterEntries;
    public Dictionary<long, EntityRef<SurvivorProjectile>>
        ProjectileEntries;
    public Dictionary<long, EntityRef<SurvivorPickup>>
        PickupEntries;
}
```

```csharp
public static void ReconcileAll(
    this SurvivorClientEntityReconcileComponent self)
{
    self.ReconcilePlayers();
    self.ReconcileMonsters();
    self.ReconcileProjectiles();
    self.ReconcilePickups();
    self.RemoveMissingEntries();
}

public static void ReconcileMonsters(
    this SurvivorClientEntityReconcileComponent self)
{
    self.MonsterIndex = 0;

    while (self.MonsterIndex
        < self.World().Data.MonsterOrder.Count)
    {
        self.EnsureMonsterEntry(
            self.World().Data.MonsterOrder[
                self.MonsterIndex]);
        self.MonsterIndex += 1;
    }
}

public static void EnsureMonsterEntry(
    this SurvivorClientEntityReconcileComponent self,
    long stateId)
{
    if (self.MonsterEntries.ContainsKey(stateId))
    {
        return;
    }

    self.MonsterEntries.Add(
        stateId,
        self.Room().AddChild<SurvivorMonster, long>(
            stateId));
}
```

删除逻辑以 WorldData 字典为准。Entry Entity 丢失时可以重新创建，不影响权威数据。

## 14. UGFEntity 表现

### 14.1 View Entry

```csharp
[ChildOf(typeof(SurvivorRoom))]
public class SurvivorMonsterView :
    UGFEntity<MonoSurvivorSpriteEntity>,
    IAwake<long>,
    IUGFEntityOnShow,
    IUGFEntityOnUpdate
{
    public long StateId;
}
```

### 14.2 创建

```csharp
public static async UniTask EnsureMonsterView(
    this SurvivorViewReconcileComponent self,
    long stateId)
{
    if (self.MonsterViews.ContainsKey(stateId))
    {
        return;
    }

    self.PendingMonsterView =
        self.Room().AddChild<SurvivorMonsterView, long>(
            stateId);
    await self.PendingMonsterView.ShowEntityAsync(
        SurvivorEntityId.Monster);
    self.MonsterViews.Add(
        stateId,
        self.PendingMonsterView);
    self.PendingMonsterView = null;
}
```

没有声明局部变量；异步加载状态保存在 View Reconcile Component 字段中。正式实现需要保证同类并发加载不会覆盖同一个 Pending 字段，建议每个 View Entry 自己拥有加载状态。

### 14.3 刷新 SpriteRenderer

```csharp
[EntitySystemOf(typeof(SurvivorMonsterView))]
public static partial class SurvivorMonsterViewSystem
{
    [UGFEntitySystem]
    private static void UGFEntityOnUpdate(
        this SurvivorMonsterView self,
        float elapseSeconds,
        float realElapseSeconds)
    {
        self.UGFMono.CachedTransform.position =
            new Vector3(
                self.World().Data.Monsters[
                    self.StateId].PositionX,
                self.World().Data.Monsters[
                    self.StateId].PositionY,
                0);
        self.UGFMono.SpriteRenderer.color =
            SurvivorViewConfigs.MonsterColor(
                self.World().Data.Monsters[
                    self.StateId].MonsterConfigId);
        self.UGFMono.CachedTransform.localScale =
            SurvivorViewConfigs.MonsterScale(
                self.World().Data.Monsters[
                    self.StateId].MonsterConfigId);
    }
}
```

第一版可以直接显示最新服务器位置；后续若需要插值，在 View 层保存 `From/To/RenderTime`，不能反写 WorldData。

## 15. UGFUIForm

### 15.1 UI 划分

```text
UIFormSurvivorJoin
    输入 RoomCode
    发送 C2G_JoinSurvivorRoom

UIFormSurvivorWaiting
    显示 RoomCode、玩家、准备/开始状态

UIFormSurvivorHUD
    显示生命、等级、经验、时间

UIFormSurvivorUpgrade
    显示 UpgradeOptions
    发送 C2Room_SelectUpgrade

UIFormSurvivorResult
    显示结算
```

### 15.2 HUD 刷新

```csharp
[EntitySystemOf(typeof(UIFormSurvivorHUDComponent))]
public static partial class UIFormSurvivorHUDComponentSystem
{
    [UGFUIFormSystem]
    private static void UGFUIFormOnUpdate(
        this UIFormSurvivorHUDComponent self,
        float elapseSeconds,
        float realElapseSeconds)
    {
        self.View.HealthText.text =
            self.LocalPlayerState().Health.ToString();
        self.View.LevelText.text =
            self.LocalPlayerState().Level.ToString();
        self.View.ExperienceSlider.value =
            SurvivorMath.ExperienceProgress(
                self.LocalPlayerState().Experience,
                self.LocalPlayerState()
                    .ExperienceToNextLevel);
        self.View.TimeText.text =
            SurvivorTime.FormatTick(
                self.World().Data.Tick);
    }
}
```

正式实现优先通过 ReactiveBinding 版本变化触发刷新，避免无变化时每帧重复写 UI。

## 16. 升级选择

```csharp
[MessageHandler(SceneType.SurvivorRoom)]
public sealed class C2Room_SelectUpgradeHandler :
    MessageHandler<
        SurvivorRoom,
        C2Room_SelectUpgrade>
{
    protected override UniTask Run(
        SurvivorRoom room,
        C2Room_SelectUpgrade request)
    {
        if (!room.GetComponent<SurvivorPlayerLookupComponent>()
            .State(request.PlayerId)
            .HasPendingUpgrade)
        {
            return UniTask.CompletedTask;
        }

        if (request.UpgradeSequence
            <= room.GetComponent<SurvivorPlayerLookupComponent>()
                .State(request.PlayerId)
                .LastUpgradeSequence)
        {
            return UniTask.CompletedTask;
        }

        if (!SurvivorUpgradeRules.IsValidOption(
                room.GetComponent<SurvivorPlayerLookupComponent>()
                    .State(request.PlayerId),
                request.OptionIndex))
        {
            return UniTask.CompletedTask;
        }

        room.GetComponent<SurvivorUpgradeSystemComponent>()
            .Apply(
                room.GetComponent<SurvivorPlayerLookupComponent>()
                    .State(request.PlayerId),
                request.OptionIndex);

        return UniTask.CompletedTask;
    }
}
```

升级结果只由服务器修改同步数据，客户端 UI 等待状态帧确认。

## 17. 完整快照恢复

### 17.1 服务端恢复

```text
创建 SurvivorRoom
    ↓
ET AddComponent<SurvivorWorldComponent, SurvivorWorldRole>(
    SnapshotConsumer)
    ↓
不要初始化内部 Data
    ↓
创建 SyncContext
    ↓
worldComponent.AttachTo(context)
    ↓
context.Apply(fullSnapshot)
    ↓
校验 RoomCode / Tick / 数据版本
    ↓
ReconcileAll 创建 ET Entry Entity
    ↓
从 Tick + 1 继续运行
```

伪代码：

```csharp
public static void Restore(
    this SurvivorRestoreComponent self,
    byte[] fullSnapshot)
{
    self.Buffer.ResetForRead(fullSnapshot);
    self.Context.Apply(self.Buffer.Reader);
    self.Room()
        .GetComponent<SurvivorRuntimeReconcileComponent>()
        .ReconcileAll();
    self.Room()
        .GetComponent<SurvivorSimulationComponent>()
        .ResumeAt(
            self.World().Data.Tick + 1);
}
```

### 17.2 客户端完整重建

```text
清理旧 ET Entry Entity 和 UGFEntity
    ↓
创建新的空 WorldComponent + SyncContext
    ↓
Apply 完整快照
    ↓
Reconcile ET Entry Entity
    ↓
Reconcile UGFEntity
    ↓
刷新 UGFUIForm
```

恢复点只允许位于完整 Tick 结束后。不会恢复到某个 System 方法体执行到一半的状态。

## 18. 目录建议

```text
Model/Share/SurvivorOnline/
├── Data/
│   ├── SurvivorWorldData.cs
│   ├── SurvivorPlayerData.cs
│   ├── SurvivorMonsterData.cs
│   ├── SurvivorProjectileData.cs
│   └── SurvivorPickupData.cs
├── Room/
├── Player/
├── Monster/
├── Projectile/
└── Message/

Model/Server/SurvivorOnline/
├── Lobby/
├── Room/
└── Runtime/

Hotfix/Share/SurvivorOnline/
├── Simulation/
├── Snapshot/
└── Rules/

Hotfix/Server/SurvivorOnline/
├── Lobby/
├── Room/
├── Input/
└── Snapshot/

ModelView/Client/SurvivorOnline/
├── Entity/
└── UI/

HotfixView/Client/SurvivorOnline/
├── Entity/
└── UI/
```

## 19. 自动化约束与测试

### 19.1 Analyzer

对 Survivor System 扫描并拒绝：

```text
LocalDeclarationStatement
ForEachStatement / ForEachVariableStatement
SimpleLambdaExpression / ParenthesizedLambdaExpression
LocalFunctionStatement
YieldStatement
LINQ 调用
静态可变玩法字段
核心 Tick async 方法
```

### 19.2 数据测试

```text
完整快照 Round Trip
增量字段修改
字典新增节点
字典删除与 tombstone
对象替换
玩家加入
怪物生成/死亡
投射物生成/销毁
升级选择
完整快照恢复后数据一致
```

### 19.3 运行时重建测试

```text
WorldData -> ET Entry Entity 数量一致
WorldData -> UGFEntity 数量一致
Entry Entity 全部引用有效 StateId
删除 View 不影响 WorldData
删除 Entry 后可由 WorldData 重建
完整恢复后可以从下一个 Tick 继续
```

### 19.4 网络测试

```text
相同 RoomCode 加入同一房间
Running 房间拒绝加入
房间满员拒绝加入
乱序/缺失增量触发完整快照请求
完整帧后继续接收增量
客户端不能提交伤害结果
```

## 20. 已落地参数与后续占位

当前实现已固定：

- 不存在的房间号自动创建；
- 每房最多 4 人；
- 首位玩家为房主，房主点击开始；
- 固定逻辑频率 20 Tick/s；
- 每 2 Tick 发送状态，每 50 Tick 发送完整快照；
- 不启用客户端预测和断线重连；
- 使用独立 `SurvivorOnlineOuter.proto`、`SurvivorOnlineInner.proto`；
- 使用新的 `SurvivorLobby`、`SurvivorRoom`、`SurvivorClient`、`SurvivorView` SceneType。

后续仍需设计：

- 独立启动入口；
- 客户端位置插值；
- 升级选择、Buff、Boss 和结算；
- 断线重连、录像或战斗回放。
