using System;
using System.Collections.Generic;
using GameFramework;
using UnityEditor;

namespace Game.Editor
{
    public class SpriteAssetPostprocessor : AssetPostprocessor
    {
        private readonly HashSet<string> spritePaths = new HashSet<string>()
        {
            "Assets/Res/Sprite/",
            "Assets/Res/UI/UISprite/"
        };

        private void OnPreprocessTexture()
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            string aPath = Utility.Path.GetRegularPath(textureImporter.assetPath);
            foreach (var spritePath in spritePaths)
            {
                if (aPath.StartsWith(spritePath, StringComparison.Ordinal))
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.mipmapEnabled = false;
                    break;
                }
            }
        }
    }
}