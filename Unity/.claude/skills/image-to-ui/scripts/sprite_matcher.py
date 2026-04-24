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

MIN_MATCH_PIXELS = 16
WHITE_COLOR_STD_MAX = 3.0
CORR_THRESHOLD = 0.85
VERIFY_MEDIAN_MAX = 20
DEFAULT_MAX_DIFF = 40
ORIGINAL_MATCH_MAX_DIFF = 0
ORIGINAL_MATCH_RATIO_MIN = 1.0
ERODE_PX = 3
MIN_TEMPLATE_DIM = 8
MAX_CANDIDATES = 50
MAX_LAYERS = 10
WHITE_SOURCE_UNIFORM_MAX_DIFF = 3
WHITE_SOURCE_RING_DILATE_PX = 2
WHITE_SOURCE_RING_MIN_CONTRAST = 20
WHITE_SOURCE_RING_MIN_RATIO = 0.6
WHITE_SOURCE_RING_MIN_PIXELS = 64
WHITE_SOURCE_COMPOSITE_MAX_DIFF = 12
WHITE_SOURCE_COMPOSITE_MEDIAN_MAX = 8
WHITE_SOURCE_COMPOSITE_RATIO_MIN = 0.9
WHITE_SOURCE_COMPOSITE_RING_MIN_RATIO = 0.35
WHITE_SOURCE_OVERLAY_MONO_MAX_DIFF = 8
WHITE_SOURCE_OVERLAY_MONO_MIN_RATIO = 0.95
UNDERLAY_BORDER_COVERAGE_MIN = 0.9
UNDERLAY_MATCH_RATIO_MIN = 0.93
UNDERLAY_SMALL_MATCH_RATIO_MIN = 0.95
WHITE_DIRECT_COMPONENT_THRESHOLD = 0.9
BORDER_DIRECT_COMPONENT_THRESHOLD = 0.88
SPECIAL_DIRECT_COMPONENT_MAX = 32
WHITE_DIRECT_MIN_OPAQUE_PIXELS = 350
RESCUE_CORR_THRESHOLD = 0.55
RESCUE_MAX_CANDIDATES = 8
SOFT_MATCH_RATIO_MIN = 0.72
SOFT_SMALL_MATCH_RATIO_MIN = 0.9
SOFT_SMALL_VALID_PIXELS_MAX = 384
ICON_LIKE_MATCH_RATIO_MIN = 0.92
ICON_LIKE_SMALL_MATCH_RATIO_MIN = 0.97
TEXT_MIN_PIXELS = 9
TEXT_MASK_CLOSE_PX = 1
TEXT_MASK_EXPAND_PX = 1
TEXT_MAX_AREA_RATIO = 0.35
TEXT_MIN_BASE_RATIO = 0.55
TEXT_MIN_CLEAN_RATIO = 1.0
TEXT_MAX_GROUPS = 10
TEXT_MAX_GROUP_AREA_RATIO = 0.18
TEXT_MAX_BBOX_AREA_RATIO = 0.45
TEXT_MAX_FILL_RATIO = 0.92
TEXT_BORDER_BAND_PX = 3
TEXT_BORDER_TOTAL_MIN = 0.55
TEXT_BORDER_SIDE_MIN = 0.2
TEXT_CORNER_RATIO_MIN = 0.2
TEXT_MIN_CORNER_HITS = 2
TEXT_EXCLUSION_BLOCK_SIZE = 31
TEXT_EXCLUSION_C = 15
TEXT_EXCLUSION_MIN_AREA = 6
TEXT_EXCLUSION_MAX_AREA = 2500
TEXT_EXCLUSION_MAX_BBOX_W = 160
TEXT_EXCLUSION_MAX_BBOX_H = 120
TEXT_EXCLUSION_MAX_FILL = 0.75
TEXT_EXCLUSION_GROUP_DILATE_X = 13
TEXT_EXCLUSION_GROUP_DILATE_Y = 5
TEXT_EXCLUSION_GROUP_MIN_COMPONENTS = 3
TEXT_EXCLUSION_GROUP_MIN_W = 18
TEXT_EXCLUSION_GROUP_MIN_H = 10
TEXT_EXCLUSION_GROUP_MAX_H = 140
TEXT_EXCLUSION_GROUP_MAX_AREA_RATIO = 0.06
TEXT_EXCLUSION_GROUP_MAX_FILL = 0.55
TEXT_EXCLUSION_GROUP_MIN_ASPECT = 1.8
TEXT_CANDIDATE_OVERLAP_MIN = 0.55
TEXT_CANDIDATE_SOURCE_MAX_DIM = 96
TEXT_CANDIDATE_COVERAGE_MAX = 0.72
TEXT_CANDIDATE_COMPONENTS_MIN = 2
NINESLICE_EDGE_SAMPLE_SPAN = 3
NINESLICE_MAX_SIZE_CANDIDATES = 32

LAYER_COLORS_BGR = [
    (34, 87, 255),    # layer 0: red
    (80, 175, 76),    # layer 1: green
    (243, 150, 33),   # layer 2: blue
    (0, 152, 255),    # layer 3: orange
    (180, 100, 200),  # layer 4: purple
    (0, 255, 255),    # layer 5: yellow
    (255, 100, 100),  # layer 6: light blue
    (100, 200, 255),  # layer 7: light orange
    (200, 200, 0),    # layer 8: cyan
    (128, 128, 255),  # layer 9: pink
]


# ──────────────────── 九宫格渲染 ────────────────────

def render_9slice(img, border, target_w, target_h):
    h, w = img.shape[:2]
    L = min(border['left'], w - 1)
    R = min(border['right'], w - L - 1)
    T = min(border['top'], h - 1)
    B = min(border['bottom'], h - T - 1)

    center_w = max(0, target_w - L - R)
    center_h = max(0, target_h - T - B)
    if center_w <= 0 or center_h <= 0:
        return cv2.resize(img, (target_w, target_h), interpolation=cv2.INTER_AREA)

    ch_count = img.shape[2] if len(img.shape) == 3 else 1
    result = np.zeros((target_h, target_w, ch_count) if ch_count > 1 else (target_h, target_w),
                       dtype=img.dtype)

    src_rows = [(0, T), (T, h - B), (h - B, h)]
    src_cols = [(0, L), (L, w - R), (w - R, w)]
    dst_rows = [(0, T), (T, T + center_h), (T + center_h, target_h)]
    dst_cols = [(0, L), (L, L + center_w), (L + center_w, target_w)]

    for ri in range(3):
        sy1, sy2 = src_rows[ri]
        dy1, dy2 = dst_rows[ri]
        if dy2 - dy1 <= 0 or sy2 <= sy1:
            continue
        for ci in range(3):
            sx1, sx2 = src_cols[ci]
            dx1, dx2 = dst_cols[ci]
            dw, dh = dx2 - dx1, dy2 - dy1
            if dw <= 0 or sx2 <= sx1:
                continue
            patch = img[sy1:sy2, sx1:sx2]
            if (ri == 0 or ri == 2) and (ci == 0 or ci == 2):
                result[dy1:dy1 + patch.shape[0], dx1:dx1 + patch.shape[1]] = patch
            else:
                resized = cv2.resize(patch, (dw, dh), interpolation=cv2.INTER_AREA)
                result[dy1:dy2, dx1:dx2] = resized
    return result


# ──────────────────── 加载 ────────────────────

def load_sprites(sprite_dirs, borders_map, whites_map):
    sprites = []
    sliced_indices = []
    white_sprites = []

    for sprite_dir in sprite_dirs:
        for root, _, files in os.walk(sprite_dir):
            for fname in files:
                if not fname.lower().endswith('.png'):
                    continue
                fpath = os.path.join(root, fname)
                img = cv2.imread(fpath, cv2.IMREAD_UNCHANGED)
                if img is None or img.size == 0:
                    continue
                h, w = img.shape[:2]
                if h == 0 or w == 0:
                    continue
                if img.dtype == np.uint16:
                    img = (img / 257).astype(np.uint8)

                rel_path = os.path.relpath(fpath, sprite_dir).replace('\\', '/')
                border = borders_map.get(rel_path)
                idx = len(sprites)
                entry = {
                    'path': fpath, 'rel_path': rel_path, 'img': img,
                    'w': w, 'h': h, 'ar': w / h, 'border': border,
                    'base_sprite_idx': idx,
                    'is_white_source': rel_path in whites_map,
                }
                entry.update(compute_sprite_metadata(img))
                sprites.append(entry)
                if border:
                    sliced_indices.append(idx)
                if entry['is_white_source']:
                    entry['white_idx'] = len(white_sprites)
                    white_sprites.append(entry)
                else:
                    entry['white_idx'] = -1

    print(f"Loaded {len(sprites)} sprites ({len(sliced_indices)} 9-slice, "
          f"{len(white_sprites)} white)")
    return sprites, sliced_indices, white_sprites


def resize_sprite(sprite, tw, th):
    if sprite['border']:
        return render_9slice(sprite['img'], sprite['border'], tw, th)
    return cv2.resize(sprite['img'], (tw, th), interpolation=cv2.INTER_AREA)


def compute_sprite_metadata(img):
    bgra = ensure_bgra(img)
    alpha = bgra[:, :, 3] > 250
    area = max(1, alpha.shape[0] * alpha.shape[1])
    opaque_pixels = int(np.count_nonzero(alpha))
    coverage = float(opaque_pixels / area)
    labels = cv2.connectedComponents(alpha.astype(np.uint8), connectivity=8)[0] - 1
    return {
        'opaque_coverage': coverage,
        'opaque_pixels': opaque_pixels,
        'component_count': max(0, int(labels)),
    }


