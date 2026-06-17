# 验证分支

## 适用场景

用户要求编译、日志、测试、截图、Runtime/UI 验证、回归确认，或实施分支完成后需要验证时，进入验证分支。

## 进入规则

1. 先确认项目偏好中验证分支已启用。
2. 读取 `harness-readiness.md`，优先使用 `.aibridge/harness/capabilities.json` 或 `$CLI harness status` 的 compact 结果。
3. 根据项目默认验证级别选择命令。

## 默认验证级别

- `compileOnly`：执行 `$CLI compile unity`。
- `compileAndLogs`：执行 `$CLI compile unity` 和 `$CLI get_logs --logType Error`。
- `compileLogsAndRuntime`：执行编译、Error 日志检查，并在 Runtime target 可用时补充 Runtime 证据。

## 输出规则

- 只报告实际执行过的验证，不把静态检查或 dotnet build 冒充 Unity 编译。
- 如果 Unity、Runtime、测试或日志检查不可用，说明不可用原因和剩余风险。
- Runtime 证据偏好启用时，Runtime/UI/Player 相关验证应优先尝试可用 target。
