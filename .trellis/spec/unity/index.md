# Unity Development Specifications

适用范围：整个 `Unity/` 工程，以及与 Unity 客户端、ET 服务端/客户端、GameHot 热更、Luban、Proto2CS 和 AgentBridge 共同组成的 GDK 工作流。

文档中的 `Game/...` 路径均指 `Unity/Assets/Scripts/Game/...`；涉及 `Design/`、`Share/`、`Book/` 或完整 `Unity/Assets/...` 的路径则从仓库根目录解析。
GDK 不是单一的 MonoBehaviour 项目。运行时存在两条互斥业务路径：

- **GameHot**：`Unity/Assets/Scripts/Game/Hot/Loader/` 的稳定加载层与 `Game/Hot/Code/` 的热更业务层。
- **ET**：`Unity/Assets/Scripts/Game/ET/Loader/` 的桥接/启动层，以及 `Game/ET/Code/{Model,ModelView,Hotfix,HotfixView}/` 的 ET 分层。

`UNITY_ET` 与 `UNITY_GAMEHOT` 互斥；`UNITY_HOTFIX` 控制热更相关装载，可在相应模式中启用。模式选择由 `Game/Procedure/ProcedurePreset.cs` 驱动，不能把两条路径的类型、生成目录或生命周期混写。

## Specification Index

| 文档 | 适用范围 | 何时阅读 |
|---|---|---|
| [Project Architecture](./project-architecture.md) | Unity 启动、GameHot/ET 模式、Loader/Code、程序集和视图边界 | 修改启动流程、程序集、模式宏或跨模式代码 |
| [Framework Lifecycle](./framework-lifecycle.md) | GF UIForm/UIWidget/Entity 与 ETUI/ETEntity 的 owner/lifecycle | 修改框架托管 UI、Entity、Widget 或异步资源 |
| [Business Code Conventions](./business-code-conventions.md) | C# 代码、组件/系统所有权、配置契约、生成文件边界 | 新增或修改运行时代码 |
| [Reuse and Error Scope](./reuse-and-error-scope.md) | 复用已有 API、抽象准入、错误处理所有权 | 新增 helper/interface/wrapper、判空或 catch |
| [HybridCLR and UniTask](./hybridclr-and-async.md) | 热更程序集、CodeRunner、AOT 边界和异步所有权 | 修改热更装载、网络、资源或异步流程 |
| [Server and Runtime Data Model](./server-data-model.md) | ET Entity/Component、GameHot EntityData 和共享协议数据落地 | 新增或修改网络数据、领域状态或持久化模型 |
| [Network Synchronization](./server-sync-and-http.md) | ET 消息、GameHot WebSocket、取消/stale 和可选 HTTP | 修改消息 handler、网络请求或异步 Apply |
| [Reactive State and UI Refresh](./reactivebinding-refresh.md) | ReactiveBinding 包和 ET reactive source/bind 两套机制 | 新增响应式状态、观察或 UI 刷新 |
| [Localization Workflow](./localization-workflow.md) | Localization.xlsx、动态文案、导出和运行时加载 | 新增或修改玩家可见文本 |
| [Luban and Proto Generation](./data-generation.md) | Excel/Proto 源、Luban/Proto2CS、生成物和导出验证 | 修改表格、协议、生成器或导出目录 |
| [AgentBridge Diagnostics](./agentbridge-bug-diagnostics.md) | Unity Editor bridge 的诊断、读回和受控修改 | 静态分析不足以解释 Unity 运行时状态 |
| [AgentCallable and Scenario Tests](./agentcallable-reusable-tests.md) | AgentCallable、Unity Test Framework 和可复用场景入口 | 新增流程探针、smoke 或 Editor 测试入口 |
| [Quality and Testing](./quality-and-testing.md) | 跨层检查、生成物审查、编译和场景验证 | 完成跨层改动或准备提交 |

## Pre-Development Checklist

### Any Unity task

- [ ] 先确认任务属于 GameHot、ET、共享代码、Editor 工具还是资产/生成流程；不要按目录名称猜测程序集。
- [ ] 涉及 `UNITY_ET`/`UNITY_GAMEHOT`/`UNITY_HOTFIX` 时，检查 `ProcedurePreset`、相关 asmdef 和两条生成输出路径。
- [ ] 涉及 Unity 状态或资产修改时，先阅读已安装 AgentBridge 包的 `AGENT.md`；Bridge session 第一条请求必须是运行时发现的 `list_commands`。
- [ ] 修改 Excel、Proto、Luban、CodeBind 或生成代码时，先确定源文件、generator、输出目录和回读/编译验证；不要手工改生成物。
- [ ] 修改框架托管 UI/Entity 时，确认 owner、进入/退出 API、回调和异步清理；普通 MonoBehaviour、UICell 和普通 ET Entity 不机械套用 GF 生命周期。
- [ ] 先搜索现有 API、扩展方法、容器、消息 handler 和生成配置；能复用就不增加同义 wrapper。

### GameHot task

- [ ] 业务脚本放在 `Unity/Assets/Scripts/Game/Hot/Code/`，稳定初始化放在对应 Loader/非热更层。
- [ ] UI 和 Entity 使用 Game Framework 的 `OnInit`、`OnOpen`/`OnShow`、`OnClose`/`OnHide` 等回调和已有 `GameEntry` API。
- [ ] `UniTask` 结果在应用前检查取消、上下文和对象生命周期；不要用空数据把失败伪装为成功。

### ET task

- [ ] Model 放实体/组件和共享契约，ModelView 放客户端 ET 与 GF view 关联，Hotfix 放逻辑，HotfixView 放客户端 view/system 逻辑。
- [ ] 用 `EntitySystemOf`、`ComponentOf`、`IAwake`/`IDestroy` 和 UGF lifecycle marker 连接 ET owner 与系统；不要在 Mono view 中持有长期业务状态。
- [ ] ETUI/ETEntity 通过 `UIComponent`/`GFEntityComponent` 的 Component 或 Child API 管理数量和所有权。

## Quality Check

- [ ] 成功、失败、取消、stale、版本/顺序和合法空态的行为都有明确 owner。
- [ ] 配置和协议来源、active 状态、生成目录及生成物差异可追溯。
- [ ] 生成文件、Prefab CodeBind 文件和导出资源没有手工补丁。
- [ ] UI/Entity 的根对象没有绕过 owner 的 `SetActive`、手动生命周期回调或重复清理。
- [ ] 新抽象能指出真实第二消费者/实现或外部边界测试替身。
- [ ] Unity 编译/domain reload、`Kit.sln` 工具构建、目标生成检查和必要的 Editor/PlayMode 验证与风险匹配；未运行项已明确记录。
- [ ] 最终 `git status` 没有混入任务范围外的产品、生成或资源变更。

## Canonical Project References

- 总体约定：`CLAUDE.md`、`AGENTS.md`
- 模式选择：`Unity/Assets/Scripts/Game/Procedure/ProcedurePreset.cs`
- GameHot 入口：`Unity/Assets/Scripts/Game/Hot/Loader/`、`Unity/Assets/Scripts/Game/Hot/Code/`
- ET 启动和装载：`Unity/Assets/Scripts/Game/ET/Loader/Init.cs`、`CodeLoader.cs`
- UI/Entity 流程：`Book/UI开发.md`、`Book/Entity开发.md`
- Luban/Proto 流程：`Book/Luban配置.md`、`Book/Proto生成工具.md`
- AgentBridge canonical contract：`Unity/Library/PackageCache/me.xw.unityagentbridge@*/AGENT.md`
