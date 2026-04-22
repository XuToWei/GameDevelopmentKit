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
ERODE_PX = 3
SCALES = [0.5, 0.75, 1.0, 1.33, 1.5, 2.0, 3.0]
MIN_TEMPLATE_DIM = 8
MAX_CANDIDATES = 50
MAX_LAYERS = 10

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
                entry = {
                    'path': fpath, 'rel_path': rel_path, 'img': img,
                    'w': w, 'h': h, 'ar': w / h, 'border': border,
                }
                if rel_path in whites_map:
                    white_sprites.append(entry)
                else:
                    idx = len(sprites)
                    sprites.append(entry)
                    if border:
                        sliced_indices.append(idx)

    print(f"Loaded {len(sprites)} normal sprites ({len(sliced_indices)} 9-slice), "
          f"{len(white_sprites)} white sprites")
    return sprites, sliced_indices, white_sprites


def resize_sprite(sprite, tw, th):
    if sprite['border']:
        return render_9slice(sprite['img'], sprite['border'], tw, th)
    return cv2.resize(sprite['img'], (tw, th), interpolation=cv2.INTER_AREA)


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


def verify_pixel_match(screenshot, template_bgr, alpha_mask, x, y, max_diff=DEFAULT_MAX_DIFF):
    th, tw = template_bgr.shape[:2]
    sh, sw = screenshot.shape[:2]
    if x < 0 or y < 0 or x + tw > sw or y + th > sh:
        return 999.0, 0.0, 0
    region = screenshot[y:y + th, x:x + tw]
    vmask = erode_mask(alpha_mask)
    if vmask is not None:
        valid_count = np.count_nonzero(vmask)
    else:
        valid_count = tw * th
        vmask = np.ones((th, tw), dtype=bool)
    if valid_count < MIN_MATCH_PIXELS:
        return 999.0, 0.0, 0
    diff = np.abs(region.astype(np.int16) - template_bgr.astype(np.int16))
    per_px_max = diff.max(axis=2)
    masked_diffs = per_px_max[vmask]
    median_diff = float(np.median(masked_diffs))
    matched = np.count_nonzero(masked_diffs <= max_diff)
    ratio = matched / valid_count
    return median_diff, ratio, valid_count


# ──────────────────── 九宫辅助 ────────────────────

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


def _find_9slice_size(img, border, target_img, tl_x, tl_y, tw_max, th_max):
    h, w = img.shape[:2]
    L = min(border['left'], w - 1)
    R = min(border['right'], w - L - 1)
    T = min(border['top'], h - 1)
    B = min(border['bottom'], h - T - 1)

    tr_bgr, tr_mask = _extract_patch_bgr_mask(img[0:T, w - R:w])
    bl_bgr, bl_mask = _extract_patch_bgr_mask(img[h - B:h, 0:L])
    if tr_bgr is None or bl_bgr is None:
        return []

    def _patch_matches(region, ref_bgr, ref_mask):
        diff = np.abs(region.astype(np.int16) - ref_bgr.astype(np.int16))
        if ref_mask is not None:
            d = diff[ref_mask]
        else:
            d = diff.reshape(-1, diff.shape[-1]) if len(diff.shape) == 3 else diff.ravel()
        return d.size > 0 and d.max() <= DEFAULT_MAX_DIFF

    widths = []
    for rw in range(L + R + 1, min(tw_max - tl_x + 1, L + R + 801)):
        tr_x = tl_x + rw - R
        if tr_x < 0 or tr_x + R > tw_max or tl_y + T > th_max:
            continue
        if _patch_matches(target_img[tl_y:tl_y + T, tr_x:tr_x + R], tr_bgr, tr_mask):
            widths.append(rw)

    results = []
    for rw in widths:
        for rh in range(T + B + 1, min(th_max - tl_y + 1, T + B + 801)):
            bl_y = tl_y + rh - B
            if bl_y < 0 or bl_y + B > th_max or tl_x + L > tw_max:
                continue
            if _patch_matches(target_img[bl_y:bl_y + B, tl_x:tl_x + L], bl_bgr, bl_mask):
                results.append((rw, rh))
    return results


# ──────────────────── 去重 ────────────────────

def deduplicate_matches(matches):
    if not matches:
        return matches
    matches = sorted(matches, key=lambda m: m.get('median_diff', 999))
    keep = []
    for m in matches:
        r = m['rect']
        dup = False
        for i, k in enumerate(keep):
            kr = k['rect']
            if (abs(r['x'] - kr['x']) <= 2 and abs(r['y'] - kr['y']) <= 2 and
                    abs(r['width'] - kr['width']) <= 2 and abs(r['height'] - kr['height']) <= 2):
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
                if smaller > 0 and inter / smaller > 0.3:
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
        candidates = [nb for j, nb in enumerate(match_nodes)
                      if j != i and rect_contains(nb['rect'], ra)]
        if not candidates:
            continue
        direct = [c for c in candidates
                  if not any(rect_contains(c['rect'], o['rect'])
                             for o in candidates if o is not c)]
        if direct:
            direct.sort(key=lambda n: rect_area(n['rect']))
            na['parentId'] = direct[0]['node_id']
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
    return overlay


