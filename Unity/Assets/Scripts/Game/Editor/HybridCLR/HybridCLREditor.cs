using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class HybridCLREditor
    {
        static readonly string s_ResDir = "Assets/Res/HybridCLR";
        
        [MenuItem("HybridCLR/CopyAotDlls")]
        public static void CopyAotDll()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            string fromDir = Path.Combine(HybridCLRSettings.Instance.strippedAOTDllOutputRootDir, target.ToString());
            FileTool.CleanDirectoryFiles(s_ResDir, "*.dll.bytes");
            FileTool.CleanDirectoryFiles(s_ResDir, "*.dll.bytes.meta");
            foreach (string aotDll in HybridCLRSettings.Instance.patchAOTAssemblies)
            {
                string file = Path.Combine(fromDir, $"{aotDll}.dll");
                if(!File.Exists(file))
                    continue;
                File.Copy(file, Path.Combine(s_ResDir, $"{aotDll}.dll.bytes"), true);
            }
            AssetDatabase.ImportAsset(s_ResDir, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            // 设置aot dlls
            List<TextAsset> aotAssemblyList = new List<TextAsset>();
            for (int i = 0; i < HybridCLRSettings.Instance.patchAOTAssemblies.Length; i++)
            {
                string file = Path.Combine(s_ResDir, $"{HybridCLRSettings.Instance.patchAOTAssemblies[i]}.dll.bytes");
                if(!File.Exists(file))
                    continue;
                aotAssemblyList.Add(AssetDatabase.LoadAssetAtPath<TextAsset>(file));
            }
            HybridCLRConfig hybridCLRConfig = AssetDatabase.LoadAssetAtPath<HybridCLRConfig>(HybridCLRHelper.ConfigAsset);
            if (hybridCLRConfig == null)
            {
                hybridCLRConfig = ScriptableObject.CreateInstance<HybridCLRConfig>();
                AssetDatabase.CreateAsset(hybridCLRConfig, HybridCLRHelper.ConfigAsset);
            }
            hybridCLRConfig.aotAssemblies = aotAssemblyList.ToArray();
            EditorUtility.SetDirty(hybridCLRConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Debug.Log($"{HybridCLRHelper.ConfigAsset} 链接AotDlls！");
        }

        [MenuItem("HybridCLR/Do All", false, int.MaxValue)]
        public static void HybridCLRDoAll()
        {
            EditorApplication.ExecuteMenuItem("Game/Define Symbol/Refresh");
#if UNITY_HOTFIX && UNITY_GAMEHOT
            EditorApplication.ExecuteMenuItem("GameHot/Compile Dll");
#endif
#if UNITY_HOTFIX && UNITY_ET
            EditorApplication.ExecuteMenuItem("ET/Compile Dll");
#endif
            EditorApplication.ExecuteMenuItem("HybridCLR/Generate/All");
            EditorApplication.ExecuteMenuItem("HybridCLR/CopyAotDlls");
        }
    }
}