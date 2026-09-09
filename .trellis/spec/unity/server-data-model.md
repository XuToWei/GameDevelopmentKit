# Server and Runtime Data Model

## Scope

本规范约束网络消息、配置和持久化数据进入运行时后的所有权。GDK 有 ET 与 GameHot 两条路径：ET 以 Entity/Component 为领域状态 owner；GameHot 以 EntityData、EntityLogic、HotComponent 或对应领域组件为 owner。传输消息和 GF/Mono view 不是长期领域状态。

## Define each data type

新增运行时数据前明确：

| 项目 | 要求 |
|---|---|
| 身份 | 实例 ID、类型 ID、UID、Actor/fiber 或其他稳定主键；不可用显示文本代替 |
| 字段 | wire 名称、本地类型和业务含义 |
| 单位 | 秒、毫秒、数量、比例、角度或 ID |
| 特殊语义 | 仅记录协议真实定义的可选值、特殊值或合法空态 |
| 更新方式 | 全量、增量、请求响应、Push、消息 handler 或本地操作 |
| 本地 owner | ET Entity/Component、GameHot EntityData/Component 或配置 Tables |
| 生命周期 | 是否池化、由谁创建/清理、何时失效 |
| 观察方式 | 版本、事件、系统回调或显式重建 |

成功响应的必需字段应直接进入 owner；UI 从 owner 读取，避免每个 UI 保存一份 response。

## ET model ownership

ET Model 声明 Entity/Component 和共享消息/数据；Hotfix/HotfixView 系统执行更新；ModelView 负责客户端 ET 与 GF view 关联。使用 ET 的 `AddChild`/`AddComponent`、`IAwake`、`IDestroy` 和现有 owner API 管理生命周期，不能在 view 中创建平行状态。

## GameHot data ownership

GameHot EntityLogic 接收 `EntityData`，通常在 `OnShow` 捕获并在 `OnHide`/`OnRecycle` 清理。配置型数据由 `HotEntry.Tables` 或对应 Tables owner 提供；不要把 Luban 生成记录复制成每个 UI 的手写 DTO，除非存在明确的领域转换边界。

## Full, incremental, and message updates

协议支持全量/增量时，共用领域落地入口但保留协议语义：全量重建或覆盖完整对象；增量只应用生成消息明确标记的字段；没有更新标记的字段不得用默认值覆盖。消息顺序、重复消息和版本由协议/owner 处理，不由展示层猜测。

不应把不确定的成功 payload 转成空实体、空集合或 `Invalid` 结果继续运行。只有协议明确可选或业务合法空态才进行分支处理。

## Pooling and lifecycle

ET Entity 的 `IDestroy`、GF Entity 的 `OnHide`/`OnRecycle`、资源容器和引用池都有各自 owner。清理必须对称：本对象创建的集合、CTS、临时引用和 view 状态由本对象释放；不清除其他 owner 的数据。异步任务在 owner 销毁、隐藏、切换上下文后不得 Apply。

## Review checklist

- [ ] 身份、单位、特殊值、更新语义和 owner 已明确。
- [ ] ET logical state 与 GF/Mono view 分离。
- [ ] GameHot `EntityData`/Tables/Component owner 清晰。
- [ ] 全量/增量只按协议更新语义应用，没有默认值覆盖未更新字段。
- [ ] 成功必需数据直接落地；失败、取消和 stale 不污染新状态。
- [ ] 池化、Dispose、OnHide/OnRecycle 和异步退出清理对称。
- [ ] UI 不缓存传输 response 作为第二事实来源。

## References

- `Unity/Assets/Scripts/Game/ET/Code/Model/`
- `Unity/Assets/Scripts/Game/ET/Code/ModelView/`
- `Unity/Assets/Scripts/Game/ET/Code/Hotfix/`
- `Unity/Assets/Scripts/Game/ET/Code/HotfixView/`
- `Unity/Assets/Scripts/Game/ET/Loader/UGF/Entity/UGFEntity.cs`
- `Unity/Assets/Scripts/Game/Hot/Code/Entity/EntityData/`
- `Unity/Assets/Scripts/Game/Hot/Code/Entity/EntityLogic/`
- `Unity/Assets/Scripts/Game/Hot/Code/Tables/TablesComponent.Load.cs`
