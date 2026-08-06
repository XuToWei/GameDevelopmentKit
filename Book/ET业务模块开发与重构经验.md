# ET 业务模块开发与重构经验

本文总结 SurvivorOnline 模块重构过程中形成的工程规则。规则不局限于 Survivor，后续开发新的联机玩法、房间模块或 ETUI 时均可复用。

核心目标是：职责清楚、依赖稳定、对象所有权明确，不通过额外的 Component、Entity、空值兜底或协议对象传递来掩盖设计问题。

## 1. 先划分运行端，再划分业务职责

一个同时包含客户端逻辑、服务器权威逻辑和 Unity 表现的模块，应像 LockStep 一样明确区分四类 `SceneType`：

| SceneType | 职责 | Survivor 示例 |
| --- | --- | --- |
| `XxxClient` | 客户端本地模拟、快照消费或客户端房间 | `SurvivorClient` |
| `XxxServer` | 服务器权威房间 Fiber | `SurvivorServer` |
| `Xxx` | 不依赖 Unity 表现的客户端业务、消息和流程 | `Survivor` |
| `XxxView` | Unity UI、Entity、相机和表现事件 | `SurvivorView` |

不要为 UI 页面或房间阶段创建 SceneType。Lobby、Running、Ended 是业务状态，不是运行容器，因此不需要 `SurvivorLobby`。`SurvivorRoom` 可以继续作为领域 Entity 使用，但它运行在哪一端，应由 `SurvivorClient` 或 `SurvivorServer` 表达。

### Survivor 代码分布

| 目录 | 内容 |
| --- | --- |
| `Hotfix/Server/SurvivorOnline/Gate/` | 接收客户端请求、校验会话和玩家、转发到房间 Actor |
| `Hotfix/Server/SurvivorOnline/Room/` | 服务器权威房间 Handler 与 Fiber 初始化 |
| `Hotfix/Share/SurvivorOnline/` | 双端可复用的确定性模拟和领域规则 |
| `Hotfix/Client/SurvivorOnline/` | 客户端快照消费、输入发送和预测 |
| `HotfixView/Client/SurvivorOnline/` | UI、UGF Entity、相机和纯表现逻辑 |

判断代码位置时先问“在哪一端运行、是否依赖 Unity 表现”，不要按页面名称或协议名称建目录。

### 组件类型与 System 可以分处不同装配

`Hotfix` 看不到 `ModelView`。如果一个组件需要被 `Hotfix` 层 `AddComponent`，但它的行为依赖 Unity 表现，就把**类型放在 `Model/Client/`、System 放在 `HotfixView/Client/`**。`SurvivorViewEntityManagerComponent` 和 `SurvivorViewComponent` 都是这个形状。

这对 ETReactive 同样成立：生成器把缓存字段生成到宿主所在装配，把 `ObserveChanges` 生成到 System 所在装配，缓存字段是 public，因此跨装配可用。

只依赖 Unity 类型的组件（`SurvivorCameraComponent`、各 `UGFEntity`/`UGFUIForm` 组件）仍然放 `ModelView`。

## 2. 登录属于 Realm/Gate，玩法只扩展登录后的 Player

标准登录链路是：

```text
Client → Realm 获取 Gate 地址和 Key → Gate 校验 Key → 创建或恢复 Player → Session 绑定 Player
```

Realm 负责分配 Gate 和签发登录凭据；Gate 持有客户端长连接、Session、Player 以及后续玩法请求的入口。因此登录后的玩家绑定必须在 Gate 完成，不能放到 Survivor 房间或 View 层。

玩法模块优先复用框架已有的 `Player` 和 `SessionPlayerComponent`。如果只需要记录玩家当前加入的 Survivor 房间，应把 `SurvivorPlayerRoomComponent` 挂到 `Player`：

```csharp
[ComponentOf(typeof(Player))]
public sealed class SurvivorPlayerRoomComponent: Entity, IAwake
{
    public ActorId RoomActorId { get; set; }
    public string RoomCode { get; set; }
}
```

这表示“属于该玩家的 Survivor 房间关系”。它虽然随 Gate 中的 Player 存活，但不是挂在 Gate Scene 上的全局状态。

不要只为改名复制出 `SurvivorPlayer`、`SurvivorSessionPlayerComponent` 等平行身份模型。只有生命周期、数据或行为确实不同，才创建新的领域实体。

## 3. Component 表示状态和生命周期，不是函数收纳盒

创建 Component 或 Entity 前，应至少满足一项：

- 需要随宿主创建、更新或销毁；
- 持有跨调用状态；
- 需要明确的父子所有权；
- 会被多个系统共同访问；
- 需要独立的 ET System 生命周期。

如果一个函数只在一个 Handler 中使用一次，直接内联到 Handler。不要为了“看起来分层”创建只装一个函数、没有独立状态和生命周期的 `SurvivorSessionOperationComponent`，也不要为它额外创建 Entity。

适合抽取的代码通常是：

- 被多个调用点复用；
- 是无协议依赖的领域规则；
- 能形成稳定、可独立测试的边界；
- 明显降低调用方复杂度，而不是把几行顺序代码搬到别处。

