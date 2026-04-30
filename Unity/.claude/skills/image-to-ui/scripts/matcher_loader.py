"""matcher_loader — Sprite loading, 9-slice rendering, template preparation."""

import cv2
import numpy as np
import os

from matcher_core import (
    ensure_bgra, render_9slice,
    WHITE_COLOR_STD_MAX, MIN_MATCH_PIXELS, DEFAULT_FUZZY_SCALES,
)


# ──────────────────── 九宫格渲染 ────────────────────

# render_9slice is in matcher_core (no dependencies)


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
                auto_white_source = _is_grayscale_white_source(img)
                entry = {
                    'path': fpath, 'rel_path': rel_path, 'img': img,
                    'w': w, 'h': h, 'ar': w / h, 'border': border,
                    'base_sprite_idx': idx,
                    'is_white_source': rel_path in whites_map or auto_white_source,
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


def _is_grayscale_white_source(img):
    bgra = ensure_bgra(img)
    alpha = bgra[:, :, 3] > 250
    if not np.any(alpha):
        return False
    pixels = bgra[:, :, :3][alpha].astype(np.int16)
    channel_span = pixels.max(axis=1) - pixels.min(axis=1)
    return bool(np.all(channel_span <= WHITE_COLOR_STD_MAX))


def normalize_match_scales(scales, default=None):
    if default is None:
        default = DEFAULT_FUZZY_SCALES
    if scales is None:
        values = default
    elif isinstance(scales, str):
        values = [p.strip() for p in scales.split(',') if p.strip()]
    else:
        values = scales
    normalized = []
    seen = set()
    for value in values:
        try:
            scale = round(float(value), 4)
        except (TypeError, ValueError):
            continue
        if scale <= 0:
            continue
        if scale in seen:
            continue
        seen.add(scale)
        normalized.append(scale)
    if normalized:
        return normalized
    return [round(float(v), 4) for v in default]


def parse_match_scales_arg(scales_arg):
    if scales_arg is None:
        return None
    parts = [p.strip() for p in str(scales_arg).split(',') if p.strip()]
    if not parts:
        return None
    return normalize_match_scales(parts)


def get_9slice_target_min_size(img, border):
    h, w = img.shape[:2]
    L = min(border['left'], w - 1)
    R = min(border['right'], w - L - 1)
    T = min(border['top'], h - 1)
    B = min(border['bottom'], h - T - 1)
    return max(1, L + R + 1), max(1, T + B + 1)


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