def _get_sprite(match, sprites, white_sprites):
    if match.get('is_white'):
        idx = match.get('white_idx', -1)
        return white_sprites[idx] if 0 <= idx < len(white_sprites) else None
    idx = match.get('sprite_idx', -1)
    return sprites[idx] if 0 <= idx < len(sprites) else None


# ──────────────────── 合成验证 ────────────────────

def composite_verify(template_bgr, alpha_mask, px, py, tw, th, overlay, screenshot,
                     max_diff=DEFAULT_MAX_DIFF):
    sh, sw = screenshot.shape[:2]
    if px < 0 or py < 0 or px + tw > sw or py + th > sh:
        return 999.0, 0.0, 0
    canvas = template_bgr.copy()
    overlay_crop = overlay[py:py + th, px:px + tw]
    alpha_composite_onto(canvas, overlay_crop, 0, 0)
    crop = screenshot[py:py + th, px:px + tw]
    vmask = erode_mask(alpha_mask) if alpha_mask is not None else np.ones((th, tw), dtype=bool)
    overlay_alpha = overlay_crop[:, :, 3] if overlay_crop.shape[2] == 4 else np.zeros((th, tw), np.uint8)
    visible = vmask & (overlay_alpha < 250)
    vc = np.count_nonzero(visible)
    if vc < MIN_MATCH_PIXELS:
        return 999.0, 0.0, 0
    diff = np.abs(canvas.astype(np.int16) - crop.astype(np.int16))
    per_px_max = diff.max(axis=2)
    masked_diffs = per_px_max[visible]
    median_diff = float(np.median(masked_diffs))
    mc = np.count_nonzero(masked_diffs <= max_diff)
    ratio = mc / vc
    return median_diff, ratio, vc


# ──────────────────── 单层匹配 ────────────────────

def match_one_layer(layer_num, sprites, sliced_indices, white_sprites,
                    screenshot, overlay, all_existing, max_diff=DEFAULT_MAX_DIFF):
    is_direct = overlay is None
    matches = []

    normal_count = len(sprites) - len(sliced_indices)
    print(f"  [1/3] normal sprites (0/{normal_count})...", flush=True)
    normal = _match_normal(sprites, sliced_indices, screenshot, overlay,
                           all_existing + matches, is_direct, max_diff=max_diff)
    matches.extend(normal)
    print(f"  [1/3] normal sprites: found {len(normal)}")

    slice_count = len(sliced_indices)
    print(f"  [2/3] 9-slice sprites (0/{slice_count})...", flush=True)
    sliced = _match_9slice(sprites, sliced_indices, screenshot, overlay,
                           all_existing + matches, is_direct, max_diff=max_diff)
    matches.extend(sliced)
    print(f"  [2/3] 9-slice sprites: found {len(sliced)}")

    matches = deduplicate_matches(matches)

    white_count = len(white_sprites)
    print(f"  [3/3] white sprites (0/{white_count})...", flush=True)
    white = _match_white(white_sprites, screenshot, overlay,
                         all_existing + matches, is_direct, max_diff=max_diff)
    matches.extend(white)
    print(f"  [3/3] white sprites: found {len(white)}")

    matches = deduplicate_matches(matches)
    return matches


# ── 普通 sprite 匹配 ──


def _match_normal(sprites, sliced_indices, screenshot, overlay, existing, is_direct,
                  max_diff=DEFAULT_MAX_DIFF):
    sh, sw = screenshot.shape[:2]
    sliced_set = set(sliced_indices)
    matches = []
    total = len(sprites) - len(sliced_set)
    processed = 0
    t_start = time.time()

    for sidx, sp in enumerate(sprites):
        if sidx in sliced_set:
            continue
        processed += 1
        name = os.path.basename(sp['rel_path'])
        sys.stdout.write(f"\r  [1/3] normal ({processed}/{total}) {name[:40]:<40} matched:{len(matches)}")
        sys.stdout.flush()

        for scale in SCALES:
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

            peaks = extract_peaks(res, CORR_THRESHOLD, tw, th)
            for cy, cx, _ in peaks:
                px, py = int(cx), int(cy)
                if _is_dup_pos(px, py, tw, th, existing + matches):
                    continue
                if not is_direct:
                    cand_r = {'x': px, 'y': py, 'width': tw, 'height': th}
                    if not any(_rects_overlap(cand_r, e['rect']) for e in existing):
                        continue

                if is_direct:
                    med_d, ratio, vp = verify_pixel_match(screenshot, tpl, amask, px, py,
                                                          max_diff=max_diff)
                else:
                    med_d, ratio, vp = composite_verify(tpl, amask, px, py, tw, th,
                                                        overlay, screenshot, max_diff=max_diff)
                if med_d <= VERIFY_MEDIAN_MAX:
                    matches.append({
                        'sprite_idx': sidx, 'is_white': False, 'is_9slice': False,
                        'rect': {'x': px, 'y': py, 'width': tw, 'height': th},
                        'match_ratio': round(ratio, 6), 'median_diff': round(med_d, 2),
                        'perfect': bool(med_d <= 3),
                        'valid_pixels': vp, 'scale': round(scale, 4),
                    })
    elapsed = time.time() - t_start
    sys.stdout.write(f"\r  [1/3] normal ({total}/{total}) done in {elapsed:.1f}s{' ':40}\n")
    sys.stdout.flush()
    return matches


