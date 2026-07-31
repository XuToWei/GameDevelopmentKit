# 联机幸存者 Demo 架构需求

> 状态：实现基线 v1.0  
> 当前阶段：第一版可编译玩法骨架已经实现，等待接入独立启动入口并进行双客户端联调  
> 配套文档：[联机幸存者Demo整体伪代码.md](./联机幸存者Demo整体伪代码.md)
> 实现说明：[联机幸存者Demo实现说明.md](./联机幸存者Demo实现说明.md)

## 0. 已确认决策

| 编号 | 决策 |
|---|---|
| D-001 | 联机模型采用服务器权威状态同步，不采用确定性帧同步。 |
| D-002 | 所有 Survivor 业务 System 字面上禁止声明局部变量。 |
| D-003 | ET Component 只作为逻辑入口和 ReactiveBinding 根载体；具体玩法数据使用内部普通数据结构表达。 |
| D-004 | 每个房间实例（服务器权威实例或客户端副本）只有一个 `SurvivorWorldComponent : IVersionSync` 和一个 `SyncContext`。 |
| D-005 | “加房”只实现输入房间号加入，不做房间列表、自动匹配等额外功能。 |
| D-006 | 游戏开始后不允许新玩家中途加入。 |
| D-007 | 不修改现有 ET Demo、LockStep Demo 的业务代码，新建 SurvivorOnline 目录实现。 |
| D-008 | 允许扩展共享 `SceneType`、启动配置、Proto、Luban 配置及生成代码；新增 Proto 和配置源文件尽量独立。 |
| D-009 | 怪物等场景表现使用 `UGFEntity + SpriteRenderer`，UI 使用 `UGFUIForm`。 |
| D-010 | 房间不存在时由第一个加入者自动创建；每房最多 4 人；首位玩家是房主，只有房主可以开始。 |
| D-011 | 第一版不做客户端预测和断线重连；客户端直接显示最近一次服务器权威状态。 |
| D-012 | 服务器逻辑 20 Tick/s，每 2 Tick 广播一次状态（10 Hz），每 50 Tick 广播一次完整快照。 |
| D-013 | 非持久的枚举器、索引和计算中间量放在 `SurvivorWorldRuntime`；只有会跨 Tick 影响结果的状态才允许进入 ReactiveBinding 数据图。 |
| D-014 | 数据派生逻辑、客户端 Entry 对账和表现刷新必须使用 `[ReactiveSource] + [ReactiveBind]`；禁止用每帧手写数据比较替代。 |
| D-015 | ReactiveObserver 定义放在 Model/ModelView；Hotfix/HotfixView 只提供无状态 Sink 和 System，实现程序集依赖单向。 |

## 1. 目标

在现有 GameDevelopmentKit 中新增一个独立的联机幸存者 Demo：

- 服务器运行权威玩法逻辑；
- 客户端只发送输入和玩家选择；
- ET `Entity / Component / System` 负责业务组织和生命周期；
- ReactiveBinding 数据图是唯一权威玩法状态；
- 服务器通过 ReactiveBinding 完整快照和增量快照同步状态；
- 恢复快照后可以重建 ET 运行时实体和客户端表现；
- 不依赖正式美术资源即可完成多人玩法闭环。

## 2. 总体架构

```text
客户端输入
    ↓
Gate / SurvivorLobby
    ↓
SurvivorRoom（服务器权威）
    ↓
Survivor Systems 修改 SurvivorWorldData
    ↓
SyncContext.CaptureFull / CaptureDelta
    ↓
SurvivorStateFrame
    ↓
客户端 SyncContext.Apply
    ↓
根据 WorldData 对账 ET Entry Entity
    ↓
UGFEntity / UGFUIForm
```

核心边界：

- ET Component 是入口，不重复保存玩法字段；
- ReactiveBinding 数据图是唯一事实来源；
- System 只通过入口 Component 找到并修改数据；
- 网络层只传命令和状态帧；
- View 层只消费状态，不写回权威状态；
- 快照恢复只恢复数据，ET Entity 和 UGF 表现通过数据重新构建。

## 3. 数据所有权

### 3.1 唯一同步根

每个房间由 ET 创建一个根 Component：

