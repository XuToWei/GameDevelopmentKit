# Recipe Schema

Purpose: define AIBridge workflow recipe JSON files used by `workflow validate`, `workflow plan`, `workflow init`, `workflow run-cli`, active run attach, external result import, and adapter export.

## Locations

```text
Templates~/Workflows/<name>.aibridge-workflow.json
.aibridge/workflows/recipes/<name>.aibridge-workflow.json
.aibridge/workflows/runs/<runId>/
```

Package templates live under `Templates~/Workflows`. Project-local recipes live under `.aibridge/workflows/recipes`. A run writes its manifest, inputs, phase/step state, command results, artifacts, gates, and report under `.aibridge/workflows/runs/<runId>/`.

## CLI

```bash
$CLI workflow list
$CLI workflow validate --recipe runtime-target-sweep
$CLI workflow plan --recipe runtime-debug-investigation --format markdown
$CLI workflow plan --recipe runtime-ui-validation --format markdown
$CLI workflow init --recipe runtime-ui-validation
$CLI workflow begin --recipe unity-change-implementation
$CLI workflow run-cli --file ".aibridge/workflows/recipes/runtime-target-sweep.aibridge-workflow.json" --inputs ".aibridge/workflows/inputs.json"
$CLI workflow run-cli --recipe unity-sharded-review --allow-partial true
$CLI workflow run-cli --recipe unity-sharded-review --resume <runId> --rerun failed
$CLI harness status
$CLI get_logs --logType Error --workflow-run <runId>
$CLI runtime screenshot --target latest --workflow-run <runId>
$CLI workflow import --run <runId> --step adversarial-verify --schema Verdict --file verdicts.json
$CLI workflow import --run <runId> --step collect-evidence --schema EvidenceRef --kind evidence --file evidence-refs.json
$CLI workflow export --recipe runtime-ui-validation --target codex-task-pack --output .aibridge/workflows/exports
$CLI workflow status --run <runId>
$CLI workflow report --run <runId> --format markdown
$CLI workflow finish --run <runId> --status passed
$CLI workflow clean --older-than 30d --dry-run true
$CLI workflow clean --older-than 3d --dry-run false --keep-failed true --keep-latest 20
$CLI workflow clean --older-than 3d --save-settings true --auto-clean true
```

`run-cli` executes only deterministic `cli`, `barrier`, and `report` steps. It records `agent` and `manual` steps as `skipped_requires_external_executor`; external tools such as Codex, Claude, or Cursor remain responsible for those steps. `workflow run-cli --resume <runId>` still requires `--file` or `--recipe`; resume selects the existing run, while file/recipe selects the recipe definition to execute. `partial` is not a CLI success unless `--allow-partial true` is passed explicitly.

`begin` creates a run and writes `.aibridge/workflows/active-run.json`. Ordinary commands attach evidence when they receive `--workflow-run <runId>`, when `AIBRIDGE_WORKFLOW_RUN_ID` is set, or when an active run exists. `workflow status` and `workflow report` do not default to the active run; pass `--run <runId>` explicitly, reading `.aibridge/workflows/active-run.json` first when needed. `finish` refreshes gates/report and clears the active run pointer. `finish --status passed` is downgraded when required gates are failed, blocked, or missing evidence.

`import` copies structured external results into run artifacts. `Verdict.status` must be `confirmed`, `refuted`, or `uncertain`; `externalVerdict` gates pass only from imported Verdict artifacts, not from prose summaries. `ValidationResult` imports use the `validation-report` artifact kind by default.

For resumed work, run `workflow status --run <runId>` before adding new evidence. Status, run-cli, finish, and JSON report outputs are compact by default; use `--detail full` only when the full manifest JSON is needed. Default handoff should keep `runDirectory`, `manifestPath`, `reportPath`, artifact ids, gate summaries, and gaps as refs; do not read full `manifest.json` or Markdown reports for routine status. Use active-run attachment only when the current task clearly belongs to that run; otherwise pass `--workflow-run <runId>` explicitly or start a new run.

`export` compiles a recipe into an external task package or script (`codex-task-pack`, `generic-cli`, `claude-workflow`). Exporters do not run external agents and do not provide an LLM runtime.

`clean` is safe by default (`dry-run=true`). Persisted auto-clean settings live in `.aibridge/workflows/settings.json`; when `autoCleanEnabled=true`, `workflow run-cli` opportunistically removes old runs before starting a new run while respecting `keepFailed`, `keepLatest`, and `maxDeletePerRun`.

## Recipe Shape

Required top-level fields:

- `schemaVersion`: must be `1`.
- `name`: lower kebab-case recipe id.
- `description`: concise purpose.
- `phases`: ordered phase definitions.
- `gates`: validation gates.

Optional fields:

- `title`
- `version`
- `inputs`
- `requiredSkills`
- `artifacts`

```json
{
  "schemaVersion": 1,
  "name": "runtime-target-sweep",
  "title": "Runtime Target Sweep",
  "description": "Collect Runtime target evidence.",
  "version": "1.0.0",
  "inputs": {
    "target": { "default": "latest" }
  },
  "requiredSkills": ["aibridge-development-workflow"],
  "phases": [],
  "gates": [],
  "artifacts": []
}
```

## Phase Shape

```json
{
  "id": "collect",
  "type": "serial",
  "description": "Collect Runtime evidence.",
  "dependsOn": ["discover"],
  "itemSource": "inputs.targets",
  "requiredSkills": ["aibridge"],
  "releaseSkillsAfter": [],
  "steps": []
}
```

Allowed `type` values:

