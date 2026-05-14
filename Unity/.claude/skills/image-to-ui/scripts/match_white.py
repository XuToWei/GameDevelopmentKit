"""match_white — White sprite matching (exact + fuzzy)."""

import cv2
import numpy as np
import os
import time

from matcher_core import (
    MIN_MATCH_PIXELS, MIN_TEMPLATE_DIM, DEFAULT_MAX_DIFF,
    FUZZY_MAX_DIFF, FUZZY_MATCH_RATIO_MIN, FUZZY_SMALL_MATCH_RATIO_MIN,
    WHITE_FUZZY_MIN_OCCLUSION_RATIO,
    SOFT_MATCH_RATIO_MIN, SOFT_SMALL_MATCH_RATIO_MIN,
    ICON_LIKE_MATCH_RATIO_MIN, ICON_LIKE_SMALL_MATCH_RATIO_MIN,
    STAGE_WHITE,
    _is_icon_like_sprite,
    _should_accept_fuzzy_majority, _compute_candidate_occlusion_ratio,
    _is_large_uniform_visible_region,
    build_fuzzy_exclusion_mask,
    _write_stage_progress, _write_stage_done,
)
from matcher_loader import resize_sprite, get_9slice_target_min_size
from matcher_verify import composite_verify

# ──────────────────── White Helpers ────────────────────

def _estimate_dominant_bgr(region, mask):
    pixels = region[mask]
    if pixels.size == 0:
        return None
    return np.median(pixels.astype(np.float32), axis=0).round().astype(np.uint8)


