"""matcher_verify — All verification functions for sprite matching."""

import cv2
import numpy as np

from matcher_core import (
    MIN_MATCH_PIXELS, VERIFY_MEDIAN_MAX, DEFAULT_MAX_DIFF,
    SOFT_MATCH_RATIO_MIN, SOFT_SMALL_MATCH_RATIO_MIN, SOFT_SMALL_VALID_PIXELS_MAX,
    FUZZY_MAX_DIFF, FUZZY_MATCH_RATIO_MIN, FUZZY_SMALL_MATCH_RATIO_MIN,
    FUZZY_LOCAL_DIFF_MAX_AREA_RATIO, FUZZY_LOCAL_DIFF_MAX_GROUPS,
    FUZZY_LOCAL_DIFF_MAX_GROUP_AREA_RATIO, FUZZY_LOCAL_DIFF_MAX_BBOX_AREA_RATIO,
    FUZZY_LOCAL_DIFF_MIN_FILL_RATIO, FUZZY_LOCAL_DIFF_BORDER_BAND_PX,
    FUZZY_LOCAL_DIFF_CLEAN_RATIO,
    FUZZY_FOCUSED_DIFF_MAX_AREA_RATIO, FUZZY_FOCUSED_DIFF_MAX_GROUPS,
    FUZZY_FOCUSED_DIFF_BORDER_CLEAN_MIN, FUZZY_FOCUSED_DIFF_CLEAN_RATIO,
    FUZZY_FOCUSED_DIFF_INSET_PX,
    WHITE_SOURCE_UNIFORM_MAX_DIFF,
    WHITE_SOURCE_RING_MIN_CONTRAST, WHITE_SOURCE_RING_MIN_RATIO, WHITE_SOURCE_RING_MIN_PIXELS,
    WHITE_SOURCE_COMPOSITE_MAX_DIFF, WHITE_SOURCE_COMPOSITE_MEDIAN_MAX,
    WHITE_SOURCE_COMPOSITE_RATIO_MIN, WHITE_SOURCE_COMPOSITE_RING_MIN_RATIO,
    WHITE_SOURCE_OVERLAY_MONO_MAX_DIFF, WHITE_SOURCE_OVERLAY_MONO_MIN_RATIO,
    WHITE_SOURCE_FUZZY_MAX_DIFF, WHITE_SOURCE_FUZZY_MEDIAN_MAX,
    WHITE_SOURCE_FUZZY_STRICT_RATIO_MIN, WHITE_SOURCE_FUZZY_STRICT_EDGE_RATIO_MIN,
    WHITE_SOURCE_FUZZY_STRICT_RING_MIN_RATIO,
    WHITE_SOURCE_FUZZY_INNER_RATIO_MIN, WHITE_SOURCE_FUZZY_INNER_EDGE_RATIO_MIN,
    WHITE_SOURCE_FUZZY_INNER_EDGE_MISMATCH_MAX, WHITE_SOURCE_FUZZY_INNER_MISMATCH_MAX,
    WHITE_SOURCE_FUZZY_INNER_RING_MIN_RATIO,
    WHITE_SOURCE_FUZZY_EDGE_RATIO_MIN, WHITE_SOURCE_FUZZY_EDGE_TOTAL_RATIO_MIN,
    WHITE_SOURCE_FUZZY_EDGE_INNER_RATIO_MIN, WHITE_SOURCE_FUZZY_EDGE_MISMATCH_MAX,
    WHITE_SOURCE_FUZZY_EDGE_GROUP_MAX_RATIO, WHITE_SOURCE_FUZZY_EDGE_RING_MIN_RATIO,
    WHITE_SOURCE_FUZZY_MAX_GROUPS,
    erode_mask, alpha_composite_onto,
    _build_border_support, _build_shape_ring, _build_shape_edge_inner_masks,
)

# ──────────────────── Mismatch Groups ────────────────────

