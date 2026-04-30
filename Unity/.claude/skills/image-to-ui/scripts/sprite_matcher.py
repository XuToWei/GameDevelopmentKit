"""
Sprite Matcher - 逐层自主匹配

逐层从顶到底搜索截图中的所有sprite：
  Layer 0: 所有切图直接与截图比较（普通→九宫→白图）
  Layer 1: 把Layer 0匹配结果合成为overlay，候选切图垫在overlay下面比较
  Layer N: 把前N-1层合成为overlay，继续向下匹配
  直到某层无任何新匹配为止

每层内按优先级依次尝试：普通sprite → 九宫格sprite → 白图sprite

参数：
  --borders: parse_sprite_borders.py 生成的九宫格JSON
  --whites:  parse_white_sprites.py 生成的白图JSON
"""

import argparse
import cv2
import json
import numpy as np
import os
import sys
import time

from matcher_core import (
    DEFAULT_MAX_DIFF, DEFAULT_FUZZY_SCALES, MAX_LAYERS,
    STAGE_NORMAL, STAGE_9SLICE, STAGE_WHITE,
    rect_contains, rect_area,
    _log_stage_start, _log_stage_end,
    _should_accept_fuzzy_majority, _is_central_exclusion_hole,
    build_fuzzy_exclusion_mask,
)
from matcher_core import *  # noqa: F401,F403 — re-export for tests
from matcher_loader import (
    load_sprites, normalize_match_scales, parse_match_scales_arg,
    compute_sprite_metadata, resize_sprite,
)
from matcher_loader import *  # noqa: F401,F403
from match_normal import _match_normal
from match_9slice import _match_9slice
from match_white import _match_white, _match_white_fuzzy, _build_uncovered_contours
from matcher_debug import (
    build_overlay, build_overlay_incremental, _get_sprite,
    generate_layer_debug, save_overlay_debug,
)
from matcher_verify import *  # noqa: F401,F403
from matcher_verify import (
    _analyze_region_match, composite_verify_solid_shape,
)


# ──────────────────── 去重 ────────────────────

def deduplicate_matches(matches):
    if not matches:
        return matches
    matches = sorted(matches, key=lambda m: (
        m.get('median_diff', 999.0),
        -m.get('match_ratio', 0.0),
        1 if m.get('rescue_candidate') else 0,
        -m.get('candidate_score', 0.0),
    ))
    keep = []
    for m in matches:
        r = m['rect']
        sprite_idx = m.get('sprite_idx', -1)
        sprite_key = (
            's',
            sprite_idx,
        ) if sprite_idx >= 0 else (
            'w',
            m.get('white_idx', -1),
        )
        dup = False
        for i, k in enumerate(keep):
            kr = k['rect']
            keep_sprite_idx = k.get('sprite_idx', -1)
            keep_sprite_key = (
                's',
                keep_sprite_idx,
            ) if keep_sprite_idx >= 0 else (
                'w',
                k.get('white_idx', -1),
            )
            if (abs(r['x'] - kr['x']) <= 2 and abs(r['y'] - kr['y']) <= 2 and
                    abs(r['width'] - kr['width']) <= 2 and abs(r['height'] - kr['height']) <= 2 and
                    sprite_key == keep_sprite_key):
                dup = True
                break
            ix1 = max(r['x'], kr['x'])
            iy1 = max(r['y'], kr['y'])
            ix2 = min(r['x'] + r['width'], kr['x'] + kr['width'])
            iy2 = min(r['y'] + r['height'], kr['y'] + kr['height'])
            if ix2 > ix1 and iy2 > iy1:
                inter = (ix2 - ix1) * (iy2 - iy1)
                area_m = r['width'] * r['height']
                area_k = kr['width'] * kr['height']
                smaller = min(area_m, area_k)
                if smaller > 0 and sprite_key == keep_sprite_key and inter / smaller > 0.85:
                    dup = True
                    break
        if not dup:
            keep.append(m)
    return keep


# ──────────────────── 覆盖关系树 ────────────────────

