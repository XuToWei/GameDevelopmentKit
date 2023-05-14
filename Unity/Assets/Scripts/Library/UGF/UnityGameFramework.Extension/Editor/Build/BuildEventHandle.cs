using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFramework.Extension.Editor
{
    public class BuildEventHandle : IBuildEventHandler
    {
        public bool ContinueOnFailure
        {
            get { return false; }
        }

        public void OnPreprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion,
            Platform platforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName,
            bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName,
            string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath,
            bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath,
            bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
            string streamingAssetsPath =
                Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets"));
            string[] fileNames = Directory.GetFiles(streamingAssetsPath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                if (fileName.Contains(".dat"))
                {
                    File.Delete(fileName);
                }
            }
            Utility.Path.RemoveEmptyDirectory(streamingAssetsPath);
            
            ResourceRuleEditor ruleEditor = ScriptableObject.CreateInstance<ResourceRuleEditor>();
            ruleEditor.RefreshResourceCollection();
            SpriteCollectionUtility.RefreshSpriteCollection();

            TypeCache.MethodCollection methodCollection = TypeCache.GetMethodsWithAttribute<UGFPreprocessBuildEventAttribute>();
            var methodInfos = methodCollection.ToList();
            for (int i = methodInfos.Count - 1; i >= 0; i--)
            {
                if (!methodInfos[i].IsStatic)
                {
                    methodInfos.RemoveAt(i);
                }
            }

            methodInfos.Sort((a, b) =>
                a.GetCustomAttribute<UGFPreprocessBuildEventAttribute>().CallbackOrder
                    .CompareTo(b.GetCustomAttribute<UGFPreprocessBuildEventAttribute>().CallbackOrder));

            foreach (var methodInfo in methodInfos)
            {
                methodInfo.Invoke(null, null);
            }
        }

        public void OnPreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected,
            string outputPackagePath,
            bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath)
        {
        }

        public void OnBuildAssetBundlesComplete(Platform platform, string workingPath, bool outputPackageSelected,
            string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected,
            string outputPackedPath, AssetBundleManifest assetBundleManifest)
        {
        }

        public void OnOutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength,
            int versionListHashCode, int versionListZipLength, int versionListZipHashCode)
        {
            ResourceBuilderController builderController = new ResourceBuilderController();
            if (!builderController.Load())
            {
                Debug.LogError("OnOutputUpdatableVersionListData Fail! ResourceBuilderController Load Fail!");
                return;
            }

            VersionInfoEditorData versionInfoEditorData =
                AssetDatabase.LoadAssetAtPath<VersionInfoEditorData>(
                    "Assets/Res/Editor/Config/VersionInfoEditorData.asset");
            if (versionInfoEditorData == null)
            {
                versionInfoEditorData = ScriptableObject.CreateInstance<VersionInfoEditorData>();
                versionInfoEditorData.VersionInfos = new List<VersionInfoWrapData> { new VersionInfoWrapData() { Key = "Normal", Value = new VersionInfoData() } };
                AssetDatabase.CreateAsset(versionInfoEditorData, "Assets/Res/Editor/Config/VersionInfoEditorData.asset");
                Debug.Log("CreateVersionInfoEditorData Success!");
                AssetDatabase.Refresh();
                Selection.activeObject = versionInfoEditorData;
            }

            VersionInfoData versionInfoData = versionInfoEditorData.GetActiveVersionInfoData();
            versionInfoData.AutoIncrementInternalGameVersion();
            versionInfoData.ForceUpdateGame = false;
            versionInfoData.ResourceVersion = builderController.ApplicableGameVersion.Replace('.', '_') + "_" + builderController.InternalResourceVersion;
            versionInfoData.Platform = (Platform)(int)platform;
            versionInfoData.LatestGameVersion = builderController.ApplicableGameVersion;
            versionInfoData.InternalResourceVersion = builderController.InternalResourceVersion;
            versionInfoData.VersionListLength = versionListLength;
            versionInfoData.VersionListHashCode = versionListHashCode;
            versionInfoData.VersionListCompressedLength = versionListZipLength;
            versionInfoData.VersionListCompressedHashCode = versionListZipHashCode;
            EditorUtility.SetDirty(versionInfoEditorData);
            AssetDatabase.SaveAssets();

            if (versionInfoEditorData.IsGenerateToFullPath)
            {
                string filePath = Path.Combine(builderController.OutputFullPath, platform.ToString(), $"{platform}Version.txt");
                if (versionInfoEditorData.Generate(filePath))
                {
                    File.Copy(filePath, Path.Combine(builderController.OutputDirectory, $"Full/{platform}Version.txt"), true);
                }
            }
        }

        public void OnPostprocessPlatform(Platform platform, string workingPath,
            bool outputPackageSelected, string outputPackagePath,
            bool outputFullSelected, string outputFullPath,
            bool outputPackedSelected, string outputPackedPath,
            bool isSuccess)
        {
            //如果有一下多个平台，自行处理

            void CopyResource(string outputPath)
            {
                string streamingAssetsPath =
                    Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets"));
                string[] fileNames = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories);
                foreach (string fileName in fileNames)
                {
                    string destFileName = Utility.Path.GetRegularPath(Path.Combine(streamingAssetsPath,
                        fileName.Substring(outputPath.Length)));
                    FileInfo destFileInfo = new FileInfo(destFileName);
                    if (destFileInfo.Directory is { Exists: false })
                    {
                        destFileInfo.Directory.Create();
                    }

                    File.Copy(fileName, destFileName);
                }
            }

            if (outputPackedSelected)
            {
                CopyResource(outputPackedPath);
            }
            else if (outputPackageSelected)
            {
                CopyResource(outputPackagePath);
            }
        }

        public void OnPostprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion,
            Platform platforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName,
            bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName,
            string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath,
            bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath,
            bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
        }
    }
}