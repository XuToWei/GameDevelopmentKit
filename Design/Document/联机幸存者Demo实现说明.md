# 联机幸存者 Demo 实现说明

> 状态：第一版可编译玩法骨架  
> 架构基线：[联机幸存者Demo架构需求.md](./联机幸存者Demo架构需求.md)  
> 伪代码：[联机幸存者Demo整体伪代码.md](./联机幸存者Demo整体伪代码.md)

## 1. 当前实现范围

已经实现：

- 服务器权威房间、玩家输入、20 Tick/s 模拟；
- 输入房间号加入，不存在则创建，最多 4 人；
- 首位玩家为房主，房主开始，开局后禁止加入；
- 玩家移动、怪物生成和追逐、接触伤害；
- 自动射击、投射物命中、怪物死亡、经验掉落和拾取升级；
- ReactiveBinding 完整/增量快照；
- 10 Hz 状态广播、序号缺口请求完整快照；
- 客户端从权威数据重建 ET Entry Entity；
- UGFEntity + SpriteRenderer 的玩家、怪物、投射物和拾取物表现；
- UGFUIForm 的加房/开始界面和局内 HUD。
- 字段级 ReactiveBind 派生逻辑与快照提交后的 ReactiveBind 表现刷新。

当前没有实现：

- 客户端预测、表现插值；
- 游戏中途加入和断线重连；
- 升级三选一、Buff、Boss、复活和完整结算；
- 新 Demo 的最终独立 Launcher/Scene 接入。

## 2. 数据与 System 边界

`SurvivorWorldComponent` 是 ET 入口和唯一 ReactiveBinding 根，只持有一个版本字段 `SurvivorWorldData`。玩家、怪物、投射物和拾取物都是普通 `IVersionSync` 数据结构，不是 ET Component。

`SurvivorWorldRuntime` 不进入快照，只保存：

- `SyncContext` 和复用流；
- 枚举器、删除列表；
- 当前 Tick 的索引、目标和距离等临时槽。

所有 Survivor System 和 Handler 均不声明局部变量。Runtime 临时槽必须在使用前重置，Capture/Apply 只发生在完整 Tick 边界。任何需要跨 Tick 保留且会影响玩法结果的数据必须放入带 `[VersionField]` 的同步数据。

### 2.1 ReactiveBind 数据流

```text
服务器 System 写 Hp / Experience
    ↓
字段级 ReactiveObserver.ObserveChanges
    ↓
ReactiveBind 回调
    ↓
无状态 ReactionSink
    ↓
Hotfix System 执行死亡、掉落、升级等派生规则
```

```text
客户端 SyncContext.Apply
    ↓
监听 MembershipRevision，对账 ET Entry
    ↓
统一提交已注册的 PresentationObserver
    ↓
UGFEntity Position / Lobby / HUD 的 ReactiveBind 回调
```

没有使用“监听整个 World 后全量扫描”的实现。集合增删分别推进四个 MembershipRevision，位置和生命变化不会触发 Entry 全量对账。UGFEntity 与 UI 也不在每帧 Update 中调用 `ObserveChanges()`；HUD 的 Update 仅保留输入采样。

由于 ET Hotfix 程序集不允许实例字段，Observer 和 ReactiveSource 定义在 Model/ModelView，Hotfix/HotfixView 使用无字段 Sink 接回 System。这同时保持 ReactiveBinding 源生成能力和 ET 单向程序集依赖。

## 3. 代码位置

| 内容 | 位置 |
|---|---|
| 共享同步数据和根 | `Unity/Assets/Scripts/Game/ET/Code/Model/Share/SurvivorOnline/` |
| 通用玩法 System | `Unity/Assets/Scripts/Game/ET/Code/Hotfix/Share/SurvivorOnline/` |
| 服务端房间和 Gate | `Unity/Assets/Scripts/Game/ET/Code/Model/Server/SurvivorOnline/`、`Hotfix/Server/SurvivorOnline/` |
| 客户端同步和 Entry 对账 | `Unity/Assets/Scripts/Game/ET/Code/Model/Client/SurvivorOnline/`、`Hotfix/Client/SurvivorOnline/` |
| UGF 表现 | `Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/SurvivorOnline/`、`HotfixView/Client/SurvivorOnline/` |
| 字段级玩法监听 | `Model/Share/SurvivorOnline/SurvivorStateReactiveObservers.cs`、`Hotfix/Share/SurvivorOnline/SurvivorStateReactionSystem.cs` |
| 表现监听 | `ModelView/Client/SurvivorOnline/**/Survivor*ReactiveObservers.cs` |
| 外部协议 | `Design/Proto/ET-Client/SurvivorOnlineOuter.proto` |
| 内部协议 | `Design/Proto/ET-ClientServer/SurvivorOnlineInner.proto` |
| Entity Prefab | `Unity/Assets/Res/Entity/SurvivorOnline/` |
| UI Prefab | `Unity/Assets/Res/UI/UIForm/SurvivorOnline/` |

## 4. Prefab 和配置

Prefab 由 Unity AgentBridge 直接在编辑器中创建、挂载组件、绑定引用并保存，没有保留任何“用代码生成 Prefab”的 Editor 工具。

UGF 配置使用现有 ET Luban 表追加独立 ID：

- Entity：81001–81004；
- UIForm：9801–9802。

协议源文件直接放入 ET 已有 Proto 目录，没有新增独立 Proto 目录或生成工程。

## 5. 已完成验证

- Unity Editor 全量脚本刷新：0 error、0 warning；
- `DotNet.Hotfix.csproj` 隔离输出编译：0 error、0 warning；
- Roslyn 语法扫描：35 个 Survivor System/Handler 均无局部变量、`foreach`、LINQ、闭包、局部函数或迭代器；
- ReactiveBinding 冒烟测试：完整快照、嵌套增量、集合新增/删除和玩法 Tick 通过；
- 字段级 HP/经验监听已由玩法冒烟测试覆盖；
- AgentBridge 验证 4 个 Entity Prefab 的组件，以及 2 个 UI Prefab 的字段引用；
- Luban 二进制和编辑器 JSON 已导出并包含全部 Survivor Entity/UI 配置。

## 6. 启动边界

为遵守“不修改现有 Demo”的要求，当前没有把 SurvivorOnline 自动挂入现有 Launcher 或 Demo 流程。客户端公开入口为：

```csharp
await SurvivorViewStarter.OpenLobby(root);
```

该入口会安装 `SurvivorClientComponent` 并打开加房界面。下一步需要确定独立启动方式，再接入新的 Unity Scene、Launcher 配置或启动参数；不应复用并修改现有 Demo 的业务入口。