### 反例：只为当父节点存在的空 Component

`SurvivorCombatFeedbackComponent` 曾经是零字段、空 `Awake` 的组件，唯一作用是给伤害数字当父节点，而创建伤害数字的方法本来就在 `SurvivorViewEntityManagerComponent` 上。这类"只为凑一层所有权"的组件应删除，子节点直接挂到真正拥有该行为的组件下。

### 反例：用可变侧表给异步创建传参

`SurvivorViewEntityManagerComponent` 曾经在 Runtime 里维护 `PlayerStates`/`MonsterStates`/`ProjectileStates`/`PickupStates` 四个字典：Reconcile 时写入 State，异步 Show 完成后由表现组件按 Id 反查取回。这本质上是**因为异步 Show 路径传不了参数，就用字典充当构造参数**。

正确做法是让所有权节点持有状态，通过 `IAwake<TState>` 传入：

```csharp
[ChildOf(typeof(SurvivorClientComponent))]
public sealed class SurvivorPlayerEntry: Entity, IAwake<SurvivorPlayerState>, IDestroy
{
    public SurvivorPlayerState State { get; set; }
}
```

```csharp
client.AddChildWithId<SurvivorPlayerEntry, SurvivorPlayerState>(state.StateId, state);
```

表现组件在 `OnShow` 只缓存稳定的 `Entry`，Source 通过 `this.Entry.State.X` 实时读取。这样：

- 删掉四个字典和每次成员变化的 `Clear`；
- 空 Entity 变成有状态、有所有权的实体，满足本章的判定条件；
- 修掉一类真实缺陷——同步层重建 State 实例（例如 `ResetForLobby` 用 `CreatePlayer` 替换玩家实例）之后，表现组件里缓存的 State 会变成不再收到更新的孤儿对象。Reconcile 时刷新 `entry.State`，表现侧就永远看的是当前实例。

## 4. 协议对象只停留在网络边界

协议类型用于序列化和传输，不应成为长期业务状态。

### 应遵守

- Handler、Sender 等网络边界可以局部使用协议对象；
- Entity、Component 字段不保存 Request、Response 或 Message；
- 跨层传递的数据使用模块自己定义的逻辑结构；
- Handler 收到协议后，尽快提取普通值或映射为领域结构；
- 返回网络前，再把领域结果复制到 Response。

例如，跨多个系统使用的加入房间结果应定义逻辑结构，而不是把 `G2C_SurvivorJoinRoom` 返回给 UI：

```csharp
public readonly struct SurvivorJoinRoomResult
{
    public readonly int Error;
    public readonly string Message;
    public readonly long PlayerId;
    public readonly bool IsHost;

    public SurvivorJoinRoomResult(int error, string message, long playerId, bool isHost)
    {
        this.Error = error;
        this.Message = message;
        this.PlayerId = playerId;
        this.IsHost = isHost;
    }

    public bool Success => this.Error == ErrorCode.ERR_Success;
}
```

协议变化时，这种边界可以避免 UI、房间状态和领域逻辑连锁修改。

`Error` 和 `Message` 属于结果的一部分，应一起映射进逻辑结构；不要为了拿这两个字段就把整个 Response 交给 UI。只关心成败与提示文本的请求可以共用一个 `SurvivorRequestResult`。

### 边界收口同时解决所有权

映射成逻辑结构之后，Response 的最终使用者就落在网络方法内部，可以直接用 `using` 覆盖：

```csharp
public static async UniTask<SurvivorJoinRoomResult> JoinRoom(this SurvivorClientComponent self, string roomCode)
{
    self.PrepareSnapshotConsumer(roomCode);
    using C2G_SurvivorJoinRoom request = C2G_SurvivorJoinRoom.Create(true);
    request.RoomCode = roomCode;
    using G2C_SurvivorJoinRoom response = (G2C_SurvivorJoinRoom)await self.ClientSender.Call(request, false);
    if (response.Error != ErrorCode.ERR_Success)
    {
        return new SurvivorJoinRoomResult(response.Error, response.Message, 0, false);
    }

    self.PlayerId = response.PlayerId;
    self.IsHost = response.IsHost;
    self.ApplyStateFrame(response.Sequence, true, response.FullSnapshot);
    return new SurvivorJoinRoomResult(response.Error, response.Message, response.PlayerId, response.IsHost);
}
```

生成的消息 `Dispose()` 首行就是 `if (!this.IsFromPool) { return; }`，因此对非池化实例调用 `using` 是安全的，不需要先判断来源。

### 服务器内部同样适用

`BroadcastStateFrame` 曾经把 `SurvivorRoom2C_StateFrame` 存进 `Runtime.Frame`，好让 Join Handler 事后读取 `Sequence`/`ServerTick`/`Payload`。这既是 Component 字段保存协议对象，也让协议对象的生命周期跨出了发送方法。改为返回逻辑结构即可：

```csharp
public readonly struct SurvivorStateFrameInfo
{
    public readonly long Sequence;
    public readonly long ServerTick;
    public readonly byte[] Payload;
}
```