def get_9slice_target_min_size(img, border):
    h, w = img.shape[:2]
    L = min(border['left'], w - 1)
    R = min(border['right'], w - L - 1)
    T = min(border['top'], h - 1)
    B = min(border['bottom'], h - T - 1)
    return max(1, L + R + 1), max(1, T + B + 1)


# ──────────────────── alpha 合成 ────────────────────

def alpha_composite_onto(base_bgr, overlay_bgra, ox, oy):
    """BGRA overlay onto BGR base (in-place)."""
    bh, bw = base_bgr.shape[:2]
    oh, ow = overlay_bgra.shape[:2]
    sx, sy = max(0, -ox), max(0, -oy)
    ex, ey = min(ow, bw - ox), min(oh, bh - oy)
    if ex <= sx or ey <= sy:
        return
    roi = base_bgr[oy + sy:oy + ey, ox + sx:ox + ex]
    patch = overlay_bgra[sy:ey, sx:ex]
    a = patch[:, :, 3:4].astype(np.float32) / 255.0
    blended = (a * patch[:, :, :3].astype(np.float32) +
               (1.0 - a) * roi.astype(np.float32) + 0.5).astype(np.uint8)
    base_bgr[oy + sy:oy + ey, ox + sx:ox + ex] = blended


def alpha_composite_bgra(base_bgra, overlay_bgra, ox, oy):
    """BGRA overlay onto BGRA base (Porter-Duff over, in-place)."""
    bh, bw = base_bgra.shape[:2]
    oh, ow = overlay_bgra.shape[:2]
    sx, sy = max(0, -ox), max(0, -oy)
    ex, ey = min(ow, bw - ox), min(oh, bh - oy)
    if ex <= sx or ey <= sy:
        return
    roi = base_bgra[oy + sy:oy + ey, ox + sx:ox + ex].astype(np.float32)
    patch = overlay_bgra[sy:ey, sx:ex].astype(np.float32)
    fg_a = patch[:, :, 3:4] / 255.0
    bg_a = roi[:, :, 3:4] / 255.0
    out_a = fg_a + bg_a * (1.0 - fg_a)
    safe_a = np.where(out_a > 0, out_a, 1.0)
    out_rgb = (patch[:, :, :3] * fg_a + roi[:, :, :3] * bg_a * (1.0 - fg_a)) / safe_a
    result = np.empty_like(roi)
    result[:, :, :3] = np.clip(out_rgb + 0.5, 0, 255)
    result[:, :, 3:4] = np.clip(out_a * 255.0 + 0.5, 0, 255)
    base_bgra[oy + sy:oy + ey, ox + sx:ox + ex] = result.astype(np.uint8)


def ensure_bgra(img):
    if len(img.shape) == 2:
        bgr = cv2.cvtColor(img, cv2.COLOR_GRAY2BGR)
        return np.concatenate([bgr, np.full((*bgr.shape[:2], 1), 255, np.uint8)], axis=2)
    if img.shape[2] == 3:
        return np.concatenate([img, np.full((*img.shape[:2], 1), 255, np.uint8)], axis=2)
    return img


# ──────────────────── 模板工具 ────────────────────

def prepare_template(sprite_img, tw, th, border=None):
    if border:
        resized = render_9slice(sprite_img, border, tw, th)
    else:
        resized = cv2.resize(sprite_img, (tw, th), interpolation=cv2.INTER_AREA)
    has_alpha = len(resized.shape) == 3 and resized.shape[2] == 4
    if has_alpha:
        alpha_mask = resized[:, :, 3] > 250
        bgr = resized[:, :, :3].copy()
        bgr[~alpha_mask] = 0
        return bgr, alpha_mask
    if len(resized.shape) == 2:
        return cv2.cvtColor(resized, cv2.COLOR_GRAY2BGR), None
    return resized[:, :, :3].copy() if resized.shape[2] >= 3 else resized, None


def make_mask_3ch(alpha_mask):
    if alpha_mask is None:
        return None
    m = alpha_mask.astype(np.uint8) * 255
    return cv2.merge([m, m, m])


def erode_mask(alpha_mask, px=ERODE_PX):
    if alpha_mask is None or px <= 0:
        return alpha_mask
    kernel = np.ones((px * 2 + 1, px * 2 + 1), np.uint8)
    return cv2.erode(alpha_mask.astype(np.uint8), kernel).astype(bool)


def detect_text_regions(screenshot_bgr):
    gray = cv2.cvtColor(screenshot_bgr, cv2.COLOR_BGR2GRAY)
    dark = cv2.adaptiveThreshold(
        gray, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV,
        TEXT_EXCLUSION_BLOCK_SIZE, TEXT_EXCLUSION_C)
    light = cv2.adaptiveThreshold(
        255 - gray, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV,
        TEXT_EXCLUSION_BLOCK_SIZE, TEXT_EXCLUSION_C)
    raw = (dark > 0) | (light > 0)

    filtered = np.zeros_like(raw, dtype=np.uint8)
    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(raw.astype(np.uint8), connectivity=8)
    for label in range(1, num_labels):
        x, y, w, h, area = stats[label]
        if area < TEXT_EXCLUSION_MIN_AREA or area > TEXT_EXCLUSION_MAX_AREA:
            continue
        if w > TEXT_EXCLUSION_MAX_BBOX_W or h > TEXT_EXCLUSION_MAX_BBOX_H:
            continue
        fill_ratio = area / max(1, w * h)
        if fill_ratio > TEXT_EXCLUSION_MAX_FILL:
            continue
        filtered[labels == label] = 255

    if not np.any(filtered):
        return []

    kernel = np.ones((TEXT_EXCLUSION_GROUP_DILATE_Y, TEXT_EXCLUSION_GROUP_DILATE_X), np.uint8)
    grouped = cv2.dilate(filtered, kernel)
    num_groups, group_labels, group_stats, _ = cv2.connectedComponentsWithStats(grouped, connectivity=8)
    sh, sw = screenshot_bgr.shape[:2]
    max_group_area = int(sh * sw * TEXT_EXCLUSION_GROUP_MAX_AREA_RATIO)
    regions = []

    for label in range(1, num_groups):
        x, y, w, h, _ = group_stats[label]
        if w < TEXT_EXCLUSION_GROUP_MIN_W or h < TEXT_EXCLUSION_GROUP_MIN_H or h > TEXT_EXCLUSION_GROUP_MAX_H:
            continue
        if w * h > max_group_area:
            continue

        core_mask = (group_labels == label) & (filtered > 0)
        core_area = int(np.count_nonzero(core_mask))
        if core_area <= 0:
            continue

        fill_ratio = core_area / max(1, w * h)
        component_ids = np.unique(labels[core_mask])
        component_ids = component_ids[component_ids != 0]
        component_count = int(component_ids.size)
        aspect = w / max(1, h)

        if fill_ratio > TEXT_EXCLUSION_GROUP_MAX_FILL:
            continue
        if component_count < TEXT_EXCLUSION_GROUP_MIN_COMPONENTS and aspect < TEXT_EXCLUSION_GROUP_MIN_ASPECT:
            continue

        pad = 4
        regions.append({
            'x': max(0, int(x - pad)),
            'y': max(0, int(y - pad)),
            'width': min(sw - max(0, int(x - pad)), int(w + pad * 2)),
            'height': min(sh - max(0, int(y - pad)), int(h + pad * 2)),
        })
    return regions


def _rect_intersection_area(a, b):
    ix1 = max(a['x'], b['x'])
    iy1 = max(a['y'], b['y'])
    ix2 = min(a['x'] + a['width'], b['x'] + b['width'])
    iy2 = min(a['y'] + a['height'], b['y'] + b['height'])
    if ix2 <= ix1 or iy2 <= iy1:
        return 0
    return (ix2 - ix1) * (iy2 - iy1)


def _text_overlap_ratio(rect, text_regions):
    area = max(1, rect['width'] * rect['height'])
    overlap = 0
    for region in text_regions:
        overlap += _rect_intersection_area(rect, region)
        if overlap >= area:
            break
    return min(1.0, overlap / area)


def _is_text_like_false_positive(sprite, rect, text_regions):
    if not text_regions:
        return False
    overlap_ratio = _text_overlap_ratio(rect, text_regions)
    if overlap_ratio < TEXT_CANDIDATE_OVERLAP_MIN:
        return False

    source_max_dim = max(sprite['w'], sprite['h'])
    coverage = float(sprite.get('opaque_coverage', 1.0))
    component_count = int(sprite.get('component_count', 1))
    icon_like = (
        source_max_dim <= TEXT_CANDIDATE_SOURCE_MAX_DIM
        or coverage <= TEXT_CANDIDATE_COVERAGE_MAX
        or component_count >= TEXT_CANDIDATE_COMPONENTS_MIN
    )
    return icon_like


def _is_icon_like_sprite(sprite):
    source_max_dim = max(sprite['w'], sprite['h'])
    coverage = float(sprite.get('opaque_coverage', 1.0))
    component_count = int(sprite.get('component_count', 1))
    return (
        source_max_dim <= TEXT_CANDIDATE_SOURCE_MAX_DIM
        or coverage <= TEXT_CANDIDATE_COVERAGE_MAX
        or component_count >= TEXT_CANDIDATE_COMPONENTS_MIN
    )


def extract_peaks(result_map, threshold, tw, th, max_peaks=MAX_CANDIDATES):
    """Extract top peaks from matchTemplate result using NMS."""
    locs = np.where(result_map >= threshold)
    if len(locs[0]) == 0:
        return []
    scores = result_map[locs]
    order = np.argsort(-scores)[:max_peaks * 10]
    ys = locs[0][order]
    xs = locs[1][order]
    scores = scores[order]

    peaks = []
    for i in range(len(ys)):
        y, x, s = int(ys[i]), int(xs[i]), float(scores[i])
        too_close = False
        for py, px, _ in peaks:
            if abs(y - py) < th and abs(x - px) < tw:
                too_close = True
                break
        if not too_close:
            peaks.append((y, x, s))
            if len(peaks) >= max_peaks:
                break
    return peaks


