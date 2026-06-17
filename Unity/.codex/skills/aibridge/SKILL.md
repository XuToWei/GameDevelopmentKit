---
name: aibridge
description: Unity Editor and Player Runtime CLI integration for AIBridge. Use for harness snapshot reads, Unity compile/logs/assets/scenes/prefabs/Inspector, screenshots/GIFs, Play Mode input, Runtime Player targets, focus/menu/game view, AIBridgeCLI syntax, or shellless external-tool routing through exec. Route batch/multi, complex prefab patching, and direct UnityYAML edits to specialized Skills
---

# AI Bridge Unity Skill

## Invocation

Run commands from the Unity project root

**CLI Path:** `./.aibridge/cli/AIBridgeCLI.exe`

`$CLI` means the platform-appropriate AIBridge CLI invocation. On Windows PowerShell, prefer the call operator:

```powershell
& "./.aibridge/cli/AIBridgeCLI.exe" <command> [action] [options]
```

Common global options: `--timeout <ms>`, `--raw`, `--pretty`, `--json <json>`, `--stdin`, `--help`, `--on-dialog <mode>`

Generated command references are installed under `references/` in the target assistant Skill directory. In the package source tree they may be absent until Skill installation/refresh runs

## Host Tools

For external host tools such as `rg`, `git`, `dotnet`, `python`, `node`, `sg`, or `grep`, prefer `exec run --stdin` when arguments are non-trivial, include regex/globs/JSON/spaces, require stdin, need timeout/output limits, or run multiple `jobs`. Keep AIBridge/Unity commands direct: `compile unity`, `get_logs`, `asset`, `inspector`, `runtime`, `workflow`, `multi`, and `code execute`. Use `multi --stdin` for multiple AIBridge commands and `exec` `jobs[]` for multiple external host commands.

## Harness Snapshot

Unity Editor writes `.aibridge/harness/capabilities.json` during Skill installation/refresh

```bash
$CLI harness status
$CLI harness status --detail full
```

Default `harness status` output is compact. Use `--detail full` or `--include-snapshot true` only when the full snapshot JSON is needed

## Operating Rules

- Use `compile unity` for Unity validation. `compile dotnet` is an explicit extra solution-build check, not a Unity fallback.
- For Unity imported asset path discovery, including script, prefab, scene, ScriptableObject, texture, material, audio, animation, shader, font, and model assets by name/type/filter, prefer `asset search/find --format paths` before host `rg --files` when the Editor is available. Use `rg --files` for ordinary repository files, unimported files, `.meta`, ProjectSettings, and arbitrary path regexes; use host file reads for file contents, and `asset read_text` only when host reads are unavailable.
- Resource edit order: `inspector set_property/set_properties` -> `aibridge-prefab-patch` for supported complex Prefab edits -> Unity Editor scripts for high-level generated assets -> `unity-yaml-editing` only for unsupported serialized-file structure work.
- For scene objects, Prefabs, and serialized Unity assets, discover targets with `inspector get_components/get_properties/find_property`, then write with AIBridge/Unity APIs when possible.
- For prefab asset edits, use `assetPath + objectPath + componentName` or `componentIndex`; `componentInstanceId` is scene-only.
- In PowerShell, avoid inline complex `--json`; build JSON in a variable or file, and use `exec run --stdin` for complex external-tool argv instead of composing a shell string.
- `focus` is Windows CLI-only. `dialog` is CLI-only and omits dialog fields when no modal dialog is detected.
- `input` requires Play Mode and an active EventSystem; pair it with `gameview`, `screenshot`, and `get_logs` for UI interaction checks.
- `runtime` talks to `AIBridgeRuntime` in a Player or Play Mode target. Run quick `runtime list_targets` first, use `runtime list_targets --probe true` only when local port scanning is needed, then target `latest` or a specific target id. For UI work, start with `runtime.ui.snapshot`; button entries include screen coordinates and screen rects for direct clicks, and `runtime.input.key` covers semantic submit/cancel/tab/move input.
- Code Index is optional. For C# semantic lookup use `aibridge-code-index` only when installed and enabled; use `rg`, file reads, and regular AIBridge commands for literal, non-C#, asset, scene, prefab, or unavailable Code Index searches.

## Related Skills

- `aibridge-batch-script`: `batch` / `multi` automation and long stdin scripts
- `aibridge-prefab-patch`: supported complex Prefab asset edits with dry-run
- `aibridge-code-index`: optional read-only semantic C# lookup
- `unity-yaml-editing`: direct UnityYAML fallback when supported APIs cannot express the operation
