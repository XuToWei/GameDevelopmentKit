from __future__ import annotations

import argparse
import importlib.util
import sys
from datetime import datetime
from pathlib import Path


SCREENSHOT_NAME = "4_Play_Stage_Start_1_normal 1.png"
DEFAULT_FUZZY_SCALES = None


def resolve_optional_file(primary_path: Path, fallback_path: Path) -> Path | None:
    if primary_path.is_file():
        return primary_path
    if fallback_path.is_file():
        return fallback_path
    return None


def load_match_sprites(sprite_matcher_path: Path):
    spec = importlib.util.spec_from_file_location(
        "image_to_ui_sprite_matcher",
        sprite_matcher_path,
    )
    if spec is None or spec.loader is None:
        raise ImportError(f"Cannot load module spec from {sprite_matcher_path}")

    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)

    match_sprites = getattr(module, "match_sprites", None)
    if match_sprites is None:
        raise AttributeError(f"match_sprites() not found in {sprite_matcher_path}")
    return match_sprites


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Run the direct play-stage-start sprite matcher test")
    parser.add_argument(
        "--fuzzy-scales",
        help="Comma-separated layer0 fuzzy scales to pass into sprite_matcher.py",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    script_path = Path(__file__).resolve()
    skill_root = script_path.parents[1]
    unity_root = script_path.parents[4]
    sprite_matcher_path = skill_root / "scripts" / "sprite_matcher.py"
    screenshot_path = unity_root / "Assets" / "Res" / "UI" / "Test" / "Design" / SCREENSHOT_NAME
    sprite_dir_path = unity_root / "Assets" / "Res" / "UI" / "Test" / "Sprite"
    output_root = unity_root / "AIBridge" / "image-to-ui"
    shared_metadata_root = output_root / "_shared"
    cache_metadata_root = unity_root / "AIBridgeCache" / "image-to-ui" / "Test"
    borders_path = resolve_optional_file(
        shared_metadata_root / "borders.json",
        cache_metadata_root / "borders.json",
    )
    whites_path = resolve_optional_file(
        shared_metadata_root / "whites.json",
        cache_metadata_root / "whites.json",
    )
    if not sprite_matcher_path.is_file():
        raise FileNotFoundError(f"Missing sprite_matcher.py: {sprite_matcher_path}")
    if not screenshot_path.is_file():
        raise FileNotFoundError(f"Missing screenshot: {screenshot_path}")
    if not sprite_dir_path.is_dir():
        raise FileNotFoundError(f"Missing sprite directory: {sprite_dir_path}")

    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S_%f")
    run_dir = output_root / f"{screenshot_path.stem}_{timestamp}"
    output_json_path = run_dir / "sprite_match.json"
    debug_dir_path = run_dir / "debug"

    run_dir.mkdir(parents=True, exist_ok=False)

    if borders_path is None:
        print(
            "Skip --borders, file not found in "
            f"{shared_metadata_root} or {cache_metadata_root}"
        )
    if whites_path is None:
        print(
            "Skip --whites, file not found in "
            f"{shared_metadata_root} or {cache_metadata_root}"
        )

    print(f"Run directory: {run_dir}")
    print(f"Python: {sys.executable}")
    print(f"Matcher: {sprite_matcher_path}")
    print(f"Screenshot: {screenshot_path}")
    print(f"Sprite dir: {sprite_dir_path}")

    match_sprites = load_match_sprites(sprite_matcher_path)
    fuzzy_scales = args.fuzzy_scales if args.fuzzy_scales is not None else DEFAULT_FUZZY_SCALES
    if fuzzy_scales is not None:
        print(f"Layer0 fuzzy scales: {fuzzy_scales}")
    match_sprites(
        str(screenshot_path),
        str(output_json_path),
        [str(sprite_dir_path)],
        str(borders_path) if borders_path is not None else None,
        str(whites_path) if whites_path is not None else None,
        str(debug_dir_path),
        fuzzy_scales=fuzzy_scales,
    )

    print(f"Saved output to: {output_json_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
