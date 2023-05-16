using System.IO;
using System.Linq;
using HybridCLR.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game.Editor
{
    public static class HybridCLRTool
    {
        public static void EnableHybridCLR()
        {
            HybridCLRSettings.Instance.enable = true;
            string linkFile = $"{Application.dataPath}/{HybridCLRSettings.Instance.outputLinkFile}";
            string linkDisableFile = $"{linkFile}.DISABLED";
            if (File.Exists(linkDisableFile))
            {
                File.Move(linkDisableFile, linkFile);
                File.Delete(linkDisableFile);
                File.Delete($"{linkDisableFile}.meta");
            }
        }

        public static void DisableHybridCLR()
        {
            HybridCLRSettings.Instance.enable = false;
            string linkFile = $"{Application.dataPath}/{HybridCLRSettings.Instance.outputLinkFile}";
            Debug.Log(linkFile);
            string linkDisableFile = $"{linkFile}.DISABLED";
            if (File.Exists(linkFile))
            {
                File.Move(linkFile, linkDisableFile);
                File.Delete(linkFile);
                File.Delete($"{linkFile}.meta");
            }
        }

        public static void AddHotfixAssemblyDefinition(string fileName)
        {
            fileName += ".asmdef";
            string guid = AssetDatabase.FindAssets("t:asmdef").First(a => AssetDatabase.GUIDToAssetPath(a).EndsWith(fileName));
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AssemblyDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath);
            if (asset == null)
                return;
            var allAssets = HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.ToList();
            if (allAssets.Contains(asset))
                return;
            allAssets.Add(asset);
            HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = allAssets.ToArray();
        }
        
        public static void RemoveHotfixAssemblyDefinition(string fileName)
        {
            fileName += ".asmdef";
            string guid = AssetDatabase.FindAssets("t:asmdef").First(a => AssetDatabase.GUIDToAssetPath(a).EndsWith(fileName));
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AssemblyDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath);
            if (asset == null)
                return;
            var allAssets = HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.ToList();
            if (!allAssets.Contains(asset))
                return;
            allAssets.Remove(asset);
            HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = allAssets.ToArray();
        }

        public static void Save()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            HybridCLRSettings.Save();
        }
    }
}
