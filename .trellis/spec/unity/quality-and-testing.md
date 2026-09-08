# Quality and Testing

## Cross-layer review sequence

提交跨层改动前，按实际模式追踪：

```text
Design/Excel or Design/Proto
  → generator/config target
  → generated code/data
  → Loader / ET Model or GameHot owner
  → Hotfix/ModelView/HotfixView or GF logic
  → UI/Entity view
```

逐项确认源文件、格式、owner、错误语义、生命周期和生成输出没有在边界处漂移。

## Required checks by change type

| 改动 | 至少检查 |
|---|---|
| GameHot 业务 | 对应 `GameEntry`/HotEntry、GF lifecycle、EntityData、资源和表引用 |
| ET 业务 | Model/ModelView/Hotfix/HotfixView 分层、EntitySystem、owner、生成消息和 domain reload |
| UI/Entity | Open/Close 或 Show/Hide、View binding、owner cleanup、两轮进入/退出和异步退出 |
| Proto | `proto.conf` active、codeType、opcode 区间、消息排序、所有 output dirs |
| Luban/Excel | target active、validate/export、生成 C#/data、无手工生成物改动 |
| AgentBridge | `list_commands`、运行时 schema、single-flight、原子 exchange、response ack |
| 热更/异步 | CodeRunner 入口、程序集/code bytes、UniTask cancellation 和 await 后有效性 |

## Test and verification reality

目标游戏代码目前没有成熟的统一领域 NUnit fixture 目录；ET 的 demo test component、robot case 和 UGF entity test 是运行时代码示例。安装的 AgentBridge 包包含 Unity Test Framework 测试和 `run_tests`，但它们主要验证 bridge package，不自动证明游戏领域行为。

因此：

- 确定性流程可在已有 Editor-only asmdef 中实现 `AgentCallable`，但必须满足无参、可断言、可恢复和无未授权副作用条件。
- 参数矩阵、长期回归和标准报告使用当前包的 `run_tests`/Unity Test Framework，不手工编排第二套 runner。
- 没有真实 fixture 或可运行 Unity 环境时，不声称场景测试已通过；明确区分静态审查、工具构建和运行时验证。

## Minimum matrix

### Configuration and generated output

- 目标 `luban.conf`/`proto.conf` active 状态正确。
- 修改源后 `Kit.sln` 构建成功，validate/export 或 Proto2CS 成功。
- 生成代码、bin/json、Localization、`Config/Luban` 和多 output dirs 与源变更匹配。
- 生成文件、`.Bind.cs`、`.bytes` 没有手工补丁或无关 diff。

### Runtime modes

- 当前目标模式的 `ProcedurePreset` 分支启动正确。
- GameHot 运行验证 `Game.Hot.Init`；ET 运行验证 `ET.Init`、World/CodeLoader 和对应系统注册。
- 如果改动共享边界，至少静态检查另一模式的 asmdef、生成路径和编译约束。

### Lifecycle and async

- UIForm/Widget/Entity 连续进入和退出两轮不累积事件、资源、子对象或任务。
- ET entity dispose 与 GF hide 的语义没有混淆。
- 异步完成前关闭/销毁/切换上下文不会旧结果回写。
- 失败、取消、stale 和合法空态与成功数据的处理可区分。

### AgentBridge and tools

- session 首个 exchange 是 `list_commands`，后续使用当前 schema 和新 ID。
- command 操作按 canonical fixed-slot 协议完成；修改后等待 compile/domain reload。
- 文件修改有版本/冲突保护；结果从源表、生成目录或 Unity 状态读回。
- 诊断探针有 baseline、readback 和恢复；不能只返回 `done` 或依赖截图。

## Anti-patterns

- 把 ET MemoryPack、UGF Protobuf 和 GameHot packet 类型混用。
- 把成功失败转成默认对象/空列表并继续执行。
- 在 UI 直接解析 response，与领域 owner 并行维护状态。
- 对框架管理的根节点 `SetActive`，或手动调用生命周期回调。
- 在每个 consumer 创建同义解析、请求、容器和 fallback。
- 直接修改生成代码/资源，或未确认 schema 就猜 AgentBridge 参数。
- 用随机输入或真实不可逆远端副作用作为确定性回归。

## Completion checklist

- [ ] 已阅读适用 Unity spec 和 shared guides。
- [ ] 已检查完整跨层数据流与所有权。
- [ ] 已完成与风险匹配的静态、Kit.sln、生成、Unity compile、Editor/PlayMode 或 run_tests 验证。
- [ ] 未运行项及原因已记录。
- [ ] 已检查源/生成/资源 diff，未混入无关文件。
- [ ] 文档引用路径存在，spec index 与实际文件集一致。

## References

- `Unity/Assets/Scripts/Game/Procedure/ProcedurePreset.cs`
- `Unity/Assets/Scripts/Game/ET/Code/Model/Client/Test/TestComponent.cs`
- `Unity/Assets/Scripts/Game/ET/Code/Hotfix/Client/Test/TestComponentSystem.cs`
- `Unity/Assets/Scripts/Game/ET/Code/Hotfix/Server/Demo/Map/C2M_TestRobotCaseHandler.cs`
- `Unity/Library/PackageCache/me.xw.unityagentbridge@*/Editor/Testing/RunTestsHandler.cs`
- `Unity/Library/PackageCache/me.xw.unityagentbridge@*/Tests/ProductEditMode/`
- `Kit.sln`