## 5. 缓存稳定依赖，不缓存动态实体

重复 `GetComponent<T>()` 是否应缓存，取决于引用在宿主生命周期内是否稳定。

| 情况 | 做法 |
| --- | --- |
| 生命周期内始终是同一实例，且高频使用 | 在 `Awake` 获取一次，保存普通引用 |
| Entity 可能被替换、销毁或跨房间重建 | 保存 `EntityRef<T>`，或使用时重新获取 |
| 只使用一次 | 使用局部变量，不增加字段 |
| 只是为了避免空引用异常 | 修复初始化顺序，不增加兜底 |

当前典型用法：

- `UIFormSurvivorLobbyComponent`、HUD、技能选择和结算界面在 `Awake` 缓存稳定的 `SurvivorClientComponent Client`；
- `SurvivorClientComponent` 在 `Awake` 缓存稳定的 `ClientSenderComponent ClientSender`；
- `SurvivorViewComponent`、`SurvivorViewEntityManagerComponent` 在 `Awake` 缓存父节点 `Client`；
- `SurvivorRoomServerComponent` 在 `Awake` 缓存 `World`（房间 Fiber 内的 World 与组件同生共死）；
- 表现组件在 `OnShow` 缓存稳定的 `Entry`，但**不缓存 Entry 里的 State**；
- 客户端的 `Room`、`World` 会在加入或切换房间时变化，继续使用 `EntityRef`，不能改成普通缓存；
- Event 或 MessageHandler 中只访问一次的 Component 继续使用局部获取。

缓存字段只负责减少重复查找，不改变所有权。字段所属对象销毁时，不要额外设计无意义的替换或容错路径。

### 父节点稳定就不要写成属性转发

`public SurvivorClientComponent Client => this.GetParent<SurvivorClientComponent>();` 看起来无害，但如果它被多个 `ETReactiveSource` 引用，就等于每帧多走几次父查找。父节点在生命周期内稳定属于本章第一行的情况，应在 `Awake` 缓存成普通字段。

同理，`GetParent<SurvivorRoom>().GetComponent<SurvivorWorldComponent>()` 这种链式取值在一个 `Update` 里出现四次，说明缺一个 `Awake` 缓存或一个函数开头的局部变量。

### 同步层可能替换实例，缓存要停在稳定的那一层

`VersionSyncDictionary` 在 `Apply` 时会通过 `SyncContext.__Objects` 复用已注册的实例，所以正常增量下 State 引用是稳定的；但只要业务侧主动重建（`ResetForLobby` 用 `CreatePlayer` 换掉 Player 实例），或者客户端换了整个 World 和 SyncContext，实例就变了。因此缓存的边界要选在"业务上不会被替换"的那一层：Entry 稳定、State 不稳定。

## 6. 依赖应由固定初始化点创建

如果组件是流程必需项，应在唯一、明确的启动阶段创建。例如客户端启动 Survivor 登录流程时先添加 `SurvivorClientComponent`，后续 UI 的 `Awake` 直接获取并缓存。

对必然存在的依赖，不使用：

- `?.`；
- `??`；
- `TryGetComponent` 后静默跳过；
- 缺失时临时创建另一套默认对象。

这些写法会把初始化错误延迟成状态错误。必需依赖缺失时应尽早暴露并修复初始化顺序。

只对真正动态或可选的状态做判断，例如：

- 尚未收到首个完整快照；
- 玩家还没有加入房间；
- 某个可开关 UI 当前未打开；
- 网络 Session 已断开。

不要把“业务状态暂未建立”和“架构依赖缺失”混为一谈。

## 7. 网络对象池必须明确所有权

生成协议的 `Create(true)` 表示从 `ObjectPool` 获取对象。它只设置池来源，不会自动决定何时回收；真正回收发生在对象调用 `Dispose()` 时。

### Call 请求

`ClientSenderComponent.Call`、`MessageSender.Call` 和 `Session.Call` 都不会替调用者释放传入的 Request。调用者应让 `using` 覆盖完整的异步调用：

```csharp
using C2G_SurvivorJoinRoom request = C2G_SurvivorJoinRoom.Create(true);
request.RoomCode = roomCode;
G2C_SurvivorJoinRoom response = (G2C_SurvivorJoinRoom)await self.ClientSender.Call(request, false);
```

不能在 `await` 完成前释放 Request，因为发送链路或异步队列仍可能读取它。

### Call 响应

`Call` 返回的 Response 所有权交给最终使用者。若 Response 来自池，最终使用者读取或复制完数据后必须 `Dispose()`。如果一个中间方法要把 Response 原样返回，就不能在中间方法内提前释放；更好的边界是复制成逻辑结果，然后在网络方法内部释放 Response。

### Send 消息

`Send` 是所有权转移：发送方提交后不能立即回收仍在队列中的消息，最终接收处理链应在处理完成后回收。

因此**一次广播不能把同一个消息实例 `Send` 给多个收件人**。所有权已经交出去，任何一个接收链回收它，其余收件人拿到的就是已归还对象池的实例。正确写法是每个收件人各创建一条：