```text
SurvivorRoom
└── SurvivorWorldComponent : Entity, IVersionSync
    └── SurvivorWorldData : IVersionSync
        ├── Room
        ├── Players
        ├── Monsters
        ├── Projectiles
        └── Pickups

    SurvivorWorldRuntime（不进入快照）
        ├── SyncContext / Stream
        ├── Enumerator
        ├── Removal StateId List
        └── 当前 Tick 的临时计算槽
```

`SurvivorWorldComponent` 由 ET `AddComponent` 创建，然后作为根节点调用：

```csharp
worldComponent.AttachTo(syncContext);
```

ReactiveBinding 在 Apply 时动态创建的对象全部是普通数据类型，不是 ET Entity 或 ET Component，因此不会绕过 ET 生命周期。

服务端权威实例创建 Component 时初始化完整 Data；客户端副本和服务端恢复实例只创建空根，内部 Data 由完整快照 Apply 创建。该差异通过 Awake 参数明确表达，不能依赖隐含初始化顺序。

### 3.2 动态数据

建议的数据容器：

```text
VersionSyncDictionary<long, SurvivorPlayerData>
VersionSyncDictionary<long, SurvivorMonsterData>
VersionSyncDictionary<long, SurvivorProjectileData>
VersionSyncDictionary<long, SurvivorPickupData>

VersionSyncList<long> PlayerOrder
VersionSyncList<long> MonsterOrder
VersionSyncList<long> ProjectileOrder
VersionSyncList<long> PickupOrder
```

字典负责按稳定 ID 查找数据；顺序列表负责定义明确的遍历顺序。

### 3.3 所有权规则

- 同一 `IVersionSync` 对象只能属于一个 ReactiveBinding 父节点；
- 不把 ET Entity、ET Component、UGFEntity、GameObject 或 Unity Component 放入同步集合；
- 跨玩法对象引用只保存稳定 ID；
- ET Entry Component 只保存对应的 `StateId`；
- Entry Component 不持有第二份生命、位置、冷却等玩法状态；
- View 映射只保存运行时引用，可以从同步数据重建；
- 配置通过 `ConfigId` 引用 Luban 表，不把配置对象放入快照。

## 4. ET 运行时结构

### 4.1 服务端房间

```text
SurvivorRoom
├── SurvivorWorldComponent : IVersionSync
├── SurvivorSimulationComponent
├── SurvivorCommandQueueComponent
├── SurvivorSnapshotComponent
├── SurvivorRuntimeEntityComponent
└── SurvivorRoomSessionComponent
```

### 4.2 服务端逻辑 Entry Entity

```text
SurvivorPlayer
└── SurvivorPlayerEntryComponent(StateId)

SurvivorMonster
└── SurvivorMonsterEntryComponent(StateId)

SurvivorProjectile
└── SurvivorProjectileEntryComponent(StateId)

SurvivorPickup
└── SurvivorPickupEntryComponent(StateId)
```

Entry Entity 提供：

- ET 生命周期；
- System 分发入口；
- `StateId -> WorldData` 的查找入口；
- 可重建的运行时组织。

Entry Entity 不属于 ReactiveBinding 对象图。

### 4.3 客户端

```text
SurvivorRoom（客户端副本，SceneType.SurvivorClient）
├── SurvivorWorldComponent : IVersionSync
├── SurvivorStateFrameApplyComponent
├── SurvivorClientEntityReconcileComponent
├── SurvivorViewRegistryComponent
└── UIComponent
```

客户端先 Apply 完整状态帧，再根据 Players、Monsters、Projectiles 和 Pickups 对账客户端 ET Entry Entity 与 UGFEntity。

## 5. System 规则

### 5.1 零局部变量

所有 Survivor 业务 System：

- 禁止局部变量声明；
- 禁止 `foreach`；
- 禁止捕获变量和闭包；
- 禁止局部函数；
- 禁止 LINQ；
- 禁止迭代器；
- 禁止静态可变玩法状态；
- 核心 Tick System 禁止 `async/await`；
- 方法参数允许使用；
- 循环索引和临时计算值使用 `SurvivorWorldRuntime` 字段；
- 会影响恢复后结果的中间状态必须进入 ReactiveBinding 数据图。
- Runtime 临时槽必须由每个 System 在使用前覆盖或清空，且只允许在一个完整 Tick 内生效；
- 快照只在完整 Tick 边界 Capture/Apply，绝不恢复到 System 方法执行中间。