- `serial`
- `parallel`
- `pipeline`
- `barrier`
- `report`

`dependsOn` may only reference earlier phases. `itemSource` is syntax-only in the current CLI and is intended for later parallel/pipeline expansion by an external executor.

## Step Shape

```json
{
  "id": "runtime-status",
  "kind": "cli",
  "description": "Check target status.",
  "command": "runtime status --target {{target}}",
  "requiredSkills": ["aibridge"],
  "releaseSkillsAfter": [],
  "outputs": ["RuntimeTargetRef", "ValidationResult"]
}
```

Allowed `kind` values:

- `cli`: executed by `workflow run-cli`.
- `agent`: external AI executor; recorded but not executed by AIBridge.
- `manual`: main-agent or human decision; recorded but not executed by AIBridge.
- `barrier`: lightweight merge/check step; recorded as passed by `run-cli`.
- `report`: final reporting step; recorded as passed by `run-cli`.

Template variables use `{{name}}` or `{{inputs.name}}` and are resolved from the merged recipe defaults plus `--inputs`. Prefer passing `--inputs` as a JSON file path; inline JSON is shell-quoting sensitive, especially in PowerShell.

## Skill Routing And Scope Metadata

`requiredSkills` and `releaseSkillsAfter` are workflow metadata for external AI harnesses and exported task packs. AIBridge CLI validates and surfaces these fields, but it does not install, unload, or physically remove Skills from a model context.

Skill routing is a preflight step, not a recipe phase or business mode. It computes baseline, active, deferred, and guarded Skills before entering a mode. Release is evaluated at Mode Exit, phase boundary, or step handoff, not when routing finishes.

When an AI harness executes a recipe, this metadata must also be reflected in short visible status blocks at phase/step entry and handoff. Use explicit labels: `【入口：...】` only for routing, `【模式：...】` for business modes, and `-> ...` for phase/step progress. Put active Skills, expected output, handoff, and gate status on separate lines instead of packing them into one sentence.

Allowed locations:

- Recipe `requiredSkills`: baseline Skills expected for the whole recipe.
- Phase `requiredSkills`: active Skills needed during Mode Enter / phase execution.
- Phase `releaseSkillsAfter`: phase-scoped active Skills that should be released at phase Exit after the handoff is written.
- Step `requiredSkills`: active Skills needed by that step, especially `agent` and `manual` steps.
- Step `releaseSkillsAfter`: step-scoped active Skills that should be released after the step output is imported.

At a phase or step boundary, pass a compact handoff instead of previous Skill details:

```json
{
  "completedMode": "runtime-validation",
  "releasedSkills": ["aibridge-workflow-orchestration"],
  "nextRecommendedSkills": ["aibridge"],
  "summary": "Runtime target evidence collected; compile and console gates remain pending.",
  "artifactRefs": ["art_runtime_status_001"],
  "gates": [
    {
      "id": "runtime-reachable",
      "status": "passed"
    }
  ],
  "openRisks": []
}
```

## ArtifactRef

Run artifacts are normalized into manifest `artifactRefs` and individual `artifacts/<artifactId>/artifact.json` files.

Standard kinds:

- `command-result`
- `command-evidence`
- `console-log`
- `screenshot`
- `gif`
- `code-index-result`
- `runtime-status`
- `runtime-log`
- `runtime-screenshot`
- `runtime-perf`
- `runtime-handler-result`
- `patch-proposal`
- `verdict`
- `finding`
- `evidence`
- `skill-handoff`
- `validation-report`
- `workflow-report`

Artifacts may include `stepId` and `schema` for imported structured results. Screenshots, GIFs, and Runtime screenshots are referenced from their existing `.aibridge` cache paths by default. Readable non-image output files are copied into the run artifact directory when they are under the copy limit; large files may be referenced by `sourcePath`.

## Gates

Allowed `kind` values:

- `unityCompile`
- `dotnetBuild`
- `consoleErrors`
- `testRun`
- `screenshotExists`
- `runtimeReachable`
- `runtimeErrors`
- `artifactRequired`
- `externalVerdict`
- `patchProposalRequired`

Required gates failing make the run `failed` or `blocked`. Optional gate failures make evidence visible without forcing the run to fail.

`artifactRequired` may filter by `artifactKind`, `schema`, and `stepId`. `externalVerdict` uses `allow` values such as `confirmed`. `uncertain` is reported as an evidence gap and does not count as pass or fail.

## External Result Schemas

Use `EvidenceRef`, `CommandEvidence`, `Finding`, `Verdict`, `PatchProposal`, `ValidationResult`, and `SkillHandoff` when `agent` or `manual` steps are executed by the harness and imported back into a run. Field definitions live in `evidence-schema.md`.

Import examples:

```bash
$CLI workflow import --run <runId> --step verify-findings --schema Verdict --kind verdict --file verdicts.json
$CLI workflow import --run <runId> --step collect-evidence --schema EvidenceRef --kind evidence --file evidence-refs.json
$CLI workflow import --run <runId> --step collect-evidence --schema CommandEvidence --kind command-evidence --file command-evidence.json
$CLI workflow import --run <runId> --step phase-handoff --schema SkillHandoff --kind skill-handoff --file skill-handoff.json
```

## Boundaries

- Do not use workflow recipes as a generic LLM scheduler.
- Do not imply `agent` or `manual` steps are executed by AIBridge.
- Treat adapter exports as handoff artifacts only; execution and result return must be explicit.
- Keep parallel agents read-only unless isolated worktrees, ownership, merge, and validation gates are explicit.
- Never parallel-write Prefab, Scene, `.asset`, or `.meta` files.
