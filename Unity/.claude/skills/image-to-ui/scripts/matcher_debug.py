"""matcher_debug — Debug output and overlay visualization."""

import cv2
import numpy as np
import os

from matcher_core import (
    ensure_bgra, alpha_composite_onto, alpha_composite_bgra, LAYER_COLORS_BGR,
)
from matcher_loader import resize_sprite


def _get_sprite(match, sprites, white_sprites):
    if match.get('is_white'):
        idx = match.get('white_idx', -1)
        return white_sprites[idx] if 0 <= idx < len(white_sprites) else None
    idx = match.get('sprite_idx', -1)
    return sprites[idx] if 0 <= idx < len(sprites) else None


def build_overlay(all_matches, sprites, white_sprites, sh, sw):
    """
    把所有已匹配sprite合成为一张 BGRA overlay 图。
    按layer从大到小（底层先画，顶层后画覆盖）。
    """
    overlay = np.zeros((sh, sw, 4), dtype=np.uint8)
    _composite_matches(overlay, all_matches, sprites, white_sprites)
    return overlay


def build_overlay_incremental(base_overlay, new_matches, sprites, white_sprites):
    """在已有overlay上增量合成新matches，返回新overlay（不修改原始）。"""
    overlay = base_overlay.copy()
    _composite_matches(overlay, new_matches, sprites, white_sprites)
    return overlay


def _composite_matches(overlay, matches, sprites, white_sprites):
    """把matches合成到overlay上（in-place）。"""
    sorted_matches = sorted(matches, key=lambda m: m.get('layer', 0), reverse=True)
    for m in sorted_matches:
        r = m['rect']
        rendered = m.get('_rendered_bgra')
        if rendered is None:
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
            m['_rendered_bgra'] = rendered
        alpha_composite_bgra(overlay, rendered, r['x'], r['y'])


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


def save_overlay_debug(overlay_bgra, screenshot_bgr, debug_dir, stem, matches=None):
    os.makedirs(debug_dir, exist_ok=True)
    raw_path = os.path.join(debug_dir, f"{stem}_overlay.png")
    preview_path = os.path.join(debug_dir, f"{stem}_overlay_preview.png")
    compare_path = os.path.join(debug_dir, f"{stem}_compare.png")

    cv2.imwrite(raw_path, overlay_bgra)
    preview = _render_overlay_preview(overlay_bgra)
    cv2.imwrite(preview_path, preview)

    panel_w = max(screenshot_bgr.shape[1], preview.shape[1])
    left = _make_labeled_panel(screenshot_bgr.copy(), "Screenshot", width=panel_w)
    right = _make_labeled_panel(preview, "Reconstructed", width=panel_w)
    compare = np.hstack([left, right])
    cv2.imwrite(compare_path, compare)

    print(f"  Overlay saved: {raw_path}")
    print(f"  Compare saved: {compare_path}")
    return raw_path, compare_path


def generate_layer_debug(screenshot, layer_num, layer_matches, all_matches_before,
                         sprites, white_sprites, debug_dir):
    canvas = screenshot.copy()

    for m in all_matches_before:
        r = m['rect']
        c = _layer_color(m.get('layer', 0))
        dim = tuple(v // 2 for v in c)
        cv2.rectangle(canvas, (r['x'], r['y']),
                      (r['x'] + r['width'], r['y'] + r['height']), dim, 1)

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
        label = f"L{layer_num} {sp_name}"
        if tags:
            label += f" [{' '.join(tags)}]"
        cv2.putText(canvas, label, (r['x'] + 2, r['y'] - 6),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.45, color, 1)
    out_path = os.path.join(debug_dir, f"layer_{layer_num}.png")
    cv2.imwrite(out_path, canvas)
    print(f"  Debug saved: {out_path}")
    return out_path