# ── 九宫格 sprite 匹配 ──

def _match_9slice(sprites, sliced_indices, screenshot, overlay, existing, is_direct,
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
        tl_locs = extract_peaks(res, CORR_THRESHOLD, L, T)

        for cy, cx, _ in tl_locs:
            tl_x, tl_y = int(cx), int(cy)
            if tl_x < 0 or tl_y < 0:
                continue
            sizes = _find_9slice_size(img, border, screenshot, tl_x, tl_y, sw, sh)
            for (rw, rh) in sizes:
                if _is_dup_pos(tl_x, tl_y, rw, rh, existing + matches):
                    continue
                tpl, amask = prepare_template(sp['img'], rw, rh, border=border)
                if is_direct:
                    med_d, ratio, vp = verify_pixel_match(screenshot, tpl, amask, tl_x, tl_y,
                                                          max_diff=max_diff)
                else:
                    med_d, ratio, vp = composite_verify(tpl, amask, tl_x, tl_y, rw, rh,
                                                        overlay, screenshot, max_diff=max_diff)
                if med_d <= VERIFY_MEDIAN_MAX:
                    matches.append({
                        'sprite_idx': sidx, 'is_white': False, 'is_9slice': True,
                        'rect': {'x': tl_x, 'y': tl_y, 'width': rw, 'height': rh},
                        'match_ratio': round(ratio, 6), 'median_diff': round(med_d, 2),
                        'perfect': bool(med_d <= 3),
                        'valid_pixels': vp, 'scale': round(rw / sp['w'], 4),
                    })
    elapsed = time.time() - t_start
    sys.stdout.write(f"\r  [2/3] 9-slice ({total}/{total}) done in {elapsed:.1f}s{' ':40}\n")
    sys.stdout.flush()
    return matches


# ── 白图 sprite 匹配 ──

def _match_white(white_sprites, screenshot, overlay, existing, is_direct, max_diff=DEFAULT_MAX_DIFF):
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
        pixels = region[rect_mask].astype(np.float32)
        if not np.all(pixels.std(axis=0) <= WHITE_COLOR_STD_MAX):
            continue

        best = None
        best_cov = 0.0
        for widx, wsp in enumerate(white_sprites):
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

            cpx = region[check].astype(np.float32)
            if not np.all(cpx.std(axis=0) <= WHITE_COLOR_STD_MAX):
                continue

            if is_direct:
                cov = cc / np.count_nonzero(rect_mask)
            else:
                tpl_bgr = np.zeros((h, w, 3), dtype=np.uint8)
                mean_bgr = cpx.mean(axis=0).round().astype(np.uint8)
                tpl_bgr[opaque] = mean_bgr
                med_d, ratio, _ = composite_verify(tpl_bgr, opaque, x, y, w, h,
                                            overlay, screenshot, max_diff=max_diff)
                if med_d > VERIFY_MEDIAN_MAX:
                    continue
                cov = cc / np.count_nonzero(rect_mask)

            if cov > best_cov:
                mean_bgr_val = cpx.mean(axis=0).round().astype(int)
                best_cov = cov
                best = {
                    'white_idx': widx, 'sprite_idx': -1,
                    'is_white': True, 'is_9slice': wsp['border'] is not None,
                    'rect': {'x': x, 'y': y, 'width': w, 'height': h},
                    'match_ratio': 1.0, 'perfect': True,
                    'valid_pixels': cc,
                    'scale': round(w / wsp['w'], 4),
                    'tint_color': [int(mean_bgr_val[2]), int(mean_bgr_val[1]),
                                   int(mean_bgr_val[0])],
                    'coverage': round(best_cov, 4),
                }
        if best:
            matches.append(best)
    elapsed = time.time() - t_start
    sys.stdout.write(f"\r  [3/3] white: done in {elapsed:.1f}s{' ':40}\n")
    sys.stdout.flush()
    return matches


# ──────────────────── Debug 图片 ────────────────────

def _layer_color(layer_num):
    return LAYER_COLORS_BGR[layer_num % len(LAYER_COLORS_BGR)]


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
        tag = '9s' if m.get('is_9slice') else ('w' if m.get('is_white') else '')
        label = f"L{layer_num} {sp_name}"
        if tag:
            label += f" [{tag}]"
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
            screenshot, overlay, all_matches, max_diff=max_diff)

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
        layer += 1

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
            }
        }
        if m.get('tint_color'):
            mn['match']['tint_color'] = m['tint_color']
        if m.get('coverage'):
            mn['match']['coverage'] = m['coverage']
        match_nodes.append(mn)

    build_coverage_tree(match_nodes)

    # ── 统计 + 输出 ──
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
    print(f"Total: {len(match_nodes)} (perfect:{perfect} 9slice:{slices} white:{whites} layers:{layer})")
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