```csharp
byte[] payload = isFull ? self.World.CaptureFull() : self.World.CaptureDelta();
while (self.Runtime.PlayerIdEnumerator.MoveNext())
{
    SurvivorRoom2C_StateFrame frame = SurvivorRoom2C_StateFrame.Create(true);
    frame.Sequence = self.Runtime.Sequence;
    frame.Payload = payload;
    sender.Get(LocationType.GateSession).Send(self.Runtime.PlayerIdEnumerator.Current, frame);
}
```

`payload` 这类不参与对象池的引用类型字段可以在多条消息间共享，只有消息本身需要一人一份。反过来，如果发现"必须写 `Create()` 不能池化"才能让广播正确，那是所有权设计的问题，不是池化的问题。

### 当前框架行为必须注意

当前 `MessageSessionHandler<Request, Response>` 使用 `using Response response = ObjectPool.Instance.Fetch<Response>()` 管理服务器响应，但没有释放收到的 Request；`MessageSessionHandler<Message>` 同样没有释放收到的 Message。因此目前不能宣称“接收端已经自动回收 Request/Message”。

相关代码：

- `Unity/Assets/Scripts/Game/ET/Code/Model/Share/Module/Message/MessageSessionHandler.cs`
- `Unity/Assets/Scripts/Game/ET/Code/Hotfix/Client/Demo/Main/ClientSenderComponentSystem.cs`
- `Unity/Assets/Scripts/Game/ET/Code/Model/Share/Module/Message/Session.cs`

如果统一补齐回收，应放在 Handler 基类完成 `Run` 后的 `finally`，并同时审计多 Handler 分发、Actor 消息转发和 Response 所有权，避免重复回收。补齐前，新增池化消息必须逐条确认最终所有者。

## 8. Gate Handler 负责边界编排，房间负责权威逻辑

客户端的 `C2G_*` 请求先到 Gate。Gate Handler 应完成：

1. 从 `SessionPlayerComponent` 获取已登录的 `Player`；
2. 校验请求和玩家当前房间关系；
3. 查找或创建 `SurvivorServer` 房间 Fiber；
4. 向房间 Actor 发送内部消息；
5. 把房间结果写入外部 Response；
6. 更新挂在 Player 上的房间关系组件。

只在该 Handler 使用的 Join 编排应直接写在 `C2G_SurvivorJoinRoomHandler` 中。移动、技能、生成、碰撞、升级等权威规则放在 Room 或 Share 层，不放在 Gate。

## 9. ETReactive：UI 刷新统一走它，权威逻辑不靠它

需要随状态变化的文本、显隐、按钮状态和界面切换，通过 `ETReactiveSource` 与 `ETReactiveBind` 刷新。异步按钮方法只修改逻辑字段或发起业务调用，不同时维护另一套 UI 刷新路径。

本章前半部分是基本形状，`9.1` 起是边界与机制细节。先读 `9.1`：**它决定了一段逻辑该不该用 ETReactive**，用错的代价比写错形状大得多。

采用 Entity 持有 Source、System 持有 Bind 的形状：

```csharp
public sealed partial class UIFormExampleComponent: UGFUIForm<MonoUIFormExample>, IETReactive
{
    public ExampleClientComponent Client { get; set; }

    [ETReactiveSource]
    public int Hp => this.Client.Player.Hp;
}

[EntitySystemOf(typeof(UIFormExampleComponent))]
[ETReactiveSystem]
public static partial class UIFormExampleComponentSystem
{
    [ETReactiveBind(nameof(UIFormExampleComponent.Hp))]
    private static void OnHpChanged(this UIFormExampleComponent self, int hp)
    {
        self.View.HpText.text = hp.ToString();
    }
}
```

`ETReactiveSource` 可以标记 Entity 的 public 实例字段、可读非索引属性，以及无参、非泛型、有返回值且非 ref 返回的实例方法，不能声明在 System 上。方法 Source 的 Reactive ID 是方法名，Bind 必须直接使用 `nameof(EntityType.Member)`，禁止写字符串字面量或字符串常量；生成代码会在每次观察时调用方法 Source 取值。实现 `IETReactive` 的 Entity 必须是 `partial`，生成器会把初始化状态和各 Source 的旧值缓存字段直接生成到 Entity partial 中；不创建旁路 Observer Entity，也没有运行时 `ETReactiveSystem` 单例、DLL 版本或热重载分支。

生成缓存字段统一带 `NonSerialized`、MongoDB 和 MemoryPack 忽略标记，暂不处理 JSON。字段以 `__` 开头，只供生成代码使用；分析器禁止业务代码直接访问任何 `__` 字段，应始终读写公开 Source 成员。

Bind 有三种签名：只接收 Entity、接收当前值、接收每个 Source 的旧值和当前值。首次观察会执行前两种刷新型 Bind，不执行旧值/当前值型变更 Bind；后续只在 Source 真正变化时执行。热重载不做额外重放，Bind 代码变化会在下一次 Source 变化或生命周期重新初始化后生效。

推荐生命周期：