def extract_top_peaks(result_map, tw, th, max_peaks=MAX_CANDIDATES, min_threshold=None):
    if max_peaks <= 0 or result_map.size == 0:
        return []
    flat = result_map.ravel()
    if min_threshold is not None:
        idxs = np.flatnonzero(flat >= min_threshold)
        if idxs.size == 0:
            return []
        limit = min(idxs.size, max_peaks * 20)
        if idxs.size > limit:
            part = np.argpartition(flat[idxs], -limit)[-limit:]
            idxs = idxs[part]
    else:
        limit = min(flat.size, max_peaks * 20)
        idxs = np.argpartition(flat, -limit)[-limit:]
    idxs = idxs[np.argsort(-flat[idxs])]

    peaks = []
    cols = result_map.shape[1]
    for idx in idxs:
        y = int(idx // cols)
        x = int(idx % cols)
        s = float(flat[idx])
        too_close = False
        for py, px, _ in peaks:
            if abs(y - py) < th and abs(x - px) < tw:
                too_close = True
                break
        if not too_close:
            peaks.append((y, x, s))
            if len(peaks) >= max_peaks:
                break
    return peaks


def collect_candidate_peaks(result_map, tw, th):
    strong = extract_peaks(result_map, CORR_THRESHOLD, tw, th)
    rescue = extract_top_peaks(
        result_map, tw, th, max_peaks=RESCUE_MAX_CANDIDATES, min_threshold=RESCUE_CORR_THRESHOLD)

    merged = []

    def _append(peaks, force_rescue):
        for y, x, score in peaks:
            if any(abs(y - py) < th and abs(x - px) < tw for py, px, _, _ in merged):
                continue
            merged.append((y, x, score, force_rescue or score < CORR_THRESHOLD))

    _append(strong, False)
    _append(rescue, True)
    return merged


def collect_component_candidate_peaks(result_map, threshold, max_peaks=SPECIAL_DIRECT_COMPONENT_MAX):
    if max_peaks <= 0 or result_map.size == 0:
        return []

    mask = (result_map >= threshold).astype(np.uint8)
    if not np.any(mask):
        return []

    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(mask, connectivity=8)
    peaks = []
    for label in range(1, num_labels):
        x, y, w, h, area = stats[label]
        if area <= 0 or w <= 0 or h <= 0:
            continue
        roi = result_map[y:y + h, x:x + w]
        idx = int(np.argmax(roi))
        py = int(y + idx // w)
        px = int(x + idx % w)
        peaks.append((py, px, float(result_map[py, px]), False))

    peaks.sort(key=lambda item: -item[2])
    return peaks[:max_peaks]


def collect_cover_candidate_peaks(result_map, existing, tw, th, white_only=False, max_per_match=64):
    if result_map.size == 0 or not existing:
        return []
    rh, rw = result_map.shape[:2]
    peaks = []
    seen = set()
    for e in existing:
        if white_only and not e.get('is_white'):
            continue
        rect = e.get('rect') or {}
        ew = int(rect.get('width', 0))
        eh = int(rect.get('height', 0))
        ex = int(rect.get('x', 0))
        ey = int(rect.get('y', 0))
        if ew <= 0 or eh <= 0 or ew > tw or eh > th:
            continue
        x0 = max(0, ex - (tw - ew))
        x1 = min(rw - 1, ex)
        y0 = max(0, ey - (th - eh))
        y1 = min(rh - 1, ey)
        if x1 < x0 or y1 < y0:
            continue
        roi = result_map[y0:y1 + 1, x0:x1 + 1]
        if roi.size == 0:
            continue
        flat = roi.ravel()
        limit = min(int(flat.size), max_per_match)
        idxs = np.argpartition(flat, -limit)[-limit:]
        idxs = idxs[np.argsort(-flat[idxs])]
        cols = roi.shape[1]
        for idx in idxs:
            py = int(y0 + idx // cols)
            px = int(x0 + idx % cols)
            key = (py, px)
            if key in seen:
                continue
            seen.add(key)
            peaks.append((py, px, float(result_map[py, px]), False))
    peaks.sort(key=lambda item: -item[2])
    return peaks


def _mask_ratio(mask, base_mask):
    denom = np.count_nonzero(base_mask)
    if denom <= 0:
        return 1.0
    return float(np.count_nonzero(mask & base_mask) / denom)


def _build_border_support(ok_mask, valid_mask):
    h, w = valid_mask.shape
    band = max(1, min(TEXT_BORDER_BAND_PX, max(1, min(h, w) // 2)))
    corner = max(1, min(band * 2, min(h, w)))
    ys, xs = np.indices(valid_mask.shape)

    border = valid_mask & (
        (ys < band) | (ys >= h - band) | (xs < band) | (xs >= w - band)
    )
    side_masks = {
        'top': border & (ys < band),
        'bottom': border & (ys >= h - band),
        'left': border & (xs < band),
        'right': border & (xs >= w - band),
    }
    corner_masks = {
        'tl': border & (ys < corner) & (xs < corner),
        'tr': border & (ys < corner) & (xs >= w - corner),
        'bl': border & (ys >= h - corner) & (xs < corner),
        'br': border & (ys >= h - corner) & (xs >= w - corner),
    }
    return {
        'total': _mask_ratio(ok_mask, border),
        'sides': {k: _mask_ratio(ok_mask, m) for k, m in side_masks.items()},
        'corners': {k: _mask_ratio(ok_mask, m) for k, m in corner_masks.items()},
    }


def _extract_text_groups(mismatch_mask, valid_mask):
    mismatch_count = np.count_nonzero(mismatch_mask)
    if mismatch_count < TEXT_MIN_PIXELS:
        return None, []

    grouped = mismatch_mask.astype(np.uint8) * 255
    if TEXT_MASK_CLOSE_PX > 0:
        kernel = np.ones((TEXT_MASK_CLOSE_PX * 2 + 1, TEXT_MASK_CLOSE_PX * 2 + 1), np.uint8)
        grouped = cv2.morphologyEx(grouped, cv2.MORPH_CLOSE, kernel)

    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(grouped, connectivity=8)
    valid_count = np.count_nonzero(valid_mask)
    max_group_pixels = max(TEXT_MIN_PIXELS, int(valid_count * TEXT_MAX_GROUP_AREA_RATIO))
    max_bbox_area = max(TEXT_MIN_PIXELS, int(valid_count * TEXT_MAX_BBOX_AREA_RATIO))
    h, w = valid_mask.shape

    text_mask = np.zeros_like(mismatch_mask, dtype=bool)
    groups = []

    for label in range(1, num_labels):
        x, y, bw, bh, _ = stats[label]
        raw_mask = mismatch_mask & (labels == label)
        raw_pixels = int(np.count_nonzero(raw_mask))
        if raw_pixels <= 0:
            continue

        bbox_area = int(bw * bh)
        fill_ratio = raw_pixels / max(1, bbox_area)
        if raw_pixels > max_group_pixels or bbox_area > max_bbox_area:
            return None, []
        if fill_ratio > TEXT_MAX_FILL_RATIO and raw_pixels >= 32:
            return None, []
        if bw >= max(2, int(w * 0.98)) and bh >= max(2, int(h * 0.85)):
            return None, []

        text_mask |= raw_mask
        groups.append({
            'x': int(x),
            'y': int(y),
            'width': int(bw),
            'height': int(bh),
            'pixels': raw_pixels,
            'fill_ratio': round(fill_ratio, 4),
        })

    if not groups or len(groups) > TEXT_MAX_GROUPS:
        return None, []

    if TEXT_MASK_EXPAND_PX > 0:
        kernel = np.ones((TEXT_MASK_EXPAND_PX * 2 + 1, TEXT_MASK_EXPAND_PX * 2 + 1), np.uint8)
        text_mask = cv2.dilate(text_mask.astype(np.uint8) * 255, kernel) > 0

    return text_mask & valid_mask, groups


def _build_text_overlay_patch(region_bgr, text_mask):
    if text_mask is None or np.count_nonzero(text_mask) < TEXT_MIN_PIXELS:
        return None
    patch = np.zeros((region_bgr.shape[0], region_bgr.shape[1], 4), dtype=np.uint8)
    patch[:, :, :3] = region_bgr
    patch[:, :, 3] = text_mask.astype(np.uint8) * 255
    return patch


def _analyze_region_match(region_bgr, expected_bgr, valid_mask, max_diff=DEFAULT_MAX_DIFF,
                          min_match_ratio=SOFT_MATCH_RATIO_MIN,
                          small_match_ratio=SOFT_SMALL_MATCH_RATIO_MIN,
                          allow_text_rescue=True):
    valid_mask = valid_mask.astype(bool)
    valid_count = np.count_nonzero(valid_mask)
    result = {
        'accepted': False,
        'median_diff': 999.0,
        'match_ratio': 0.0,
        'valid_pixels': int(valid_count),
        'perfect': False,
        'text_rescued': False,
        'text_pixels': 0,
        'text_ratio': 0.0,
        'text_groups': [],
        'text_overlay_patch': None,
        'border_support': None,
    }
    if valid_count < MIN_MATCH_PIXELS:
        return result

    diff = np.abs(region_bgr.astype(np.int16) - expected_bgr.astype(np.int16))
    per_px_max = diff.max(axis=2)
    ok_mask = valid_mask & (per_px_max <= max_diff)
    diffs = per_px_max[valid_mask]
    median_diff = float(np.median(diffs))
    match_ratio = float(np.count_nonzero(ok_mask) / valid_count)

    result.update({
        'median_diff': median_diff,
        'match_ratio': match_ratio,
        'perfect': False,
    })
    mismatch_mask = valid_mask & ~ok_mask
    mismatch_count = np.count_nonzero(mismatch_mask)
    result['perfect'] = bool(mismatch_count == 0)
    if mismatch_count == 0:
        result['accepted'] = True
        return result
    if mismatch_count < TEXT_MIN_PIXELS:
        ratio_min = (small_match_ratio if valid_count < SOFT_SMALL_VALID_PIXELS_MAX
                     else min_match_ratio)
        if median_diff <= VERIFY_MEDIAN_MAX and match_ratio >= ratio_min:
            result['accepted'] = True
        return result

    if allow_text_rescue and match_ratio >= TEXT_MIN_BASE_RATIO and mismatch_count / valid_count <= TEXT_MAX_AREA_RATIO:
        border_support = _build_border_support(ok_mask, valid_mask)
        side_hits = sum(1 for v in border_support['sides'].values() if v >= TEXT_BORDER_SIDE_MIN)
        corner_hits = sum(1 for v in border_support['corners'].values() if v >= TEXT_CORNER_RATIO_MIN)
        result['border_support'] = border_support
        if border_support['total'] >= TEXT_BORDER_TOTAL_MIN and side_hits >= 3 and corner_hits >= TEXT_MIN_CORNER_HITS:
            text_mask, groups = _extract_text_groups(mismatch_mask, valid_mask)
            if text_mask is not None:
                clean_mask = valid_mask & ~text_mask
                clean_count = np.count_nonzero(clean_mask)
                if clean_count >= MIN_MATCH_PIXELS:
                    clean_diffs = per_px_max[clean_mask]
                    clean_median = float(np.median(clean_diffs))
                    clean_ratio = float(np.count_nonzero(clean_diffs <= max_diff) / clean_count)
                    if clean_median <= VERIFY_MEDIAN_MAX and clean_ratio >= TEXT_MIN_CLEAN_RATIO:
                        text_pixels = int(np.count_nonzero(text_mask))
                        result.update({
                            'accepted': True,
                            'median_diff': clean_median,
                            'match_ratio': clean_ratio,
                            'valid_pixels': int(clean_count),
                            'perfect': False,
                            'text_rescued': True,
                            'text_pixels': text_pixels,
                            'text_ratio': float(text_pixels / valid_count),
                            'text_groups': groups,
                            'text_overlay_patch': _build_text_overlay_patch(region_bgr, text_mask),
                        })
                        return result

    ratio_min = (small_match_ratio if valid_count < SOFT_SMALL_VALID_PIXELS_MAX
                 else min_match_ratio)
    if median_diff <= VERIFY_MEDIAN_MAX and match_ratio >= ratio_min:
        result['accepted'] = True
    return result


def verify_pixel_match(screenshot, template_bgr, alpha_mask, x, y, max_diff=DEFAULT_MAX_DIFF,
                       min_match_ratio=SOFT_MATCH_RATIO_MIN,
                       small_match_ratio=SOFT_SMALL_MATCH_RATIO_MIN,
                       allow_text_rescue=True):
    th, tw = template_bgr.shape[:2]
    sh, sw = screenshot.shape[:2]
    if x < 0 or y < 0 or x + tw > sw or y + th > sh:
        return _analyze_region_match(np.zeros((th, tw, 3), dtype=np.uint8), template_bgr,
                                     np.zeros((th, tw), dtype=bool), max_diff=max_diff,
                                     min_match_ratio=min_match_ratio,
                                     small_match_ratio=small_match_ratio,
                                     allow_text_rescue=allow_text_rescue)
    region = screenshot[y:y + th, x:x + tw]
    vmask = erode_mask(alpha_mask)
    if vmask is None:
        vmask = np.ones((th, tw), dtype=bool)
    return _analyze_region_match(region, template_bgr, vmask, max_diff=max_diff,
                                 min_match_ratio=min_match_ratio,
                                 small_match_ratio=small_match_ratio,
                                 allow_text_rescue=allow_text_rescue)


def _build_shape_ring(mask, px=WHITE_SOURCE_RING_DILATE_PX):
    if mask is None:
        return None
    mask = mask.astype(bool)
    if not np.any(mask) or px <= 0:
        return None
    kernel = np.ones((px * 2 + 1, px * 2 + 1), np.uint8)
    dilated = cv2.dilate(mask.astype(np.uint8), kernel) > 0
    ring = dilated & ~mask
    return ring if np.any(ring) else None


def _analyze_solid_shape_region(region_bgr, visible_mask, base_mask):
    visible_mask = visible_mask.astype(bool)
    base_mask = base_mask.astype(bool)
    visible_count = np.count_nonzero(visible_mask)
    result = {
        'accepted': False,
        'median_diff': 999.0,
        'match_ratio': 0.0,
        'valid_pixels': int(visible_count),
        'perfect': False,
        'text_rescued': False,
        'text_pixels': 0,
        'text_ratio': 0.0,
        'text_groups': [],
        'text_overlay_patch': None,
        'tint_color': None,
    }
    if visible_count < MIN_MATCH_PIXELS:
        return result

    fg_pixels = region_bgr[visible_mask].astype(np.float32)
    tint_bgr = np.median(fg_pixels, axis=0).round().astype(np.uint8)
    diff = np.abs(region_bgr.astype(np.int16) - tint_bgr.astype(np.int16)).max(axis=2)
    fg_diffs = diff[visible_mask]
    if fg_diffs.size <= 0:
        return result

    match_ratio = float(np.count_nonzero(fg_diffs <= WHITE_SOURCE_UNIFORM_MAX_DIFF) / fg_diffs.size)
    median_diff = float(np.median(fg_diffs))
    ring_mask = _build_shape_ring(base_mask)
    ring_ok = False
    if ring_mask is not None:
        ring_pixels = diff[ring_mask]
        if ring_pixels.size >= WHITE_SOURCE_RING_MIN_PIXELS:
            ring_ratio = float(np.count_nonzero(ring_pixels >= WHITE_SOURCE_RING_MIN_CONTRAST) / ring_pixels.size)
            ring_ok = ring_ratio >= WHITE_SOURCE_RING_MIN_RATIO

    result.update({
        'median_diff': median_diff,
        'match_ratio': match_ratio,
        'perfect': bool(np.max(fg_diffs) <= WHITE_SOURCE_UNIFORM_MAX_DIFF),
        'tint_color': [int(tint_bgr[2]), int(tint_bgr[1]), int(tint_bgr[0])],
    })
    if match_ratio >= 1.0 and ring_ok:
        result['accepted'] = True
    return result


def _analyze_composited_solid_shape_region(region_bgr, base_mask, overlay_crop):
    base_mask = base_mask.astype(bool)
    overlay_alpha = overlay_crop[:, :, 3] > 0 if overlay_crop.shape[2] == 4 else np.zeros(base_mask.shape, dtype=bool)
    visible_mask = base_mask & ~overlay_alpha
    valid_mask = base_mask | overlay_alpha
    valid_count = int(np.count_nonzero(valid_mask))
    result = {
        'accepted': False,
        'median_diff': 999.0,
        'match_ratio': 0.0,
        'valid_pixels': valid_count,
        'perfect': False,
        'text_rescued': False,
        'text_pixels': 0,
        'text_ratio': 0.0,
        'text_groups': [],
        'text_overlay_patch': None,
        'tint_color': None,
    }
    visible_count = int(np.count_nonzero(visible_mask))
    if visible_count < MIN_MATCH_PIXELS or valid_count < MIN_MATCH_PIXELS:
        return result

    edge_mask = base_mask & ~erode_mask(base_mask, px=1)
    tint_mask = edge_mask & ~overlay_alpha
    if np.count_nonzero(tint_mask) < MIN_MATCH_PIXELS:
        tint_mask = visible_mask

    fg_pixels = region_bgr[tint_mask].astype(np.float32)
    tint_bgr = np.median(fg_pixels, axis=0).round().astype(np.uint8)

    canvas = np.zeros_like(region_bgr)
    canvas[base_mask] = tint_bgr
    alpha_composite_onto(canvas, overlay_crop, 0, 0)

    diff = np.abs(region_bgr.astype(np.int16) - canvas.astype(np.int16)).max(axis=2)
    valid_diffs = diff[valid_mask]
    if valid_diffs.size <= 0:
        return result

    match_ratio = float(np.count_nonzero(valid_diffs <= WHITE_SOURCE_COMPOSITE_MAX_DIFF) / valid_diffs.size)
    median_diff = float(np.median(valid_diffs))

    ring_mask = _build_shape_ring(base_mask)
    ring_ok = False
    if ring_mask is not None:
        tint_diff = np.abs(region_bgr.astype(np.int16) - tint_bgr.astype(np.int16)).max(axis=2)
        ring_pixels = tint_diff[ring_mask]
        if ring_pixels.size >= WHITE_SOURCE_RING_MIN_PIXELS:
            ring_ratio = float(np.count_nonzero(ring_pixels >= WHITE_SOURCE_RING_MIN_CONTRAST) / ring_pixels.size)
            ring_ok = ring_ratio >= WHITE_SOURCE_COMPOSITE_RING_MIN_RATIO

    result.update({
        'median_diff': median_diff,
        'match_ratio': match_ratio,
        'perfect': bool(np.max(valid_diffs) <= WHITE_SOURCE_COMPOSITE_MAX_DIFF),
        'tint_color': [int(tint_bgr[2]), int(tint_bgr[1]), int(tint_bgr[0])],
    })
    if median_diff <= WHITE_SOURCE_COMPOSITE_MEDIAN_MAX and match_ratio >= WHITE_SOURCE_COMPOSITE_RATIO_MIN and ring_ok:
        result['accepted'] = True
    return result


def _overlay_is_monochrome(overlay_crop):
    if overlay_crop is None or overlay_crop.size == 0 or overlay_crop.shape[2] < 4:
        return False
    overlay_alpha = overlay_crop[:, :, 3] > 0
    count = int(np.count_nonzero(overlay_alpha))
    if count < MIN_MATCH_PIXELS:
        return False
    values = overlay_crop[:, :, :3][overlay_alpha].astype(np.float32)
    median_bgr = np.median(values, axis=0)
    diff = np.abs(values - median_bgr).max(axis=1)
    mono_ratio = float(np.count_nonzero(diff <= WHITE_SOURCE_OVERLAY_MONO_MAX_DIFF) / diff.size)
    return mono_ratio >= WHITE_SOURCE_OVERLAY_MONO_MIN_RATIO


def verify_solid_shape_match(screenshot, alpha_mask, x, y):
    th, tw = alpha_mask.shape[:2]
    sh, sw = screenshot.shape[:2]
    if x < 0 or y < 0 or x + tw > sw or y + th > sh:
        return _analyze_solid_shape_region(
            np.zeros((th, tw, 3), dtype=np.uint8),
            np.zeros((th, tw), dtype=bool),
            np.zeros((th, tw), dtype=bool),
        )
    region = screenshot[y:y + th, x:x + tw]
    base_mask = alpha_mask.astype(bool)
    visible_mask = erode_mask(base_mask)
    if visible_mask is None:
        visible_mask = base_mask
    return _analyze_solid_shape_region(region, visible_mask, base_mask)


def composite_verify_solid_shape(alpha_mask, px, py, tw, th, overlay, screenshot):
    sh, sw = screenshot.shape[:2]
    if px < 0 or py < 0 or px + tw > sw or py + th > sh:
        return _analyze_solid_shape_region(
            np.zeros((th, tw, 3), dtype=np.uint8),
            np.zeros((th, tw), dtype=bool),
            np.zeros((th, tw), dtype=bool),
        )
    region = screenshot[py:py + th, px:px + tw]
    overlay_crop = overlay[py:py + th, px:px + tw]
    base_mask = alpha_mask.astype(bool)
    if overlay_crop.shape[2] == 4 and np.any(overlay_crop[:, :, 3] > 0):
        if not _overlay_is_monochrome(overlay_crop):
            return _analyze_solid_shape_region(
                np.zeros((th, tw, 3), dtype=np.uint8),
                np.zeros((th, tw), dtype=bool),
                np.zeros((th, tw), dtype=bool),
            )
        return _analyze_composited_solid_shape_region(region, base_mask, overlay_crop)
    visible_mask = erode_mask(base_mask)
    if visible_mask is None:
        visible_mask = base_mask
    return _analyze_solid_shape_region(region, visible_mask, base_mask)


# ──────────────────── 九宫辅助 ────────────────────

def _patch_diff_score(region, ref_bgr, ref_mask):
    if region is None or ref_bgr is None or region.shape[:2] != ref_bgr.shape[:2]:
        return 999999.0
    diff = np.abs(region.astype(np.int16) - ref_bgr.astype(np.int16))
    if ref_mask is not None:
        if diff.shape[:2] != ref_mask.shape:
            return 999999.0
        values = diff[ref_mask]
    else:
        values = diff.reshape(-1, diff.shape[-1]) if len(diff.shape) == 3 else diff.ravel()
    if values.size <= 0:
        return 999999.0
    values = values.reshape(-1, 3) if values.ndim == 2 else values
    return float(np.median(values.max(axis=-1)))


def _extract_patch_bgr_mask(patch):
    if patch.size == 0:
        return None, None
    if len(patch.shape) == 3 and patch.shape[2] == 4:
        mask = patch[:, :, 3] > 250
        if np.count_nonzero(mask) < 4:
            return None, None
        bgr = patch[:, :, :3].copy()
        bgr[~mask] = 0
        return bgr, mask
    if len(patch.shape) == 2:
        return cv2.cvtColor(patch, cv2.COLOR_GRAY2BGR), None
    return (patch[:, :, :3].copy(), None) if patch.shape[2] >= 3 else (None, None)


def _extract_mid_strip_bgr_mask(patch, horizontal):
    if patch.size == 0:
        return None, None
    h, w = patch.shape[:2]
    if horizontal:
        sample_w = min(NINESLICE_EDGE_SAMPLE_SPAN, w)
        x0 = max(0, (w - sample_w) // 2)
        sub = patch[:, x0:x0 + sample_w]
    else:
        sample_h = min(NINESLICE_EDGE_SAMPLE_SPAN, h)
        y0 = max(0, (h - sample_h) // 2)
        sub = patch[y0:y0 + sample_h, :]
    return _extract_patch_bgr_mask(sub)


def _find_9slice_size(img, border, target_img, tl_x, tl_y, tw_max, th_max):
    h, w = img.shape[:2]
    L = min(border['left'], w - 1)
    R = min(border['right'], w - L - 1)
    T = min(border['top'], h - 1)
    B = min(border['bottom'], h - T - 1)

    tr_bgr, tr_mask = _extract_patch_bgr_mask(img[0:T, w - R:w])
    bl_bgr, bl_mask = _extract_patch_bgr_mask(img[h - B:h, 0:L])
    br_bgr, br_mask = _extract_patch_bgr_mask(img[h - B:h, w - R:w])
    if tr_bgr is None or bl_bgr is None or br_bgr is None:
        return []

    top_mid_bgr, top_mid_mask = _extract_mid_strip_bgr_mask(img[0:T, L:w - R], horizontal=True)
    bottom_mid_bgr, bottom_mid_mask = _extract_mid_strip_bgr_mask(img[h - B:h, L:w - R], horizontal=True)
    left_mid_bgr, left_mid_mask = _extract_mid_strip_bgr_mask(img[T:h - B, 0:L], horizontal=False)
    right_mid_bgr, right_mid_mask = _extract_mid_strip_bgr_mask(img[T:h - B, w - R:w], horizontal=False)

    def _patch_matches(region, ref_bgr, ref_mask):
        diff = np.abs(region.astype(np.int16) - ref_bgr.astype(np.int16))
        if ref_mask is not None:
            d = diff[ref_mask]
        else:
            d = diff.reshape(-1, diff.shape[-1]) if len(diff.shape) == 3 else diff.ravel()
        return d.size > 0 and d.max() <= DEFAULT_MAX_DIFF

    min_width, min_height = get_9slice_target_min_size(img, border)
    widths = []
    for rw in range(min_width, min(tw_max - tl_x + 1, L + R + 801)):
        tr_x = tl_x + rw - R
        if tr_x < 0 or tr_x + R > tw_max or tl_y + T > th_max:
            continue
        if _patch_matches(target_img[tl_y:tl_y + T, tr_x:tr_x + R], tr_bgr, tr_mask):
            widths.append(rw)

    results = []
    for rw in widths:
        br_x = tl_x + rw - R
        if br_x < 0 or br_x + R > tw_max:
            continue
        for rh in range(min_height, min(th_max - tl_y + 1, T + B + 801)):
            bl_y = tl_y + rh - B
            if bl_y < 0 or bl_y + B > th_max or tl_x + L > tw_max:
                continue
            if (_patch_matches(target_img[bl_y:bl_y + B, tl_x:tl_x + L], bl_bgr, bl_mask) and
                    _patch_matches(target_img[bl_y:bl_y + B, br_x:br_x + R], br_bgr, br_mask)):
                score = 0.0
                center_w = max(1, rw - L - R)
                center_h = max(1, rh - T - B)

                if top_mid_bgr is not None:
                    sample_w = top_mid_bgr.shape[1]
                    tx = tl_x + L + max(0, (center_w - sample_w) // 2)
                    top_region = target_img[tl_y:tl_y + T, tx:tx + sample_w]
                    score += _patch_diff_score(top_region, top_mid_bgr, top_mid_mask)
                if bottom_mid_bgr is not None:
                    sample_w = bottom_mid_bgr.shape[1]
                    bx = tl_x + L + max(0, (center_w - sample_w) // 2)
                    bottom_region = target_img[bl_y:bl_y + B, bx:bx + sample_w]
                    score += _patch_diff_score(bottom_region, bottom_mid_bgr, bottom_mid_mask)
                if left_mid_bgr is not None:
                    sample_h = left_mid_bgr.shape[0]
                    ly = tl_y + T + max(0, (center_h - sample_h) // 2)
                    left_region = target_img[ly:ly + sample_h, tl_x:tl_x + L]
                    score += _patch_diff_score(left_region, left_mid_bgr, left_mid_mask)
                if right_mid_bgr is not None:
                    sample_h = right_mid_bgr.shape[0]
                    ry = tl_y + T + max(0, (center_h - sample_h) // 2)
                    right_region = target_img[ry:ry + sample_h, br_x:br_x + R]
                    score += _patch_diff_score(right_region, right_mid_bgr, right_mid_mask)

                results.append((rw, rh, score))
    if len(results) <= NINESLICE_MAX_SIZE_CANDIDATES:
        return [(rw, rh) for rw, rh, _ in results]
    results.sort(key=lambda item: (item[2], -(item[0] * item[1])))
    return [(rw, rh) for rw, rh, _ in results[:NINESLICE_MAX_SIZE_CANDIDATES]]


# ──────────────────── 去重 ────────────────────

def deduplicate_matches(matches):
    if not matches:
        return matches
    matches = sorted(matches, key=lambda m: (
        m.get('median_diff', 999.0),
        -m.get('match_ratio', 0.0),
        1 if m.get('text_rescued') else 0,
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


def _is_dup_pos(px, py, tw, th, existing):
    for e in existing:
        er = e['rect']
        if (abs(er['x'] - px) <= 2 and abs(er['y'] - py) <= 2 and
                abs(er['width'] - tw) <= 2 and abs(er['height'] - th) <= 2):
            return True
    return False


# ──────────────────── 覆盖关系树 ────────────────────

def rect_contains(outer, inner):
    return (outer['x'] <= inner['x'] and outer['y'] <= inner['y'] and
            outer['x'] + outer['width'] >= inner['x'] + inner['width'] and
            outer['y'] + outer['height'] >= inner['y'] + inner['height'])


def rect_area(r):
    return r['width'] * r['height']


def _rects_overlap(a, b):
    return not (a['x'] + a['width'] <= b['x'] or b['x'] + b['width'] <= a['x'] or
                a['y'] + a['height'] <= b['y'] or b['y'] + b['height'] <= a['y'])


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


# ──────────────────── Overlay 构建 ────────────────────

def build_overlay(all_matches, sprites, white_sprites, sh, sw):
    """
    把所有已匹配sprite合成为一张 BGRA overlay 图。
    按layer从大到小（底层先画，顶层后画覆盖）。
    """
    overlay = np.zeros((sh, sw, 4), dtype=np.uint8)
    sorted_matches = sorted(all_matches, key=lambda m: m.get('layer', 0), reverse=True)
    for m in sorted_matches:
        r = m['rect']
        sp = _get_sprite(m, sprites, white_sprites)
        if sp is None:
            continue
        rendered = resize_sprite(sp, r['width'], r['height'])
        rendered = ensure_bgra(rendered)

        if m.get('is_white') and m.get('tint_color'):
            tint = m['tint_color']
            bgr_tint = np.array([tint[2], tint[1], tint[0]], dtype=np.uint8)
            mask = rendered[:, :, 3] > 0
            rendered[:, :, :3][mask] = bgr_tint

        alpha_composite_bgra(overlay, rendered, r['x'], r['y'])
        text_patch = m.get('text_overlay_patch')
        if text_patch is not None:
            alpha_composite_bgra(overlay, text_patch, r['x'], r['y'])
    return overlay


def _get_sprite(match, sprites, white_sprites):
    if match.get('is_white'):
        idx = match.get('white_idx', -1)
        return white_sprites[idx] if 0 <= idx < len(white_sprites) else None
    idx = match.get('sprite_idx', -1)
    return sprites[idx] if 0 <= idx < len(sprites) else None


# ──────────────────── 合成验证 ────────────────────

def composite_verify(template_bgr, alpha_mask, px, py, tw, th, overlay, screenshot,
                     max_diff=DEFAULT_MAX_DIFF,
                     min_match_ratio=SOFT_MATCH_RATIO_MIN,
                     small_match_ratio=SOFT_SMALL_MATCH_RATIO_MIN,
                     allow_text_rescue=True):
    sh, sw = screenshot.shape[:2]
    if px < 0 or py < 0 or px + tw > sw or py + th > sh:
        return _analyze_region_match(np.zeros((th, tw, 3), dtype=np.uint8), template_bgr,
                                     np.zeros((th, tw), dtype=bool), max_diff=max_diff,
                                     min_match_ratio=min_match_ratio,
                                     small_match_ratio=small_match_ratio,
                                     allow_text_rescue=allow_text_rescue)
    canvas = template_bgr.copy()
    overlay_crop = overlay[py:py + th, px:px + tw]
    alpha_composite_onto(canvas, overlay_crop, 0, 0)
    crop = screenshot[py:py + th, px:px + tw]
    vmask = erode_mask(alpha_mask) if alpha_mask is not None else np.ones((th, tw), dtype=bool)
    overlay_alpha = overlay_crop[:, :, 3] if overlay_crop.shape[2] == 4 else np.zeros((th, tw), np.uint8)
    visible = vmask & (overlay_alpha < 250)
    return _analyze_region_match(crop, canvas, visible, max_diff=max_diff,
                                 min_match_ratio=min_match_ratio,
                                 small_match_ratio=small_match_ratio,
                                 allow_text_rescue=allow_text_rescue)


# ──────────────────── 单层匹配 ────────────────────

def match_one_layer(layer_num, sprites, sliced_indices, white_sprites,
                    screenshot, overlay, all_existing, text_regions,
                    max_diff=DEFAULT_MAX_DIFF):
    is_direct = overlay is None
    matches = []

    normal_count = len(sprites)
    print(f"  [1/3] original sprites (0/{normal_count})...", flush=True)
    normal = _match_normal(sprites, sliced_indices, screenshot, overlay,
                           all_existing + matches, is_direct, text_regions, max_diff=max_diff)
    matches.extend(normal)
    print(f"  [1/3] original sprites: found {len(normal)}")

    slice_count = len(sliced_indices)
    print(f"  [2/3] 9-slice sprites (0/{slice_count})...", flush=True)
    sliced = _match_9slice(sprites, sliced_indices, screenshot, overlay,
                           all_existing + matches, is_direct, text_regions, max_diff=max_diff)
    matches.extend(sliced)
    print(f"  [2/3] 9-slice sprites: found {len(sliced)}")

    matches = deduplicate_matches(matches)

    white_count = len(white_sprites)
    print(f"  [3/3] white sprites (0/{white_count})...", flush=True)
    white = _match_white(white_sprites, screenshot, overlay,
                         all_existing + matches, is_direct, text_regions, max_diff=max_diff)
    matches.extend(white)
    print(f"  [3/3] white sprites: found {len(white)}")

    matches = deduplicate_matches(matches)
    return matches


# ── 普通 sprite 匹配 ──


def _match_normal(sprites, sliced_indices, screenshot, overlay, existing, is_direct, text_regions,
                  max_diff=DEFAULT_MAX_DIFF):
    sh, sw = screenshot.shape[:2]
    matches = []
    total = len(sprites)
    t_start = time.time()

    for sidx, sp in enumerate(sprites):
        name = os.path.basename(sp['rel_path'])
        sys.stdout.write(f"\r  [1/3] original ({sidx+1}/{total}) {name[:40]:<40} matched:{len(matches)}")
        sys.stdout.flush()

        is_white_source = bool(sp.get('is_white_source'))
        if is_white_source and sp.get('opaque_pixels', 0) < WHITE_DIRECT_MIN_OPAQUE_PIXELS:
            continue
        is_underlay_border = (
            not is_direct
            and not is_white_source
            and bool(sp.get('border'))
            and float(sp.get('opaque_coverage', 0.0)) >= UNDERLAY_BORDER_COVERAGE_MIN
        )

        scales = [1.0]

        for scale in scales:
            tw = max(1, round(sp['w'] * scale))
            th = max(1, round(sp['h'] * scale))
            if tw < MIN_TEMPLATE_DIM or th < MIN_TEMPLATE_DIM or tw > sw or th > sh:
                continue
            if is_underlay_border:
                match_max_diff = DEFAULT_MAX_DIFF
                min_ratio = UNDERLAY_MATCH_RATIO_MIN
                small_ratio = UNDERLAY_SMALL_MATCH_RATIO_MIN
            else:
                match_max_diff = ORIGINAL_MATCH_MAX_DIFF
                min_ratio = ORIGINAL_MATCH_RATIO_MIN
                small_ratio = ORIGINAL_MATCH_RATIO_MIN
            allow_text_rescue = False

            tpl, amask = prepare_template(sp['img'], tw, th)
            if amask is not None and np.count_nonzero(amask) < MIN_MATCH_PIXELS:
                continue

            mask_3ch = make_mask_3ch(amask)
            if mask_3ch is not None:
                res = cv2.matchTemplate(screenshot, tpl, cv2.TM_CCORR_NORMED, mask=mask_3ch)
            else:
                res = cv2.matchTemplate(screenshot, tpl, cv2.TM_CCORR_NORMED)

            if scale == 1.0 and is_white_source:
                peaks = collect_component_candidate_peaks(
                    res, WHITE_DIRECT_COMPONENT_THRESHOLD)
                if not is_direct:
                    priority_peaks = collect_cover_candidate_peaks(
                        res, existing, tw, th, white_only=True)
                    merged = []
                    seen_exact = set()
                    for y0, x0, score0, rescue0 in priority_peaks:
                        key = (y0, x0)
                        if key in seen_exact:
                            continue
                        seen_exact.add(key)
                        merged.append((y0, x0, score0, rescue0))
                    for y0, x0, score0, rescue0 in peaks:
                        if any(abs(y0 - py0) < th and abs(x0 - px0) < tw for py0, px0, _, _ in merged):
                            continue
                        merged.append((y0, x0, score0, rescue0))
                    peaks = merged
            elif scale == 1.0 and sp.get('border'):
                if is_underlay_border:
                    peaks = collect_cover_candidate_peaks(
                        res, existing, tw, th, white_only=True, max_per_match=24)
                else:
                    peaks = collect_component_candidate_peaks(
                        res, BORDER_DIRECT_COMPONENT_THRESHOLD)
            else:
                peaks = collect_candidate_peaks(res, tw, th)
            for cy, cx, score, rescue_candidate in peaks:
                px, py = int(cx), int(cy)
                if _is_dup_pos(px, py, tw, th, existing + matches):
                    continue
                cand_r = {'x': px, 'y': py, 'width': tw, 'height': th}
                if _is_text_like_false_positive(sp, cand_r, text_regions):
                    continue
                if not is_direct:
                    if not any(_rects_overlap(cand_r, e['rect']) for e in existing):
                        continue

                if is_white_source:
                    if is_direct:
                        verify = verify_solid_shape_match(screenshot, amask, px, py)
                    else:
                        verify = composite_verify_solid_shape(amask, px, py, tw, th, overlay, screenshot)
                else:
                    if is_direct:
                        verify = verify_pixel_match(
                            screenshot, tpl, amask, px, py, max_diff=match_max_diff,
                            min_match_ratio=min_ratio, small_match_ratio=small_ratio,
                            allow_text_rescue=allow_text_rescue)
                    else:
                        verify = composite_verify(
                            tpl, amask, px, py, tw, th, overlay, screenshot, max_diff=match_max_diff,
                            min_match_ratio=min_ratio, small_match_ratio=small_ratio,
                            allow_text_rescue=allow_text_rescue)
                if verify['accepted']:
                    matches.append({
                        'sprite_idx': sidx,
                        'white_idx': sp.get('white_idx', -1) if is_white_source else -1,
                        'is_white': is_white_source,
                        'is_9slice': False,
                        'rect': {'x': px, 'y': py, 'width': tw, 'height': th},
                        'match_ratio': round(verify['match_ratio'], 6),
                        'median_diff': round(verify['median_diff'], 2),
                        'perfect': bool(verify['perfect']),
                        'valid_pixels': verify['valid_pixels'],
                        'scale': round(scale, 4),
                        'candidate_score': round(float(score), 6),
                        'rescue_candidate': bool(rescue_candidate),
                        'text_rescued': bool(verify['text_rescued']),
                        'text_pixels': int(verify['text_pixels']),
                        'text_ratio': round(float(verify['text_ratio']), 6),
                        'text_groups': verify['text_groups'],
                        'text_overlay_patch': verify['text_overlay_patch'],
                        'tint_color': verify.get('tint_color'),
                    })
    elapsed = time.time() - t_start
    sys.stdout.write(f"\r  [1/3] original ({total}/{total}) done in {elapsed:.1f}s{' ':40}\n")
    sys.stdout.flush()
    return matches


# ── 九宫格 sprite 匹配 ──

def _match_9slice(sprites, sliced_indices, screenshot, overlay, existing, is_direct, text_regions,
                  max_diff=DEFAULT_MAX_DIFF):
    sh, sw = screenshot.shape[:2]
    matches = []
    total = len(sliced_indices)
    t_start = time.time()

    for si, sidx in enumerate(sliced_indices):
        sp = sprites[sidx]
        border = sp['border']
        if not border:
            continue
        name = os.path.basename(sp['rel_path'])
        sys.stdout.write(f"\r  [2/3] 9-slice ({si+1}/{total}) {name[:40]:<40} matched:{len(matches)}")
        sys.stdout.flush()
        is_icon_like = _is_icon_like_sprite(sp)
        min_ratio = ICON_LIKE_MATCH_RATIO_MIN if is_icon_like else SOFT_MATCH_RATIO_MIN
        small_ratio = ICON_LIKE_SMALL_MATCH_RATIO_MIN if is_icon_like else SOFT_SMALL_MATCH_RATIO_MIN
        allow_text_rescue = not is_icon_like
        img = sp['img']
        h, w = img.shape[:2]
        L = min(border['left'], w - 1)
        R = min(border['right'], w - L - 1)
        T = min(border['top'], h - 1)
        B = min(border['bottom'], h - T - 1)
        if T < MIN_TEMPLATE_DIM or L < MIN_TEMPLATE_DIM:
            continue

        tl_bgr, tl_mask = _extract_patch_bgr_mask(img[0:T, 0:L])
        if tl_bgr is None:
            continue

        mask_3ch = make_mask_3ch(tl_mask)
        if mask_3ch is not None:
            res = cv2.matchTemplate(screenshot, tl_bgr, cv2.TM_CCORR_NORMED, mask=mask_3ch)
        else:
            res = cv2.matchTemplate(screenshot, tl_bgr, cv2.TM_CCORR_NORMED)
        tl_locs = collect_candidate_peaks(res, L, T)

        for cy, cx, score, rescue_candidate in tl_locs:
            tl_x, tl_y = int(cx), int(cy)
            if tl_x < 0 or tl_y < 0:
                continue
            sizes = _find_9slice_size(img, border, screenshot, tl_x, tl_y, sw, sh)
            for (rw, rh) in sizes:
                if _is_dup_pos(tl_x, tl_y, rw, rh, existing + matches):
                    continue
                cand_r = {'x': tl_x, 'y': tl_y, 'width': rw, 'height': rh}
                if _is_text_like_false_positive(sp, cand_r, text_regions):
                    continue
                tpl, amask = prepare_template(sp['img'], rw, rh, border=border)
                if is_direct:
                    verify = verify_pixel_match(
                        screenshot, tpl, amask, tl_x, tl_y, max_diff=max_diff,
                        min_match_ratio=min_ratio, small_match_ratio=small_ratio,
                        allow_text_rescue=allow_text_rescue)
                else:
                    verify = composite_verify(
                        tpl, amask, tl_x, tl_y, rw, rh, overlay, screenshot, max_diff=max_diff,
                        min_match_ratio=min_ratio, small_match_ratio=small_ratio,
                        allow_text_rescue=allow_text_rescue)
                if verify['accepted']:
                    matches.append({
                        'sprite_idx': sidx, 'is_white': False, 'is_9slice': True,
                        'rect': {'x': tl_x, 'y': tl_y, 'width': rw, 'height': rh},
                        'match_ratio': round(verify['match_ratio'], 6),
                        'median_diff': round(verify['median_diff'], 2),
                        'perfect': bool(verify['perfect']),
                        'valid_pixels': verify['valid_pixels'],
                        'scale': round(rw / sp['w'], 4),
                        'candidate_score': round(float(score), 6),
                        'rescue_candidate': bool(rescue_candidate),
                        'text_rescued': bool(verify['text_rescued']),
                        'text_pixels': int(verify['text_pixels']),
                        'text_ratio': round(float(verify['text_ratio']), 6),
                        'text_groups': verify['text_groups'],
                        'text_overlay_patch': verify['text_overlay_patch'],
                    })
    elapsed = time.time() - t_start
    sys.stdout.write(f"\r  [2/3] 9-slice ({total}/{total}) done in {elapsed:.1f}s{' ':40}\n")
    sys.stdout.flush()
    return matches


# ── 白图 sprite 匹配 ──

def _estimate_dominant_bgr(region, mask):
    pixels = region[mask]
    if pixels.size == 0:
        return None
    return np.median(pixels.astype(np.float32), axis=0).round().astype(np.uint8)


def _match_white(white_sprites, screenshot, overlay, existing, is_direct, text_regions,
                 max_diff=DEFAULT_MAX_DIFF):
    sh, sw = screenshot.shape[:2]
    if not white_sprites:
        return []

    covered = np.zeros((sh, sw), dtype=bool)
    for e in existing:
        r = e['rect']
        x1, y1 = max(0, r['x']), max(0, r['y'])
        x2 = min(sw, r['x'] + r['width'])
        y2 = min(sh, r['y'] + r['height'])
        covered[y1:y2, x1:x2] = True

    uncovered = ~covered
    if np.count_nonzero(uncovered) < MIN_MATCH_PIXELS:
        return []

    uncov_u8 = uncovered.astype(np.uint8) * 255
    contours, _ = cv2.findContours(uncov_u8, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    total_contours = len(contours)
    t_start = time.time()
    matches = []
    for ci, contour in enumerate(contours):
        sys.stdout.write(f"\r  [3/3] white: region ({ci+1}/{total_contours}) matched:{len(matches)}")
        sys.stdout.flush()
        x, y, w, h = cv2.boundingRect(contour)
        if w < MIN_TEMPLATE_DIM or h < MIN_TEMPLATE_DIM:
            continue
        if cv2.contourArea(contour) < MIN_MATCH_PIXELS:
            continue

        rect_mask = uncovered[y:y + h, x:x + w]
        if np.count_nonzero(rect_mask) < MIN_MATCH_PIXELS:
            continue

        region = screenshot[y:y + h, x:x + w]

        best = None
        for widx, wsp in enumerate(white_sprites):
            cand_r = {'x': x, 'y': y, 'width': w, 'height': h}
            if _is_text_like_false_positive(wsp, cand_r, text_regions):
                continue
            is_icon_like = _is_icon_like_sprite(wsp)
            min_ratio = ICON_LIKE_MATCH_RATIO_MIN if is_icon_like else SOFT_MATCH_RATIO_MIN
            small_ratio = ICON_LIKE_SMALL_MATCH_RATIO_MIN if is_icon_like else SOFT_SMALL_MATCH_RATIO_MIN
            allow_text_rescue = not is_icon_like
            if wsp['border']:
                min_w, min_h = get_9slice_target_min_size(wsp['img'], wsp['border'])
                if w < min_w or h < min_h:
                    continue
            else:
                if w != wsp['w'] or h != wsp['h']:
                    continue
            resized = resize_sprite(wsp, w, h)
            if len(resized.shape) == 3 and resized.shape[2] == 4:
                wa = resized[:, :, 3]
            else:
                wa = np.full((h, w), 255, dtype=np.uint8)
            opaque = wa > 250
            if np.any(rect_mask & ~opaque):
                continue
            check = rect_mask & opaque
            cc = np.count_nonzero(check)
            if cc < MIN_MATCH_PIXELS:
                continue

            fill_bgr = _estimate_dominant_bgr(region, check)
            if fill_bgr is None:
                continue

            tpl_bgr = np.zeros((h, w, 3), dtype=np.uint8)
            tpl_bgr[opaque] = fill_bgr
            if is_direct:
                verify = verify_pixel_match(
                    screenshot, tpl_bgr, check, x, y, max_diff=max_diff,
                    min_match_ratio=min_ratio, small_match_ratio=small_ratio,
                    allow_text_rescue=allow_text_rescue)
            else:
                verify = composite_verify(
                    tpl_bgr, check, x, y, w, h, overlay, screenshot, max_diff=max_diff,
                    min_match_ratio=min_ratio, small_match_ratio=small_ratio,
                    allow_text_rescue=allow_text_rescue)
            if not verify['accepted']:
                continue

            cov = cc / np.count_nonzero(rect_mask)
            cand = (
                1 if verify.get('text_rescued') else 0,
                round(verify['median_diff'], 2),
                -round(verify['match_ratio'], 6),
                -round(cov, 4),
            )
            if best is None or cand < best['_rank']:
                tint_color = [int(fill_bgr[2]), int(fill_bgr[1]), int(fill_bgr[0])]
                best = {
                    '_rank': cand,
                    'white_idx': widx,
                    'sprite_idx': wsp.get('base_sprite_idx', -1),
                    'is_white': True, 'is_9slice': wsp['border'] is not None,
                    'rect': {'x': x, 'y': y, 'width': w, 'height': h},
                    'match_ratio': round(verify['match_ratio'], 6),
                    'median_diff': round(verify['median_diff'], 2),
                    'perfect': bool(verify['perfect']),
                    'valid_pixels': verify['valid_pixels'],
                    'scale': round(w / wsp['w'], 4),
                    'tint_color': tint_color,
                    'coverage': round(cov, 4),
                    'text_rescued': bool(verify['text_rescued']),
                    'text_pixels': int(verify['text_pixels']),
                    'text_ratio': round(float(verify['text_ratio']), 6),
                    'text_groups': verify['text_groups'],
                    'text_overlay_patch': verify['text_overlay_patch'],
                }
        if best:
            best.pop('_rank', None)
            matches.append(best)
    elapsed = time.time() - t_start
    sys.stdout.write(f"\r  [3/3] white: done in {elapsed:.1f}s{' ':40}\n")
    sys.stdout.flush()
    return matches


# ──────────────────── Debug 图片 ────────────────────

def _layer_color(layer_num):
    return LAYER_COLORS_BGR[layer_num % len(LAYER_COLORS_BGR)]


def _make_checkerboard(h, w, tile=24):
    y_idx, x_idx = np.indices((h, w))
    cells = ((x_idx // tile) + (y_idx // tile)) % 2
    light = np.full((h, w, 3), 244, dtype=np.uint8)
    dark = np.full((h, w, 3), 220, dtype=np.uint8)
    return np.where(cells[:, :, None] == 0, light, dark)


def _make_labeled_panel(img, title, width=None):
    panel = img
    if width is not None and panel.shape[1] != width:
        if panel.shape[1] < width:
            pad = np.full((panel.shape[0], width - panel.shape[1], 3), 255, dtype=np.uint8)
            panel = np.hstack([panel, pad])
        else:
            panel = panel[:, :width]
    header_h = 44
    out = np.full((panel.shape[0] + header_h, panel.shape[1], 3), 255, dtype=np.uint8)
    out[header_h:, :] = panel
    cv2.rectangle(out, (0, 0), (out.shape[1] - 1, header_h - 1), (235, 235, 235), -1)
    cv2.putText(out, title, (12, 28), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (40, 40, 40), 2)
    return out


def _render_overlay_preview(overlay_bgra):
    preview = _make_checkerboard(overlay_bgra.shape[0], overlay_bgra.shape[1])
    alpha_composite_onto(preview, overlay_bgra, 0, 0)
    return preview


def save_overlay_debug(overlay_bgra, screenshot_bgr, debug_dir, stem):
    os.makedirs(debug_dir, exist_ok=True)
    raw_path = os.path.join(debug_dir, f"{stem}_overlay.png")
    preview_path = os.path.join(debug_dir, f"{stem}_overlay_preview.png")
    compare_path = os.path.join(debug_dir, f"{stem}_compare.png")

    cv2.imwrite(raw_path, overlay_bgra)
    preview = _render_overlay_preview(overlay_bgra)
    cv2.imwrite(preview_path, preview)

    panel_w = max(screenshot_bgr.shape[1], preview.shape[1])
    left = _make_labeled_panel(screenshot_bgr, "Screenshot", width=panel_w)
    right = _make_labeled_panel(preview, "Reconstructed", width=panel_w)
    compare = np.hstack([left, right])
    cv2.imwrite(compare_path, compare)

    print(f"  Overlay saved: {raw_path}")
    print(f"  Compare saved: {compare_path}")
    return raw_path, compare_path


def generate_layer_debug(screenshot, layer_num, layer_matches, all_matches_before,
                         sprites, white_sprites, debug_dir):
    canvas = screenshot.copy()

    # draw previous layers with thin lines (dimmed)
    for m in all_matches_before:
        r = m['rect']
        c = _layer_color(m.get('layer', 0))
        dim = tuple(v // 2 for v in c)
        cv2.rectangle(canvas, (r['x'], r['y']),
                      (r['x'] + r['width'], r['y'] + r['height']), dim, 1)

    # draw current layer with thick lines + fill + label
    color = _layer_color(layer_num)
    for m in layer_matches:
        r = m['rect']
        overlay = canvas.copy()
        cv2.rectangle(overlay, (r['x'], r['y']),
                      (r['x'] + r['width'], r['y'] + r['height']), color, -1)
        cv2.addWeighted(overlay, 0.2, canvas, 0.8, 0, canvas)
        cv2.rectangle(canvas, (r['x'], r['y']),
                      (r['x'] + r['width'], r['y'] + r['height']), color, 2)

        sp = _get_sprite(m, sprites, white_sprites)
        sp_name = os.path.basename(sp['rel_path']) if sp else '?'
        tags = []
        if m.get('is_9slice'):
            tags.append('9s')
        if m.get('is_white'):
            tags.append('w')
        if m.get('text_rescued'):
            tags.append('txt')
        label = f"L{layer_num} {sp_name}"
        if tags:
            label += f" [{' '.join(tags)}]"
        cv2.putText(canvas, label, (r['x'] + 2, r['y'] - 6),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.45, color, 1)

    out_path = os.path.join(debug_dir, f"layer_{layer_num}.png")
    cv2.imwrite(out_path, canvas)
    print(f"  Debug saved: {out_path}")
    return out_path


# ──────────────────── 主流程 ────────────────────

def match_sprites(screenshot_path, output_path, sprite_dirs, borders_path, whites_path,
                   debug_dir=None, max_diff=DEFAULT_MAX_DIFF):
    t0 = time.time()

    screenshot = cv2.imread(screenshot_path)
    if screenshot is None:
        print(f"ERROR: Cannot read screenshot: {screenshot_path}")
        sys.exit(1)
    sh, sw = screenshot.shape[:2]
    print(f"Screenshot: {sw}x{sh}, max_diff: {max_diff}")
    text_regions = detect_text_regions(screenshot)
    print(f"Detected {len(text_regions)} text exclusion regions")

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

    while layer < MAX_LAYERS:
        print(f"\n=== Layer {layer} ===")

        if layer == 0:
            overlay = None
        else:
            overlay = build_overlay(all_matches, sprites, white_sprites, sh, sw)

        layer_matches = match_one_layer(
            layer, sprites, sliced_indices, white_sprites,
            screenshot, overlay, all_matches, text_regions, max_diff=max_diff)

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

        if debug_dir:
            generate_layer_debug(screenshot, layer, layer_matches,
                                 [m for m in all_matches if m.get('layer', 0) < layer],
                                 sprites, white_sprites, debug_dir)
            layer_overlay = build_overlay(all_matches, sprites, white_sprites, sh, sw)
            save_overlay_debug(layer_overlay, screenshot, debug_dir, f"layer_{layer}")
        layer += 1

    final_overlay = build_overlay(all_matches, sprites, white_sprites, sh, sw)
    if debug_dir:
        save_overlay_debug(final_overlay, screenshot, debug_dir, "final")

    # ── 构建节点 + 覆盖树 ──
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
                'text_rescued': m.get('text_rescued', False),
            }
        }
        if m.get('tint_color'):
            mn['match']['tint_color'] = m['tint_color']
        if m.get('coverage'):
            mn['match']['coverage'] = m['coverage']
        if m.get('text_pixels'):
            mn['match']['text_pixels'] = m['text_pixels']
            mn['match']['text_ratio'] = m.get('text_ratio', 0.0)
        if m.get('text_groups'):
            mn['match']['text_groups'] = m['text_groups']
        match_nodes.append(mn)

    build_coverage_tree(match_nodes)

    # ── 统计 + 输出 ──
    elapsed = time.time() - t0
    perfect = sum(1 for n in match_nodes if n['match']['perfect'])
    whites = sum(1 for n in match_nodes if n['match'].get('is_white'))
    slices = sum(1 for n in match_nodes if n['match'].get('is_9slice'))
    text_rescued = sum(1 for n in match_nodes if n['match'].get('text_rescued'))

    output = {
        'screenshot': screenshot_path,
        'sprite_dirs': sprite_dirs,
        'elapsed_seconds': round(elapsed, 2),
        'summary': {
            'total_matches': len(match_nodes),
            'perfect_matches': perfect,
            'slice_matches': slices,
            'white_matches': whites,
            'text_rescued_matches': text_rescued,
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
        f"white:{whites} text:{text_rescued} layers:{layer})")
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
    args = parser.parse_args()
    match_sprites(args.screenshot, args.output, args.sprite_dirs,
                  args.borders, args.whites, args.debug_dir, args.max_diff)
