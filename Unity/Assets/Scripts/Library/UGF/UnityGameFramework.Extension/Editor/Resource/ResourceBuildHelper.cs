using System;
using System.IO;
using GameFramework;
using GameFramework.Resource;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFramework.Extension.Editor
{
    /// <summary>
    /// 资源生成器。
    /// </summary>
    public static class ResourceBuildHelper
    {
        private static ResourceBuilderController m_Controller = null;
        private static Platform m_OriginalPlatform;
        public const string OutputDirectory = "../Temp/Bundle";

        /// <summary>
        /// build resource
        /// </summary>
        /// <param name="specificPlatform">为Undefined使用设置的平台</param>
        public static void StartBuild(Platform specificPlatform)
        {
            //获取启动场景的资源加载类型来打包
            ResourceMode resourceMode = EntryUtility.GetEntryResourceMode();
            if (resourceMode == ResourceMode.Unspecified)
            {
                throw new GameFrameworkException("Resource mode is invalid.");
            }
            
            m_Controller = new ResourceBuilderController();
            m_Controller.OnLoadingResource += OnLoadingResource;
            m_Controller.OnLoadingAsset += OnLoadingAsset;
            m_Controller.OnLoadCompleted += OnLoadCompleted;
            m_Controller.OnAnalyzingAsset += OnAnalyzingAsset;
            m_Controller.OnAnalyzeCompleted += OnAnalyzeCompleted;
            m_Controller.ProcessingAssetBundle += OnProcessingAssetBundle;
            m_Controller.ProcessingBinary += OnProcessingBinary;
            m_Controller.ProcessResourceComplete += OnProcessResourceComplete;
            m_Controller.BuildResourceError += OnBuildResourceError;

            if (m_Controller.Load())
            {
                if (!string.Equals(m_Controller.OutputDirectory, OutputDirectory))
                {
                    throw new GameFrameworkException($"Please set OutputDirectory: {m_Controller.OutputDirectory} to {OutputDirectory}");
                }
                if (!Directory.Exists(OutputDirectory))
                {
                    Directory.CreateDirectory(OutputDirectory);
                }

                if (resourceMode == ResourceMode.Package)
                {
                    m_Controller.OutputPackageSelected = true;
                    m_Controller.OutputFullSelected = false;
                    m_Controller.OutputPackedSelected = false;
                }
                else
                {
                    m_Controller.OutputPackageSelected = false;
                    m_Controller.OutputFullSelected = true;
                    m_Controller.OutputPackedSelected = true;
                }

                m_OriginalPlatform = m_Controller.Platforms;
                if (specificPlatform != Platform.Undefined)
                {
                    m_Controller.Platforms = specificPlatform;
                }

                Debug.Log("Load configuration success.");

                m_Controller.RefreshCompressionHelper();

                m_Controller.RefreshBuildEventHandler();
            }
            else
            {
                Debug.LogWarning("Load configuration failure.");
            }

            GetBuildMessage(out string buildMessage, out MessageType buildMessageType);
            switch (buildMessageType)
            {
                case MessageType.None:
                case MessageType.Info:
                    Debug.Log(buildMessage);
                    BuildResources();
                    break;
                case MessageType.Warning:
                    Debug.LogWarning(buildMessage);
                    BuildResources();
                    break;
                case MessageType.Error:
                    Debug.LogError(buildMessage);
                    break;
            }
        }

        public static string GetNewestBundlePath()
        {
            if (m_Controller.OutputPackageSelected)
            {
                return m_Controller.OutputPackagePath;
            }
            else
            {
                return m_Controller.OutputPackedPath;
            }
        }

        private static void GetBuildMessage(out string message, out MessageType messageType)
        {
            message = string.Empty;
            messageType = MessageType.Error;
            if (m_Controller.Platforms == Platform.Undefined)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += Environment.NewLine;
                }

                message += $"Platform {m_Controller.Platforms} is invalid.";
            }

            if (string.IsNullOrEmpty(m_Controller.CompressionHelperTypeName))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += Environment.NewLine;
                }

                message += "Compression helper is invalid.";
            }

            if (!m_Controller.IsValidOutputDirectory)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += Environment.NewLine;
                }

                message += $"Output directory {m_Controller.OutputDirectory} is invalid.";
            }

            if (!string.IsNullOrEmpty(message))
            {
                return;
            }

            messageType = MessageType.Info;
            if (Directory.Exists(m_Controller.OutputPackagePath))
            {
                message += Utility.Text.Format("{0} will be overwritten.",
                    m_Controller.OutputPackagePath);
                messageType = MessageType.Warning;
            }

            if (Directory.Exists(m_Controller.OutputFullPath))
            {
                if (message.Length > 0)
                {
                    message += " ";
                }

                message += Utility.Text.Format("{0} will be overwritten.", m_Controller.OutputFullPath);
                messageType = MessageType.Warning;
            }

            if (Directory.Exists(m_Controller.OutputPackedPath))
            {
                if (message.Length > 0)
                {
                    message += " ";
                }

                message += Utility.Text.Format("{0} will be overwritten.", m_Controller.OutputPackedPath);
                messageType = MessageType.Warning;
            }

            if (messageType == MessageType.Warning)
            {
                return;
            }

            message = "Ready to build.";
        }

        private static void BuildResources()
        {
            if (m_Controller.BuildResources())
            {
                Debug.Log("Build resources success.");
                SaveConfiguration();
            }
            else
            {
                Debug.LogError($"Build resources failure. <a href=\"file:///{Utility.Path.GetRegularPath(Path.Combine(m_Controller.BuildReportPath, "BuildLog.txt"))}\" line=\"0\">[ Open BuildLog.txt ]</a>");
            }
        }

        private static void SaveConfiguration()
        {
            m_Controller.Platforms = m_OriginalPlatform;
            if (m_Controller.Save())
            {
                Debug.Log("Save configuration success.");
            }
            else
            {
                Debug.LogWarning("Save configuration failure.");
            }
        }

        private static void OnLoadingResource(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Resources",
                Utility.Text.Format("Loading resources, {0}/{1} loaded.", index, count),
                (float)index / count);
        }

        private static void OnLoadingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Assets",
                Utility.Text.Format("Loading assets, {0}/{1} loaded.", index, count),
                (float)index / count);
        }

        private static void OnLoadCompleted()
        {
            EditorUtility.ClearProgressBar();
        }

        private static void OnAnalyzingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Analyzing Assets",
                Utility.Text.Format("Analyzing assets, {0}/{1} analyzed.", index, count),
                (float)index / count);
        }

        private static void OnAnalyzeCompleted()
        {
            EditorUtility.ClearProgressBar();
        }

        private static bool OnProcessingAssetBundle(string assetBundleName, float progress)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Processing AssetBundle",
                    Utility.Text.Format("Processing '{0}'...", assetBundleName), progress))
            {
                EditorUtility.ClearProgressBar();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool OnProcessingBinary(string binaryName, float progress)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Processing Binary",
                    Utility.Text.Format("Processing '{0}'...", binaryName), progress))
            {
                EditorUtility.ClearProgressBar();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void OnProcessResourceComplete(Platform platform)
        {
            EditorUtility.ClearProgressBar();
            Debug.Log(Utility.Text.Format("Build resources {0}({1}) for '{2}' complete.",
                m_Controller.ApplicableGameVersion, m_Controller.InternalResourceVersion, platform));
        }

        private static void OnBuildResourceError(string errorMessage)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogWarning(Utility.Text.Format("Build resources error with error message '{0}'.",
                errorMessage));
        }
    }
}