```csharp
[UGFUIFormSystem]
private static void UGFUIFormOnUpdate(this UIFormExampleComponent self, float elapseSeconds, float realElapseSeconds)
{
    self.ObserveChanges();
}

[UGFUIFormSystem]
private static void UGFUIFormOnClose(this UIFormExampleComponent self, bool isShutdown)
{
    self.ClearReactive();
}
```

规则如下：

- `ObserveChanges()` 只在 UI 的 `OnUpdate` 调用一次；
- 不在 `OnOpen` 再调用一次，避免同一生命周期出现两条观察入口；
- `OnOpen` 只做按钮绑定和初始逻辑字段赋值；
- `OnClose` 调用 `ClearReactive()`；
- UI 值刷新放在 `[ETReactiveBind]` 方法；
- Source 只声明为 Entity 的 public 实例字段、可读属性或合法无参方法，Bind 中使用对应成员的 `nameof`；
- 业务代码不访问生成的 `__` 缓存字段；
- 动态 `World` 数据从缓存的 `Client` 实时获取，不把 `World` 再缓存到 UI。

Survivor UI 审计后，Lobby、HUD、技能选择均只在各自 `OnUpdate` 调用 `ObserveChanges()`。

### 9.1 适用边界：ETReactive 是表现层机制，不驱动权威逻辑

`ObserveChanges()` 是**轮询式**的：它在被调用的那一刻读取每个 Source，与缓存的旧值比较，变化了才执行 Bind。它没有"立即"语义，什么时候发生完全取决于谁调用了它。

因此：

- **允许**：UI 文本、显隐、按钮可交互性、UGF Entity 的位置与血条等表现刷新；
- **禁止**：死亡判定、升级结算、掉落生成、对局阶段推进等决定模拟结果的规则，交给挂在自己 `IUpdate` 上的观察者去轮询。

反例的破坏方式很具体。Survivor 曾经为每个 Player/Monster 挂一个 `IUpdate` 观察者 Entity，靠轮询 `Hp` 触发死亡结算。伤害在 `TickAuthority` 内同步施加，但 `Alive = false`、移出 `Monsters`、生成经验球都发生在观察者自己的 `Update` 里——这两个 `Update` 在 UpdateQueue 里互不保证顺序，于是：

- 0 血怪物在结算前仍是活的实体，继续被索敌、继续吃子弹和剑气，伤害被浪费；
- 0 血玩家继续移动、继续被索敌；
- 快照可能发出 `Hp <= 0 && Alive == true` 的自相矛盾帧，移除和掉落落在下一帧的 delta；
- `Phase = Ended` 迟到，还得额外补一次 tick 节奏外的广播来兜底；
- 客户端是快照消费方，从不挂观察者，所以这些放在 Share 的"共享规则"实际上是服务器独占的，无法按第 10 章的要求做到双端同语义。

同时每个怪物一个 Entity 加一个 UpdateQueue 条目，也是白付的开销。

不要为纯数据对象（`IVersionSync` 的 State 类）创建旁路观察者 Entity，也不要在同步数据类上加 `EntityRef` 回指实体系统。

### 9.2 权威侧确实要用 Reactive 时，必须在 tick 内显式驱动

如果确认要复用 Reactive 的差分能力（例如"Hp 变化"这个语义本身值得集中一处结算），做法是**去掉观察者的 `IUpdate`，把 `ObserveChanges()` 手写进权威 tick**，让顺序变成代码顺序：

```csharp
public static void TickAuthority(this SurvivorWorldComponent self)
{
    if (self.Data.Phase != SurvivorRoomPhase.Running) { return; }

    self.Data.ServerTick++;
    self.TickPlayerMovement();
    if (spawnTick) { self.SpawnMonster(); }
    self.TickMonsterMovementAndContact();
    self.ObserveStateReactions();          // 接触伤害 → 玩家死亡/对局结束
    if (self.Data.Phase != SurvivorRoomPhase.Running) { return; }

    self.TickWeapons();
    self.TickProjectiles();
    self.TickPickups();
    self.ObserveStateReactions();          // 武器伤害 → 怪物死亡+掉落；拾取 → 升级
}
```

驱动函数需要注意两点：

1. **先快照再遍历。** Bind 内部可能销毁观察者或改动世界字典，直接遍历会抛"集合已修改"。`Entity.Children` 是 `SortedDictionary<long, Entity>`，按 Entity Id 有序，所以先收集 Id 列表，再逐个点查 + 观察，顺序稳定可复现：

```csharp
self.Runtime.ObserverIds.Clear();
self.Runtime.ObserverEnumerator = self.Children.Values.GetEnumerator();
while (self.Runtime.ObserverEnumerator.MoveNext())
{
    self.Runtime.ObserverIds.Add(self.Runtime.ObserverEnumerator.Current.Id);
}
```

2. **驱动方不能是 Bind 所在的静态类。** `ObserveChanges` 是生成到 System partial 里的扩展方法，如果由同一个静态类调用，而该类的 Bind 又回调这个类的结算函数，分析器会报 `ET0013 静态类函数引用存在环形依赖`。把驱动放在 tick 所在的类（`SurvivorSimulationSystem`），结算留在 `SurvivorStateReactionSystem`，依赖就是单向的。

