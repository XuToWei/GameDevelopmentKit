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

## 4. 协议对象只停留在网络边界

协议类型用于序列化和传输，不应成为长期业务状态。

### 应遵守

- Handler、Sender 等网络边界可以局部使用协议对象；
- Entity、Component 字段不保存 Request、Response 或 Message；
- 跨层传递的数据使用模块自己定义的逻辑结构；
- Handler 收到协议后，尽快提取普通值或映射为领域结构；
- 返回网络前，再把领域结果复制到 Response。

例如，跨多个系统使用的加入房间结果应定义逻辑结构，而不是保存 `G2C_SurvivorJoinRoom`：

```csharp
public readonly struct SurvivorJoinRoomResult
{
    public readonly long PlayerId;
    public readonly string RoomCode;
    public readonly bool IsHost;

    public SurvivorJoinRoomResult(long playerId, string roomCode, bool isHost)
    {
        this.PlayerId = playerId;
        this.RoomCode = roomCode;
        this.IsHost = isHost;
    }
}
```

协议变化时，这种边界可以避免 UI、房间状态和领域逻辑连锁修改。

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
- `Room`、`World` 会在加入或切换房间时变化，继续使用 `EntityRef`，不能改成普通缓存；
- Event 或 MessageHandler 中只访问一次的 Component 继续使用局部获取。

缓存字段只负责减少重复查找，不改变所有权。字段所属对象销毁时，不要额外设计无意义的替换或容错路径。

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

## 9. UI 刷新统一走 ETReactive

需要随状态变化的文本、显隐、按钮状态和流程事件，通过 `ETReactiveSource` 与 `ETReactiveBind` 刷新。异步按钮方法只修改逻辑字段或发起业务调用，不同时维护另一套 UI 刷新路径。

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

## 10. 玩法数值变化保持语义

### 升级增加最大生命时保持生命比例

升级不应直接回满血。应保存旧最大生命，并按比例换算：

```csharp
int oldMaxHp = state.MaxHp;
state.MaxHp += maxHpIncrease;
state.Hp = (int)((long)state.Hp * state.MaxHp / oldMaxHp);
```

中间值使用 `long`，避免乘法溢出。整数取整规则应保持双端一致。

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

1. `SceneType` 是否准确区分 Client、Server、逻辑和 View；
2. UI 阶段是否被误建成 SceneType；
3. Component 是否真的持有状态或生命周期；
4. 单一调用点函数是否可以内联；
5. Component 字段中是否保存了协议对象；
6. 高频稳定依赖是否在 `Awake` 缓存；
7. 动态 Entity 是否错误地保存成普通引用；
8. 必需依赖是否被 `?.`、`??` 掩盖；
9. 每个 `Create(true)` 是否有明确的最终 `Dispose` 所有者；
10. `ObserveChanges()` 是否只在对应 Update 生命周期调用一次；
11. UI 刷新是否全部由 Reactive Bind 完成；
12. `ETReactiveSource` 是否只声明在 Entity 的合法字段、属性或方法上；
13. 是否有业务代码直接访问 `__` 生成字段；
14. 权威玩法规则是否位于 Server/Share，而不是 Gate/View；
15. 函数和条件是否保持单行格式；
16. 是否通过 Unity AgentBridge 完成真实 Unity 编译验证。

常用静态审计命令：

```powershell
rg -n "GetComponent<SurvivorClientComponent>|GetComponent<ClientSenderComponent>" Unity/Assets/Scripts/Game/ET/Code
rg -n "Create\(\)" Unity/Assets/Scripts/Game/ET/Code/Hotfix/Client/SurvivorOnline Unity/Assets/Scripts/Game/ET/Code/Hotfix/Server/SurvivorOnline
rg -n "ObserveChanges\(" Unity/Assets/Scripts/Game/ET/Code
rg -n "\.__[A-Za-z_]" Unity/Assets/Scripts/Game/ET/Code
rg -n "\?\.|\?\?" Unity/Assets/Scripts/Game/ET/Code/HotfixView/Client/SurvivorOnline
```

静态搜索只能发现候选问题。最终必须结合所有权和生命周期判断，并通过 AgentBridge 触发 Unity 重编译，确认错误和警告均为零。
