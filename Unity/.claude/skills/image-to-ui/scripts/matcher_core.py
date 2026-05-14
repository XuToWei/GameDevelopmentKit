"""matcher_core — Constants + pure utility functions for sprite matching."""

import sys
import cv2
import numpy as np

# ──────────────────── Constants ────────────────────

MIN_MATCH_PIXELS = 16
WHITE_COLOR_STD_MAX = 3.0
CORR_THRESHOLD = 0.85
VERIFY_MEDIAN_MAX = 20
DEFAULT_MAX_DIFF = 40
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
WHITE_SOURCE_FUZZY_MAX_DIFF = 14
WHITE_SOURCE_FUZZY_MEDIAN_MAX = 8
WHITE_SOURCE_FUZZY_EDGE_PX = 1
WHITE_SOURCE_FUZZY_STRICT_RATIO_MIN = 0.96
WHITE_SOURCE_FUZZY_STRICT_EDGE_RATIO_MIN = 0.95
WHITE_SOURCE_FUZZY_STRICT_RING_MIN_RATIO = 0.55
WHITE_SOURCE_FUZZY_INNER_RATIO_MIN = 0.7
WHITE_SOURCE_FUZZY_INNER_EDGE_RATIO_MIN = 0.92
WHITE_SOURCE_FUZZY_INNER_EDGE_MISMATCH_MAX = 0.1
WHITE_SOURCE_FUZZY_INNER_MISMATCH_MAX = 0.45
WHITE_SOURCE_FUZZY_INNER_RING_MIN_RATIO = 0.35
WHITE_SOURCE_FUZZY_EDGE_RATIO_MIN = 0.82
WHITE_SOURCE_FUZZY_EDGE_TOTAL_RATIO_MIN = 0.82
WHITE_SOURCE_FUZZY_EDGE_INNER_RATIO_MIN = 0.94
WHITE_SOURCE_FUZZY_EDGE_MISMATCH_MAX = 0.22
WHITE_SOURCE_FUZZY_EDGE_GROUP_MAX_RATIO = 0.14
WHITE_SOURCE_FUZZY_EDGE_RING_MIN_RATIO = 0.3
WHITE_SOURCE_FUZZY_MAX_GROUPS = 4
WHITE_SOURCE_CENTRAL_EXCLUSION_MIN_RATIO = 0.18
WHITE_SOURCE_CENTRAL_EXCLUSION_MAX_RATIO = 0.82
WHITE_SOURCE_CENTRAL_EXCLUSION_CENTER_DIST_RATIO = 0.24
WHITE_SOURCE_CENTRAL_EXCLUSION_BBOX_RATIO_MIN = 0.2
DEFAULT_FUZZY_SCALES = [0.9, 1.0, 1.1]
FUZZY_MAX_DIFF = 24
FUZZY_MATCH_RATIO_MIN = 0.9
FUZZY_SMALL_MATCH_RATIO_MIN = 0.95
FUZZY_CORR_THRESHOLD = 0.75
FUZZY_MAX_CANDIDATES = 12
FUZZY_REFINE_SCALE_DELTAS = [0.0, -0.05, 0.05, -0.03, 0.03, -0.02, 0.02, -0.01, 0.01]
FUZZY_REFINE_POS_DELTAS = [-1, 0, 1]
FUZZY_REFINE_MIN_RATIO = 0.8
FUZZY_REFINE_MAX_PEAKS = 4
FUZZY_LOCAL_DIFF_MAX_AREA_RATIO = 0.22
FUZZY_LOCAL_DIFF_MAX_GROUPS = 3
FUZZY_LOCAL_DIFF_MAX_GROUP_AREA_RATIO = 0.16
FUZZY_LOCAL_DIFF_MAX_BBOX_AREA_RATIO = 0.22
FUZZY_LOCAL_DIFF_MIN_FILL_RATIO = 0.28
FUZZY_LOCAL_DIFF_BORDER_BAND_PX = 3
FUZZY_LOCAL_DIFF_CLEAN_RATIO = 0.98
FUZZY_FOCUSED_DIFF_MAX_AREA_RATIO = 0.4
FUZZY_FOCUSED_DIFF_MAX_GROUPS = 8
FUZZY_FOCUSED_DIFF_BORDER_CLEAN_MIN = 0.98
FUZZY_FOCUSED_DIFF_CLEAN_RATIO = 0.995
FUZZY_FOCUSED_DIFF_INSET_PX = 10
FUZZY_MAJORITY_RATIO_MIN = 0.66
FUZZY_MAJORITY_CORR_MIN = 0.93
FUZZY_MAJORITY_MEDIAN_MAX = 4.0
FUZZY_OCCLUSION_RATIO_MIN = 0.03
WHITE_FUZZY_UNIFORM_REGION_MIN_PIXELS = 256
WHITE_FUZZY_UNIFORM_REGION_MAX_DIFF = 6
WHITE_FUZZY_UNIFORM_REGION_RATIO_MIN = 0.985
WHITE_FUZZY_MIN_OCCLUSION_RATIO = 0.08
WHITE_DIRECT_COMPONENT_THRESHOLD = 0.9
SPECIAL_DIRECT_COMPONENT_MAX = 32
WHITE_DIRECT_MIN_OPAQUE_PIXELS = 350
RESCUE_CORR_THRESHOLD = 0.55
RESCUE_MAX_CANDIDATES = 8
SOFT_MATCH_RATIO_MIN = 0.72
SOFT_SMALL_MATCH_RATIO_MIN = 0.9
SOFT_SMALL_VALID_PIXELS_MAX = 384
ICON_LIKE_MATCH_RATIO_MIN = 0.92
ICON_LIKE_SMALL_MATCH_RATIO_MIN = 0.97
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

