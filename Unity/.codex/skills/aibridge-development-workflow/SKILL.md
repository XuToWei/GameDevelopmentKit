---
name: aibridge-development-workflow
description: "AIBridge/Unity 多分支开发工作流入口。Use when a task requires code or Unity asset changes, persistent AGENTS/Skill/workflow rule changes, root-cause debugging, Runtime/log evidence, risk review, validation verdicts, or workflow recipes. Do not use for pure Q&A, simple explanation, simple search/display, or read-only analysis with no changes and no verdict"
---

# AIBridge Development Workflow

## 目标

RootRule 的快速任务不进入本 Skill：纯问答、代码解释、简单查找/显示，且不需要修改代码或 Unity 资源、不输出审查/验证/根因结论时，直接回答或执行。作为 AIBridge/Unity 任务入口，本 Skill 完成 Preflight / Skill 路由、Harness 能力探测、任务分流、风险判断、执行和收尾验证。需要修改、调试根因、采集 Runtime/日志证据、输出审查/验证结论或处理 workflow 任务时，必须用轻量明文标出入口步骤、当前业务模式、phase/step 和正在使用的 Skills；完整探测表、完整模式表和冗长内部细节默认不展开。

## 按需读取

- `references/project-workflow-preferences.md`：项目级 Workflow 偏好。安装到 assistant Skill 目录后由 `AIBridge/Workflows` 生成；存在时必须先读取。
- `references/branch-selection.md`：任务分流规则、触发条件和交接边界。安装后的版本会根据项目偏好生成。
- `references/branches/implementation.md`：实施分支规则，仅进入实施分支后读取。
- `references/branches/debug.md`：调试诊断分支规则，仅进入调试诊断分支后读取。
- `references/branches/review.md`：审查分支规则，仅进入审查分支后读取。
- `references/branches/validation.md`：验证分支规则，仅进入验证分支后读取。
- `references/branches/orchestration.md`：编排分支规则，仅进入编排分支后读取。
- `references/branches/requirements.md`：需求讨论分支、方案确认和方案写入规则，仅在需求未明确、边界待定或用户要求先讨论时读取。
- `references/harness-readiness.md`：Harness 能力探测模式、snapshot、fallback、resume、证据回传。
- `references/risk-gates.md`：需求确认、风险分级、必须暂停确认的场景。
- `references/coding-rules.md`：C#、Unity、注释、硬编码、重复代码规则。
- `references/editor-generation.md`：复杂一次性 Editor C# 脚本规范。
- `references/checklist.md`：实施分支最终检查清单。
- `references/debug-investigation-workflow.md`：调试诊断分支流程和证据规则。
- `references/debug-investigation-checklist.md`：调试诊断分支检查清单。

## 可观测输出

开发者需要追踪 AI 当前行为和阶段。进入任务、切换模式、进入 workflow phase/step、释放或重新匹配 Skill 时，必须输出简短状态块；不要只在内部记录。

状态块统一使用三层格式：

- `【入口：...】`：只用于 Preflight / Skill 路由或 Transition Preflight，不表示业务模式。
- `【模式：...】`：只用于需求讨论、实施、调试诊断、审查、验证、编排、检查清单等业务模式。
- `【交接：...】`：只用于 Mode Exit、phase 结束或 step handoff。

进度变化可直接写在对应模式块内，不强制单独步骤前缀。

最小格式：

```text
【入口：Preflight / Skill 路由】
baselineSkills：aibridge-development-workflow
activeSkills：aibridge
主分支：调试诊断分支
理由：用户目标是排查运行时异常，当前输出目标是证据和根因结论。

【模式：调试诊断分支】
Skills：aibridge-development-workflow、aibridge
已加载规范：debug-investigation-workflow
输出目标：收集基线证据并给出根因判断。
使用 Skills：aibridge-development-workflow、aibridge
```

需求未清晰时，先输出需求讨论分支块：

```text
【模式：需求讨论分支】
Skills：aibridge-development-workflow
已加载规范：requirements.md、risk-gates.md
输出目标：收敛需求边界并输出 `.aibridge/plan` 工作底稿。
```

执行中如需说明进度变化，直接写在当前模式块内，不强制单独步骤前缀。需求讨论、方案说明、最终结果、验证结果、错误或阻塞说明仍按任务需要完整说明。