改完之后服务端不再有任何 `IETReactive` 宿主，为迟到的 `Phase` 补发广播的那条 Bind 也一并删除——阶段变化在结算它的那个 tick 的快照里就到位了。

### 9.3 生成器机制中会影响正确性的部分

这些是从生成代码得出的事实，写业务时会踩到：

- **Source 求值顺序是成员名的 ordinal 顺序，不是声明顺序。** 生成器在收集 Source 后按名字排序。生成代码逐个 Source"读当前值 → 判变 → 执行该 Source 的单源 Bind"，再读下一个 Source——也就是前一个 Bind 的副作用**会**被后面的 Source 看到。这意味着改名一个 Source 就会改变求值顺序。**不要用 Source 之间的先后表达语义**；确实需要顺序时，拆成多次显式驱动或多个观察者。
- **多源 Bind 在所有单源 Bind 之后执行**，且拿到的是全部更新后的值。
- **首次观察只执行刷新型 Bind**（签名为 `(self)` 或 `(self, 当前值...)`），不执行旧值/当前值型 Bind。这正好让"仅在下降时飘伤害数字"这类差分表现不会在单位刚生成时误触发：

```csharp
[ETReactiveBind(nameof(SurvivorHealthBarUGFEntity.Hp), nameof(SurvivorHealthBarUGFEntity.MaxHp))]
private static void OnHealthChanged(this SurvivorHealthBarUGFEntity self, int oldHp, int hp, int oldMaxHp, int maxHp)
{
    // 刷新血条…
    if (hp >= oldHp) { return; }
    // 掉血才飘伤害数字
}
```

  要意识到这种表现是**有损的**：它由快照差分推导，同一快照间隔内的多次命中会合并成一条，被击杀那一击因为实体先被移除而完全不飘。需要精确战斗表现时应由权威侧下发伤害事件，而不是在 View 侧 diff。
- **`ClearReactive()` 与 `ResetReactive()` 不同。** `ResetReactive()` 只把初始化标记置回 false，下次观察会重新初始化并再执行一遍刷新型 Bind；`ClearReactive()` 还会清空所有缓存值。生命周期结束用 `ClearReactive()`。
- **`float`/`double` Source 用带 epsilon 的比较**（`1e-6f` / `1e-9d`）并单独处理 `NaN` 与 `Infinity`，不是裸 `!=`。
- **`IVersion`/`IVersionSync` 类型作为 Source 走版本快路径**：只比较引用与 `__Version`，不逐字段读。因此一个整对象 Source 就能覆盖它的任意字段变化，适合"整块刷新"的 Bind（例如技能选择界面的三个选项加标题一起刷）。反过来，如果每个字段各写各的 Text，继续用叶子 Source 更省——否则位置每秒变十次会把所有文本一起重建。
- **自定义 struct 作为 Source 必须提供 `operator ==`**，否则报 `ET1113`。
- **没有被任何 Bind 引用的 Source 会报 `ET1114`**，不会被求值。

### 9.4 用无参方法 Source 避免同帧重复取值

需要判空或多步取值的 Source，写成属性会出现 `A == null ? 0 : A.X` 这种双重求值。Source 支持无参方法，用局部变量收一次即可：

```csharp
[ETReactiveSource]
public int Hp()
{
    SurvivorPlayerState player = this.Client.LocalPlayer;
    return player == null ? 0 : player.Hp;
}
```

Bind 侧照常 `nameof(UIFormSurvivorHudComponent.Hp)`。

### 9.5 跨层信号不要用 Publish 桥接

需要 View 层响应逻辑层状态时，**把 Reactive 宿主放在 View 层去读逻辑层**，而不是从逻辑层 `PublishAsync(...).Forget()`。

Survivor 曾经在 `SurvivorClientComponentSystem`（Hotfix/Client）的 Bind 里 Publish 两个事件，交给 HotfixView 的 `AEvent` 去开关技能选择界面和结算界面。这个 Publish 不是随手加的——它在跨 `Hotfix` → `HotfixView` 的层边界。但它带来三个问题：

1. **事件载荷不可信，被迫二次校验。** 处理方拿到 `args.Revision` 后又回头读一遍当前 `SkillChoiceRevision` 做比对。Reactive 已经算出了正确值，穿过异步事件总线之后反而不敢用了。
2. **`.Forget()` 丢掉顺序和异常。** 连续两次升级会有两个 in-flight Publish，`AddUIFormComponentAsync` 与 `RemoveComponent` 的交错不受控。
3. **UI 显隐本来就该走 Bind**，开关一个 UIForm 就是显隐。一个信号两套机制。

正确形状是新增一个 View 层的编排宿主。**组件类型放在 `Model`（这样 `Hotfix` 能 `AddComponent`），System 放在 `HotfixView`**——生成器把缓存字段生成到宿主所在装配、把 `ObserveChanges` 生成到 System 所在装配，缓存字段是 public，跨装配可用：

