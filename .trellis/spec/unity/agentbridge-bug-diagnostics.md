# AgentBridge Bug Diagnostics

## Scope

当静态代码和普通测试不足以确认 Unity Editor 中的调用顺序、对象状态、配置读回或运行时内存时，可用 AgentBridge 建立定向诊断。动态探针是证据和验证手段，不替代正式测试、编译或真实端到端流程。

Canonical contract 必须每次从当前安装包读取：`Unity/Library/PackageCache/me.xw.unityagentbridge@*/AGENT.md`。目标 Bridge root 是已存在的 `Unity/.agentbridge/`；找不到同时包含 `Assets/` 与既有 `.agentbridge/` 的工程时停止，不自行创建目录。

## Exchange invariants

每个 session：

1. 第一条请求是 `list_commands`，缓存 command schema、batch policy 和 `commandsVersion`。
2. 只使用运行时返回的 command 和 params schema，不凭本文或源码猜测字段。
3. 一次只发一个 exchange；请求先写 `request.json.tmp`，再原子 rename 为 `request.json`。
4. 使用全新、非空且不超过 64 字符的 ID；完整读取并核对对应 `response.json`。
5. 等待 Unity 删除 `processing.json` 后，才删除 `response.json` 作为 ack。
6. 只有版本变化、扩展启停或 `UNKNOWN_COMMAND` 才重新 discovery。

不要删除 `processing.json`，不要覆盖正在处理的 request/response，也不要并发发送第二个固定槽位请求。

## Diagnostic method

先读后改：

```text
Read-only baseline
  → 最小、可恢复的现有 API 操作
  → 从真正 owner 读回
  → try/finally 恢复临时内存/场景/UI
  → 修复后用同一探针复跑
```

返回值使用有界、结构化 JSON；不要返回整个 World、GameEntry、UnityEngine.Object 图或未经限制的集合。64 位 ID、版本和计数跨 JSON 时优先转成十进制字符串。动态 C# 只引用当前 AppDomain 已加载程序集，不在 snippet 中触发 refresh、recompile、domain reload 或远端高副作用请求。

优先调用公开领域入口：GameHot 可从 `Game.Hot.HotEntry`/现有组件读取；ET 可从 `World`、fiber、Entity/component 或 UGF owner 读取。不要把反射修改私有字段作为默认复现方式，不要建立第二套状态容器。

如果必须临时写入 Unity `GameObject`/`Component`，遵守当前包的 object creation/modification/destroy tracking；普通托管对象、GameData 或 ET Entity 没有自动回滚，必须保存旧值并在 `finally` 恢复，或明确由生命周期清理。

## Error handling

| 结果 | 处理 |
|---|---|
| `INVALID_REQUEST`/`INVALID_PARAMS` | 按当前 schema 修正，换新 ID；不原样重试。 |
| `UNKNOWN_COMMAND` | 重新 `list_commands`，确认命令仍启用。 |
| `COMMAND_DISABLED` | 不绕过禁用；让 Unity 管理器启用。 |
| `HANDLER_EXCEPTION`/领域命令错误 | 先检查写操作是否已发生，再根据 message 定位 handler、探针或产品逻辑。 |
| `INTERRUPTED` | 副作用未知；先只读检查实际状态，再决定恢复或重试。 |
| `RESPONSE_TOO_LARGE` | 缩小 root、depth、range、limit 或返回投影。 |
| Unity compiling/importing/PlayMode switching | 等稳定并使用新 ID；脚本修改后先等 compile/domain reload。 |

默认保持 safety/filesystem checks，不因为有检查就执行文件、进程、无限循环或未授权远端操作。

## Validation

- [ ] 修复前 baseline 能证明具体症状和对象身份。
- [ ] Act 使用现有公开 API，失败立即停止，不继续假设成功。
- [ ] Readback 从状态 owner 重新读取，不复述刚写入的输入。
- [ ] 临时变更有可证明的恢复路径，成功和失败都清理。
- [ ] 修复后以相同初始条件复跑，再补跑正式 Unity/ET 测试。
- [ ] 编译和 Console 无本次操作产生的未解释错误。

## References

- `Unity/Library/PackageCache/me.xw.unityagentbridge@*/AGENT.md`
- `Unity/.agentbridge/`
- `Unity/Assets/Scripts/Game/Editor/AgentBridge/`
- `Unity/Assets/Scripts/Game/Hot/Code/Base/HotEntry.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/Init.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/CodeLoader.cs`
