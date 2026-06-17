# 项目 Workflow 偏好

> 本文件由 AIBridge/Workflows 根据项目设置生成。不要手动编辑；修改请回到 Unity 的 `AIBridge/Workflows > Workflow Options`。

- Assistant: Codex (`codex`)

## 启用分支

| 分支 | 状态 | 分支文档 |
|---|---|---|
| 实施分支 | 启用 | `references/branches/implementation.md` |
| 调试诊断分支 | 启用 | `references/branches/debug.md` |
| 审查分支 | 启用 | `references/branches/review.md` |
| 验证分支 | 启用 | `references/branches/validation.md` |
| 编排分支 | 启用 | `references/branches/orchestration.md` |

## 默认验证策略

- 默认验证级别：Unity 编译 + Error 日志 (`compileAndLogs`)
- Runtime 证据偏好：仅在任务明确需要时收集 Runtime 证据
- Code Index 偏好：Code Index 可用时优先用于 C# 语义查询

## 附加提示词

### 通用提示词前缀

未设置。

### Codex 专属提示词前缀

未设置。


## 执行规则

1. Preflight / Skill 路由前必须先读取本文件。
2. 只在启用分支中选择默认主分支；禁用分支不能自动进入。
3. 如果用户明确要求进入禁用分支，先说明该分支已关闭，并请求用户确认是否临时继续或回到 Workflows 面板启用。
4. 选择主分支后，只读取该分支对应的 `references/branches/<branch>.md`，不要预加载其它分支文档。
5. 验证和证据收集默认遵守本文件中的验证策略。

## 元数据

- Settings Hash: `e5467d2ad0001df0a5c25f52ae1b5f4bdce4574783286b53106aaf594c34f7a0`
