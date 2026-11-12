using System.Globalization;
using System.Threading.Tasks;
using Unity.Pipeline.Commands;
using ThunderFireUITool;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Editor
{
    /// <summary>Small UXTool-backed editor operations.</summary>
    // Pipeline uses Task return types to await commands and exclude them from synchronous job batches.
#pragma warning disable ET0501
    public static class UXToolCommand
    {
        private const string ErrorCode = "UXTOOL_ERROR";
        private const string Group = "game.uxtool";

        [CliGroupCommand(Group, "调用 UXTool QuickBackground 添加临时设计稿背景，通常用于编辑预览。")]
        public static Task<object> AddBackground(
            [CliArg("design_image", "Sprite 或 Texture2D 资源路径；省略时使用 UXTool 默认背景。")] string designImage = null,
            [CliArg("tint", "背景颜色：r,g,b 或 r,g,b,a，每个分量必须在 0..1 范围内。")] string color = null)
        {
            return Task.FromResult(AddBackgroundCore(designImage, color));
        }

        private static object AddBackgroundCore(string designImagePath, string colorString)
        {
            Sprite sprite = null;
            Texture2D texture = null;
            if (!string.IsNullOrEmpty(designImagePath))
            {
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(designImagePath);
                if (sprite == null)
                {
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(designImagePath);
                }
                else
                {
                    texture = sprite.texture;
                }

                if (sprite == null && texture == null)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        $"Design image not found or is not a Sprite/Texture2D: {designImagePath}");
                }
            }

            Color? requestedColor = string.IsNullOrEmpty(colorString) ? null : ParseColor(colorString);

            QuickBackground.CreateBackGround();

            GameObject bgRoot = FindQuickBackground();
            if (bgRoot == null)
            {
                throw new GameCommandException(ErrorCode,
                    "QuickBackground.CreateBackGround() failed. Check whether UXTool QuickBackground is enabled.");
            }

            Transform imageTransform = bgRoot.transform.childCount > 0 ? bgRoot.transform.GetChild(0) : null;
            if (imageTransform == null)
            {
                throw new GameCommandException(ErrorCode, "UXQuickBackground has no child image node.");
            }

            Image image = imageTransform.GetComponent<Image>();
            RectTransform rect = imageTransform.GetComponent<RectTransform>();
            if (image == null || rect == null)
            {
                throw new GameCommandException(ErrorCode,
                    "UXQuickBackground child must contain both Image and RectTransform components.");
            }

            if (!string.IsNullOrEmpty(designImagePath))
            {
                if (sprite == null)
                {
                    sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                    sprite.name = texture.name;
                    sprite.hideFlags = HideFlags.DontSave;
                }

                image.sprite = sprite;
                rect.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
            }

            if (requestedColor.HasValue)
            {
                image.color = requestedColor.Value;
            }

            string resultPath = image != null && image.sprite != null && image.sprite.texture != null
                ? AssetDatabase.GetAssetPath(image.sprite.texture)
                : string.Empty;
            object resultSize = rect != null
                ? new { width = Mathf.RoundToInt(rect.sizeDelta.x), height = Mathf.RoundToInt(rect.sizeDelta.y) }
                : null;

            return new
            {
                action = "add_background",
                designImage = resultPath,
                size = resultSize
            };
        }

        private static GameObject FindQuickBackground()
        {
            GameObject go = GameObject.Find("/UXQuickBackground");
            if (go != null)
            {
                return go;
            }

            PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                Transform transform = prefabStage.prefabContentsRoot.transform.Find("UXQuickBackground");
                if (transform != null)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }

        private static Color ParseColor(string colorString)
        {
            string[] parts = colorString.Split(',');
            if (parts.Length != 3 && parts.Length != 4)
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    $"color must contain exactly 3 or 4 comma-separated components, got {parts.Length}.");
            }

            var components = new float[4] { 0f, 0f, 0f, 1f };
            for (int i = 0; i < parts.Length; i++)
            {
                string value = parts[i].Trim();
                if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture,
                        out float component) || float.IsNaN(component) || float.IsInfinity(component) ||
                    component < 0f || component > 1f)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        $"color component {i + 1} must be a finite 0-1 number using '.', got '{value}'.");
                }
                components[i] = component;
            }

            return new Color(components[0], components[1], components[2], components[3]);
        }

    }
#pragma warning restore ET0501
}
