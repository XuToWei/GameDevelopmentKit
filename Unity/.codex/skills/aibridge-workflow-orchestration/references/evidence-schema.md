# Evidence Schema

Use these compact schemas for workflow `agent` / `manual` results and imported artifacts. Reference evidence ids or artifact ids instead of pasting large logs.

- `EvidenceRef`: `id`, `kind`, `summary`, `artifactPath`, `sourceCommand`, `targetId`, `createdAtUtc`.
- `CommandEvidence`: `id`, `command`, `status`, `exitCode`, `summary`, `artifactIds`, `startedAtUtc`, `endedAtUtc`.
- `Finding`: `id`, `severity`, `file`, `line`, `claim`, `evidence`, `repro`, `confidence`.
- `Verdict`: `claimId`, `status`, `evidenceRefs`, `reason`, `remainingRisk`; status must be `confirmed`, `refuted`, or `uncertain`.
- `PatchProposal`: `id`, `files`, `summary`, `risk`, `validation`.
- `ValidationResult`: `gate`, `status`, `command`, `evidence`, `artifacts`.
- `SkillHandoff`: `completedMode`, `releasedSkills`, `nextRecommendedSkills`, `summary`, `artifactRefs`, `gates`, `openRisks`.
- `ArtifactRef`: `kind`, `path`, `summary`, `sourceCommand`.
- `RuntimeTargetRef`: `targetId`, `url`, `platform`, `status`, `evidence`.

Import examples:

```bash
$CLI workflow import --run <runId> --step verify-findings --schema Verdict --kind verdict --file verdicts.json
$CLI workflow import --run <runId> --step collect-evidence --schema EvidenceRef --kind evidence --file evidence-refs.json
$CLI workflow import --run <runId> --step collect-evidence --schema CommandEvidence --kind command-evidence --file command-evidence.json
$CLI workflow import --run <runId> --step phase-handoff --schema SkillHandoff --kind skill-handoff --file skill-handoff.json
```
