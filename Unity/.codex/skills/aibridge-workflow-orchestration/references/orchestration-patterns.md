# Orchestration Patterns

Purpose: choose safe workflow structure for AIBridge tasks that need more than a single linear agent pass.

## Boundaries

| Surface | Responsibility | Do not use for |
|---|---|---|
| Workflow orchestration | Control flow, agent roles, dependencies, artifacts, gates, and final report shape | Direct Unity object mutation or command syntax lookup |
| Skill | Domain rules loaded by an AI agent | Persisted run state or execution scheduling |
| `batch` / `multi` | Linear AIBridge CLI automation inside one command stream | Multi-agent reasoning, voting, merge ownership, or adversarial review |
| Runtime Bridge | Player or Play Mode data plane for status, logs, screenshots, perf, handlers, and calls | Agent scheduling or workflow recipe execution |
| Code Index | Read-only semantic code evidence | Runtime state, asset mutation, or project build validation |

Current workflow CLI support covers recipe list/validate/plan/init, active run begin/attach/finish, deterministic `run-cli` steps, ordinary-command artifact attach, external result import, adapter export, gates, and reports. It is not a cross-tool agent runtime; `agent` and `manual` steps still require an external executor.

## Harness Responsibility

Use this split when a recipe leaves AIBridge CLI:

| Step kind | Executed by `workflow run-cli` | Responsible executor | Required handoff |
|---|---:|---|---|
| `cli` | Yes | AIBridge CLI | Command result and artifacts are attached automatically |
| `barrier` | Yes, as lightweight pass | AIBridge CLI or main agent | Merge result when a real merge decision is needed |
| `report` | Yes, as lightweight pass | AIBridge CLI or main agent | Final report should reference artifacts and gates |
| `agent` | No | Current AI harness or exported task pack executor | Import `Finding`, `Verdict`, `PatchProposal`, `ValidationResult`, `EvidenceRef`, or `CommandEvidence` |
| `manual` | No | Main agent or human operator | Import the structured result or mark the gap explicitly |

If the harness cannot create sub-agents or run external steps, keep execution single-agent and treat those steps as manual work owned by the main agent. Do not imply AIBridge has completed skipped `agent` or `manual` steps.

## Pattern Selection

Use parallel when:

- Work items are independent.
- Agents are read-only or produce proposals only.
- Results can be merged by schema, path, target id, or artifact id.
- Examples: directory-sharded review, multiple Runtime target status checks, independent log/screenshot collection.

Use pipeline when:

- Each item must pass through ordered stages.
- A later stage depends on the prior stage's evidence or verdict.
- Examples: discover target -> collect evidence -> validate claim -> report; inspect Prefab -> propose patch -> apply serially -> validate.

Use a barrier when:

- A downstream step needs every upstream result.
- Findings must be deduplicated, ranked, voted, or compared globally.
- A final implementer needs all patch proposals before editing.

Use a single linear `batch` / `multi` script when:

- The work is deterministic command automation.
- There is no need for independent agent reasoning.
- Commands share one Unity Editor state and must run in order.

## Parallel Read, Serial Write

- Parallel agents default to read-only behavior.
- Parallel agents return `Finding`, `Verdict`, `PatchProposal`, `ValidationResult`, `ArtifactRef`, or `RuntimeTargetRef` objects.
- The main agent or one implementer applies edits serially after reviewing proposals.
- Parallel writes require explicit file ownership, separate worktrees or isolated generated outputs, a merge plan, and post-merge validation.
- Never let two write agents modify the same Unity serialized asset, `.meta` file, scene, Prefab, package manifest, or generated command reference.

## Structured Outputs

Use structured outputs for intermediate data and prose for the final human report. Read `evidence-schema.md` for `Finding`, `Verdict`, `PatchProposal`, `ValidationResult`, `EvidenceRef`, `CommandEvidence`, `ArtifactRef`, and `RuntimeTargetRef` fields.

Large outputs should be saved as artifacts and referenced by path or id instead of pasted into the main context.

## Skill Routing And Scope

- Treat Skill routing as a preflight step, not as a business mode or phase.
- During execution, emit a short visible status block when entering a phase or step: use `【模式：...】` for the active business mode and `-> ...` for phase/step progress, then put active Skills, expected output, and handoff or gate status on separate lines.
- Each phase or external task pack should declare only the Skills required for that phase; candidate Skills that are not read yet should stay deferred.
- Release mode-specific Skill dependencies only at Mode Exit, phase boundary, or step handoff, then pass a compact handoff instead of the previous phase's full Skill details.
- A handoff should use the `SkillHandoff` schema and include `completedMode`, `releasedSkills`, `nextRecommendedSkills`, `summary`, `artifactRefs`, `gates`, and `openRisks`.
- If the next phase needs a released Skill, match and load it again from the current phase's requirements.
- This is a workflow-level context slimming rule. It does not guarantee that already-read Skill text is physically removed from the model context; actual removal depends on the current AI harness, sub-agent, or compaction support.

For `agent` and `manual` steps, provide the executor with only its required Skills, input summary, artifact refs, gates, and expected output schema. Do not pass role-specific Skill bodies from earlier phases unless the new phase explicitly requires them.

## Resume And Active Runs

- For a resumed workflow task, identify the run id from the user, prior output, or `.aibridge/workflows/active-run.json`, then check `workflow status --run <runId>` before continuing.
- Use existing artifact ids and gate status to decide the next missing step; do not repeat expensive collection unless evidence is stale or incomplete.
- Re-collect logs, screenshots, Runtime status, or perf samples when the target state may have changed.
- Before `workflow finish --status passed`, refresh gates/report and ensure required gates are not missing, failed, or blocked.

## Adversarial Verification

- Split generator and verifier roles when correctness is high risk.
- Give the verifier claims and evidence, not the generator's full reasoning.
- Require one of three verdicts: `confirmed`, `refuted`, or `uncertain`.
- Treat `uncertain` as actionable: request more evidence, narrow scope, or downgrade the claim.
- Prefer verification gates that can be repeated: compile, tests, logs, screenshots, Runtime calls, or semantic lookup.

## AIBridge Evidence Gates

Choose gates that match the change:

- `compile unity`: Unity compile validation.
- `get_logs --logType Error`: Editor Console error check.
- `test run`: targeted Unity tests when available.
- `screenshot game`, `screenshot scene_view`, `screenshot gif`: visual evidence.
- `runtime list_targets`, `runtime status`, `runtime logs`, `runtime screenshot`, `runtime perf`, `runtime handlers`, `runtime call`: Player or Play Mode validation.
- `code_index`: optional read-only semantic evidence only when the Skill and project settings enable it.

Evidence should include the command, relevant output summary, artifact path, target id or URL when applicable, and the final verdict.
