using System.Collections.Generic;
using GameFramework;
using UnityEditor;

namespace UGF.Editor
{
    public class SpritePostprocessor : AssetPostprocessor
    {
        private readonly List<string> SpritePaths = new List<string>()
        {
            "Assets/Res/UI/UISprite"
        };

        private void OnPreprocessTexture()
        {
            TextureImporter textureImporter = assetImporter as TextureImporter;
            string regularPath = Utility.Path.GetRegularPath(textureImporter.assetPath);
            foreach (var path in SpritePaths)
            {
                if (regularPath.StartsWith(path))
                {
                    if (textureImporter.textureType == TextureImporterType.Default)
                    {
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.mipmapEnabled = false;
                    }
                    break;
                }
            }
        }
    }
}