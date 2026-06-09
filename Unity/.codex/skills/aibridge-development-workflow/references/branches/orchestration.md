# 编排分支

## 适用场景

用户要求 workflow recipe、多 Agent、并行 sweep、对抗验证、结构化 artifact、Runtime 多目标 sweep 或跨阶段任务时，进入编排分支。

## 进入规则

1. 先确认项目偏好中编排分支已启用。
2. 加载 `aibridge-workflow-orchestration`。
3. 根据任务类型读取该 Skill 的 `references/orchestration-patterns.md`、`references/recipe-schema.md`、`references/evidence-schema.md` 或 `references/builtin-recipes.md`。

## 执行规则

- 编排分支只定义流程、角色、artifact 和 gate。
- 具体 Unity 对象修改仍交给实施分支串行完成；如果实施分支被禁用，先请求确认。
- `agent` / `manual` step 需要外部 executor，AIBridge CLI 只负责记录、导出和导入结构化结果。
- 每个阶段结束时输出 compact handoff：已完成模式、释放的 Skills、下一步建议 Skills、artifact refs、gate 状态和未关闭风险。