该规则必须用 Roslyn Analyzer 或工程现有分析器自动检查。

### 5.2 System 职责

`Simulation System`：

- 读取当前 WorldData 和本 Tick 已确认输入；
- 修改 WorldData；
- 不操作网络、UGF 或 Unity 对象。

`Synchronization System`：

- 捕获、发送和应用快照；
- 不包含移动、伤害、刷怪或升级规则。

`Runtime Reconcile System`：

- 根据 WorldData 创建或删除 ET Entry Entity；
- 不修改权威玩法数据。

`View System`：

- 根据 WorldData 创建、更新和回收 UGFEntity；
- 刷新 UGFUIForm；
- 不直接修改权威玩法数据。

### 5.3 ReactiveBind 粒度与调度

- 玩家派生规则分别监听 `Hp`、`Experience`；
- 怪物死亡规则只监听 `Hp`；
- 集合结构变化使用 `PlayerSetRevision`、`MonsterSetRevision`、`ProjectileSetRevision`、`PickupSetRevision`；
- ET Entry 对账只监听上述 MembershipRevision，不因位置或生命变化重复全量对账；
- UGFEntity 分别监听对应状态的 `PositionX`、`PositionY`；
- Lobby/HUD 分别监听实际显示字段；
- 服务端在相关字段写入完成后调用对应 Observer；
- 客户端只在完整或增量快照 Apply 完成后统一提交表现 Observer；
- 新创建的 UGFEntity/UI 在 OnShow/OnOpen 执行一次初始 Observe；
- 除输入采样外，不在 UGF Update 中轮询数据或调用 `ObserveChanges()`。

ReactiveBind 回调不直接承载 ET Hotfix 状态。Model/ModelView Observer 只识别变化并调用无状态 Sink，Sink 再进入 Hotfix/HotfixView System。

## 6. 服务器权威状态同步

### 6.1 客户端上行

客户端只发送：

- 移动输入；
- 升级选择；
- 准备、开始等房间操作；
- 必要的状态帧确认或完整快照请求。

客户端不发送：

- 最终位置；
- 最终生命；
- 命中结果；
- 伤害结果；
- 怪物死亡结果；
- 经验结算结果。

### 6.2 服务器下行

服务器发送：

```text
SurvivorStateFrame
├── RoomActorId
├── Tick
├── Sequence
├── IsFull
└── SnapshotPayload
```

动态对象新增、删除和字段变化由唯一 WorldData 根的 ReactiveBinding 完整/增量帧表达，不再为每个 ET Component 建立独立 `SyncContext`。

### 6.3 快照原则

- 客户端第一次进入待开始房间时接收完整快照；
- 游戏开始前发送最终完整基线；
- 游戏中发送增量快照；
- 每 2 个逻辑 Tick 捕获并广播状态，每 50 个逻辑 Tick 捕获并广播完整快照；
- 客户端按 Sequence 顺序 Apply；
- 增量序号缺失时停止继续 Apply，并请求完整快照；
- 同一个服务器 `SyncContext` 捕获的完整快照必须广播给房间内全部客户端，不能只发给单个客户端，否则完整捕获会清除待发送 Dirty 状态，其他客户端可能漏掉增量；
- Apply 完整状态帧后再统一刷新 ET Entry Entity、UGFEntity 和 UI；
- 恢复服务器时，先创建根 Component 和 SyncContext，再 Apply 完整快照，最后重建 ET Entry Entity。

## 7. 房间流程

第一版只实现输入房间号加入：

```text
连接/登录
    ↓
输入 RoomCode
    ↓
C2G_JoinSurvivorRoom
    ↓
SurvivorLobby 查询房间
    ├── 房间不存在：自动创建，首位玩家成为房主
    ├── Waiting 且未满：加入
    ├── Running：拒绝
    ├── Finished：拒绝
    └── 已满：拒绝
```

第一版不实现：

- 房间列表；
- 自动匹配；
- 复杂大厅；
- 游戏开始后的中途加入。