def _extract_localized_mismatch_groups(mismatch_mask, valid_mask):
    mismatch_count = int(np.count_nonzero(mismatch_mask))
    valid_count = int(np.count_nonzero(valid_mask))
    if mismatch_count < MIN_MATCH_PIXELS or valid_count <= 0:
        return None, []
    if mismatch_count / valid_count > FUZZY_LOCAL_DIFF_MAX_AREA_RATIO:
        return None, []

    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(
        (mismatch_mask.astype(np.uint8) * 255), connectivity=8)
    max_group_pixels = max(MIN_MATCH_PIXELS, int(valid_count * FUZZY_LOCAL_DIFF_MAX_GROUP_AREA_RATIO))
    max_bbox_area = max(MIN_MATCH_PIXELS, int(valid_count * FUZZY_LOCAL_DIFF_MAX_BBOX_AREA_RATIO))
    h, w = valid_mask.shape
    border_band = max(1, min(FUZZY_LOCAL_DIFF_BORDER_BAND_PX, min(h, w) // 2))

    rescue_mask = np.zeros_like(mismatch_mask, dtype=bool)
    groups = []
    for label in range(1, num_labels):
        x, y, bw, bh, _ = stats[label]
        raw_mask = mismatch_mask & (labels == label)
        raw_pixels = int(np.count_nonzero(raw_mask))
        if raw_pixels <= 0:
            continue

        bbox_area = int(bw * bh)
        if raw_pixels > max_group_pixels or bbox_area > max_bbox_area:
            return None, []

        fill_ratio = raw_pixels / max(1, bbox_area)
        touches_border = (
            x <= border_band or y <= border_band
            or x + bw >= w - border_band or y + bh >= h - border_band
        )
        if not touches_border and fill_ratio < FUZZY_LOCAL_DIFF_MIN_FILL_RATIO:
            return None, []

        rescue_mask |= raw_mask
        groups.append({
            'x': int(x),
            'y': int(y),
            'width': int(bw),
            'height': int(bh),
            'pixels': raw_pixels,
            'fill_ratio': round(fill_ratio, 4),
            'touches_border': bool(touches_border),
        })

    if not groups or len(groups) > FUZZY_LOCAL_DIFF_MAX_GROUPS:
        return None, []

    return rescue_mask & valid_mask, groups


def _extract_focused_mismatch_groups(mismatch_mask, valid_mask):
    mismatch_count = int(np.count_nonzero(mismatch_mask))
    valid_count = int(np.count_nonzero(valid_mask))
    if mismatch_count < MIN_MATCH_PIXELS or valid_count <= 0:
        return None, []
    if mismatch_count / valid_count > FUZZY_FOCUSED_DIFF_MAX_AREA_RATIO:
        return None, []

    border_support = _build_border_support(~mismatch_mask & valid_mask, valid_mask)
    if border_support['total'] < FUZZY_FOCUSED_DIFF_BORDER_CLEAN_MIN:
        return None, []

    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(
        (mismatch_mask.astype(np.uint8) * 255), connectivity=8)
    h, w = valid_mask.shape
    inset = max(1, min(FUZZY_FOCUSED_DIFF_INSET_PX, min(h, w) // 4))

    rescue_mask = np.zeros_like(mismatch_mask, dtype=bool)
    groups = []
    for label in range(1, num_labels):
        x, y, bw, bh, _ = stats[label]
        raw_mask = mismatch_mask & (labels == label)
        raw_pixels = int(np.count_nonzero(raw_mask))
        if raw_pixels <= 0:
            continue

        touches_outer = (
            x < inset or y < inset
            or x + bw > w - inset or y + bh > h - inset
        )
        if touches_outer:
            return None, []

        bbox_area = int(bw * bh)
        fill_ratio = raw_pixels / max(1, bbox_area)
        rescue_mask |= raw_mask
        groups.append({
            'x': int(x),
            'y': int(y),
            'width': int(bw),
            'height': int(bh),
            'pixels': raw_pixels,
            'fill_ratio': round(fill_ratio, 4),
        })

    if not groups or len(groups) > FUZZY_FOCUSED_DIFF_MAX_GROUPS:
        return None, []

    return rescue_mask & valid_mask, groups

# ──────────────────── Region Analysis ────────────────────

def _analyze_region_match(region_bgr, expected_bgr, valid_mask, max_diff=DEFAULT_MAX_DIFF,
                          min_match_ratio=SOFT_MATCH_RATIO_MIN,
                          small_match_ratio=SOFT_SMALL_MATCH_RATIO_MIN,
                          allow_local_diff_rescue=False):
    valid_mask = valid_mask.astype(bool)
    valid_count = np.count_nonzero(valid_mask)
    result = {
        'accepted': False,
        'median_diff': 999.0,
        'match_ratio': 0.0,
        'valid_pixels': int(valid_count),
        'perfect': False,
        'local_diff_rescued': False,
        'local_diff_pixels': 0,
        'local_diff_groups': [],
        'focused_diff_rescued': False,
        'focused_diff_pixels': 0,
        'focused_diff_groups': [],
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
    if mismatch_count < MIN_MATCH_PIXELS:
        ratio_min = (small_match_ratio if valid_count < SOFT_SMALL_VALID_PIXELS_MAX
                     else min_match_ratio)
        if median_diff <= VERIFY_MEDIAN_MAX and match_ratio >= ratio_min:
            result['accepted'] = True
        return result

    if allow_local_diff_rescue:
        local_diff_mask, local_groups = _extract_localized_mismatch_groups(mismatch_mask, valid_mask)
        if local_diff_mask is not None:
            clean_mask = valid_mask & ~local_diff_mask
            clean_count = int(np.count_nonzero(clean_mask))
            if clean_count >= MIN_MATCH_PIXELS:
                clean_diffs = per_px_max[clean_mask]
                clean_median = float(np.median(clean_diffs))
                clean_ratio = float(np.count_nonzero(clean_diffs <= max_diff) / clean_count)
                if clean_median <= VERIFY_MEDIAN_MAX and clean_ratio >= FUZZY_LOCAL_DIFF_CLEAN_RATIO:
                    local_pixels = int(np.count_nonzero(local_diff_mask))
                    result.update({
                        'accepted': True,
                        'median_diff': clean_median,
                        'match_ratio': clean_ratio,
                        'valid_pixels': clean_count,
                        'perfect': False,
                        'local_diff_rescued': True,
                        'local_diff_pixels': local_pixels,
                        'local_diff_groups': local_groups,
                    })
                    return result

        focused_diff_mask, focused_groups = _extract_focused_mismatch_groups(mismatch_mask, valid_mask)
        if focused_diff_mask is not None:
            clean_mask = valid_mask & ~focused_diff_mask
            clean_count = int(np.count_nonzero(clean_mask))
            if clean_count >= MIN_MATCH_PIXELS:
                clean_diffs = per_px_max[clean_mask]
                clean_median = float(np.median(clean_diffs))
                clean_ratio = float(np.count_nonzero(clean_diffs <= max_diff) / clean_count)
                if clean_median <= VERIFY_MEDIAN_MAX and clean_ratio >= FUZZY_FOCUSED_DIFF_CLEAN_RATIO:
                    focused_pixels = int(np.count_nonzero(focused_diff_mask))
                    result.update({
                        'accepted': True,
                        'median_diff': clean_median,
                        'match_ratio': clean_ratio,
                        'valid_pixels': clean_count,
                        'perfect': False,
                        'focused_diff_rescued': True,
                        'focused_diff_pixels': focused_pixels,
                        'focused_diff_groups': focused_groups,
                    })
                    return result

    ratio_min = (small_match_ratio if valid_count < SOFT_SMALL_VALID_PIXELS_MAX
                 else min_match_ratio)
    if median_diff <= VERIFY_MEDIAN_MAX and match_ratio >= ratio_min:
        result['accepted'] = True
    return result

# ──────────────────── Pixel Verification ────────────────────

def verify_pixel_match(screenshot, template_bgr, alpha_mask, x, y, max_diff=DEFAULT_MAX_DIFF,
                       min_match_ratio=SOFT_MATCH_RATIO_MIN,
                       small_match_ratio=SOFT_SMALL_MATCH_RATIO_MIN,
                       allow_local_diff_rescue=False):
    th, tw = template_bgr.shape[:2]
    sh, sw = screenshot.shape[:2]
    if x < 0 or y < 0 or x + tw > sw or y + th > sh:
        return _analyze_region_match(np.zeros((th, tw, 3), dtype=np.uint8), template_bgr,
                                     np.zeros((th, tw), dtype=bool), max_diff=max_diff,
                                     min_match_ratio=min_match_ratio,
                                     small_match_ratio=small_match_ratio,
                                     allow_local_diff_rescue=allow_local_diff_rescue)
    region = screenshot[y:y + th, x:x + tw]
    vmask = erode_mask(alpha_mask)
    if vmask is None:
        vmask = np.ones((th, tw), dtype=bool)
    return _analyze_region_match(region, template_bgr, vmask, max_diff=max_diff,
                                 min_match_ratio=min_match_ratio,
                                 small_match_ratio=small_match_ratio,
                                 allow_local_diff_rescue=allow_local_diff_rescue)

# ──────────────────── Shape Mismatch Groups ────────────────────

def _collect_shape_mismatch_groups(mismatch_mask, edge_mask, inner_mask, total_count, edge_count):
    groups = []
    if not np.any(mismatch_mask):
        return groups
    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(
        mismatch_mask.astype(np.uint8), connectivity=8)
    for label in range(1, num_labels):
        x, y, w, h, area = stats[label]
        if area <= 0 or w <= 0 or h <= 0:
            continue
        comp_mask = labels == label
        edge_pixels = int(np.count_nonzero(comp_mask & edge_mask))
        inner_pixels = int(np.count_nonzero(comp_mask & inner_mask))
        groups.append({
            'pixels': int(area),
            'edge_pixels': edge_pixels,
            'inner_pixels': inner_pixels,
            'bbox_area': int(w * h),
            'area_ratio': float(area / max(1, total_count)),
            'edge_ratio': float(edge_pixels / max(1, edge_count)),
        })
    groups.sort(key=lambda g: (-g['pixels'], -g['edge_pixels']))
    return groups

# ──────────────────── Solid Shape Analysis ────────────────────

def _analyze_fuzzy_solid_shape_region(region_bgr, base_mask, visible_mask=None):
    base_mask = base_mask.astype(bool)
    if visible_mask is None:
        visible_mask = base_mask.copy()
    else:
        visible_mask = visible_mask.astype(bool) & base_mask
    visible_count = int(np.count_nonzero(visible_mask))
    result = {
        'accepted': False,
        'median_diff': 999.0,
        'match_ratio': 0.0,
        'valid_pixels': visible_count,
        'perfect': False,
        'tint_color': None,
        'white_fuzzy_mode': None,
        'mode_score': -1.0,
        'edge_match_ratio': 0.0,
        'inner_match_ratio': 0.0,
        'ring_ratio': 0.0,
    }
    if visible_count < MIN_MATCH_PIXELS:
        return result

    edge_mask, inner_mask = _build_shape_edge_inner_masks(base_mask)
    edge_visible = edge_mask & visible_mask
    inner_visible = inner_mask & visible_mask
    edge_count = int(np.count_nonzero(edge_visible))
    inner_count = int(np.count_nonzero(inner_visible))

    tint_mask = edge_visible if edge_count >= MIN_MATCH_PIXELS else visible_mask
    fg_pixels = region_bgr[tint_mask].astype(np.float32)
    if fg_pixels.size <= 0:
        return result
    tint_bgr = np.median(fg_pixels, axis=0).round().astype(np.uint8)
    diff = np.abs(region_bgr.astype(np.int16) - tint_bgr.astype(np.int16)).max(axis=2)
    good_mask = diff <= WHITE_SOURCE_FUZZY_MAX_DIFF
    visible_good = visible_mask & good_mask
    visible_diffs = diff[visible_mask]
    if visible_diffs.size <= 0:
        return result

    match_ratio = float(np.count_nonzero(visible_good) / visible_count)
    median_diff = float(np.median(visible_diffs))
    edge_match_ratio = float(np.count_nonzero(edge_visible & good_mask) / max(1, edge_count))
    inner_match_ratio = float(np.count_nonzero(inner_visible & good_mask) / max(1, inner_count)) if inner_count > 0 else match_ratio
    mismatch_mask = visible_mask & ~good_mask
    mismatch_count = int(np.count_nonzero(mismatch_mask))
    edge_mismatch_pixels = int(np.count_nonzero(edge_visible & ~good_mask))
    inner_mismatch_pixels = int(np.count_nonzero(inner_visible & ~good_mask))
    edge_mismatch_ratio = float(edge_mismatch_pixels / max(1, edge_count))
    inner_mismatch_ratio = float(inner_mismatch_pixels / max(1, inner_count)) if inner_count > 0 else 0.0
    ring_mask = _build_shape_ring(base_mask)
    ring_ratio = 0.0
    if ring_mask is not None:
        ring_pixels = diff[ring_mask]
        if ring_pixels.size >= WHITE_SOURCE_RING_MIN_PIXELS:
            ring_ratio = float(np.count_nonzero(ring_pixels >= WHITE_SOURCE_RING_MIN_CONTRAST) / ring_pixels.size)

    groups = _collect_shape_mismatch_groups(mismatch_mask, edge_visible, inner_visible, visible_count, edge_count)
    group_count = len(groups)
    max_edge_group_ratio = max((g['edge_ratio'] for g in groups), default=0.0)

    result.update({
        'median_diff': median_diff,
        'match_ratio': match_ratio,
        'perfect': bool(mismatch_count == 0),
        'tint_color': [int(tint_bgr[2]), int(tint_bgr[1]), int(tint_bgr[0])],
        'edge_match_ratio': edge_match_ratio,
        'inner_match_ratio': inner_match_ratio,
        'ring_ratio': ring_ratio,
    })

    mode_candidates = []

    def _append_mode(mode_name, score):
        mode_candidates.append((score, mode_name))

    if (
        median_diff <= WHITE_SOURCE_FUZZY_MEDIAN_MAX
        and match_ratio >= WHITE_SOURCE_FUZZY_STRICT_RATIO_MIN
        and edge_match_ratio >= WHITE_SOURCE_FUZZY_STRICT_EDGE_RATIO_MIN
        and ring_ratio >= WHITE_SOURCE_FUZZY_STRICT_RING_MIN_RATIO
    ):
        strict_score = edge_match_ratio * 4.0 + match_ratio * 3.0 + ring_ratio
        _append_mode('strict', strict_score)

    if (
        median_diff <= WHITE_SOURCE_FUZZY_MEDIAN_MAX
        and match_ratio >= WHITE_SOURCE_FUZZY_INNER_RATIO_MIN
        and edge_match_ratio >= WHITE_SOURCE_FUZZY_INNER_EDGE_RATIO_MIN
        and edge_mismatch_ratio <= WHITE_SOURCE_FUZZY_INNER_EDGE_MISMATCH_MAX
        and inner_mismatch_ratio <= WHITE_SOURCE_FUZZY_INNER_MISMATCH_MAX
        and ring_ratio >= WHITE_SOURCE_FUZZY_INNER_RING_MIN_RATIO
        and group_count <= WHITE_SOURCE_FUZZY_MAX_GROUPS
    ):
        inner_score = (
            edge_match_ratio * 4.0
            + match_ratio * 2.0
            + inner_match_ratio
            + ring_ratio * 0.5
            - edge_mismatch_ratio * 2.0
            - group_count * 0.03
        )
        _append_mode('inner_occlusion', inner_score)

    if (
        median_diff <= WHITE_SOURCE_FUZZY_MEDIAN_MAX
        and match_ratio >= WHITE_SOURCE_FUZZY_EDGE_TOTAL_RATIO_MIN
        and edge_match_ratio >= WHITE_SOURCE_FUZZY_EDGE_RATIO_MIN
        and inner_match_ratio >= WHITE_SOURCE_FUZZY_EDGE_INNER_RATIO_MIN
        and edge_mismatch_ratio <= WHITE_SOURCE_FUZZY_EDGE_MISMATCH_MAX
        and max_edge_group_ratio <= WHITE_SOURCE_FUZZY_EDGE_GROUP_MAX_RATIO
        and ring_ratio >= WHITE_SOURCE_FUZZY_EDGE_RING_MIN_RATIO
        and group_count <= WHITE_SOURCE_FUZZY_MAX_GROUPS
    ):
        edge_score = (
            inner_match_ratio * 3.0
            + edge_match_ratio * 2.5
            + match_ratio * 1.5
            + ring_ratio * 0.5
            - edge_mismatch_ratio
            - max_edge_group_ratio
            - group_count * 0.03
        )
        _append_mode('edge_occlusion', edge_score)

    if mode_candidates:
        best_score, best_mode = max(mode_candidates, key=lambda item: item[0])
        result['accepted'] = True
        result['white_fuzzy_mode'] = best_mode
        result['mode_score'] = float(best_score)

    return result


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


def _analyze_composited_solid_shape_region(region_bgr, base_mask, overlay_crop, exclusion_mask=None):
    base_mask = base_mask.astype(bool)
    overlay_alpha = overlay_crop[:, :, 3] > 0 if overlay_crop.shape[2] == 4 else np.zeros(base_mask.shape, dtype=bool)
    visible_mask = base_mask & ~overlay_alpha
    valid_mask = base_mask | overlay_alpha
    if exclusion_mask is not None:
        exclusion_mask = exclusion_mask.astype(bool)
        visible_mask = visible_mask & ~exclusion_mask
        valid_mask = valid_mask & ~exclusion_mask
    valid_count = int(np.count_nonzero(valid_mask))
    result = {
        'accepted': False,
        'median_diff': 999.0,
        'match_ratio': 0.0,
        'valid_pixels': valid_count,
        'perfect': False,
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

# ──────────────────── Public Verify Functions ────────────────────

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


def verify_solid_shape_fuzzy_match(screenshot, alpha_mask, x, y):
    th, tw = alpha_mask.shape[:2]
    sh, sw = screenshot.shape[:2]
    if x < 0 or y < 0 or x + tw > sw or y + th > sh:
        return _analyze_fuzzy_solid_shape_region(
            np.zeros((th, tw, 3), dtype=np.uint8),
            np.zeros((th, tw), dtype=bool),
            np.zeros((th, tw), dtype=bool),
        )
    region = screenshot[y:y + th, x:x + tw]
    base_mask = alpha_mask.astype(bool)
    return _analyze_fuzzy_solid_shape_region(region, base_mask, base_mask)


def composite_verify_solid_shape(alpha_mask, px, py, tw, th, overlay, screenshot, exclusion_mask=None):
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
        return _analyze_composited_solid_shape_region(region, base_mask, overlay_crop, exclusion_mask=exclusion_mask)
    visible_mask = erode_mask(base_mask)
    if visible_mask is None:
        visible_mask = base_mask
    if exclusion_mask is not None:
        exclusion_mask = exclusion_mask.astype(bool)
        visible_mask = visible_mask & ~exclusion_mask
        base_mask = base_mask & ~exclusion_mask
    return _analyze_solid_shape_region(region, visible_mask, base_mask)


def composite_verify_solid_shape_fuzzy(alpha_mask, px, py, tw, th, overlay, screenshot, exclusion_mask=None):
    sh, sw = screenshot.shape[:2]
    if px < 0 or py < 0 or px + tw > sw or py + th > sh:
        return _analyze_fuzzy_solid_shape_region(
            np.zeros((th, tw, 3), dtype=np.uint8),
            np.zeros((th, tw), dtype=bool),
            np.zeros((th, tw), dtype=bool),
        )
    region = screenshot[py:py + th, px:px + tw]
    overlay_crop = overlay[py:py + th, px:px + tw]
    base_mask = alpha_mask.astype(bool)
    if overlay_crop.shape[2] == 4 and np.any(overlay_crop[:, :, 3] > 0):
        if not _overlay_is_monochrome(overlay_crop):
            return _analyze_fuzzy_solid_shape_region(
                np.zeros((th, tw, 3), dtype=np.uint8),
                np.zeros((th, tw), dtype=bool),
                np.zeros((th, tw), dtype=bool),
            )
        visible_mask = base_mask & ~(overlay_crop[:, :, 3] > 0)
    else:
        visible_mask = base_mask.copy()
    if exclusion_mask is not None:
        exclusion_mask = exclusion_mask.astype(bool)
        visible_mask = visible_mask & ~exclusion_mask
    return _analyze_fuzzy_solid_shape_region(region, base_mask, visible_mask)

# ──────────────────── Composite Verification ────────────────────

def composite_verify(template_bgr, alpha_mask, px, py, tw, th, overlay, screenshot,
                     max_diff=DEFAULT_MAX_DIFF,
                     min_match_ratio=SOFT_MATCH_RATIO_MIN,
                     small_match_ratio=SOFT_SMALL_MATCH_RATIO_MIN,
                     exclusion_mask=None,
                     allow_local_diff_rescue=False):
    sh, sw = screenshot.shape[:2]
    if px < 0 or py < 0 or px + tw > sw or py + th > sh:
        return _analyze_region_match(np.zeros((th, tw, 3), dtype=np.uint8), template_bgr,
                                     np.zeros((th, tw), dtype=bool), max_diff=max_diff,
                                     min_match_ratio=min_match_ratio,
                                     small_match_ratio=small_match_ratio,
                                     allow_local_diff_rescue=allow_local_diff_rescue)
    canvas = template_bgr.copy()
    overlay_crop = overlay[py:py + th, px:px + tw]
    alpha_composite_onto(canvas, overlay_crop, 0, 0)
    crop = screenshot[py:py + th, px:px + tw]
    vmask = erode_mask(alpha_mask) if alpha_mask is not None else np.ones((th, tw), dtype=bool)
    overlay_alpha = overlay_crop[:, :, 3] if overlay_crop.shape[2] == 4 else np.zeros((th, tw), np.uint8)
    visible = vmask & (overlay_alpha < 250)
    if exclusion_mask is not None:
        visible = visible & ~exclusion_mask.astype(bool)
    return _analyze_region_match(crop, canvas, visible, max_diff=max_diff,
                                 min_match_ratio=min_match_ratio,
                                 small_match_ratio=small_match_ratio,
                                 allow_local_diff_rescue=allow_local_diff_rescue)

# ──────────────────── Evaluate Fuzzy Candidate ────────────────────

def _evaluate_fuzzy_candidate(screenshot, tpl, amask, px, py,
                              min_ratio=FUZZY_MATCH_RATIO_MIN,
                              small_ratio=FUZZY_SMALL_MATCH_RATIO_MIN,
                              is_white_source=False):
    th, tw = tpl.shape[:2]
    cand_r = {'x': px, 'y': py, 'width': tw, 'height': th}
    if is_white_source:
        verify = verify_solid_shape_fuzzy_match(screenshot, amask, px, py)
    else:
        verify = verify_pixel_match(
            screenshot, tpl, amask, px, py, max_diff=FUZZY_MAX_DIFF,
            min_match_ratio=min_ratio, small_match_ratio=small_ratio,
            allow_local_diff_rescue=True)
    return cand_r, verify
