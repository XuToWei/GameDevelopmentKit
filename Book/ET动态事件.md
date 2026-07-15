# ET DynamicEvent

DynamicEvent 是按“参数类型 + SceneType + 实体类型”分发的进程内事件系统。它不保存业务委托，而是从 CodeTypes 发现处理器实例，适合 ET 热重载环境中的多实体广播。

## 与普通 Event 的区别

| 能力 | DynamicEvent |
| --- | --- |
| 接收者 | 已注册的某一实体类型的全部实例 |
| 路由条件 | 参数类型、SceneType、实体精确类型 |
| 处理器发现 | `[DynamicEvent]` + CodeTypes 反射 |
| 同步方式 | Fire-and-forget 或等待全部处理器 |
| 生命周期 | 实体注册 / 反注册 |

它是当前进程内广播，不会自动跨 Fiber、跨进程或跨网络传递。

## 初始化

默认入口已通过 `EntryEvent1_InitDynamicEvent` 自动完成：

```csharp
World.Instance.AddSingleton<DynamicEventSystem>();
scene.AddComponent<DynamicEventSystemUpdateComponent>();
```

正常启动链路中不要重复添加 Singleton。只有脱离默认 `EntryEvent1` 自建入口时，才需要提供等价初始化。

## 1. 定义事件参数

参数必须是 struct：

```csharp
namespace ET
{
    public readonly struct PlayerLevelChanged
    {
        public readonly int Level;

        public PlayerLevelChanged(int level)
        {
            Level = level;
        }
    }
}
```

分发键是参数的运行时 Type。同名但不同命名空间的 struct 是不同事件。

## 2. 定义处理器

```csharp
using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [DynamicEvent(SceneType.Client)]
    public sealed class RefreshPlayerLevel :
        ADynamicEvent<Player, PlayerLevelChanged>
    {
        protected override UniTask Run(
            Player self,
            PlayerLevelChanged arg)
        {
            self.Level = arg.Level;
            return UniTask.CompletedTask;
        }
    }
}
```

第一个泛型参数是接收实体类型，第二个是事件参数。当前 `Run` 签名只有 `self` 与 `arg`，不再传入 Scene。

`[DynamicEvent]` 可在同一处理器类上使用多次，为不同 SceneType 注册同一实现。默认参数是 `SceneType.All`。

## 3. 注册实体

推荐通过组件跟随实体生命周期：

```csharp
player.AddComponent<DynamicEventComponent>();
```

组件 Awake 时注册 Parent，Destroy 时反注册。实体销毁组件后，无需再手工操作。

也可直接调用：

```csharp
DynamicEventSystem.Instance.RegisterEntity(player);
DynamicEventSystem.Instance.UnRegisterEntity(player);
```

直接注册时，调用方必须保证反注册。重复注册会被去重。

## 4. 发布事件

### 不等待

```csharp
DynamicEventSystem.Instance.Publish(
    scene,
    new PlayerLevelChanged(10));
```

`Publish` 对每个处理器调用 `Handle(...).Forget()`，发布方法立即返回。适合相互独立、不影响后续流程的通知。

### 等待全部处理完成

```csharp
await DynamicEventSystem.Instance.PublishAsync(
    scene,
    new PlayerLevelChanged(10));
```

`PublishAsync` 收集任务并使用 `UniTask.WhenAll`。处理器之间仍并发执行，不保证顺序。

### SceneType 重载

| API | SceneType 来源 |
| --- | --- |
| `Publish(arg)` | `SceneType.All` |
| `Publish(scene, arg)` | `scene.SceneType` |
| `Publish(sceneType, arg)` | 调用方显式传入 |

Async 版本提供相同三组重载。

## 分发规则

一次发布按以下顺序筛选：

1. 用参数 Type 查找所有 DynamicEvent 处理器。
2. 使用 `HasSameFlag` 匹配发布 SceneType 与处理器 SceneType。
3. 用处理器声明的 EntityType 查注册实体列表。
4. 对仍有效的 EntityRef 调用处理器。

实体按 `entity.GetType()` 注册，处理器按精确 EntityType 查找。为基类声明的处理器不会自动接收所有派生实体，除非这些实体以相同类型键注册或系统实现被扩展。

## 异常处理

`ADynamicEvent.Handle` 会捕获每个处理器的异常并写入日志，避免一个接收者中断其他接收者。

因此 `PublishAsync` 的完成只表示所有处理器任务已结束，不代表所有业务处理都成功。需要事务语义时，应使用明确的请求/响应流程。

## 清理机制

反注册先加入待移除列表，在 `DynamicEventSystem.Update()` 中统一从类型列表删除。

系统还会每 60 秒扫描一次所有注册列表，清理已经失效的 EntityRef。发布过程中也会移除当前列表中发现的失效引用。

延迟清理可以避免在事件遍历期间修改集合，但意味着调用 `UnRegisterEntity` 到下一次 Update 之间实体仍在列表中；EntityRef 失效后不会真正调用处理器。

## 热重载

处理器类型由带 `[Code]` 的 `DynamicEventTypeSystem` 从 CodeTypes 构建。ET Hotfix Reload 重建 CodeTypes 后，新处理器实现会进入后续分发。

已注册实体列表属于 `DynamicEventSystem` Singleton，不因 HotfixView/Hotfix 重载自动丢失。修改事件参数类型会创建新的分发键，旧注册处理器不会匹配。

## 性能边界

一次发布的成本与匹配实体数量线性相关。系统适合低频状态通知，不适合每帧向大量实体广播位置、动画或网络同步数据。

优化优先级：

1. 缩小 SceneType。
2. 缩小接收实体类型。
3. 降低发布频率并合并参数。
4. 大规模场景改为组件索引、消息队列或专用系统。

`PublishAsync` 会为本次调用创建任务列表；大量微小处理器会放大调度成本。

## 常见问题

### 处理器没有触发

依次检查：实体是否注册、参数是否是完全相同的 struct Type、SceneType 是否匹配、处理器是否进入当前 CodeMode 的程序集。

### 派生实体收不到基类事件

当前索引按精确运行时类型存储，不做继承查找。为具体类型声明处理器，或明确扩展注册与查询策略。

### 需要保证执行顺序

DynamicEvent 不保证处理器顺序。把有依赖的步骤合并到一个处理器，或改用显式编排。

### 需要知道业务是否成功

处理器异常会被记录而不是向发布者传播。使用专用 UniTask API、Actor RPC 或返回结果的领域服务。

## 关键代码

| 作用 | 文件 |
| --- | --- |
| 处理器接口 | `Game/ET/Code/Model/Share/Module/DynamicEvent/IDynamicEvent.cs` |
| 注册与发布 | `Game/ET/Code/Model/Share/Module/DynamicEvent/DynamicEventSystem.cs` |
| 处理器发现 | `Game/ET/Code/Model/Share/Module/DynamicEvent/DynamicEventSystem.Define.cs` |
| 自动初始化 | `Game/ET/Code/Model/Share/Module/DynamicEvent/EntryEvent1_InitDynamicEvent.cs` |
| 生命周期组件 | `Game/ET/Code/Model/Share/Module/DynamicEvent/DynamicEventComponent.cs` |
