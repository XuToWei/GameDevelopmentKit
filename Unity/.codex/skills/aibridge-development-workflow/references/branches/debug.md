# 调试诊断分支

## 适用场景

用户要求排查问题、追踪日志、复现异常、分析 Runtime/Player/Play Mode 行为、定位 UI/性能/输入问题时，进入调试诊断分支。

## 进入规则

1. 先确认项目偏好中调试诊断分支已启用。
2. 读取 `debug-investigation-workflow.md` 和 `debug-investigation-checklist.md`。
3. 需要 Runtime、Player、截图、性能、handler 或多目标 sweep 时，按需加载 `aibridge-workflow-orchestration`。

## 执行规则

- 默认目标是证据和根因判断，不是立即改代码。
- 先收集日志、状态、截图、Runtime target 或可复现步骤，再提出候选根因。
- 不在弱证据下修改代码或资源。
- confirmed 根因且用户要求修复时，生成 handoff 并交接到实施分支；如果实施分支被禁用，先请求确认。

## Runtime 偏好

如果 `project-workflow-preferences.md` 中启用了 Runtime 证据偏好，Runtime/Player/Play Mode 相关问题应优先尝试 Runtime target、截图或 runtime_execute。Runtime 不可用时，明确标记为未验证。
