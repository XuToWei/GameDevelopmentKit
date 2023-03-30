using System.IO;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Editor{
    public static class HybridCLREditor
    {
        [MenuItem("HybridCLR/CopyAotDlls")]
        public static void CopyAotDll()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            string fromDir = Path.Combine(HybridCLRSettings.Instance.strippedAOTDllOutputRootDir, target.ToString());
            const string toDir = "Assets/Res/HybridCLR";
            FileTool.CleanDirectoryFiles(toDir, "*.dll.bytes");
            foreach (string aotDll in HybridCLRSettings.Instance.patchAOTAssemblies)
            {
                File.Copy(Path.Combine(fromDir, aotDll), Path.Combine(toDir, $"{aotDll}.bytes"), true);
            }
            AssetDatabase.ImportAsset(toDir, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            // 设置aot dlls的group
            HybridCLRConfig hybridCLRConfig = AssetDatabase.LoadAssetAtPath<HybridCLRConfig>(HybridCLRHelper.ConfigAsset);
            hybridCLRConfig.aotAssemblies = new TextAsset[HybridCLRSettings.Instance.patchAOTAssemblies.Length];
            for (int i = 0; i < HybridCLRSettings.Instance.patchAOTAssemblies.Length; i++)
            {
                hybridCLRConfig.aotAssemblies[i] = AssetDatabase.LoadAssetAtPath<TextAsset>(
                    Path.Combine(toDir, $"{HybridCLRSettings.Instance.patchAOTAssemblies[i]}.bytes"));
            }
            EditorUtility.SetDirty(hybridCLRConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}