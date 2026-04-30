"""match_9slice — 9-slice sprite matching (unified scoring)."""

import cv2
import numpy as np
import os
import time

from matcher_core import (
    MIN_MATCH_PIXELS, MIN_TEMPLATE_DIM, DEFAULT_MAX_DIFF,
    NINESLICE_EDGE_SAMPLE_SPAN, NINESLICE_MAX_SIZE_CANDIDATES,
    FUZZY_MAX_DIFF, FUZZY_MATCH_RATIO_MIN, FUZZY_SMALL_MATCH_RATIO_MIN,
    STAGE_9SLICE,
    DupPosIndex,
    _is_dup_pos,
    _should_accept_fuzzy_majority,
    build_fuzzy_exclusion_mask,
    collect_candidate_peaks, make_mask_3ch,
    _write_stage_progress, _write_stage_done,
)
from matcher_loader import prepare_template, get_9slice_target_min_size
from matcher_verify import composite_verify

# ──────────────────── 9-slice Helpers ────────────────────

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

# ──────────────────── 9-slice Matching (unified exact + fuzzy) ────────────────────

def _match_9slice(sprites, sliced_indices, screenshot, overlay, existing):
    sh, sw = screenshot.shape[:2]
    matches = []
    total = len(sliced_indices)
    t_start = time.time()
    stage_idx, stage_label = STAGE_9SLICE

    dup_index = DupPosIndex()
    dup_index.add_all(existing)

    for si, sidx in enumerate(sliced_indices):
        sp = sprites[sidx]
        border = sp['border']
        if not border:
            continue
        if sp.get('is_white_source'):
            continue
        name = os.path.basename(sp['rel_path'])
        _write_stage_progress(stage_idx, stage_label, si + 1, total, name, len(matches))
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
                if dup_index.has_dup(tl_x, tl_y, rw, rh):
                    continue
                exclusion_mask = build_fuzzy_exclusion_mask(existing, tl_x, tl_y, rw, rh)
                tpl, amask = prepare_template(sp['img'], rw, rh, border=border)

                verify = composite_verify(
                    tpl, amask, tl_x, tl_y, rw, rh, overlay, screenshot,
                    max_diff=FUZZY_MAX_DIFF,
                    min_match_ratio=FUZZY_MATCH_RATIO_MIN,
                    small_match_ratio=FUZZY_SMALL_MATCH_RATIO_MIN,
                    exclusion_mask=exclusion_mask,
                    allow_local_diff_rescue=True)
                if not _should_accept_fuzzy_majority(score, verify):
                    continue
                match_entry = {
                    'sprite_idx': sidx,
                    'is_white': False,
                    'is_9slice': True,
                    'rect': {'x': tl_x, 'y': tl_y, 'width': rw, 'height': rh},
                    'match_ratio': round(verify['match_ratio'], 6),
                    'median_diff': round(verify['median_diff'], 2),
                    'perfect': bool(verify['perfect']),
                    'valid_pixels': verify['valid_pixels'],
                    'scale': round(rw / sp['w'], 4),
                    'candidate_score': round(float(score), 6),
                    'rescue_candidate': bool(rescue_candidate),
                    'majority_rescued': bool(verify.get('majority_rescued', False)),
                    'fuzzy_match': not verify.get('perfect', False),
                }
                matches.append(match_entry)
                dup_index.add(match_entry)

    elapsed = time.time() - t_start
    _write_stage_done(stage_idx, stage_label, total, elapsed)
    return matches
