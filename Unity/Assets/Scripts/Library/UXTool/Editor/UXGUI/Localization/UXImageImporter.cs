using System;
using UnityEditor;
using ThunderFireUITool;
using System.IO;
using System.Linq;
using UnityGameFramework.Extension;

namespace UnityEngine.UI
{
    class MyTexturePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            foreach (var deletedAsset in deletedAssets)
            {
                if (!deletedAsset.Contains(UXGUIConfig.LocalizationFolder))
                {
                    continue;
                }
                string assetPath = Path.ChangeExtension(deletedAsset, ".asset");
                if (File.Exists(assetPath))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
            }
            for(int i = 0; i < movedAssets.Length; i++)
            {
                if (!movedAssets[i].Contains(UXGUIConfig.LocalizationFolder))
                {
                    continue;
                }
                Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(movedAssets[i]);
                TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(movedAssets[i]);
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    continue;
                }
                string oldAssetPath = Path.ChangeExtension(movedFromAssetPaths[i], ".asset");
                string newAssetPath = Path.ChangeExtension(movedAssets[i], ".asset");
                if (File.Exists(oldAssetPath))
                {
                    AssetDatabase.MoveAsset(oldAssetPath, newAssetPath);
                    SpriteCollection spriteCollection = AssetDatabase.LoadAssetAtPath<SpriteCollection>(newAssetPath);
                    spriteCollection.Objects.Clear();
                    spriteCollection.Objects.Add(texture2D);
                    spriteCollection.Pack();
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    if(File.Exists(newAssetPath))
                    {
                        SpriteCollection spriteCollection = AssetDatabase.LoadAssetAtPath<SpriteCollection>(newAssetPath);
                        spriteCollection.Objects.Clear();
                        spriteCollection.Objects.Add(texture2D);
                        spriteCollection.Pack();
                        AssetDatabase.SaveAssets();
                    }
                    else
                    {
                        SpriteCollection spriteCollection = ScriptableObject.CreateInstance<SpriteCollection>();
                        spriteCollection.Objects.Clear();
                        spriteCollection.Objects.Add(texture2D);
                        spriteCollection.Pack();
                        AssetDatabase.CreateAsset(spriteCollection, newAssetPath);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }

        private void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            if (sprites.Length < 1)
            {
                return;
            }
            string texturePath = AssetDatabase.GetAssetPath(texture);
            if (!texturePath.Contains(UXGUIConfig.LocalizationFolder))
            {
                return;
            }
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
            if (importer.textureType != TextureImporterType.Sprite)
            {
                return;
            }
            string collectionPath = Path.ChangeExtension(texturePath, ".asset");
            if (File.Exists(collectionPath))
            {
                SpriteCollection spriteCollection = AssetDatabase.LoadAssetAtPath<SpriteCollection>(collectionPath);
                spriteCollection.Objects.Clear();
                spriteCollection.Objects.Add(texture);
                spriteCollection.Pack();
                AssetDatabase.SaveAssets();
            }
            else
            {
                SpriteCollection spriteCollection = ScriptableObject.CreateInstance<SpriteCollection>();
                spriteCollection.Objects.Clear();
                spriteCollection.Objects.Add(texture);
                spriteCollection.Pack();
                AssetDatabase.CreateAsset(spriteCollection, collectionPath);
                AssetDatabase.SaveAssets();
            }
        }

        void OnPreprocessTexture()
        {
            if (assetPath.Contains(UXGUIConfig.LocalizationFolder) && SwitchSetting.CheckValid(SwitchSetting.SwitchType.AutoConvertTex))
            {
                TextureImporter importer = (TextureImporter)assetImporter;
                importer.textureType = TextureImporterType.Sprite;
                string collectionPath = Path.ChangeExtension(assetPath, ".asset");
                Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (File.Exists(collectionPath))
                {
                    SpriteCollection spriteCollection = AssetDatabase.LoadAssetAtPath<SpriteCollection>(collectionPath);
                    spriteCollection.Objects.Clear();
                    spriteCollection.Objects.Add(texture2D);
                    spriteCollection.Pack();
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    SpriteCollection spriteCollection = ScriptableObject.CreateInstance<SpriteCollection>();
                    spriteCollection.Objects.Clear();
                    spriteCollection.Objects.Add(texture2D);
                    spriteCollection.Pack();
                    AssetDatabase.CreateAsset(spriteCollection, collectionPath);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }

    public class UXImageImporter
    {
        [MenuItem(ThunderFireUIToolConfig.Menu_Localization + "/导入图片包 (Import Localization Images)", false, 54)]
        private static void ImportImages()
        {
            string path = Utils.SelectFolder(false);
            if (path != null)
            {
                string[] files = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
                foreach (LocalizationHelper.LanguageType languageType in EditorLocalizationTool.ReadyLanguageTypes)
                {
                    Directory.CreateDirectory($"{UXGUIConfig.LocalizationFolder}/{languageType}");
                }
                foreach (string file in files)
                {
                    string dirName = Path.GetDirectoryName(file);
                    if(!Enum.TryParse(dirName, out LocalizationHelper.LanguageType languageType))
                        continue;
                    if(!EditorLocalizationTool.ReadyLanguageTypes.Contains(languageType))
                        continue;
                    string fileName = file.Split('\\', '/').Last();
                    string dest = $"{UXGUIConfig.LocalizationFolder}/{languageType}/{fileName}";
                    File.Copy(file, dest, true);
                    AssetDatabase.ImportAsset(dest);
                }
            }
        }
    }
}