using System.Globalization;
using System.Threading.Tasks;
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
        public CommandBatchMode BatchMode => CommandBatchMode.NotAllowed;

        // AgentBridge's Task<object> contract requires an async method builder, while ET0501
        // normally forbids non-UniTask async methods in project code.
        public Task<object> ExecuteAsync(JObject @params)
        {
            string action = GetString(@params, "action", string.Empty).ToLowerInvariant();
            switch (action)
            {
                case "add_background":
                    return Task.FromResult<object>(AddBackground(@params));
                default:
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Unknown action: {action}. Supported: add_background");
            }
        }

        public JObject ParamsSchema { get; } = JObject.Parse(@"{
  ""type"": ""object"",
  ""additionalProperties"": false,
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
      ""description"": ""可选。背景 tint，格式 r,g,b 或 r,g,b,a；分量必须是使用小数点的 0-1 浮点数，否则返回 INVALID_PARAMS。""
    }
  },
  ""required"": [""action""]
}");

        private static object AddBackground(JObject @params)
        {
            string designImagePath = GetString(@params, "designImage", string.Empty);
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
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Design image not found or is not a Sprite/Texture2D: {designImagePath}");
                }
            }

            string colorString = GetString(@params, "color", string.Empty);
            Color? requestedColor = string.IsNullOrEmpty(colorString) ? null : ParseColor(colorString);

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
            if (image == null || rect == null)
            {
                throw new CommandException(ErrorCode,
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
                throw new CommandException(ErrorCodes.InvalidParams,
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
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"color component {i + 1} must be a finite 0-1 number using '.', got '{value}'.");
                }
                components[i] = component;
            }

            return new Color(components[0], components[1], components[2], components[3]);
        }

        private static string GetString(JObject @params, string name, string defaultValue)
        {
            return @params?[name]?.Value<string>() ?? defaultValue;
        }
    }
}