def build_coverage_tree(match_nodes):
    for node in match_nodes:
        node['parentId'] = None
        node['children'] = []
    for i, na in enumerate(match_nodes):
        ra = na['rect']
        layer_a = na.get('layer', 0)
        lower_layer = []
        same_layer = []
        for j, nb in enumerate(match_nodes):
            if j == i or not rect_contains(nb['rect'], ra):
                continue
            layer_b = nb.get('layer', 0)
            if layer_b > layer_a:
                lower_layer.append(nb)
            elif layer_b == layer_a:
                same_layer.append(nb)

        parent = None
        if lower_layer:
            lower_layer.sort(key=lambda n: (n.get('layer', 0) - layer_a, rect_area(n['rect']), n['node_id']))
            parent = lower_layer[0]
        elif same_layer:
            same_layer.sort(key=lambda n: (rect_area(n['rect']), n['node_id']))
            parent = same_layer[0]

        if parent is not None:
            na['parentId'] = parent['node_id']
    id_map = {n['node_id']: n for n in match_nodes}
    for node in match_nodes:
        pid = node.get('parentId')
        if pid is not None and pid in id_map:
            id_map[pid]['children'].append(node['node_id'])


# ──────────────────── 单层匹配 ────────────────────

def match_one_layer(layer_num, sprites, sliced_indices, white_sprites,
                   screenshot, overlay, all_existing,
                   fuzzy_scales=None,
                   max_diff=DEFAULT_MAX_DIFF):
    combined = list(all_existing)

    normal_count = len(sprites)
    stage_idx, stage_label = STAGE_NORMAL
    _log_stage_start(stage_idx, stage_label, normal_count)
    normal = _match_normal(sprites, sliced_indices, screenshot, overlay,
                           combined,
                           fuzzy_scales=fuzzy_scales)
    combined.extend(normal)
    _log_stage_end(stage_idx, stage_label, len(normal))

    slice_count = len(sliced_indices)
    stage_idx, stage_label = STAGE_9SLICE
    _log_stage_start(stage_idx, stage_label, slice_count)
    sliced = _match_9slice(sprites, sliced_indices, screenshot, overlay,
                           combined)
    combined.extend(sliced)
    _log_stage_end(stage_idx, stage_label, len(sliced))

    matches = deduplicate_matches(normal + sliced)

    uncovered_cache = _build_uncovered_contours(screenshot, combined)

    white_count = len(white_sprites)
    stage_idx, stage_label = STAGE_WHITE
    _log_stage_start(stage_idx, stage_label, white_count, unit='templates')
    white = _match_white(white_sprites, screenshot, overlay,
                         combined, max_diff=max_diff,
                         uncovered_cache=uncovered_cache)
    combined.extend(white)
    matches.extend(white)
    white_fuzzy = _match_white_fuzzy(
        white_sprites, screenshot, overlay,
        combined,
        uncovered_cache=uncovered_cache)
    matches.extend(white_fuzzy)
    _log_stage_end(stage_idx, stage_label, len(white) + len(white_fuzzy))

    matches = deduplicate_matches(matches)
    return matches


# ──────────────────── 主流程 ────────────────────

