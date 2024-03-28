using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game.Editor
{
    public static class HybridCLRTool
    {
        public static string ExternalHotUpdateAssemblyDir => "./Temp/HybridCLRBin";

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
            HybridCLRSettings.Save();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public static void DisableHybridCLR()
        {
            HybridCLRSettings.Instance.enable = false;
            string linkFile = $"{Application.dataPath}/{HybridCLRSettings.Instance.outputLinkFile}";
            string linkDisableFile = $"{linkFile}.DISABLED";
            if (File.Exists(linkFile))
            {
                File.Move(linkFile, linkDisableFile);
                File.Delete(linkFile);
                File.Delete($"{linkFile}.meta");
            }
            HybridCLRSettings.Save();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public static void AddHotfixAssemblyDefinition(string fileName)
        {
            fileName += ".asmdef";
            string guid = AssetDatabase.FindAssets("t:asmdef").FirstOrDefault(a => AssetDatabase.GUIDToAssetPath(a).EndsWith(fileName));
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AssemblyDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath);
            if (asset == null)
                return;
            var allAssets = HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.ToList();
            if (allAssets.Contains(asset))
                return;
            allAssets.Add(asset);
            HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = allAssets.ToArray();
            HybridCLRSettings.Save();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        public static void RemoveHotfixAssemblyDefinition(string fileName)
        {
            fileName += ".asmdef";
            string guid = AssetDatabase.FindAssets("t:asmdef").FirstOrDefault(a => AssetDatabase.GUIDToAssetPath(a).EndsWith(fileName));
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AssemblyDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath);
            if (asset == null)
                return;
            var allAssets = HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.ToList();
            if (!allAssets.Contains(asset))
                return;
            allAssets.Remove(asset);
            HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = allAssets.ToArray();
            HybridCLRSettings.Save();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public static void RefreshSettings()
        {
            RefreshExternalHotUpdateDirs();
            RefreshSettingsByLinkXML();
            HybridCLRSettings.Save();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static void RefreshSettingsByLinkXML()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(LinkXMLHelper.LinkXMLPath);
            XmlNode xmlRoot = xmlDocument.SelectSingleNode("linker");
            XmlNodeList xmlLinker = xmlRoot.ChildNodes;

            List<string> assemblyNames = new List<string>();
            for (int i = 0; i < xmlLinker.Count; i++)
            {
                XmlNode xmlNodeString = xmlLinker.Item(i);
                if (xmlNodeString.Name != "assembly")
                {
                    continue;
                }

                string assemblyName = xmlNodeString.Attributes.GetNamedItem("fullname").Value;
                assemblyNames.Add(assemblyName);
            }

            HybridCLRSettings.Instance.patchAOTAssemblies = assemblyNames.ToArray();
        }

        private static void RefreshExternalHotUpdateDirs()
        {
            List<string> dirList = new List<string>();
            string[] dirs = HybridCLRSettings.Instance.externalHotUpdateAssembliyDirs;
            if (dirs != null)
            {
                dirList.AddRange(dirs);
            }
            if (!dirList.Contains(ExternalHotUpdateAssemblyDir))
            {
                dirList.Add(ExternalHotUpdateAssemblyDir);
            }
            HybridCLRSettings.Instance.externalHotUpdateAssembliyDirs = dirList.ToArray();
        }
    }
}
