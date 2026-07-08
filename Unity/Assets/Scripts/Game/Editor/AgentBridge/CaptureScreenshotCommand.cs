using System;
using System.IO;
using AgentBridge;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Editor
{
    /// <summary>
    /// Unity Agent Bridge command for visual verification screenshots.
    /// </summary>
    public sealed class CaptureScreenshotCommand : ICommandHandler
    {
        private const string ErrorCode = "SCREENSHOT_ERROR";
        private const string GameViewTarget = "game_view";
        private const string SceneViewTarget = "scene_view";
        private const int MinSize = 16;
        private const int MaxSize = 8192;

        public string Command => "capture_screenshot";
        public string Description => "截图用于可视化验证：target=game_view/scene_view，输出 PNG 到 .agentbridge/artifacts/screenshots/ 并返回绝对路径。";
        public string Group => "Game";
        public bool CanDisable => true;

        public object Execute(JObject @params)
        {
            ValidateKnownParams(@params);

            string target = GetString(@params, "target", SceneViewTarget).ToLowerInvariant();
            bool hasWidth = HasParam(@params, "width");
            bool hasHeight = HasParam(@params, "height");
            if (hasWidth != hasHeight)
            {
                throw new CommandException(ErrorCodes.InvalidParams, "width and height must be provided together.");
            }

            int? requestedWidth = null;
            int? requestedHeight = null;
            if (hasWidth)
            {
                requestedWidth = GetInteger(@params, "width");
                requestedHeight = GetInteger(@params, "height");
                ValidateDimension("width", requestedWidth.Value);
                ValidateDimension("height", requestedHeight.Value);
            }

            byte[] pngBytes;
            int width;
            int height;
            switch (target)
            {
                case GameViewTarget:
                    if (hasWidth || hasHeight)
                    {
                        throw new CommandException(ErrorCodes.InvalidParams,
                            "width and height are only supported for target=scene_view.");
                    }
                    pngBytes = CaptureGameView(out width, out height);
                    break;
                case SceneViewTarget:
                    pngBytes = CaptureSceneView(requestedWidth, requestedHeight, out width, out height);
                    break;
                default:
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Unknown target: {target}. Supported: {GameViewTarget}, {SceneViewTarget}.");
            }

            string rootDir = GetBridgeRootDir();
            string fileName = GetSafeFileName(GetString(@params, "fileName", string.Empty), target);
            string relativePath = Path.Combine("artifacts", "screenshots", fileName).Replace('\\', '/');
            string fullPath = Path.GetFullPath(Path.Combine(rootDir, "artifacts", "screenshots", fileName));
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllBytes(fullPath, pngBytes);

            return new
            {
                path = fullPath,
                relativePath,
                target,
                width,
                height,
                bytes = pngBytes.Length
            };
        }

        public JObject GetParamsSchema()
        {
            return JObject.Parse(@"{
  ""type"": ""object"",
  ""properties"": {
    ""target"": {
      ""type"": ""string"",
      ""default"": ""scene_view"",
      ""enum"": [""game_view"", ""scene_view""],
      ""description"": ""可选。scene_view 渲染当前 Scene View 摄像机；game_view 捕获当前 Game View。默认 scene_view。""
    },
    ""fileName"": {
      ""type"": ""string"",
      ""description"": ""可选。PNG 文件名，不允许目录分隔符或 ..；未传时自动生成。""
    },
    ""width"": {
      ""type"": ""integer"",
      ""minimum"": 16,
      ""maximum"": 8192,
      ""description"": ""可选，仅 scene_view 使用；必须与 height 同时提供。""
    },
    ""height"": {
      ""type"": ""integer"",
      ""minimum"": 16,
      ""maximum"": 8192,
      ""description"": ""可选，仅 scene_view 使用；必须与 width 同时提供。""
    }
  },
  ""additionalProperties"": false
}");
        }

        private static byte[] CaptureGameView(out int width, out int height)
        {
            Texture2D texture = null;
            try
            {
                if (!EditorApplication.isPlaying)
                {
                    throw new CommandException(ErrorCode,
                        "Game View screenshot requires Play Mode. Use target=scene_view for editor visual verification.");
                }

                texture = ScreenCapture.CaptureScreenshotAsTexture();
                if (texture == null)
                {
                    throw new CommandException(ErrorCode,
                        "Game View screenshot returned null. Make sure the Game View has rendered and Unity is not running headless.");
                }

                width = texture.width;
                height = texture.height;
                if (width <= 0 || height <= 0)
                {
                    throw new CommandException(ErrorCode, $"Game View screenshot has invalid size: {width}x{height}.");
                }

                return EncodePng(texture, GameViewTarget);
            }
            catch (CommandException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CommandException(ErrorCode, $"Failed to capture Game View screenshot: {ex.Message}");
            }
            finally
            {
                if (texture != null)
                {
                    Object.DestroyImmediate(texture);
                }
            }
        }

        private static byte[] CaptureSceneView(int? requestedWidth, int? requestedHeight, out int width, out int height)
        {
            SceneView sceneView = SceneView.lastActiveSceneView ?? GetFirstSceneView();
            if (sceneView == null)
            {
                throw new CommandException(ErrorCode, "No Scene View is available to capture.");
            }

            Camera camera = sceneView.camera;
            if (camera == null)
            {
                throw new CommandException(ErrorCode, "The selected Scene View has no camera.");
            }

            width = requestedWidth ?? GetDefaultWidth(sceneView, camera);
            height = requestedHeight ?? GetDefaultHeight(sceneView, camera);
            ValidateDimension("width", width);
            ValidateDimension("height", height);

            RenderTexture previousTarget = camera.targetTexture;
            RenderTexture previousActive = RenderTexture.active;
            RenderTexture renderTexture = null;
            Texture2D texture = null;

            try
            {
                sceneView.Repaint();
                renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
                camera.targetTexture = renderTexture;
                camera.Render();

                RenderTexture.active = renderTexture;
                texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture.Apply();

                return EncodePng(texture, SceneViewTarget);
            }
            catch (CommandException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CommandException(ErrorCode, $"Failed to capture Scene View screenshot: {ex.Message}");
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;

                if (renderTexture != null)
                {
                    RenderTexture.ReleaseTemporary(renderTexture);
                }
                if (texture != null)
                {
                    Object.DestroyImmediate(texture);
                }
            }
        }

        private static byte[] EncodePng(Texture2D texture, string target)
        {
            byte[] pngBytes = texture.EncodeToPNG();
            if (pngBytes == null || pngBytes.Length == 0)
            {
                throw new CommandException(ErrorCode, $"Failed to encode {target} screenshot as PNG.");
            }
            return pngBytes;
        }

        private static SceneView GetFirstSceneView()
        {
            foreach (object item in SceneView.sceneViews)
            {
                SceneView view = item as SceneView;
                if (view != null)
                {
                    return view;
                }
            }
            return null;
        }

        private static int GetDefaultWidth(SceneView sceneView, Camera camera)
        {
            int width = camera.pixelWidth;
            if (width < MinSize)
            {
                width = Mathf.RoundToInt(sceneView.position.width);
            }
            return width >= MinSize ? width : 1280;
        }

        private static int GetDefaultHeight(SceneView sceneView, Camera camera)
        {
            int height = camera.pixelHeight;
            if (height < MinSize)
            {
                height = Mathf.RoundToInt(sceneView.position.height);
            }
            return height >= MinSize ? height : 720;
        }

        private static string GetBridgeRootDir()
        {
            string rootDir = BridgeSettings.RootDir;
            if (string.IsNullOrEmpty(rootDir))
            {
                rootDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, ".agentbridge");
            }
            else if (!Path.IsPathRooted(rootDir))
            {
                string projectRoot = Directory.GetParent(Application.dataPath).FullName;
                rootDir = Path.Combine(projectRoot, rootDir);
            }
            return Path.GetFullPath(rootDir);
        }

        private static string GetSafeFileName(string fileName, string target)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"{DateTime.Now:yyyyMMdd-HHmmss-fff}-{target}.png";
            }

            if (Path.IsPathRooted(fileName) || fileName.Contains("/") || fileName.Contains("\\") || fileName.Contains(".."))
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    "fileName must be a file name only and must not contain path separators or '..'.");
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new CommandException(ErrorCodes.InvalidParams, $"fileName contains invalid characters: {fileName}");
            }

            if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".png";
            }
            return fileName;
        }

        private static void ValidateKnownParams(JObject @params)
        {
            if (@params == null)
            {
                return;
            }

            foreach (JProperty property in @params.Properties())
            {
                switch (property.Name)
                {
                    case "target":
                    case "fileName":
                    case "width":
                    case "height":
                        break;
                    default:
                        throw new CommandException(ErrorCodes.InvalidParams, $"Unknown parameter: {property.Name}");
                }
            }
        }

        private static bool HasParam(JObject @params, string name)
        {
            JToken token;
            return @params != null && @params.TryGetValue(name, out token) && token.Type != JTokenType.Null;
        }

        private static string GetString(JObject @params, string name, string defaultValue)
        {
            if (!HasParam(@params, name))
            {
                return defaultValue;
            }

            JToken token = @params[name];
            if (token.Type != JTokenType.String)
            {
                throw new CommandException(ErrorCodes.InvalidParams, $"{name} must be a string.");
            }
            return token.Value<string>();
        }

        private static int GetInteger(JObject @params, string name)
        {
            JToken token = @params[name];
            if (token == null || token.Type != JTokenType.Integer)
            {
                throw new CommandException(ErrorCodes.InvalidParams, $"{name} must be an integer.");
            }
            return token.Value<int>();
        }

        private static void ValidateDimension(string name, int value)
        {
            if (value < MinSize || value > MaxSize)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"{name} must be between {MinSize} and {MaxSize}, got {value}.");
            }
        }
    }
}