def _build_uncovered_contours(screenshot, existing):
    sh, sw = screenshot.shape[:2]
    covered = np.zeros((sh, sw), dtype=bool)
    for e in existing:
        r = e['rect']
        x1, y1 = max(0, r['x']), max(0, r['y'])
        x2 = min(sw, r['x'] + r['width'])
        y2 = min(sh, r['y'] + r['height'])
        covered[y1:y2, x1:x2] = True
    uncovered = ~covered
    if np.count_nonzero(uncovered) < MIN_MATCH_PIXELS:
        return uncovered, []
    uncov_u8 = uncovered.astype(np.uint8) * 255
    contours, _ = cv2.findContours(uncov_u8, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    return uncovered, contours


# ──────────────────── White Matching ────────────────────

def _match_white(white_sprites, screenshot, overlay, existing,
                 max_diff=DEFAULT_MAX_DIFF, uncovered_cache=None):
    sh, sw = screenshot.shape[:2]
    if not white_sprites:
        return []

    if uncovered_cache is not None:
        uncovered, contours = uncovered_cache
    else:
        uncovered, contours = _build_uncovered_contours(screenshot, existing)
    if not contours:
        return []

    total_contours = len(contours)
    t_start = time.time()
    matches = []
    stage_idx, stage_label = STAGE_WHITE
    for ci, contour in enumerate(contours):
        _write_stage_progress(
            stage_idx, stage_label, ci + 1, total_contours, 'region', len(matches))
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
            is_icon_like = _is_icon_like_sprite(wsp)
            min_ratio = ICON_LIKE_MATCH_RATIO_MIN if is_icon_like else SOFT_MATCH_RATIO_MIN
            small_ratio = ICON_LIKE_SMALL_MATCH_RATIO_MIN if is_icon_like else SOFT_SMALL_MATCH_RATIO_MIN
            if wsp['border']:
                min_w, min_h = get_9slice_target_min_size(wsp['img'], wsp['border'])
                if w < min_w or h < min_h:
                    continue
                resized = resize_sprite(wsp, w, h)
            else:
                if w != wsp['w'] or h != wsp['h']:
                    continue
                resized = wsp['img']
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
            exclusion_mask = build_fuzzy_exclusion_mask(existing, x, y, w, h)
            verify = composite_verify(
                tpl_bgr, check, x, y, w, h, overlay, screenshot, max_diff=max_diff,
                min_match_ratio=min_ratio, small_match_ratio=small_ratio,
                exclusion_mask=exclusion_mask)
            if not verify['accepted']:
                continue

            cov = cc / np.count_nonzero(rect_mask)
            cand = (
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
                }
        if best:
            best.pop('_rank', None)
            matches.append(best)
    elapsed = time.time() - t_start
    _write_stage_done(stage_idx, stage_label, total_contours, elapsed)
    return matches


def _match_white_fuzzy(white_sprites, screenshot, overlay, existing,
                       uncovered_cache=None):
    sh, sw = screenshot.shape[:2]
    if not white_sprites:
        return []
    if overlay is None:
        overlay = np.zeros((sh, sw, 4), dtype=np.uint8)
    stage_idx, stage_label = STAGE_WHITE

    if uncovered_cache is not None:
        uncovered, contours = uncovered_cache
    else:
        uncovered, contours = _build_uncovered_contours(screenshot, existing)
    if not contours:
        return []

    total_contours = len(contours)
    t_start = time.time()
    matches = []

    for ci, contour in enumerate(contours):
        _write_stage_progress(
            stage_idx, stage_label, ci + 1, total_contours, 'region', len(matches))
        x, y, w, h = cv2.boundingRect(contour)
        if w < MIN_TEMPLATE_DIM or h < MIN_TEMPLATE_DIM:
            continue
        if cv2.contourArea(contour) < MIN_MATCH_PIXELS:
            continue

        rect_mask = uncovered[y:y + h, x:x + w]
        if np.count_nonzero(rect_mask) < MIN_MATCH_PIXELS:
            continue
        cand_r = {'x': x, 'y': y, 'width': w, 'height': h}
        region = screenshot[y:y + h, x:x + w]
        exclusion_mask = build_fuzzy_exclusion_mask(existing, x, y, w, h)

        best = None
        for widx, wsp in enumerate(white_sprites):
            if wsp['border']:
                min_w, min_h = get_9slice_target_min_size(wsp['img'], wsp['border'])
                if w < min_w or h < min_h:
                    continue
                resized = resize_sprite(wsp, w, h)
            else:
                if w != wsp['w'] or h != wsp['h']:
                    continue
                resized = wsp['img']

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

            verify = composite_verify(
                tpl_bgr, check, x, y, w, h, overlay, screenshot, max_diff=FUZZY_MAX_DIFF,
                min_match_ratio=FUZZY_MATCH_RATIO_MIN,
                small_match_ratio=FUZZY_SMALL_MATCH_RATIO_MIN,
                exclusion_mask=exclusion_mask,
                allow_local_diff_rescue=True)
            occlusion_ratio = _compute_candidate_occlusion_ratio(
                check, overlay, x, y, w, h, exclusion_mask=exclusion_mask)

            if exclusion_mask is None:
                visible_mask = check
            else:
                visible_mask = check & ~exclusion_mask.astype(bool)
            if _is_large_uniform_visible_region(region, visible_mask) and occlusion_ratio < WHITE_FUZZY_MIN_OCCLUSION_RATIO:
                continue
            if not _should_accept_fuzzy_majority(1.0, verify):
                continue

            rank = (
                1 if verify.get('majority_rescued') else 0,
                1 if verify.get('local_diff_rescued') or verify.get('focused_diff_rescued') else 0,
                round(verify['median_diff'], 2),
                -round(verify['match_ratio'], 6),
            )
            if best is None or rank < best['_rank']:
                tint_color = [int(fill_bgr[2]), int(fill_bgr[1]), int(fill_bgr[0])]
                best = {
                    '_rank': rank,
                    'white_idx': widx,
                    'sprite_idx': wsp.get('base_sprite_idx', -1),
                    'is_white': True,
                    'is_9slice': wsp['border'] is not None,
                    'rect': cand_r,
                    'match_ratio': round(verify['match_ratio'], 6),
                    'median_diff': round(verify['median_diff'], 2),
                    'perfect': bool(verify['perfect']),
                    'valid_pixels': verify['valid_pixels'],
                    'scale': round(w / wsp['w'], 4),
                    'candidate_score': 1.0,
                    'rescue_candidate': False,
                    'tint_color': tint_color,
                    'majority_rescued': bool(verify.get('majority_rescued', False)),

                    'fuzzy_match': True,
                }
        if best:
            best.pop('_rank', None)
            matches.append(best)

    elapsed = time.time() - t_start
    _write_stage_done(stage_idx, stage_label, total_contours, elapsed)
    return matches