```text
读取 WorkflowReportWriter 和 Skill 入口
定位状态输出模板和导出任务包提示。
使用 Skills：aibridge-development-workflow、aibridge-workflow-orchestration
```

Mode Exit、phase 结束或 step 交接时输出 compact handoff 摘要，至少包含已完成模式、释放的 Skills、下一步建议 Skills、artifact/gate 状态。除非用户明确要求静默或只要最终结果，不要省略这些可观测状态。

## Preflight / Skill 路由

Skill 路由是进入业务模式前的入口步骤，不是可释放的业务模式。它只计算 `baselineSkills`、`activeSkills`、`deferredSkills` 和 `guardedSkills`；释放发生在业务模式、workflow phase 或 step 的 Exit 阶段。

- 如果存在 `references/project-workflow-preferences.md`，必须先读取它，再读取 `references/branch-selection.md`。分支开关、默认验证级别、Runtime 证据偏好、Code Index 偏好和 assistant 提示词前缀以该文件为准。
- 如果需求边界、验收标准或方案方向不清晰，先进入 `references/branches/requirements.md` 对应的需求讨论分支，确认后再继续正式分支选择。
- 主分支只能从项目偏好启用的分支中自动选择。用户明确要求进入禁用分支时，先说明该分支已关闭，并请求用户确认是否临时继续或回到 `AIBridge/Workflows` 启用。
- 需求讨论完成并确认后，再重新执行 Preflight / Skill 路由进入正式主分支。
- 选定主分支后，只读取当前分支对应的 `references/branches/<branch>.md` 和必要 checklist；不要预加载其它分支文档。
- 纯问答、代码解释、简单查找/显示，且不需要修改代码或 Unity 资源、不输出审查/验证/根因结论时，直接回答或执行；如误入本 Skill，停止展开分支和 Harness 探测。
- 只有需要实施修改、调试根因、风险审查/验证结论、Runtime/日志证据，或持久化 AGENTS/Skill/workflow 规则修改的任务，才进入本 workflow 并至少包含 `aibridge-development-workflow`。
- 涉及 AIBridge CLI、Unity 编译、日志、资源、Prefab、场景或 Inspector 操作时，加入 `aibridge`。
- 涉及 Runtime/Player/Play Mode、输入、截图、性能、handler 或运行时调用分析时，加入 `aibridge`；需要多目标采集、对抗验证或 recipe 时，再加入 `aibridge-workflow-orchestration`。
- C# 代码查找、源码导航、符号、定义、引用、实现、派生类型、调用者或诊断查询：只有 `aibridge-code-index` 已安装且项目规则未关闭 Code Index 时，优先加入 `aibridge-code-index`；否则使用 `rg` 和常规文件读取。
- Unity 已导入资源路径查找（脚本、Prefab、Scene、ScriptableObject、贴图、材质、音频、动画、Shader、字体、模型等按资源名/类型/过滤器定位）：当 `aibridge` 可用且 Editor 可用时，优先用 `$CLI asset search/find --format paths`；只有普通仓库文件、未导入文件、`.meta`、ProjectSettings、任意路径正则或 AIBridge 不可用时，才用 `rg --files`。
- 字面量字符串、注释、配置、YAML、非 C# 文件内容或代码内容搜索：使用 `rg -n` 和文件读取；`asset search/find` 不替代全文内容搜索，`asset read_text` 只作为 host 文件读取不可用时的 fallback。
- 复杂 Prefab 资源修改、批量 SerializedProperty、子物体/组件创建或引用写入：加入 `aibridge-prefab-patch`。
- 直接修改 Unity YAML 文本序列化文件，或 AIBridge 不支持的 Prefab/Scene/ScriptableObjectTable 结构修改：加入 `unity-yaml-editing`。
- batch、multi、长脚本、stdin 或脚本自动化：加入 `aibridge-batch-script`。
- Workflow recipe、多 Agent 编排、并行/流水线分工、对抗验证、多目标 sweep、结构化 artifact 或 workflow 方案设计：加入 `aibridge-workflow-orchestration`。
- 创建或修改 Skill：加入 `skill-creator`。
- 复杂一次性 Editor 侧 C# 任务：读取 `references/editor-generation.md`，优先评估 `.aibridge/code/*.csx`。

## 模式生命周期与 Skill 作用域