TOTAL_LAYER_STAGES = 3
STAGE_NORMAL = (1, 'normal')
STAGE_9SLICE = (2, '9-slice')
STAGE_WHITE = (3, 'white')

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

# ──────────────────── Alpha Compositing ────────────────────

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

# ──────────────────── Mask Operations ────────────────────

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


def _mask_ratio(mask, base_mask):
    denom = np.count_nonzero(base_mask)
    if denom <= 0:
        return 1.0
    return float(np.count_nonzero(mask & base_mask) / denom)


def _build_border_support(ok_mask, valid_mask):
    h, w = valid_mask.shape
    band = max(1, min(3, max(1, min(h, w) // 2)))
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


def _build_shape_edge_inner_masks(base_mask, px=WHITE_SOURCE_FUZZY_EDGE_PX):
    base_mask = base_mask.astype(bool)
    if not np.any(base_mask):
        return base_mask.copy(), np.zeros_like(base_mask, dtype=bool)
    inner_mask = erode_mask(base_mask, px=px)
    if inner_mask is None:
        inner_mask = np.zeros_like(base_mask, dtype=bool)
    inner_mask = inner_mask.astype(bool)
    edge_mask = base_mask & ~inner_mask
    if np.count_nonzero(edge_mask) < MIN_MATCH_PIXELS:
        edge_mask = base_mask.copy()
        inner_mask = np.zeros_like(base_mask, dtype=bool)
    return edge_mask, inner_mask

# ──────────────────── Rect Utilities ────────────────────

def _clip_rect(rect, sw, sh):
    x = max(0, int(rect.get('x', 0)))
    y = max(0, int(rect.get('y', 0)))
    w = int(rect.get('width', 0))
    h = int(rect.get('height', 0))
    if w <= 0 or h <= 0 or x >= sw or y >= sh:
        return None
    w = min(w, sw - x)
    h = min(h, sh - y)
    if w <= 0 or h <= 0:
        return None
    clipped = {'x': x, 'y': y, 'width': w, 'height': h}
    if rect.get('text') is not None:
        clipped['text'] = rect.get('text')
    return clipped


def _expand_rect(rect, pad, sw, sh):
    return _clip_rect({
        'x': int(rect.get('x', 0)) - pad,
        'y': int(rect.get('y', 0)) - pad,
        'width': int(rect.get('width', 0)) + pad * 2,
        'height': int(rect.get('height', 0)) + pad * 2,
        'text': rect.get('text'),
    }, sw, sh)


def _rect_intersection_area(a, b):
    ix1 = max(a['x'], b['x'])
    iy1 = max(a['y'], b['y'])
    ix2 = min(a['x'] + a['width'], b['x'] + b['width'])
    iy2 = min(a['y'] + a['height'], b['y'] + b['height'])
    if ix2 <= ix1 or iy2 <= iy1:
        return 0
    return (ix2 - ix1) * (iy2 - iy1)


def _rects_overlap(a, b):
    return not (a['x'] + a['width'] <= b['x'] or b['x'] + b['width'] <= a['x'] or
                a['y'] + a['height'] <= b['y'] or b['y'] + b['height'] <= a['y'])


def rect_contains(outer, inner):
    return (outer['x'] <= inner['x'] and outer['y'] <= inner['y'] and
            outer['x'] + outer['width'] >= inner['x'] + inner['width'] and
            outer['y'] + outer['height'] >= inner['y'] + inner['height'])


def rect_area(r):
    return r['width'] * r['height']

# ──────────────────── Peak Extraction ────────────────────

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


def collect_candidate_peaks(result_map, tw, th, max_peaks=MAX_CANDIDATES):
    strong = extract_peaks(result_map, CORR_THRESHOLD, tw, th, max_peaks=max_peaks)
    rescue = extract_top_peaks(
        result_map, tw, th,
        max_peaks=max(4, max_peaks // 3),
        min_threshold=RESCUE_CORR_THRESHOLD if max_peaks >= MAX_CANDIDATES else FUZZY_CORR_THRESHOLD,
    )

    merged = []

    def _append(peaks, force_rescue):
        for y, x, score in peaks:
            if any(abs(y - py) < th and abs(x - px) < tw for py, px, _, _ in merged):
                continue
            merged.append((y, x, score, force_rescue or score < CORR_THRESHOLD))
            if len(merged) >= max_peaks:
                break

    _append(strong, False)
    if len(merged) < max_peaks:
        _append(rescue, True)
    return merged[:max_peaks]


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


# ──────────────────── Sprite Helpers ────────────────────

def _is_icon_like_sprite(sprite):
    source_max_dim = max(sprite['w'], sprite['h'])
    coverage = float(sprite.get('opaque_coverage', 1.0))
    component_count = int(sprite.get('component_count', 1))
    return (
        source_max_dim <= 96
        or coverage <= 0.72
        or component_count >= 2
    )


class DupPosIndex:
    """Spatial grid index for fast duplicate position checks."""
    _CELL = 32

    def __init__(self):
        self._grid = {}

    def _key(self, x, y):
        return (x // self._CELL, y // self._CELL)

    def add(self, match):
        r = match['rect']
        k = self._key(r['x'], r['y'])
        self._grid.setdefault(k, []).append(match)

    def add_all(self, matches):
        for m in matches:
            self.add(m)

    def has_dup(self, px, py, tw, th):
        cx, cy = px // self._CELL, py // self._CELL
        for dx in (-1, 0, 1):
            for dy in (-1, 0, 1):
                for e in self._grid.get((cx + dx, cy + dy), ()):
                    er = e['rect']
                    if (abs(er['x'] - px) <= 2 and abs(er['y'] - py) <= 2 and
                            abs(er['width'] - tw) <= 2 and abs(er['height'] - th) <= 2):
                        return True
        return False


def _is_dup_pos(px, py, tw, th, existing):
    for e in existing:
        er = e['rect']
        if (abs(er['x'] - px) <= 2 and abs(er['y'] - py) <= 2 and
                abs(er['width'] - tw) <= 2 and abs(er['height'] - th) <= 2):
            return True
    return False


def _is_fuzzy_match_entry(match):
    return bool(match.get('fuzzy_match'))

# ──────────────────── Fuzzy Helpers ────────────────────

def _fuzzy_rank(verify):
    return (
        1 if verify.get('accepted') else 0,
        1 if verify.get('majority_rescued') else 0,
        1 if verify.get('perfect') else 0,
        float(verify.get('mode_score', -1.0)),
        float(verify.get('match_ratio', 0.0)),
        -float(verify.get('median_diff', 999.0)),
    )


def _should_accept_fuzzy_majority(score, verify):
    if verify.get('accepted'):
        return True
    if float(score) < FUZZY_MAJORITY_CORR_MIN:
        return False
    if float(verify.get('median_diff', 999.0)) > FUZZY_MAJORITY_MEDIAN_MAX:
        return False
    if float(verify.get('match_ratio', 0.0)) < FUZZY_MAJORITY_RATIO_MIN:
        return False
    if int(verify.get('valid_pixels', 0)) < MIN_MATCH_PIXELS:
        return False
    verify['accepted'] = True
    verify['majority_rescued'] = True
    return True


def _compute_candidate_occlusion_ratio(alpha_mask, overlay, px, py, tw, th, exclusion_mask=None):
    if overlay is None or tw <= 0 or th <= 0:
        return 0.0
    sh, sw = overlay.shape[:2]
    if px < 0 or py < 0 or px + tw > sw or py + th > sh:
        return 0.0
    if alpha_mask is None:
        visible_mask = np.ones((th, tw), dtype=bool)
    else:
        visible_mask = erode_mask(alpha_mask)
        if visible_mask is None or np.count_nonzero(visible_mask) < MIN_MATCH_PIXELS:
            visible_mask = alpha_mask.astype(bool)
    visible_count = int(np.count_nonzero(visible_mask))
    if visible_count < MIN_MATCH_PIXELS:
        return 0.0
    overlay_crop = overlay[py:py + th, px:px + tw]
    occluded = np.zeros((th, tw), dtype=bool)
    if overlay_crop.shape[2] >= 4:
        occluded |= overlay_crop[:, :, 3] > 0
    if exclusion_mask is not None:
        occluded |= exclusion_mask.astype(bool)
    return float(np.count_nonzero(visible_mask & occluded) / visible_count)


def _is_large_uniform_visible_region(region_bgr, visible_mask):
    visible_mask = visible_mask.astype(bool)
    visible_count = int(np.count_nonzero(visible_mask))
    if visible_count < WHITE_FUZZY_UNIFORM_REGION_MIN_PIXELS:
        return False
    pixels = region_bgr[visible_mask].astype(np.float32)
    if pixels.size <= 0:
        return False
    median_bgr = np.median(pixels, axis=0)
    diff = np.abs(pixels - median_bgr).max(axis=1)
    uniform_ratio = float(np.count_nonzero(diff <= WHITE_FUZZY_UNIFORM_REGION_MAX_DIFF) / diff.size)
    return uniform_ratio >= WHITE_FUZZY_UNIFORM_REGION_RATIO_MIN


def _is_central_exclusion_hole(alpha_mask, exclusion_mask):
    if alpha_mask is None or exclusion_mask is None:
        return False
    shape_mask = alpha_mask.astype(bool)
    excluded = shape_mask & exclusion_mask.astype(bool)
    total = int(np.count_nonzero(shape_mask))
    excluded_count = int(np.count_nonzero(excluded))
    if total < MIN_MATCH_PIXELS or excluded_count < MIN_MATCH_PIXELS:
        return False

    excluded_ratio = float(excluded_count / total)
    if excluded_ratio < WHITE_SOURCE_CENTRAL_EXCLUSION_MIN_RATIO:
        return False
    if excluded_ratio > WHITE_SOURCE_CENTRAL_EXCLUSION_MAX_RATIO:
        return False

    ys, xs = np.where(excluded)
    x1, x2 = int(xs.min()), int(xs.max())
    y1, y2 = int(ys.min()), int(ys.max())
    bbox_area = max(1, (x2 - x1 + 1) * (y2 - y1 + 1))
    bbox_ratio = float(bbox_area / total)
    if bbox_ratio < WHITE_SOURCE_CENTRAL_EXCLUSION_BBOX_RATIO_MIN:
        return False

    h, w = shape_mask.shape[:2]
    cx = (x1 + x2 + 1) / 2.0
    cy = (y1 + y2 + 1) / 2.0
    center_dx = abs(cx - (w / 2.0)) / max(1.0, w / 2.0)
    center_dy = abs(cy - (h / 2.0)) / max(1.0, h / 2.0)
    return max(center_dx, center_dy) <= WHITE_SOURCE_CENTRAL_EXCLUSION_CENTER_DIST_RATIO

# ──────────────────── Exclusion ────────────────────

def build_fuzzy_exclusion_mask(existing, px, py, tw, th):
    mask = np.zeros((th, tw), dtype=bool)
    if tw <= 0 or th <= 0:
        return mask
    x2 = px + tw
    y2 = py + th
    for e in existing:
        if not _is_fuzzy_match_entry(e):
            continue
        r = e['rect']
        ix1 = max(px, r['x'])
        iy1 = max(py, r['y'])
        ix2 = min(x2, r['x'] + r['width'])
        iy2 = min(y2, r['y'] + r['height'])
        if ix1 >= ix2 or iy1 >= iy2:
            continue
        mask[iy1 - py:iy2 - py, ix1 - px:ix2 - px] = True
    return mask

# ──────────────────── Progress ────────────────────

def _stage_prefix(stage_idx, label):
    return f"[{stage_idx}/{TOTAL_LAYER_STAGES}] {label}"


def _log_stage_start(stage_idx, label, total, unit='sprites'):
    print(f"  {_stage_prefix(stage_idx, label)}: start ({total} {unit})", flush=True)


def _log_stage_end(stage_idx, label, found):
    print(f"  {_stage_prefix(stage_idx, label)}: found {found}")


def _write_stage_progress(stage_idx, label, current, total, item_label, matched):
    sys.stdout.write(
        f"\r  {_stage_prefix(stage_idx, label)} {current}/{total} "
        f"{item_label[:40]:<40} hits:{matched}"
    )
    sys.stdout.flush()


def _write_stage_done(stage_idx, label, total, elapsed):
    sys.stdout.write(
        f"\r  {_stage_prefix(stage_idx, label)}: done ({total}/{total}) in {elapsed:.1f}s{' ':32}\n"
    )
    sys.stdout.flush()
