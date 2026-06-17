---
name: aibridge-workflow-orchestration
description: AIBridge workflow and multi-agent orchestration guidance. Use when Codex needs to design, review, or execute a multi-agent workflow plan, split Unity work into parallel or pipeline agent roles, define structured workflow artifacts, choose between batch/multi automation and agent orchestration, run adversarial verification, investigate Runtime debug evidence, sweep multiple Runtime targets, or prepare AIBridge workflow recipes
---

# AIBridge Workflow Orchestration

Focus on workflow recipes, multi-agent orchestration, parallel review, pipeline validation, adversarial verification, Runtime debug investigations, Runtime target sweeps, or structured workflow artifacts

Leave routine single-file edits, simple CLI command lookup, and ordinary Unity validation to `aibridge-development-workflow` and `aibridge`

## Core Rules

- Keep orchestration explicit: phases, roles, dependencies, gates, artifacts, and expected outputs.
- Keep execution visible: when running a workflow, use short status blocks with explicit labels, `【模式：...】` for the active business mode and `-> ...` for phase/step progress.
- Prefer parallel read and serial write. Use parallel write only when worktree isolation, file ownership, merge strategy, and validation gates are explicit.
- Use pipeline for staged per-item work. Use parallel barriers only when a downstream step needs all upstream results.
- Use structured outputs for intermediate results: findings, verdicts, plans, patch proposals, validation results, artifact references, and Runtime target references.
- Treat Skill routing as preflight; scope phase-specific Skills to the phase that needs them, then pass compact handoff summaries and artifact references across phase boundaries.
- Separate claims from evidence. Treat AIBridge CLI, Runtime, screenshots, logs, tests, and Code Index output as evidence.
- Do not describe `workflow` as a generic AI agent scheduler. Current CLI support covers recipe list/validate/plan/init, active run begin/attach/finish, deterministic `run-cli` steps, ordinary-command artifact attach, external result import, adapter export, gates, and reports; `agent` and `manual` steps require an external executor.

## Reference Loading

- Read `references/orchestration-patterns.md` before selecting parallel, pipeline, barrier, adversarial, or Runtime sweep patterns
- Read `references/recipe-schema.md` before writing or reviewing a workflow recipe document
- Read `references/evidence-schema.md` when defining or importing structured external results
- Read `references/builtin-recipes.md` before drafting common Unity implementation, review, Runtime validation, Prefab sweep, or bug-hunter workflows
- For cross-turn, resumed, or external-agent work, follow the active-run, evidence schema, and import rules in `references/orchestration-patterns.md` and `references/recipe-schema.md`
