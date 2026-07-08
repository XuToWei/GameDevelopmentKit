using AgentBridge;
using Newtonsoft.Json.Linq;
using ThunderFireUITool;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Editor
{
    /// <summary>
    /// Unity Agent Bridge command for small UXTool-backed editor operations.
    /// </summary>
    public sealed class UXToolCommand : ICommandHandler
    {
        private const string ErrorCode = "UXTOOL_ERROR";

        public string Command => "uxtool";
        public string Description => "UXTool 编辑工具：调用 QuickBackground 添加临时设计稿背景。action=add_background；可传 designImage(Sprite/Texture2D 资源路径) 和 color(r,g,b[,a])。背景对象按 UXTool 自身逻辑创建，通常用于编辑预览，不建议当作正式 Prefab 内容。";
        public string Group => "Game";
        public bool CanDisable => true;

        public object Execute(JObject @params)
        {
            string action = GetString(@params, "action", string.Empty).ToLowerInvariant();
            switch (action)
            {
                case "add_background":
                    return AddBackground(@params);
                default:
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Unknown action: {action}. Supported: add_background");
            }
        }

        public JObject GetParamsSchema()
        {
            return JObject.Parse(@"{
  ""type"": ""object"",
  ""properties"": {
    ""action"": {
      ""type"": ""string"",
      ""enum"": [""add_background""],
      ""description"": ""必填。add_background 会调用 UXTool QuickBackground 创建/刷新临时设计稿背景；通常用于编辑预览。""
    },
    ""designImage"": {
      ""type"": ""string"",
      ""description"": ""可选。Sprite 或 Texture2D 资源路径；传入后会设置为背景图，并按纹理尺寸调整背景 RectTransform。""
    },
    ""color"": {
      ""type"": ""string"",
      ""description"": ""可选。背景 tint，格式 r,g,b 或 r,g,b,a，使用 0-1 浮点数；解析失败时使用白色。""
    }
  },
  ""required"": [""action""]
}");
        }

        private static object AddBackground(JObject @params)
        {
            QuickBackground.CreateBackGround();

            GameObject bgRoot = FindQuickBackground();
            if (bgRoot == null)
            {
                throw new CommandException(ErrorCode,
                    "QuickBackground.CreateBackGround() failed. Check whether UXTool QuickBackground is enabled.");
            }

            Transform imageTransform = bgRoot.transform.childCount > 0 ? bgRoot.transform.GetChild(0) : null;
            if (imageTransform == null)
            {
                throw new CommandException(ErrorCode, "UXQuickBackground has no child image node.");
            }

            Image image = imageTransform.GetComponent<Image>();
            RectTransform rect = imageTransform.GetComponent<RectTransform>();
            string designImagePath = GetString(@params, "designImage", string.Empty);

            if (!string.IsNullOrEmpty(designImagePath))
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(designImagePath);
                Texture2D texture = null;
                if (sprite == null)
                {
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(designImagePath);
                    if (texture != null)
                    {
                        sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        sprite.name = texture.name;
                        sprite.hideFlags = HideFlags.DontSave;
                    }
                }
                else
                {
                    texture = sprite.texture;
                }

                if (sprite == null)
                {
                    throw new CommandException(ErrorCode, $"Design image not found or is not a Sprite/Texture2D: {designImagePath}");
                }

                if (image != null)
                {
                    image.sprite = sprite;
                }
                if (rect != null && texture != null)
                {
                    rect.sizeDelta = new Vector2(texture.width, texture.height);
                }
            }

            string colorString = GetString(@params, "color", string.Empty);
            if (!string.IsNullOrEmpty(colorString) && image != null)
            {
                image.color = ParseColor(colorString);
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
            if (parts.Length < 3)
            {
                return Color.white;
            }

            if (!float.TryParse(parts[0].Trim(), out float r) ||
                !float.TryParse(parts[1].Trim(), out float g) ||
                !float.TryParse(parts[2].Trim(), out float b))
            {
                return Color.white;
            }

            float a = 1f;
            if (parts.Length >= 4)
            {
                float.TryParse(parts[3].Trim(), out a);
            }
            return new Color(r, g, b, a);
        }

        private static string GetString(JObject @params, string name, string defaultValue)
        {
            return @params?[name]?.Value<string>() ?? defaultValue;
        }
    }
}
