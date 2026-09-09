# AgentCallable and Reusable Scenario Tests

## Scope and tool choice

AgentCallable 用于把已经理解清楚、需要重复执行的 Unity Editor/PlayMode 流程串成一个薄入口；它不是第二套测试框架。目标工程当前没有游戏领域的现成 AgentCallable 目录/方法模板，新增前必须确认 Editor-only assembly 和当前包 registry。

| 机制 | 适用 |
|---|---|
| `execute_csharp` | 一次性读状态、探索和临时诊断 |
| `AgentCallable` | 无参数、可重复、可断言、可恢复的完整场景 |
| `run_tests` | 参数矩阵、NUnit/Unity Test Framework、标准报告和 CI |
| Editor `ICommandHandler` | 需要固定 JSON 输入/输出的工具 |

## Admission criteria

一次性探针只有同时具备以下条件才沉淀为 callable：

- 有后续复用价值；
- 无参入口能用固定夹具、配置或隔离本地状态建立前置条件；
- 关键结果能从真实状态 owner 读回并断言；
- 重复执行不依赖随机候选、上次残留或不可控时序；
- 成功/失败都能在 `finally` 恢复 UI、场景、内存和异步状态；
- 不产生未授权的真实账号、购买、消费或远端写入副作用；
- 异常包含阶段、期望和实际值。

不满足时继续用只读诊断或 Unity Test Framework，不为一次性问题新增测试基础设施。

## Signature and placement

当前包 `AgentCallableMethodRegistry` 要求：无参、static、非开放泛型、非 `async void`；异步返回 `Task`、`ValueTask`、`UniTask` 或可 await 类型；description 非空，timeout 在当前包允许范围内。实际限制以当前包代码和 `list_agent_methods` 为准，method ID 必须从 discovery 原样复制，不能自行拼接。

项目 Editor-only 入口可放在已有：

- `Unity/Assets/Scripts/Game/Editor/`（Game.Editor，含 AgentBridge.Editor）；
- `Unity/Assets/Scripts/Game/Hot/Code/Editor/`（Game.Hot.Code.Editor）；
- `Unity/Assets/Scripts/Game/ET/Code/Editor/`（Game.ET.Code.Editor）。

AgentBridge 引用不得进入 Player Runtime。ET 场景逻辑应放在 ET owner/system 或普通可复用方法中，callable 只做编排。

## Scenario shape

```csharp
[AgentCallable("固定场景、动作和成功条件", 120)]
private static async UniTask RunScenarioAsync()
{
    ScenarioState before = CaptureState();
    try
    {
        await ArrangeAsync();
        await ActThroughProductionApiAsync();
        await WaitForExpectedStateAsync();
        ScenarioState after = ReadAuthoritativeState();
        AssertExpected(before, after);
    }
    finally
    {
        RestoreOwnedState();
    }
}
```

正常结束才表示通过；`invoke_agent_method` 忽略返回值，失败必须抛异常。入口需要防止重复运行，锁定状态要在最外层 `finally` 清除。不要复制登录、等待、UI 查找、ET owner 或资源清理 helper；先搜索已有实现。

## Determinism and side effects

固定输入优于随机选择。运行前读取账号/场景/对象身份，运行后从相同状态 owner 读回。只恢复本次创建、打开或修改的内容，不关闭用户原本拥有的 UI，不把猜测值当作旧值。真实远端动作使用隔离数据或在具体副作用前取得明确授权。

`timeoutSeconds` 是 Agent 等待预算，不会自动取消 Unity 方法；超时后继续轮询同一 exchange，不并发发第二个请求。`METHOD_NOT_FOUND` 后重新 discovery；不要猜 ID。

## Verification checklist

- [ ] compile/domain reload 后 `list_agent_methods` 能发现恰好一个完整目标 ID。
- [ ] Arrange、Act、Assert、cleanup 顺序明确，Act 失败不继续成功断言。
- [ ] 重复运行、并发运行、Arrange 失败、Assert 失败都有可诊断结果。
- [ ] 同步入口没有阻塞 Editor 主线程，异步入口没有 `async void`。
- [ ] 成功和异常都执行恢复，未遗留 dirty scene/asset、静态锁或在途任务。
- [ ] 参数矩阵和长期回归使用 `run_tests`，不把 callable 扩展成手工 test runner。

## References

- `Unity/Library/PackageCache/me.xw.unityagentbridge@*/AGENT.md`
- `Unity/Library/PackageCache/me.xw.unityagentbridge@*/Editor/Commands/Mutation/AgentCallableMethodRegistry.cs`
- `Unity/Library/PackageCache/me.xw.unityagentbridge@*/Editor/Testing/RunTestsHandler.cs`
- `Unity/Assets/Scripts/Game/Editor/Game.Editor.asmdef`
- `Unity/Assets/Scripts/Game/Hot/Code/Editor/Game.Hot.Code.Editor.asmdef`
- `Unity/Assets/Scripts/Game/ET/Code/Editor/Game.ET.Code.Editor.asmdef`
