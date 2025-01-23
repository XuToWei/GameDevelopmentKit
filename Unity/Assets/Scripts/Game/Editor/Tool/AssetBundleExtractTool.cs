using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    public class AssetBundleExtractTool
    {
        /// <summary>
        /// 导出Working目录下所有的AssetBundle为明文
        /// </summary>
        [MenuItem("Game/Tool/ExtractAllWorkingAssetBundle")]
        public static void ExtractAllWorkingAssetBundle()
        {
            string assetBundleDirectory = $"{ResourceBuildHelper.OutputDirectory}/Working";
            string[] assetBundleFiles = Directory.GetFiles(assetBundleDirectory, "*", SearchOption.AllDirectories);
            async UniTask RunAsync()
            {
                List<UniTask> tasks = new List<UniTask>();
                foreach (var assetBundleFile in assetBundleFiles)
                {
                    if(!File.Exists(assetBundleFile))
                        continue;
                    if(Path.HasExtension(assetBundleFile))
                        continue;
                    //排除scene
                    if(File.Exists($"{assetBundleFile}.sharedAssets"))
                        continue;
                    //排除已导出的
                    if(File.Exists($"{assetBundleFile}.resS"))
                        continue;
                    //排除平台AssetBundle(没有resS文件)
                    if(Path.GetFileName(assetBundleFile).StartsWith("CAB-"))
                        continue;
                    tasks.Add(ExtractAssetBundleAsync(assetBundleFile));
                }
                await UniTask.WhenAll(tasks);
            }
            RunAsync().Forget();
        }
        
        /// <summary>
        /// 导出AssetBundle为明文
        /// </summary>
        /// <param name="assetBundleFile">AssetBundle文件路径</param>
        public static async UniTask ExtractAssetBundleAsync(string assetBundleFile)
        {
            string workDirectory = $"{EditorApplication.applicationPath}/../Data/Tools/";
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            string webExtractExe = "./WebExtract";
            string binary2TextExe = "./binary2text";
            await ShellTool.RunAsync($"{webExtractExe} {assetBundleFile}", workDirectory);
#else
            string webExtractExe = ".\\WebExtract.exe";
            string binary2TextExe = ".\\binary2text.exe";
            await ShellTool.RunAsync($"{webExtractExe} {assetBundleFile}", workDirectory, "GB2312");
#endif
            string fileName = Path.GetFileName(assetBundleFile);
            string outputDirectoryName = $"{Path.GetDirectoryName(assetBundleFile)}/{fileName}_data";
            string[] files = Directory.GetFiles(outputDirectoryName).Where(file => string.IsNullOrEmpty(Path.GetExtension(file))).ToArray();
            List<UniTask> tasks = new List<UniTask>();
            foreach (var file in files)
            {
                tasks.Add(ShellTool.RunAsync($"{binary2TextExe} {file}", workDirectory));
            }
            await UniTask.WhenAll(tasks);
        }
    }
}
