"""match_normal — Normal sprite matching (unified scoring across all scales)."""

import cv2
import numpy as np
import os
import time

from matcher_core import (
    MIN_MATCH_PIXELS, MIN_TEMPLATE_DIM,
    FUZZY_MAX_DIFF, FUZZY_MATCH_RATIO_MIN, FUZZY_SMALL_MATCH_RATIO_MIN,
    FUZZY_MAX_CANDIDATES, FUZZY_REFINE_SCALE_DELTAS, FUZZY_REFINE_POS_DELTAS,
    FUZZY_REFINE_MIN_RATIO, FUZZY_REFINE_MAX_PEAKS,
    WHITE_DIRECT_COMPONENT_THRESHOLD,
    WHITE_DIRECT_MIN_OPAQUE_PIXELS,
    STAGE_NORMAL,
    DupPosIndex,
    _is_dup_pos, _fuzzy_rank, _should_accept_fuzzy_majority,
    _is_central_exclusion_hole,
    build_fuzzy_exclusion_mask,
    collect_candidate_peaks,
    collect_component_candidate_peaks,
    make_mask_3ch,
    _write_stage_progress, _write_stage_done,
)
from matcher_loader import prepare_template, normalize_match_scales
from matcher_verify import (
    composite_verify, composite_verify_solid_shape_fuzzy,
)


def _refine_normal_fuzzy_candidate(sprite, screenshot, overlay, existing,
                                   base_x, base_y, base_tw, base_th,
                                   base_scale):
    sh, sw = screenshot.shape[:2]
    is_white_source = bool(sprite.get('is_white_source'))
    center_x = base_x + base_tw / 2.0
    center_y = base_y + base_th / 2.0
    template_cache = {}
    best = None
    best_rank = None

    for delta in FUZZY_REFINE_SCALE_DELTAS:
        scale = round(base_scale + delta, 4)
        if scale <= 0:
            continue
        tw = max(1, round(sprite['w'] * scale))
        th = max(1, round(sprite['h'] * scale))
        if tw < MIN_TEMPLATE_DIM or th < MIN_TEMPLATE_DIM or tw > sw or th > sh:
            continue

        cache_key = (tw, th)
        if cache_key not in template_cache:
            tpl, amask = prepare_template(sprite['img'], tw, th)
            if amask is not None and np.count_nonzero(amask) < MIN_MATCH_PIXELS:
                template_cache[cache_key] = None
            else:
                template_cache[cache_key] = (tpl, amask)
        cached = template_cache[cache_key]
        if cached is None:
            continue
        tpl, amask = cached

        anchor_x = int(round(center_x - tw / 2.0))
        anchor_y = int(round(center_y - th / 2.0))
        for dx in FUZZY_REFINE_POS_DELTAS:
            for dy in FUZZY_REFINE_POS_DELTAS:
                px = anchor_x + dx
                py = anchor_y + dy
                if px < 0 or py < 0 or px + tw > sw or py + th > sh:
                    continue
                cand_r = {'x': px, 'y': py, 'width': tw, 'height': th}
                exclusion_mask = build_fuzzy_exclusion_mask(existing, px, py, tw, th)
                if is_white_source and _is_central_exclusion_hole(amask, exclusion_mask):
                    continue
                if is_white_source:
                    verify = composite_verify_solid_shape_fuzzy(
                        amask, px, py, tw, th, overlay, screenshot,
                        exclusion_mask=exclusion_mask)
                else:
                    verify = composite_verify(
                        tpl, amask, px, py, tw, th, overlay, screenshot,
                        max_diff=FUZZY_MAX_DIFF,
                        min_match_ratio=FUZZY_MATCH_RATIO_MIN,
                        small_match_ratio=FUZZY_SMALL_MATCH_RATIO_MIN,
                        exclusion_mask=exclusion_mask,
                        allow_local_diff_rescue=True)
                rank = _fuzzy_rank(verify)
                if best is None or rank > best_rank:
                    best = {
                        'rect': cand_r,
                        'verify': verify,
                        'scale': scale,
                        'alpha_mask': amask,
                        'exclusion_mask': exclusion_mask,
                    }
                    best_rank = rank
    return best