```csharp
// Model/Client/SurvivorOnline/SurvivorViewComponent.cs
[ComponentOf(typeof(SurvivorClientComponent))]
public sealed partial class SurvivorViewComponent: Entity, IAwake, IUpdate, IDestroy, IETReactive
{
    public SurvivorClientComponent Client { get; set; }

    public bool Switching { get; set; }

    [ETReactiveSource]
    public SurvivorRoomPhase Phase => this.Client.Phase;

    [ETReactiveSource]
    public bool SkillChoiceAvailable => this.Client.SkillChoiceAvailable;
}
```

Bind 只负责请求一次收敛，异步切换本身用单飞标记保护，并在每个 `await` 之后**重新读取期望状态**决定是否再来一轮：

```csharp
private static async UniTaskVoid ApplyViewStateAsync(SurvivorViewComponent self)
{
    EntityRef<SurvivorViewComponent> selfRef = self;
    self.Switching = true;
    try
    {
        while (true)
        {
            SurvivorRoomPhase phase = self.Phase;
            bool skillChoiceAvailable = self.SkillChoiceAvailable;
            await self.ApplyPhaseView(phase);
            self = selfRef;
            if (self == null) { return; }

            await self.ApplySkillChoiceView(skillChoiceAvailable);
            self = selfRef;
            if (self == null) { return; }

            if (phase == self.Phase && skillChoiceAvailable == self.SkillChoiceAvailable) { return; }
        }
    }
    finally
    {
        SurvivorViewComponent current = selfRef;
        if (current != null) { current.Switching = false; }
    }
}
```

收敛循环取代了"事件回传 Revision 再校验"，因为期望状态是随时可以重新读的，不需要相信一份可能过期的载荷。

### 9.6 开关界面的对象不能是界面自己

原来的 Lobby 界面在 `Phase` 变成 `Running` 的 Bind 里 `Forget()` 一个局部 async：加载战斗场景、加相机、开 HUD，最后 `uiComponent.RemoveComponent<UIFormSurvivorLobbyComponent>()` 把自己删掉；结算界面也在 `ReturnToRoom` 成功后自己打开 Lobby、关闭自己。这两处都在 `await` 之后继续访问 `self` 的成员，是明确的 use-after-dispose 风险。

规则：**UIForm 只负责自己的内容和按钮；决定哪个界面该开该关的对象放在界面之外。** 界面里跨过 `await` 之后如果还要用 `self`，必须先用 `EntityRef` 复核：

```csharp
EntityRef<UIFormSurvivorGameOverComponent> selfRef = self;
SurvivorJoinRoomResult result = await self.Client.JoinRoom(roomCode);
self = selfRef;
if (self == null || result.Success) { return; }

self.View.StatusUXText.text = result.Message;
```

成功路径上什么都不碰，因为此时界面可能已经被编排宿主关掉了。

## 10. 玩法数值变化保持语义

### 升级增加最大生命时保持生命比例

升级不应直接回满血。应保存旧最大生命，并按比例换算：

```csharp
int oldMaxHp = state.MaxHp;
state.MaxHp += SurvivorDefaults.LevelMaxHpIncrease;
state.Hp = (int)((long)state.Hp * state.MaxHp / oldMaxHp);
```

中间值使用 `long`，避免乘法溢出。整数取整规则应保持双端一致。

增量用命名常量而不是字面量。写成 `state.MaxHp += 10` 之后，后续需要旧值时只能靠 `state.MaxHp - 10` 反算，一改数值就同时错两处；显式保存 `oldMaxHp` 加常量则只有一个真值来源。

这段换算还有一个不显眼的性质：结果恒定满足 `0 < Hp <= MaxHp`。正因如此，即使它和"Hp 变化结算"分属两个 Source、求值先后不确定（见 9.3），也不会让 Hp 结算做出多余动作。这类"顺序无关"的性质应当在代码里写明，否则下一个人无法判断能不能调整顺序。

### 拾取物使用独立拾取范围

经验拾取范围与碰撞半径不是同一个概念，应使用独立配置 `ExperiencePickupRange`。范围判断使用距离平方，避免开平方并保持整数确定性：

```csharp
long distanceSquared = (long)deltaX * deltaX + (long)deltaY * deltaY;
if (distanceSquared > (long)SurvivorDefaults.ExperiencePickupRange * SurvivorDefaults.ExperiencePickupRange)
{
    continue;
}
```

这类决定模拟结果的数值和算法应放在 Share 层，保证服务器权威逻辑与客户端预测使用同一语义。

## 11. 代码格式约定

Survivor 模块新增或修改代码遵守以下约定：

- 函数声明不换行；
- 函数调用的参数列表不换行；
- `if`、`while` 等条件表达式不换行；
- 必需依赖不使用 `?.`、`??` 做容错；
- 协议池参数写为 `Create(true)`；
- 不用额外 Component 或帮助函数包装仅有一个调用点的顺序逻辑。

对象初始化器、集合初始化器和长数据表可以按字段换行；格式规则的目的在于减少业务控制流被拆散，而不是强制所有代码成为一行。

## 12. 提交前审计清单

