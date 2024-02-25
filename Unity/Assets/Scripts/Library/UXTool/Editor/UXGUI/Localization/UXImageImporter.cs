using System;
using UnityEditor;
using UnityEngine;
using ThunderFireUITool;
using System.IO;
using System.Linq;

namespace UnityEngine.UI
{
    class MyTexturePostprocessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (assetPath.Contains(UXGUIConfig.LocalizationFolder) && SwitchSetting.CheckValid(SwitchSetting.SwitchType.AutoConvertTex))
            {
                TextureImporter importer = (TextureImporter)assetImporter;
                importer.textureType = TextureImporterType.Sprite;
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