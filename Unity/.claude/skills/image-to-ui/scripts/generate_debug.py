import cv2, json, numpy as np, sys

if len(sys.argv) != 4:
    print(f'Usage: {sys.argv[0]} <image_path> <json_path> <output_path>')
    sys.exit(1)

img = cv2.imread(sys.argv[1])
if img is None:
    print(f'Cannot read image: {sys.argv[1]}')
    sys.exit(1)

with open(sys.argv[2], encoding='utf-8') as f:
    data = json.load(f)

colors_bgr = {
    'Image': (80, 175, 76), 'RawImage': (74, 195, 139), 'Text': (243, 150, 33),
    'Button': (34, 87, 255), 'Toggle': (0, 152, 255), 'Panel': (139, 125, 96),
    'HorizontalGroup': (72, 85, 121), 'VerticalGroup': (72, 85, 121),
    'GridGroup': (72, 85, 121), 'Mask': (180, 100, 200),
}
containers = {'Panel', 'HorizontalGroup', 'VerticalGroup', 'GridGroup', 'ScrollView', 'ListView'}
canvas = img.copy()

for node in data['nodes']:
    if node['type'] not in containers:
        continue
    r = node['rect']
    color = colors_bgr.get(node['type'], (128, 128, 128))
    overlay = canvas.copy()
    cv2.rectangle(overlay, (r['x'], r['y']), (r['x']+r['width'], r['y']+r['height']), color, -1)
    cv2.addWeighted(overlay, 0.15, canvas, 0.85, 0, canvas)
    cv2.rectangle(canvas, (r['x'], r['y']), (r['x']+r['width'], r['y']+r['height']), color, 1)
    label = f"{node['name']} [{node['type']}]"
    cv2.putText(canvas, label, (r['x']+2, r['y']-6), cv2.FONT_HERSHEY_SIMPLEX, 0.5, color, 1)

for node in data['nodes']:
    if node['type'] in containers:
        continue
    r = node['rect']
    color = colors_bgr.get(node['type'], (128, 128, 128))
    overlay = canvas.copy()
    cv2.rectangle(overlay, (r['x'], r['y']), (r['x']+r['width'], r['y']+r['height']), color, -1)
    cv2.addWeighted(overlay, 0.25, canvas, 0.75, 0, canvas)
    cv2.rectangle(canvas, (r['x'], r['y']), (r['x']+r['width'], r['y']+r['height']), color, 2)
    label = f"{node['name']} [{node['type']}]"
    cv2.putText(canvas, label, (r['x']+2, r['y']-6), cv2.FONT_HERSHEY_SIMPLEX, 0.5, color, 2)

cv2.imwrite(sys.argv[3], canvas)
print(f'Saved: {sys.argv[3]}')