每次新增类似模块，可按以下顺序检查：

### 分层与职责

1. `SceneType` 是否准确区分 Client、Server、逻辑和 View；
2. UI 阶段是否被误建成 SceneType；
3. 权威玩法规则是否位于 Server/Share，而不是 Gate/View；
4. 决定开关哪个界面的对象是否在界面之外（UIForm 不自己 `RemoveComponent` 自己）。

### Component 与依赖

5. Component 是否真的持有状态或生命周期，有没有零字段空 `Awake` 的组件；
6. 单一调用点函数是否可以内联；
7. 是否用可变字典/侧表替代 `IAwake<T>` 给异步创建传参；
8. 高频稳定依赖是否在 `Awake` 缓存，是否残留每帧走 `GetParent` 的属性转发；
9. 动态 Entity 是否错误地保存成普通引用；同步层可能重建的 State 是否被缓存住了；
10. 必需依赖是否被 `?.`、`??` 掩盖；剩下的 `?.` 是否都对应真实的动态/可选状态；
11. 是否存在抓不到真实故障的兜底（例如对会抛 `KeyNotFoundException` 的字典索引写 `== null` 判断）。

### 协议与对象池

12. Component 或 Runtime 字段中是否保存了协议对象；
13. `Error`/`Message` 是否连同结果一起映射进逻辑结构，而不是把 Response 交给 UI；
14. 每个 `Create(true)` 是否有明确的最终 `Dispose` 所有者；
15. 广播是否给每个收件人各创建一条消息，而不是共用一个实例 `Send` 多次。

### ETReactive

16. `ObserveChanges()` 是否只在对应 Update 生命周期调用一次（权威侧显式驱动除外，那里的调用点应当刻意且带注释）；
17. `ETReactiveSource` 是否只表达表现层可见状态，没有承担权威结算；
18. 是否存在 `[ETReactiveBind]` 内部发起 `Publish`/`Invoke`（同一个信号被两套机制表达）；
19. 跨层通知是否改为在 View 层建 Reactive 宿主读逻辑层，而不是从逻辑层 Publish；
20. 是否依赖了 Source 之间的求值先后（求值顺序按成员名 ordinal，改名即改行为）；
21. 权威侧驱动 `ObserveChanges` 的静态类是否与 Bind 所在静态类分离（`ET0013`）；
22. 观察者 Bind 会改动集合或销毁自身时，驱动方是否先快照再遍历；
23. `ETReactiveSource` 是否只声明在 Entity 的合法字段、属性或方法上；
24. 是否有业务代码直接访问 `__` 生成字段；
25. 生命周期结束用的是 `ClearReactive()` 而不是 `ResetReactive()`。

### 收尾

26. 函数和条件是否保持单行格式；
27. 是否通过 Unity AgentBridge 完成真实 Unity 编译验证（0 error / 0 warning）并跑过 EditMode 测试。

常用静态审计命令：

```powershell
# 稳定依赖是否重复查找
rg -n "GetComponent<SurvivorClientComponent>|GetComponent<ClientSenderComponent>|GetParent<SurvivorRoom>" Unity/Assets/Scripts/Game/ET/Code
# 非池化协议对象
rg -n "Create\(\)" Unity/Assets/Scripts/Game/ET/Code/Hotfix Unity/Assets/Scripts/Game/ET/Code/HotfixView
# Reactive 观察入口与 __ 字段
rg -n "ObserveChanges\(|ResetReactive\(|ClearReactive\(" Unity/Assets/Scripts/Game/ET/Code
rg -n "\.__[A-Za-z_]" Unity/Assets/Scripts/Game/ET/Code
# Bind 里发事件、逻辑层向 View 层 Publish
rg -n -B 4 "EventSystem\.Instance\.Publish" Unity/Assets/Scripts/Game/ET/Code
# 必需依赖容错
rg -n "\?\.|\?\?" Unity/Assets/Scripts/Game/ET/Code/HotfixView/Client/SurvivorOnline
# 协议对象是否被 Component/Runtime 字段持有
rg -n "public (C2G|G2C|SurvivorRoom2C|G2SurvivorRoom)\w+ \w+ \{ get; set; \}" Unity/Assets/Scripts/Game/ET/Code
```

静态搜索只能发现候选问题。最终必须结合所有权和生命周期判断，并通过 AgentBridge 触发 Unity 重编译，确认错误和警告均为零。

## 13. 重构的执行顺序

同时存在多类问题时，按依赖方向推进，避免反复改同一个文件：

1. **先修驱动与顺序**（谁在什么时候跑），这一步往往会顺带删掉一批为迟到状态打的补丁；
2. **再收协议边界**（结果结构 + `Dispose` 所有者），因为它会改变所有调用方的签名；
3. **然后调整所有权**（Entry 持有状态、删空 Component），此时调用方已经稳定；
4. **最后补动态状态判断、清容错写法与死代码、统一格式。**

每一步之后都通过 AgentBridge 触发重编译。分析器（`ET0013` 环形依赖、`ET11xx` Reactive 规则）能抓到一批肉眼难发现的结构问题，越早撞上越省事。