def _match_normal(sprites, sliced_indices, screenshot, overlay, existing,
                  fuzzy_scales=None):
    sh, sw = screenshot.shape[:2]
    if overlay is None:
        overlay = np.zeros((sh, sw, 4), dtype=np.uint8)
    matches = []
    total = len(sprites)
    t_start = time.time()
    stage_idx, stage_label = STAGE_NORMAL
    fuzzy_scales = normalize_match_scales(fuzzy_scales)
    all_scales = [1.0] + [s for s in fuzzy_scales if s != 1.0]

    dup_index = DupPosIndex()
    dup_index.add_all(existing)

    for sidx, sp in enumerate(sprites):
        name = os.path.basename(sp['rel_path'])
        _write_stage_progress(stage_idx, stage_label, sidx + 1, total, name, len(matches))

        is_white_source = bool(sp.get('is_white_source'))
        if is_white_source and sp.get('opaque_pixels', 0) < WHITE_DIRECT_MIN_OPAQUE_PIXELS:
            continue
        has_border = bool(sp.get('border'))
        if has_border and not is_white_source:
            continue

        has_perfect = False
        for scale in all_scales:
            if has_perfect and scale != 1.0:
                break
            tw = max(1, round(sp['w'] * scale))
            th = max(1, round(sp['h'] * scale))
            if tw < MIN_TEMPLATE_DIM or th < MIN_TEMPLATE_DIM or tw > sw or th > sh:
                continue

            tpl, amask = prepare_template(sp['img'], tw, th)
            if amask is not None and np.count_nonzero(amask) < MIN_MATCH_PIXELS:
                continue

            mask_3ch = make_mask_3ch(amask)
            if mask_3ch is not None:
                res = cv2.matchTemplate(screenshot, tpl, cv2.TM_CCORR_NORMED, mask=mask_3ch)
            else:
                res = cv2.matchTemplate(screenshot, tpl, cv2.TM_CCORR_NORMED)

            if is_white_source:
                peaks = collect_component_candidate_peaks(
                    res, WHITE_DIRECT_COMPONENT_THRESHOLD,
                    max_peaks=FUZZY_MAX_CANDIDATES)
            else:
                peaks = collect_candidate_peaks(
                    res, tw, th, max_peaks=FUZZY_MAX_CANDIDATES)

            for peak_idx, (cy, cx, score, rescue_candidate) in enumerate(peaks):
                px, py = int(cx), int(cy)
                if dup_index.has_dup(px, py, tw, th):
                    continue
                cand_r = {'x': px, 'y': py, 'width': tw, 'height': th}
                exclusion_mask = build_fuzzy_exclusion_mask(existing, px, py, tw, th)
                if is_white_source and _is_central_exclusion_hole(amask, exclusion_mask):
                    continue

                if is_white_source:
                    verify = composite_verify_solid_shape_fuzzy(
                        amask, px, py, tw, th, overlay, screenshot,
                        exclusion_mask=exclusion_mask)
                else:
                    verify = composite_verify(
                        tpl, amask, px, py, tw, th, overlay, screenshot,
                        max_diff=FUZZY_MAX_DIFF,
                        min_match_ratio=FUZZY_MATCH_RATIO_MIN,
                        small_match_ratio=FUZZY_SMALL_MATCH_RATIO_MIN,
                        exclusion_mask=exclusion_mask,
                        allow_local_diff_rescue=True)

                best_rect = cand_r
                best_verify = verify
                best_scale = scale
                best_mask = amask
                best_exclusion_mask = exclusion_mask

                if peak_idx < FUZZY_REFINE_MAX_PEAKS and (
                        verify['accepted']
                        or verify['match_ratio'] >= FUZZY_REFINE_MIN_RATIO):
                    refined = _refine_normal_fuzzy_candidate(
                        sp, screenshot, overlay, existing + matches,
                        px, py, tw, th, scale)
                    if refined is not None and _fuzzy_rank(refined['verify']) > _fuzzy_rank(best_verify):
                        best_rect = refined['rect']
                        best_verify = refined['verify']
                        best_scale = refined['scale']
                        best_mask = refined.get('alpha_mask', best_mask)
                        best_exclusion_mask = refined.get('exclusion_mask')

                if dup_index.has_dup(best_rect['x'], best_rect['y'], best_rect['width'], best_rect['height']):
                    continue

                if is_white_source:
                    if not best_verify.get('accepted'):
                        continue
                else:
                    if not _should_accept_fuzzy_majority(score, best_verify):
                        continue

                match_entry = {
                    'sprite_idx': sidx,
                    'white_idx': sp.get('white_idx', -1) if is_white_source else -1,
                    'is_white': is_white_source,
                    'is_9slice': False,
                    'rect': best_rect,
                    'match_ratio': round(best_verify['match_ratio'], 6),
                    'median_diff': round(best_verify['median_diff'], 2),
                    'perfect': bool(best_verify['perfect']),
                    'valid_pixels': best_verify['valid_pixels'],
                    'scale': round(best_scale, 4),
                    'candidate_score': round(float(score), 6),
                    'rescue_candidate': bool(rescue_candidate),
                    'tint_color': best_verify.get('tint_color'),
                    'majority_rescued': bool(best_verify.get('majority_rescued', False)),
                    'white_fuzzy_mode': best_verify.get('white_fuzzy_mode'),

                    'fuzzy_match': best_scale != 1.0 or not best_verify.get('perfect', False),
                }
                matches.append(match_entry)
                dup_index.add(match_entry)
                if best_verify.get('perfect') and scale == 1.0:
                    has_perfect = True
    elapsed = time.time() - t_start
    _write_stage_done(stage_idx, stage_label, total, elapsed)
    return matches
