"""
解析sprite目录下所有.meta文件，提取spriteBorder（九宫格border值）。
输出JSON：key为sprite相对路径，value为{left, bottom, right, top}。
只输出border非零的sprite。

用法：
  py -3 parse_sprite_borders.py <sprite_dir> <output.json> [sprite_dir2 ...]
"""

import json
import os
import re
import sys

BORDER_PATTERN = re.compile(
    r'spriteBorder:\s*\{x:\s*(\d+),\s*y:\s*(\d+),\s*z:\s*(\d+),\s*w:\s*(\d+)\}'
)


def parse_meta(meta_path):
    with open(meta_path, encoding='utf-8') as f:
        content = f.read()
    m = BORDER_PATTERN.search(content)
    if not m:
        return None
    left, bottom, right, top = int(m.group(1)), int(m.group(2)), int(m.group(3)), int(m.group(4))
    if left == 0 and bottom == 0 and right == 0 and top == 0:
        return None
    return {'left': left, 'bottom': bottom, 'right': right, 'top': top}


def scan_directory(sprite_dir):
    results = {}
    for root, _, files in os.walk(sprite_dir):
        for fname in files:
            if not fname.lower().endswith('.png.meta'):
                continue
            meta_path = os.path.join(root, fname)
            border = parse_meta(meta_path)
            if border is None:
                continue
            png_name = fname[:-5]  # remove .meta
            rel_path = os.path.relpath(os.path.join(root, png_name), sprite_dir).replace('\\', '/')
            results[rel_path] = border
    return results


if __name__ == '__main__':
    if len(sys.argv) < 3:
        print(f"Usage: {sys.argv[0]} <sprite_dir> <output.json> [sprite_dir2 ...]")
        sys.exit(1)

    output_path = sys.argv[2]
    sprite_dirs = [sys.argv[1]] + sys.argv[3:]

    all_borders = {}
    for d in sprite_dirs:
        all_borders.update(scan_directory(d))

    os.makedirs(os.path.dirname(output_path) or '.', exist_ok=True)
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(all_borders, f, ensure_ascii=False, indent=2)

    print(f"Found {len(all_borders)} sprites with 9-slice border")
    print(f"Saved: {output_path}")
