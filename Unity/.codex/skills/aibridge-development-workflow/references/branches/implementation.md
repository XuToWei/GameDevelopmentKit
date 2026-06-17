# 实施分支

## 适用场景

用户要求创建、修改、修复、重构、迁移、生成代码或资源，且目标和验收标准足够明确时，进入实施分支。

## 进入规则

1. 先确认项目偏好中实施分支已启用。
2. 如果需求边界、非目标、约束或验收标准还没锁定，先进入需求讨论分支并完成方案确认与必要的方案自动写入到 `.aibridge/plan`，再按需同步正式文档，不直接进入实施分支。
3. 修改前读取 `risk-gates.md` 和 `coding-rules.md`。
4. 涉及复杂一次性 Editor 侧 C# 任务时读取 `editor-generation.md`。
5. 只在当前任务确实需要时加载 `aibridge-code-index`、`aibridge-prefab-patch`、`unity-yaml-editing` 或 `aibridge-batch-script`。

## 执行规则

- 先定位真实代码路径，再修改。
- 保持改动范围贴合用户目标，不做无关重构。
- 涉及 Unity 对象、Prefab、资源、Console 时优先结合 AIBridge/Unity API 验证。
- 修改完成后按项目默认验证级别执行 `checklist.md`。

## 默认验证

验证级别以 `project-workflow-preferences.md` 为准：

- `compileOnly`：执行 `$CLI compile unity`。
- `compileAndLogs`：执行 `$CLI compile unity` 和 `$CLI get_logs --logType Error`。
- `compileLogsAndRuntime`：执行编译、Error 日志检查，并在 Runtime target 可用时补充 Runtime 证据。

无法执行的验证必须说明原因，不能标记为通过。