def match_sprites(screenshot_path, output_path, sprite_dirs, borders_path, whites_path,
                   debug_dir=None, max_diff=DEFAULT_MAX_DIFF, fuzzy_scales=None):
    t0 = time.time()

    screenshot = cv2.imread(screenshot_path)
    if screenshot is None:
        print(f"ERROR: Cannot read screenshot: {screenshot_path}")
        sys.exit(1)
    sh, sw = screenshot.shape[:2]
    print(f"Screenshot: {sw}x{sh}, max_diff: {max_diff}")
    fuzzy_scales = normalize_match_scales(fuzzy_scales)
    print(f"Fuzzy scales: {fuzzy_scales}")

    borders_map = {}
    if borders_path:
        with open(borders_path, encoding='utf-8') as f:
            borders_map = json.load(f)
        print(f"Loaded {len(borders_map)} 9-slice borders")

    whites_map = {}
    if whites_path:
        with open(whites_path, encoding='utf-8') as f:
            whites_map = json.load(f)
        print(f"Loaded {len(whites_map)} white sprites")

    sprites, sliced_indices, white_sprites = load_sprites(sprite_dirs, borders_map, whites_map)
    if not sprites and not white_sprites:
        print("ERROR: No sprites found")
        sys.exit(1)

    if debug_dir:
        os.makedirs(debug_dir, exist_ok=True)

    all_matches = []
    layer = 0
    overlay = np.zeros((sh, sw, 4), dtype=np.uint8)

    while layer < MAX_LAYERS:
        print(f"\n=== Layer {layer} ===")

        layer_matches = match_one_layer(
            layer, sprites, sliced_indices, white_sprites,
            screenshot, overlay, all_matches,
            fuzzy_scales=fuzzy_scales, max_diff=max_diff)

        if not layer_matches:
            print(f"  Layer {layer}: no matches, done")
            break

        for m in layer_matches:
            m['layer'] = layer
        count_before = len(all_matches)
        all_matches.extend(layer_matches)
        all_matches = deduplicate_matches(all_matches)
        new_count = len(all_matches) - count_before
        print(f"  Layer {layer}: {len(layer_matches)} raw, {new_count} new after dedup")

        if new_count <= 0:
            print(f"  No new unique matches, done")
            break

        overlay = build_overlay_incremental(
            overlay, layer_matches, sprites, white_sprites)

        if debug_dir:
            generate_layer_debug(screenshot, layer, layer_matches,
                                 [m for m in all_matches if m.get('layer', 0) < layer],
                                 sprites, white_sprites, debug_dir)
            save_overlay_debug(overlay, screenshot, debug_dir, f"layer_{layer}",
                               matches=all_matches)
        layer += 1

    final_overlay = overlay
    if debug_dir:
        save_overlay_debug(final_overlay, screenshot, debug_dir, "final",
                           matches=all_matches)

    match_nodes = []
    for i, m in enumerate(all_matches):
        nid = i + 1
        sp = _get_sprite(m, sprites, white_sprites)
        mn = {
            'node_id': nid,
            'rect': m['rect'],
            'layer': m.get('layer', 0),
            'match': {
                'sprite_path': sp['rel_path'] if sp else '',
                'sprite_size': [sp['w'], sp['h']] if sp else [0, 0],
                'match_ratio': m['match_ratio'],
                'median_diff': m.get('median_diff', 0.0),
                'perfect': m['perfect'],
                'valid_pixels': m['valid_pixels'],
                'scale': m['scale'],
                'is_9slice': m.get('is_9slice', False),
                'is_white': m.get('is_white', False),
                'candidate_score': m.get('candidate_score'),
                'rescue_candidate': m.get('rescue_candidate', False),
            }
        }
        if m.get('tint_color'):
            mn['match']['tint_color'] = m['tint_color']
        if m.get('coverage'):
            mn['match']['coverage'] = m['coverage']
        match_nodes.append(mn)

    build_coverage_tree(match_nodes)

    elapsed = time.time() - t0
    perfect = sum(1 for n in match_nodes if n['match']['perfect'])
    whites = sum(1 for n in match_nodes if n['match'].get('is_white'))
    slices = sum(1 for n in match_nodes if n['match'].get('is_9slice'))

    output = {
        'screenshot': screenshot_path,
        'sprite_dirs': sprite_dirs,
        'elapsed_seconds': round(elapsed, 2),
        'summary': {
            'total_matches': len(match_nodes),
            'perfect_matches': perfect,
            'slice_matches': slices,
            'white_matches': whites,
            'layers': layer,
        },
        'nodes': match_nodes,
    }

    os.makedirs(os.path.dirname(output_path) or '.', exist_ok=True)

    class NumpyEncoder(json.JSONEncoder):
        def default(self, obj):
            if isinstance(obj, (np.integer,)):
                return int(obj)
            if isinstance(obj, (np.floating,)):
                return float(obj)
            if isinstance(obj, (np.bool_,)):
                return bool(obj)
            return super().default(obj)

    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(output, f, ensure_ascii=False, indent=2, cls=NumpyEncoder)

    print(f"\nDone in {elapsed:.1f}s")
    print(
        f"Total: {len(match_nodes)} (perfect:{perfect} 9slice:{slices} "
        f"white:{whites} layers:{layer})")
    print(f"Saved: {output_path}")


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Sprite Matcher')
    parser.add_argument('screenshot', help='Screenshot image path')
    parser.add_argument('output', help='Output sprite_match.json path')
    parser.add_argument('sprite_dirs', nargs='+', help='Sprite directories')
    parser.add_argument('--borders', help='9-slice borders JSON')
    parser.add_argument('--whites', help='White sprites JSON')
    parser.add_argument('--debug-dir', help='Directory to save per-layer debug images')
    parser.add_argument('--max-diff', type=int, default=DEFAULT_MAX_DIFF,
                        help=f'Max per-channel pixel diff for verification (default {DEFAULT_MAX_DIFF})')
    parser.add_argument('--fuzzy-scales',
                        help='Comma-separated fuzzy original scales '
                             f'(default {",".join(str(v) for v in DEFAULT_FUZZY_SCALES)})')
    args = parser.parse_args()
    fuzzy_scales = parse_match_scales_arg(args.fuzzy_scales)
    match_sprites(args.screenshot, args.output, args.sprite_dirs,
                  args.borders, args.whites, args.debug_dir, args.max_diff,
                  fuzzy_scales=fuzzy_scales)
