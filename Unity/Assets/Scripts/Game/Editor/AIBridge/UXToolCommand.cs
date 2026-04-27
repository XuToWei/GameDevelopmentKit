using AIBridge.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ThunderFireUITool;

namespace Game.Editor
{
    /// <summary>
    /// AIBridge extension command for UXTool UI operations.
    /// Auto-discovered by CommandRegistry via reflection.
    ///
    /// CLI usage (via batch execute):
    ///   AIBridgeCLI batch execute --commands '[{"type":"uxtool","params":{"action":"add_background","designImage":"Assets/Res/UI/UISprite/Design/0_Tutorial_1.png"}}]'
    /// </summary>
    public class UXToolCommand : ICommand
    {
        public string Type => "uxtool";
        public bool RequiresRefresh => false;

        public string SkillDescription => @"### `uxtool` - UXTool UI Operations

Project-specific UI operations backed by UXTool's Editor tools.

**Important**: `uxtool` is a custom command type - invoke via `batch execute` passthrough:

```bash
./AIBridgeCache/CLI/AIBridgeCLI.exe batch execute --commands '[{""type"":""uxtool"",""params"":{...}}]'
```

#### `add_background` - Set Design Mockup as Preview Background

Uses UXTool's `QuickBackground` to create a temporary preview background image. The background is `DontSave` - it won't persist in prefabs or scene saves. Useful for placing a design image behind UI elements during development.

```bash
# Set design image as background (auto-reads dimensions from texture)
./AIBridgeCache/CLI/AIBridgeCLI.exe batch execute \
  --commands '[{""type"":""uxtool"",""params"":{""action"":""add_background"",""designImage"":""Assets/Res/UI/UISprite/Design/0_Tutorial_1.png""}}]'

# Default background (uses QuickBackground saved settings)
./AIBridgeCache/CLI/AIBridgeCLI.exe batch execute \
  --commands '[{""type"":""uxtool"",""params"":{""action"":""add_background""}}]'
```

**Parameters:**

| Param | Required | Default | Description |
|-------|----------|---------|-------------|
| `designImage` | No | saved setting | Design image asset path - auto-reads width/height from texture |
| `color` | No | white | Color tint as `r,g,b,a` (0-1 floats) |

**Response:**
```json
{""success"":true,""data"":{""action"":""add_background"",""designImage"":""Assets/.../0_Tutorial_1.png"",""size"":{""width"":1080,""height"":2340}}}
```";

        public CommandResult Execute(CommandRequest request)
        {
            var action = request.GetParam<string>("action", "");

            return action switch
            {
                "add_background" => AddBackground(request),
                _ => CommandResult.Failure(request.id, $"Unknown action: {action}. Supported: add_background")
            };
        }

        /// <summary>
        /// Set a design mockup image as the preview background via QuickBackground.
        ///
        /// Params:
        ///   --designImage (optional): Design image asset path — auto-reads dimensions and sets as background.
        ///                             If omitted, uses QuickBackground's saved settings.
        ///   --color (optional): Color tint as "r,g,b,a" (0-1 float).
        /// </summary>
        private CommandResult AddBackground(CommandRequest request)
        {
            QuickBackground.CreateBackGround();

            // Find the created background
            var bgRoot = FindQuickBackground();
            if (bgRoot == null)
                return CommandResult.Failure(request.id,
                    "QuickBackground.CreateBackGround() failed. Check if QuickBackground switch is enabled in UXTool settings.");

            var imageTransform = bgRoot.transform.childCount > 0 ? bgRoot.transform.GetChild(0) : null;
            if (imageTransform == null)
                return CommandResult.Failure(request.id, "UXQuickBackground has no child image node");

            var image = imageTransform.GetComponent<Image>();
            var rect = imageTransform.GetComponent<RectTransform>();

            // Handle designImage: load sprite and auto-set dimensions
            var designImagePath = request.GetParam<string>("designImage", "");
            if (!string.IsNullOrEmpty(designImagePath))
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(designImagePath);
                if (sprite == null)
                    return CommandResult.Failure(request.id, $"Design image not found: {designImagePath}");

                if (image != null) image.sprite = sprite;
                if (rect != null) rect.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height);
            }

            // Handle color tint
            var colorStr = request.GetParam<string>("color", "");
            if (!string.IsNullOrEmpty(colorStr) && image != null)
            {
                image.color = ParseColor(colorStr);
            }

            // Build response
            var resultSprite = image != null && image.sprite != null
                ? AssetDatabase.GetAssetPath(image.sprite.texture) : "";
            var resultSize = rect != null
                ? new { width = (int)rect.sizeDelta.x, height = (int)rect.sizeDelta.y }
                : null;

            return CommandResult.Success(request.id, new
            {
                action = "add_background",
                designImage = resultSprite,
                size = resultSize
            });
        }

        private static GameObject FindQuickBackground()
        {
            var go = GameObject.Find("/UXQuickBackground");
            if (go != null) return go;

            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                var t = prefabStage.prefabContentsRoot.transform.Find("UXQuickBackground");
                if (t != null) return t.gameObject;
            }

            return null;
        }

        private static Color ParseColor(string colorStr)
        {
            var parts = colorStr.Split(',');
            if (parts.Length >= 4 &&
                float.TryParse(parts[0].Trim(), out var r) &&
                float.TryParse(parts[1].Trim(), out var g) &&
                float.TryParse(parts[2].Trim(), out var b) &&
                float.TryParse(parts[3].Trim(), out var a))
                return new Color(r, g, b, a);

            if (parts.Length == 3 &&
                float.TryParse(parts[0].Trim(), out r) &&
                float.TryParse(parts[1].Trim(), out g) &&
                float.TryParse(parts[2].Trim(), out b))
                return new Color(r, g, b, 1f);

            return Color.white;
        }
    }
}
