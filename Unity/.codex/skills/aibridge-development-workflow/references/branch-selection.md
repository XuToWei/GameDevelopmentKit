# 任务分流规则

> 本文件由 AIBridge/Workflows 生成。进入分支判定前先读取 `project-workflow-preferences.md`。

## 工作流生命周期

```text
Preflight / Skill Routing
  -> Mode Enter
  -> Mode Execute
  -> Mode Exit / SkillHandoff / Release
  -> Transition Preflight
```

- Preflight / Skill Routing 是入口步骤，不是业务模式；它只选择主分支并计算 Skill 状态。
- 如果需求边界、验收标准或方案方向不清晰，先进入需求讨论分支，确认后再继续正式分支选择。
- Mode Enter 只激活当前分支真正需要的 Skill，并读取该分支文档。
- Mode Exit 生成 `SkillHandoff`，并释放下一模式不需要的模式专用 Skill。

## 需求讨论分支

需求讨论分支是 Preflight 的前置分支，不是可选主分支。它只在需求不清晰、边界待定、方案方向分歧，或用户要求先分析/先确认时触发。

- 目标是收敛目标、边界、非目标、约束、方案选项和确认结论。
- 若用户要求，或项目存在相应功能文档归类，确认后的方案必须先写入 `.aibridge/plan` 工作底稿，再按需同步到正式文档位置。
- 默认以 Markdown 工作底稿作为方案源文件；当方案包含流程图、决策树、对比表，或更适合开发者浏览时，再在每个落点目录内同步生成 HTML 展示页。
- Markdown 和 HTML 应保持同目录、同 basename；`.aibridge/plan` 负责 AI 续跑和多 agent 协作，正式文档负责 Git review 和对外呈现。

## 可选主分支

| 主分支 | 触发信号 | 默认目标 | 进入后读取 | 常用 Skills / 工具 |
|---|---|---|---|---|
| 实施分支 | 创建、修改、修复、重构、生成、迁移、提交 | 改动当前工作树并验证 | `references/branches/implementation.md` | `aibridge、aibridge-code-index、aibridge-prefab-patch、unity-yaml-editing、aibridge-batch-script` |
| 调试诊断分支 | 排查、诊断、复现、为什么、追踪、日志、Runtime、Player、Play Mode、性能、UI 异常 | 收集证据并给出根因判断 | `references/branches/debug.md` | `aibridge、aibridge-code-index、aibridge-workflow-orchestration、aibridge-batch-script` |
| 审查分支 | review、audit、检查风险、设计评审、只读分析 | 输出 confirmed findings 和剩余风险 | `references/branches/review.md` | `aibridge-code-index、rg、按需 aibridge-workflow-orchestration` |
| 验证分支 | 编译、日志、截图、测试、Runtime/UI 验证、回归确认 | 给出可重复验证结果 | `references/branches/validation.md` | `aibridge、现有 workflow recipe` |
| 编排分支 | workflow recipe、多 Agent、并行 sweep、对抗验证、结构化 artifact | 设计或执行结构化 workflow | `references/branches/orchestration.md` | `aibridge-workflow-orchestration` |

## 交接规则

- 调试诊断分支发现 confirmed 根因且用户要求修复时，交接到实施分支；如果实施分支被禁用，先请求用户确认。
- 实施分支完成改动后，按风险选择验证分支补充 Runtime、截图、UI 或多目标证据；如果验证分支被禁用，说明剩余风险。
- 审查分支发现问题后，未得到修复授权前不直接改文件。
- 编排分支只定义流程、角色、artifact 和 gate；具体 Unity 对象修改仍由实施分支串行完成。
- Mode Exit 或分支交接时同步交接 Skill 作用域：列出已释放的模式专用 Skill、下一分支建议加载的 Skill、必要 artifact refs、gate 状态和未关闭风险。

## 输出格式

```text
【入口：Preflight / Skill 路由】
baselineSkills：aibridge-development-workflow
activeSkills：<当前分支 Skills>
主分支：<启用分支之一>
理由：<进入该分支的依据>

【模式：需求讨论分支】
Skills：aibridge-development-workflow
已加载规范：requirements.md、risk-gates.md
输出目标：收敛需求边界并输出 `.aibridge/plan` 工作底稿。

【模式：<启用分支之一>】
Skills：<当前分支 Skills>
已加载规范：<当前分支文档>
输出目标：<本模式的验收目标>
```

