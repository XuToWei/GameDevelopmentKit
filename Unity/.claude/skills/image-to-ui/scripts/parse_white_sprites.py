"""
识别白图/纯色sprite。

白图：所有不透明像素颜色一致（或极接近）的sprite。
Unity中这类sprite通过Image.color属性着色，原始图只保留形状（alpha轮廓）。

输出JSON：key为sprite相对路径，value为 {color: [r,g,b], alpha_coverage: float}。
只输出被判定为纯色sprite的条目。

用法：
  py -3 parse_white_sprites.py <sprite_dir> <output.json> [sprite_dir2 ...]
"""

import cv2
import json
import numpy as np
import os
import sys

MAX_COLOR_STD = 2.0  # 归一化到0-255后，各通道std上限


def is_solid_color(img):
    """
    判断sprite是否为纯色图。
    返回 (is_solid, color_rgb_0_255, alpha_coverage)。
    处理8-bit和16-bit PNG。
    """
    if img is None or img.size == 0:
        return False, None, 0.0

    h, w = img.shape[:2]
    total_pixels = h * w

    # 归一化到0-255 float
    if img.dtype == np.uint16:
        scale = 255.0 / 65535.0
    else:
        scale = 1.0

    has_alpha = len(img.shape) == 3 and img.shape[2] == 4

    if has_alpha:
        alpha = img[:, :, 3].astype(np.float32) * scale
        opaque_mask = alpha > (10 * scale * 255.0 / 255.0)  # alpha > ~10/255
        # recalculate with proper threshold
        if img.dtype == np.uint16:
            opaque_mask = img[:, :, 3] > (10 * 65535 // 255)
        else:
            opaque_mask = img[:, :, 3] > 10

        opaque_count = np.count_nonzero(opaque_mask)
        if opaque_count < 4:
            return False, None, 0.0

        pixels = img[:, :, :3][opaque_mask].astype(np.float32) * scale
        alpha_coverage = opaque_count / total_pixels
    else:
        pixels = img.reshape(-1, 3).astype(np.float32) * scale if len(img.shape) == 3 else img.reshape(-1, 1).astype(np.float32) * scale
        alpha_coverage = 1.0

    if len(pixels) < 4:
        return False, None, 0.0

    std_per_channel = pixels.std(axis=0)
    if np.all(std_per_channel <= MAX_COLOR_STD):
        mean_color = np.clip(pixels.mean(axis=0).round(), 0, 255).astype(int).tolist()
        return True, mean_color, round(alpha_coverage, 4)

    return False, None, 0.0


def scan_directory(sprite_dir):
    results = {}
    for root, _, files in os.walk(sprite_dir):
        for fname in files:
            if not fname.lower().endswith('.png'):
                continue
            fpath = os.path.join(root, fname)
            img = cv2.imread(fpath, cv2.IMREAD_UNCHANGED)
            if img is None:
                continue

            solid, color_bgr, coverage = is_solid_color(img)
            if not solid:
                continue

            # BGR -> RGB for output
            color_rgb = [color_bgr[2], color_bgr[1], color_bgr[0]] if len(color_bgr) == 3 else color_bgr
            rel_path = os.path.relpath(fpath, sprite_dir).replace('\\', '/')
            results[rel_path] = {
                'color': color_rgb,
                'alpha_coverage': coverage
            }
    return results


if __name__ == '__main__':
    if len(sys.argv) < 3:
        print(f"Usage: {sys.argv[0]} <sprite_dir> <output.json> [sprite_dir2 ...]")
        sys.exit(1)

    output_path = sys.argv[2]
    sprite_dirs = [sys.argv[1]] + sys.argv[3:]

    all_whites = {}
    for d in sprite_dirs:
        all_whites.update(scan_directory(d))

    os.makedirs(os.path.dirname(output_path) or '.', exist_ok=True)
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(all_whites, f, ensure_ascii=False, indent=2)

    print(f"Found {len(all_whites)} solid-color sprites")
    print(f"Saved: {output_path}")
