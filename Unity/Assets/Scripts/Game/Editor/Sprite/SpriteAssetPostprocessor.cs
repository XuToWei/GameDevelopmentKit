using System;
using System.Collections.Generic;
using GameFramework;
using UnityEditor;

namespace Game.Editor
{
    public class SpriteAssetPostprocessor : AssetPostprocessor
    {
        private readonly HashSet<string> m_UISpritePaths = new HashSet<string>()
        {
            "Assets/Res/UI/UISprite/"
        };

        private void OnPreprocessTexture()
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            string aPath = Utility.Path.GetRegularPath(textureImporter.assetPath);
            foreach (var spritePath in m_UISpritePaths)
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