第一版固定规则：

- 最大玩家数为 4；
- 房主点击开始，不要求全员准备；
- 游戏开始后拒绝新玩家；
- 不保留掉线座位，不支持断线重连。

## 8. 客户端表现

### 8.1 UGFEntity

建议玩家、怪物、投射物和拾取物都统一使用 UGFEntity：

```text
Player      → 圆形/方形 Sprite，玩家颜色
Monster     → 不同颜色和大小
Elite       → 更大尺寸和特殊颜色
Boss        → 最大尺寸
Projectile  → 小尺寸 Sprite
Pickup      → 小圆形 Sprite
```

`SpriteRenderer` 只负责表现。位置、生命、类型和阵营来自同步数据。

### 8.2 UGFUIForm

最小 UI：

- 输入房间号、加入和房主开始界面；
- 局内 HUD。

升级选择、结算和独立连接入口留给后续迭代。

## 9. 独立目录

```text
Unity/Assets/Scripts/Game/ET/Code/
├── Model/Share/SurvivorOnline/
├── Model/Client/SurvivorOnline/
├── Model/Server/SurvivorOnline/
├── Hotfix/Share/SurvivorOnline/
├── Hotfix/Client/SurvivorOnline/
├── Hotfix/Server/SurvivorOnline/
├── ModelView/Client/SurvivorOnline/
└── HotfixView/Client/SurvivorOnline/
```

```text
Design/Proto/ET-Client/SurvivorOnlineOuter.proto
Design/Proto/ET-ClientServer/SurvivorOnlineInner.proto
Design/Excel/.../SurvivorOnline*.xlsx
Unity/Assets/Res/Entity/SurvivorOnline/
Unity/Assets/Res/UI/UIForm/SurvivorOnline/
```

Proto 不新建独立工程目录和 `proto.conf`，直接复用 ET 原有客户端、客户端服务器共享配置；
只通过 `SurvivorOnline` 文件名前缀隔离协议源文件。

## 10. 最小玩法闭环

建议第一版：

1. 玩家输入相同房间号进入等待房间；
2. 房间开始后拒绝新玩家加入；
3. 玩家移动；
4. 服务器生成怪物；
5. 怪物追逐玩家；
6. 玩家自动攻击；
7. 投射物或范围攻击造成伤害；
8. 怪物死亡并掉落经验；
9. 玩家拾取经验并提升等级；
10. 全部玩家死亡后进入结束状态。

升级三选一、Buff、Boss、复活、复杂技能组合和结算界面不进入当前骨架。

## 11. 验收方向

- 现有 Demo 和 LockStep Demo 业务代码未修改；
- 新业务使用独立目录、新 SceneType、新 Proto 源文件和新配置源文件；
- 相同房间号的玩家进入同一等待房间；
- 游戏开始后加房请求被拒绝；
- 每个服务器房间实例和客户端房间副本各自只有一个 ReactiveBinding 根和一个 SyncContext；
- ReactiveBinding 对象图中不存在 ET Entity 或 ET Component 子节点；
- 所有 Survivor 业务 System 无局部变量声明；
- 完整快照能够重建完整 WorldData；
- 增量快照能够同步新增、修改和删除；
- 恢复完整快照后能够重建 ET Entry Entity；
- 客户端 Apply 后能够重建 UGFEntity 和 UGFUIForm 状态；
- 玩家 HP/经验、怪物 HP 的派生逻辑由字段级 ReactiveBind 触发；
- Entry 对账只由集合 MembershipRevision 触发；
- UGFEntity 和 HUD 不通过每帧数据轮询刷新；
- 服务端与客户端 WorldData 可进行一致性校验；
- View 的创建、回收和丢失不影响权威状态。

## 12. 后续决策

当前骨架没有阻塞性架构问题。进入下一阶段前再决定：

1. 新 Demo 的独立启动入口放在新 Unity Scene、启动参数还是单独 Launcher 配置；
2. 是否加入客户端表现插值；
3. 升级三选一、Buff、Boss 和结算的范围；
4. 玩家死亡后的复活规则；
5. 是否加入断线重连、录像或战斗回放；
6. 是否把当前整型定点坐标规则配置化。
