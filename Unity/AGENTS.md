# AGENTS.md

## Basic Principles
1. Prefer concise English replies unless the user asks otherwise.
2. Add necessary comments for complex business logic.
3. Respect existing user changes and do not revert unrelated files.

## Project Validation
- Unity compilation must use `$CLI compile unity`.
- `compile dotnet` is only an extra check and must not replace or fallback from Unity compilation.

<!-- AIBRIDGE:START {"assistant":"aibridge","templateId":"unity-integration","version":7,"target":"root-rule"} -->
## AIBridge Bootstrap

**CLI Alias**: `$CLI = ./.aibridge/cli/AIBridgeCLI.exe`

**Common Commands**:
```bash
$CLI compile unity
$CLI get_logs --logType Error
$CLI editor log --message "Hello" --logType Warning
```

**Host Exec**:
- When AIBridge CLI is available, prefer `$CLI exec run --stdin` for external host tools such as `rg`, `git`, `dotnet`, `python`, `node`, `sg`, or `grep`, including quick search/display tasks; use `$CLI exec batch --stdin` for multiple jobs. Direct host shell is only for trivial one-off commands, explicit user preference, or when AIBridge CLI is unavailable.

**Routing Rules**:
- Quick tasks: answer or execute directly without loading `aibridge-development-workflow` for pure Q&A, code explanation, simple search/display, or tasks with no code or Unity asset changes and no review, validation, or root-cause verdict.
- Workflow tasks: load `aibridge-development-workflow` first when the task requires code or Unity asset changes, persistent AGENTS/Skill/workflow rule changes, root-cause debugging, Runtime/log evidence, or a risk review/validation verdict.
- After entering the workflow, `aibridge-development-workflow` probes harness readiness, chooses the task branch, and decides whether to load additional Skills.

**Skill Loading**:
- Load `aibridge-development-workflow` from `/.codex/skills/aibridge-development-workflow/SKILL.md` before workflow tasks.
- AIBridge Skills are installed under `/.codex/skills/<skill-name>/SKILL.md`; load sibling Skills from that directory when this root rule or the workflow requires them.

**Project Version**:
- Current Unity version: 6000.3.12f1
- Current C# language requirement: compatible with C# 9.0; do not use newer syntax.

**Current Capabilities**:
- Harness capability snapshot: `.aibridge/harness/capabilities.json`. RootRule only provides a compact summary; for workflow tasks that need capability confirmation, use compact `$CLI harness status` first and read the full snapshot or run full probes only when it is missing, stale, or the task needs an unconfirmed capability. Selected assistants: codex. Skill root: .codex/skills. Code Index: disabled. External agent/sub-agent capability: unknown to Unity.
- Code Index: disabled. Do not call `code_index`; use `asset search/find --format paths` for Unity imported asset name/type lookup when AIBridge and the Editor are available, and use `rg` plus file reads for ordinary code/content searches.
<!-- AIBRIDGE:END -->
