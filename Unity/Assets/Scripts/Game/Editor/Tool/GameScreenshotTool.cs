using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// Synchronously captures the current Game View to a JPG file.
    /// </summary>
    public static class GameScreenshotTool
    {
        private const int DefaultJpgQuality = 85;
        private const long MaxPixels = 8 * 1024 * 1024;

        /// <summary>
        /// Captures the current Game View and writes the JPG before returning.
        /// This method must be called from Unity's main thread.
        /// </summary>
        public static GameScreenshotResult CaptureGameView(
            string outputPath,
            int quality = DefaultJpgQuality,
            bool overwrite = false)
        {
            ValidateQuality(quality);
            string path = ResolveOutputPath(outputPath);
            Texture2D texture = null;
            try
            {
                texture = ScreenCapture.CaptureScreenshotAsTexture();
                if (texture == null)
                {
                    throw new InvalidOperationException(
                        "无法捕获 Game View：截图纹理为空。请确保 Game View 已打开并完成渲染。");
                }

                ValidateSize(texture.width, texture.height);
                byte[] bytes = texture.EncodeToJPG(quality);
                if (bytes == null || bytes.Length == 0)
                {
                    throw new InvalidOperationException("Game View 截图 JPG 编码失败。");
                }

                if (overwrite)
                {
                    File.WriteAllBytes(path, bytes);
                }
                else
                {
                    using (FileStream stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
                return new GameScreenshotResult(
                    path, texture.width, texture.height, quality, bytes.LongLength);
            }
            finally
            {
                if (texture != null)
                {
                    UnityEngine.Object.DestroyImmediate(texture);
                }
            }
        }

        private static string ResolveOutputPath(string outputPath)
        {
            string path = Path.IsPathRooted(outputPath)
                ? outputPath
                : Path.Combine(
                    Directory.GetParent(Path.GetFullPath(Application.dataPath)).FullName,
                    outputPath);
            return string.IsNullOrEmpty(Path.GetExtension(path)) ? $"{path}.jpg" : path;
        }

        private static void ValidateQuality(int quality)
        {
            if (quality < 1 || quality > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quality), quality, "JPG quality 必须在 1 到 100 之间。");
            }
        }

        private static void ValidateSize(int width, int height)
        {
            int maxSide = SystemInfo.maxTextureSize > 0 ? SystemInfo.maxTextureSize : 8192;
            if (width <= 0 || height <= 0 || width > maxSide || height > maxSide ||
                (long)width * height > MaxPixels)
            {
                throw new InvalidOperationException(
                    $"Game View 尺寸 {width}x{height} 超出安全上限（单边 {maxSide}，总像素 {MaxPixels}）。");
            }
        }
    }

    /// <summary>
    /// Metadata for a synchronously written Game View JPG.
    /// </summary>
    public readonly struct GameScreenshotResult
    {
        internal GameScreenshotResult(string path, int width, int height, int quality, long fileByteLength)
        {
            Path = path;
            Width = width;
            Height = height;
            Quality = quality;
            FileByteLength = fileByteLength;
        }

        public string Path { get; }
        public int Width { get; }
        public int Height { get; }
        public int Quality { get; }
        public long FileByteLength { get; }
    }
}