- `aibridge-development-workflow` 是常驻入口；其它 Skill 默认只在当前主分支、workflow phase 或具体操作内有效。
- 主流程按 `Preflight -> Mode Enter -> Mode Execute -> Mode Exit -> Transition Preflight` 执行。
- `Mode Enter` 激活当前模式真正需要的 Skill；`deferredSkills` 和 `guardedSkills` 只记录触发条件，不读取完整内容。
- 加载可选 Skill 前先确认当前模式需要；不要因为上一模式使用过，就继续携带其详细规则。
- 主分支、workflow phase 或 step 的 Exit 阶段释放本模式专用 Skill 的上下文依赖：停止引用其详细规则，只保留 `SkillHandoff`、artifact refs、gate 状态和必要命令。
- Workflow report/manifest 默认作为 artifact ref 或路径交接；不要为常规状态读取全文，只有追踪 gate、artifact、verdict 细节或用户明确要求时才读取。
- 下一模式如果仍需要同一 Skill，必须重新匹配并按需读取；不要假设已释放 Skill 继续生效。
- 这里的释放是工作流级上下文瘦身规则，不承诺从模型窗口物理删除已读文本；真正清理取决于当前 AI harness 的上下文管理能力。

## Harness 能力探测模式

进入开发、调试、审查、验证或 workflow 任务前，读取 `references/harness-readiness.md` 的入口规则。优先使用 RootRule compact 摘要或 `$CLI harness status` 的 compact 结果；只有 snapshot 缺失、过期、无效，或任务需要未确认能力时，才读取完整 snapshot 或补测任务必需能力。

不把静态检查、`dotnet build` 或推断说成 Unity 验证通过。`agent` / `manual` step 需要当前 harness、主 agent 或外部执行器完成，并用 `workflow import` 回传结构化结果。

## 任务分流

先读取生成的项目偏好，再读取 `references/branch-selection.md`，选择一个已启用主分支；其它分支只作为交接或验证补充。

- **实施分支**：创建、修改、修复、重构、迁移或生成代码/资源。完成后按 `references/checklist.md` 验证。
- **调试诊断分支**：排查、复现、追踪日志/Runtime/Player/Play Mode、分析根因。读取调试 workflow 和 checklist。
- **审查分支**：review/audit/检查风险且用户未要求修改。只读优先，结论区分 confirmed/refuted/uncertain。
- **验证分支**：编译、日志、截图、Runtime/UI 验证或回归确认。选择匹配的 AIBridge 命令或 recipe。
- **编排分支**：workflow recipe、多 Agent、并行 sweep、对抗验证或结构化 artifact。读取 `aibridge-workflow-orchestration` references。
- **需求讨论分支**：需求边界、验收标准或方案方向不明确，或用户要求先分析/先确认时，先进入需求讨论分支，完成方案确认和必要的方案自动写入到 `.aibridge/plan`，再继续正式分支。

## 风险门控

低风险且目标明确时继续执行；以下情况先按 `references/risk-gates.md` 进入需求讨论分支并等待确认：

- 需求目标、交互行为、数据来源或验收标准不明确。
- 多个合理方案会影响公共 API、配置格式、资源结构或用户体验。
- 可能破坏兼容性、迁移数据、批量改 Prefab/场景、删除资源或引入新依赖。
- 用户明确要求先分析、先给方案或不要直接改。

## 执行规则

- 修改前按目标选择定位工具：Unity 已导入资源/脚本/Prefab/Scene/SO 名称用 `$CLI asset search/find --format paths`；C# 语义关系用 Code Index；内容搜索、普通仓库文件和路径正则用 `rg` / `rg --files`。
- 涉及 Unity 对象、Prefab、资源、Console 时，结合 `aibridge` 查询；资源路径候选优先从 AssetDatabase 搜索获得。
- 涉及 Runtime/Player/Play Mode 调试时，先收集 target、日志、截图、性能、handler 或调用结果，再提出候选根因。
- 修改 C# 或 Unity 逻辑前读取 `references/coding-rules.md`。
- 使用一次性 Editor 脚本前读取 `references/editor-generation.md`。
- 使用多 Agent 编排前读取 `aibridge-workflow-orchestration/references/orchestration-patterns.md`。

## 收尾

结束前按主分支执行对应检查清单。最终回复只报告实际执行的验证、失败/阻塞项、未验证原因和必要的关键改动；不重复完整流程表，但不能隐藏实际进入过的模式、关键步骤和使用过的 Skills。
