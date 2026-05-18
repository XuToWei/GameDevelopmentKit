using System;
using System.Collections.Generic;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    /// <summary>
    /// 资源版本分析器。
    /// </summary>
    internal sealed partial class ResourceVersionAnalyzerEditor : EditorWindow
    {
        private const float ToolbarHeight = 30f;
        private const float ButtonWidth = 80f;
        private const float Spacing = 5f;

        private static readonly string[] ToolbarNames = new string[] { "Summary", "Resource List", "Asset List", "Version Compare" };
        private static readonly string[] CompareFilterNames = new string[] { "All", "Added", "Removed", "Modified" };

        private static readonly Color AddedColor = new Color(0.4f, 0.8f, 0.4f, 1f);
        private static readonly Color RemovedColor = new Color(0.8f, 0.4f, 0.4f, 1f);
        private static readonly Color ModifiedColor = new Color(0.8f, 0.7f, 0.3f, 1f);
        private static readonly Color SelectionColor = new Color(0.3f, 0.5f, 0.8f, 0.3f);
        private static readonly Color LinkColor = new Color(0.3f, 0.6f, 1f, 1f);

        private ResourceVersionAnalyzerController m_Controller;
        private int m_ToolbarIndex;

        private string m_VersionFilePath;
        private ResourceVersionAnalyzerController.VersionData m_VersionData;
        private ResourceVersionAnalyzerController.DistributionInfo[] m_ExtensionDistribution;
        private ResourceVersionAnalyzerController.DistributionInfo[] m_FileSystemDistribution;
        private ResourceVersionAnalyzerController.DistributionInfo[] m_ResourceGroupDistribution;
        private Vector2 m_SummaryScroll;

        private Vector2 m_ResourceListScroll;
        private Vector2 m_ResourceDetailScroll;
        private string m_SearchText;
        private int m_SortColumn;
        private bool m_SortAscending;
        private List<ResourceVersionAnalyzerController.ResourceInfo> m_FilteredResources;
        private ResourceVersionAnalyzerController.ResourceInfo m_SelectedResource;
        private List<ResourceVersionAnalyzerController.AssetInfo> m_SelectedResourceAssets;

        private Vector2 m_AssetListScroll;
        private Vector2 m_AssetDetailScroll;
        private string m_AssetSearchText;
        private int m_AssetSortColumn;
        private bool m_AssetSortAscending;
        private List<ResourceVersionAnalyzerController.AssetInfo> m_FilteredAssets;
        private ResourceVersionAnalyzerController.AssetInfo m_SelectedAsset;
        private Dictionary<string, string> m_AssetNameToResourceFullName;
        private Dictionary<string, ResourceVersionAnalyzerController.ResourceInfo> m_ResourceInfoLookup;
        private Dictionary<string, long> m_AssetDepResourceTotalSizeCache;

        private string m_CompareFilePathA;
        private string m_CompareFilePathB;
        private ResourceVersionAnalyzerController.VersionData m_CompareVersionA;
        private ResourceVersionAnalyzerController.VersionData m_CompareVersionB;
        private ResourceVersionAnalyzerController.CompareResult m_CompareResult;
        private Vector2 m_CompareScroll;
        private int m_CompareFilterIndex;
        private int m_CompareSortColumn;
        private bool m_CompareSortAscending;
        private List<ResourceVersionAnalyzerController.CompareItem> m_FilteredCompareItems;

        [MenuItem("Game Framework/Resource Tools/Resource Version Analyzer", false, 54)]
        private static void Open()
        {
            ResourceVersionAnalyzerEditor window = GetWindow<ResourceVersionAnalyzerEditor>("Resource Version Analyzer", true);
            window.minSize = new Vector2(900f, 600f);
        }

        private void OnEnable()
        {
            m_Controller = new ResourceVersionAnalyzerController();
            m_ToolbarIndex = 0;

            m_VersionFilePath = string.Empty;
            m_VersionData = null;
            m_ExtensionDistribution = null;
            m_FileSystemDistribution = null;
            m_ResourceGroupDistribution = null;
            m_SummaryScroll = Vector2.zero;

            m_ResourceListScroll = Vector2.zero;
            m_ResourceDetailScroll = Vector2.zero;
            m_SearchText = string.Empty;
            m_SortColumn = 0;
            m_SortAscending = true;
            m_FilteredResources = null;
            m_SelectedResource = null;
            m_SelectedResourceAssets = null;

            m_AssetListScroll = Vector2.zero;
            m_AssetDetailScroll = Vector2.zero;
            m_AssetSearchText = string.Empty;
            m_AssetSortColumn = 0;
            m_AssetSortAscending = true;
            m_FilteredAssets = null;
            m_SelectedAsset = null;
            m_AssetNameToResourceFullName = null;
            m_ResourceInfoLookup = null;
            m_AssetDepResourceTotalSizeCache = null;

            m_CompareFilePathA = string.Empty;
            m_CompareFilePathB = string.Empty;
            m_CompareVersionA = null;
            m_CompareVersionB = null;
            m_CompareResult = null;
            m_CompareScroll = Vector2.zero;
            m_CompareFilterIndex = 0;
            m_CompareSortColumn = 0;
            m_CompareSortAscending = true;
            m_FilteredCompareItems = null;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width), GUILayout.Height(position.height));
            {
                GUILayout.Space(Spacing);
                int toolbarIndex = GUILayout.Toolbar(m_ToolbarIndex, ToolbarNames, GUILayout.Height(ToolbarHeight));
                if (toolbarIndex != m_ToolbarIndex)
                {
                    m_ToolbarIndex = toolbarIndex;
                    GUI.FocusControl(null);
                }

                GUILayout.Space(Spacing);

                switch (m_ToolbarIndex)
                {
                    case 0:
                        DrawSummary();
                        break;
                    case 1:
                        DrawResourceList();
                        break;
                    case 2:
                        DrawAssetList();
                        break;
                    case 3:
                        DrawVersionCompare();
                        break;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawVersionFileSelector()
        {
            EditorGUILayout.BeginHorizontal();
            {
                m_VersionFilePath = EditorGUILayout.TextField("Version File", m_VersionFilePath);
                if (GUILayout.Button("Browse...", GUILayout.Width(ButtonWidth)))
                {
                    string path = EditorUtility.OpenFilePanel("Select Version List File", GetDefaultBrowsePath(), "dat");
                    if (!string.IsNullOrEmpty(path))
                    {
                        m_VersionFilePath = path;
                    }
                }

                if (GUILayout.Button("Load", GUILayout.Width(60f)))
                {
                    LoadVersion();
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(Spacing);
        }

        private void LoadVersion()
        {
            if (string.IsNullOrEmpty(m_VersionFilePath))
            {
                return;
            }

            try
            {
                m_VersionData = m_Controller.Load(m_VersionFilePath);
                m_ExtensionDistribution = m_Controller.GetDistributionByExtension(m_VersionData);
                m_FileSystemDistribution = m_Controller.GetDistributionByFileSystem(m_VersionData);
                m_ResourceGroupDistribution = m_Controller.GetDistributionByResourceGroup(m_VersionData);
                RefreshFilteredResources();
                RefreshFilteredAssets();
                BuildAssetNameToResourceLookup();
                BuildResourceInfoLookup();
                BuildAssetDepResourceTotalSizeCache();
                m_SelectedResource = null;
                m_SelectedResourceAssets = null;
                m_SelectedAsset = null;
                Debug.Log(Utility.Text.Format("Load version list '{0}' success. {1} resources, {2} assets found.", m_VersionFilePath, m_VersionData.ResourceCount, m_VersionData.AssetCount));
            }
            catch (Exception e)
            {
                m_VersionData = null;
                m_ExtensionDistribution = null;
                m_FileSystemDistribution = null;
                m_ResourceGroupDistribution = null;
                m_FilteredResources = null;
                m_FilteredAssets = null;
                m_SelectedResource = null;
                m_SelectedResourceAssets = null;
                m_SelectedAsset = null;
                m_AssetNameToResourceFullName = null;
                m_ResourceInfoLookup = null;
                m_AssetDepResourceTotalSizeCache = null;
                Debug.LogError(Utility.Text.Format("Load version list '{0}' failure. Exception: {1}", m_VersionFilePath, e.Message));
            }
        }

        private static void DrawAssetNameLink(string assetName, params GUILayoutOption[] options)
        {
            Color originalColor = GUI.contentColor;
            GUI.contentColor = LinkColor;
            EditorGUILayout.LabelField(assetName, options);
            GUI.contentColor = originalColor;

            Rect rect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
                if (asset != null)
                {
                    EditorGUIUtility.PingObject(asset);
                    Selection.activeObject = asset;
                }

                Event.current.Use();
            }
        }

        private void BuildAssetNameToResourceLookup()
        {
            m_AssetNameToResourceFullName = new Dictionary<string, string>();
            if (m_VersionData == null)
            {
                return;
            }

            ResourceVersionAnalyzerController.AssetInfo[] assets = m_VersionData.AssetInfos;
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i].ResourceFullName != null)
                {
                    m_AssetNameToResourceFullName[assets[i].Name] = assets[i].ResourceFullName;
                }
            }
        }

        private void BuildResourceInfoLookup()
        {
            m_ResourceInfoLookup = new Dictionary<string, ResourceVersionAnalyzerController.ResourceInfo>();
            if (m_VersionData == null)
            {
                return;
            }

            ResourceVersionAnalyzerController.ResourceInfo[] resources = m_VersionData.Resources;
            for (int i = 0; i < resources.Length; i++)
            {
                m_ResourceInfoLookup[resources[i].FullName] = resources[i];
            }
        }

        private void BuildAssetDepResourceTotalSizeCache()
        {
            m_AssetDepResourceTotalSizeCache = new Dictionary<string, long>();
            if (m_VersionData == null)
            {
                return;
            }

            ResourceVersionAnalyzerController.AssetInfo[] assets = m_VersionData.AssetInfos;
            HashSet<string> visitedResources = new HashSet<string>();
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i].DependencyAssetNames == null || assets[i].DependencyAssetNames.Length == 0)
                {
                    m_AssetDepResourceTotalSizeCache[assets[i].Name] = 0L;
                    continue;
                }

                visitedResources.Clear();
                long total = 0L;
                for (int j = 0; j < assets[i].DependencyAssetNames.Length; j++)
                {
                    string resourceFullName = GetResourceFullNameByAssetName(assets[i].DependencyAssetNames[j]);
                    if (string.IsNullOrEmpty(resourceFullName) || !visitedResources.Add(resourceFullName))
                    {
                        continue;
                    }

                    ResourceVersionAnalyzerController.ResourceInfo info;
                    if (m_ResourceInfoLookup.TryGetValue(resourceFullName, out info))
                    {
                        total += info.Length;
                    }
                }

                m_AssetDepResourceTotalSizeCache[assets[i].Name] = total;
            }
        }

        private string GetResourceFullNameByAssetName(string assetName)
        {
            if (m_AssetNameToResourceFullName == null)
            {
                return string.Empty;
            }

            string resourceFullName;
            if (m_AssetNameToResourceFullName.TryGetValue(assetName, out resourceFullName))
            {
                return resourceFullName;
            }

            return string.Empty;
        }

        private int GetResourceLengthByAssetName(string assetName)
        {
            string resourceFullName = GetResourceFullNameByAssetName(assetName);
            if (string.IsNullOrEmpty(resourceFullName) || m_ResourceInfoLookup == null)
            {
                return 0;
            }

            ResourceVersionAnalyzerController.ResourceInfo info;
            if (m_ResourceInfoLookup.TryGetValue(resourceFullName, out info))
            {
                return info.Length;
            }

            return 0;
        }

        private long GetDependencyResourceTotalSize(string assetName)
        {
            if (m_AssetDepResourceTotalSizeCache == null)
            {
                return 0L;
            }

            long size;
            m_AssetDepResourceTotalSizeCache.TryGetValue(assetName, out size);
            return size;
        }

        private static string GetResourceGroupsString(string[] groups)
        {
            if (groups == null || groups.Length == 0)
            {
                return string.Empty;
            }

            return string.Join(", ", groups);
        }

        private string GetResourceGroupsByAssetName(string assetName)
        {
            string resourceFullName = GetResourceFullNameByAssetName(assetName);
            if (string.IsNullOrEmpty(resourceFullName) || m_ResourceInfoLookup == null)
            {
                return string.Empty;
            }

            ResourceVersionAnalyzerController.ResourceInfo info;
            if (m_ResourceInfoLookup.TryGetValue(resourceFullName, out info))
            {
                return GetResourceGroupsString(info.ResourceGroups);
            }

            return string.Empty;
        }

        private static string GetSortColumnTitle(string name, int currentSortColumn, int column, bool ascending)
        {
            if (currentSortColumn != column)
            {
                return name;
            }

            return Utility.Text.Format("{0} {1}", name, ascending ? "▲" : "▼");
        }

        private static string GetDefaultBrowsePath()
        {
            string path = Utility.Path.GetRegularPath(System.IO.Path.Combine(Application.dataPath, "..", "..", "Temp", "Bundle"));
            if (System.IO.Directory.Exists(path))
            {
                return path;
            }

            return Application.dataPath;
        }

        private static string GetSizeString(long size)
        {
            if (size < 0)
            {
                return Utility.Text.Format("-{0}", GetSizeString(-size));
            }

            if (size < 1024L)
            {
                return Utility.Text.Format("{0} B", size);
            }

            if (size < 1024L * 1024L)
            {
                return Utility.Text.Format("{0:F2} KB", size / 1024f);
            }

            if (size < 1024L * 1024L * 1024L)
            {
                return Utility.Text.Format("{0:F2} MB", size / (1024f * 1024f));
            }

            return Utility.Text.Format("{0:F2} GB", size / (1024f * 1024f * 1024f));
        }
    }
}
