using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GameFramework;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using GFResource = UnityGameFramework.Editor.ResourceTools.Resource;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Extension.Editor
{
    /// <summary>
    /// Resource 规则编辑器，支持按规则配置自动生成 ResourceCollection.xml。
    /// </summary>
    internal sealed class ResourceRuleEditor : EditorWindow
    {
        private const string WindowTitle = "Resource Rule Editor";
        private const string DefaultConfigurationPath = "Assets/Res/Editor/Config/ResourceRuleEditor.asset";
        private const string ResourceRootPath = "Assets/Res/";
        private const string SourceAssetExceptTypeFilter = "t:Script t:SubGraphAsset t:Preset";
        private const string SourceAssetExceptLabelFilter = "l:ResourceExclusive";

        private const float Gap = 5f;
        private const float WindowMinWidth = 1550f;
        private const float WindowMinHeight = 420f;
        private const float WindowMaxHeight = 1000f;
        private const float RowHeight = 18f;
        private const float RuleListElementHeight = 22f;

        private const float ActiveWidth = 41f;
        private const float NameWidth = 200f;
        private const float LoadTypeWidth = 240f;
        private const float PackedWidth = 50f;
        private const float FileSystemWidth = 85f;
        private const float GroupsWidth = 85f;
        private const float VariantWidth = 85f;
        private const float AssetDirectoryWidthOffset = 119f;
        private const float SelectButtonWidth = 50f;
        private const float FilterTypeWidth = 150f;
        private const float PatternsWidth = 250f;

        private static readonly Regex ResourceNameRegex = new Regex(@"^([A-Za-z0-9\._-]+/)*[A-Za-z0-9\._-]+$");
        private static readonly Regex ResourceVariantRegex = new Regex(@"^[a-z0-9_-]+$");
        private static readonly char[] SplitSeparators = { ';', ',', '|' };
        private static readonly string[] EmptyStringArray = new string[0];

        private ResourceRuleEditorData m_Configuration = null;
        private ResourceCollection m_ResourceCollection = null;
        private ReorderableList m_RuleList = null;
        private Vector2 m_ScrollPosition = Vector2.zero;

        private HashSet<string> m_SourceAssetExceptTypeGuids = null;
        private HashSet<string> m_SourceAssetExceptLabelGuids = null;
        private Dictionary<string, string> m_AssetGuidCache = null;

        private int m_CurrentConfigIndex = 0;
        private string m_CurrentConfigPath = null;
        private List<string> m_AllConfigPaths = null;
        private string[] m_ConfigNames = null;

        [MenuItem("Game Framework/Resource Tools/Resource Rule Editor", false, 50)]
        private static void Open()
        {
            ResourceRuleEditor window = GetWindow<ResourceRuleEditor>(true, WindowTitle, true);
            window.minSize = new Vector2(WindowMinWidth, WindowMinHeight);
            window.maxSize = new Vector2(WindowMinWidth, WindowMaxHeight);
        }

        [MenuItem("Game Framework/Resource Tools/Refresh Activate Resource Collection", false, 51)]
        private static void RefreshActivateResourceCollection()
        {
            ResourceRuleEditorUtility.RefreshResourceCollection();
        }

        [MenuItem("Game Framework/Resource Tools/Refresh Activate Resource Collection With Optimize", false, 53)]
        private static void RefreshActivateResourceCollectionWithOptimize()
        {
            ResourceRuleEditorUtility.RefreshResourceCollectionWithOptimize();
        }

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            ResourceRuleEditorData configuration = EditorUtility.EntityIdToObject(instanceID) as ResourceRuleEditorData;
            if (configuration == null)
            {
                return false;
            }

            ResourceRuleEditor window = GetWindow<ResourceRuleEditor>(true, WindowTitle, true);
            window.minSize = new Vector2(WindowMinWidth, WindowMinHeight);
            window.maxSize = new Vector2(WindowMinWidth, WindowMaxHeight);
            window.m_CurrentConfigPath = AssetDatabase.GetAssetPath(configuration);
            window.Load();
            return true;
        }

        private void OnSelectionChange()
        {
            ResourceRuleEditorData configuration = Selection.activeObject as ResourceRuleEditorData;
            if (configuration == null || configuration == m_Configuration)
            {
                return;
            }

            m_CurrentConfigPath = AssetDatabase.GetAssetPath(configuration);
            Load();
            GetWindow<ResourceRuleEditor>().Focus();
        }

        private void OnGUI()
        {
            if (m_Configuration == null)
            {
                Load();
            }

            EditorGUILayout.BeginHorizontal();
            {
                DrawListHeader();
            }
            EditorGUILayout.EndHorizontal();

            if (m_RuleList == null)
            {
                InitializeRuleList();
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(23f);
                DrawListElementLabels();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            {
                GUILayout.Space(50f);
                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
                {
                    m_RuleList.DoLayoutList();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_Configuration);
            }
        }

        private bool CheckRule()
        {
            if (m_Configuration == null)
            {
                Debug.LogError("Resource rule configuration is invalid.");
                return false;
            }

            bool result = true;
            int count = m_Configuration.Rules.Count;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    int index = i + 1;
                    EditorUtility.DisplayProgressBar("CheckRule", Utility.Text.Format("{0}/{1} processing...", index, count), (float)index / count);

                    ResourceRule rule = m_Configuration.Rules[i];
                    if (!rule.Valid)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(rule.Name))
                    {
                        Debug.LogError(Utility.Text.Format("Rule (index '{0}') name can't be empty.", i));
                        result = false;
                    }

                    if (!string.IsNullOrEmpty(rule.Name) && !ResourceNameRegex.IsMatch(rule.Name))
                    {
                        Debug.LogError(Utility.Text.Format("Rule (index '{0}') name '{1}' is not match.", i, rule.Name));
                        result = false;
                    }

                    if (!string.IsNullOrEmpty(rule.Variant) && !ResourceVariantRegex.IsMatch(rule.Variant))
                    {
                        Debug.LogError(Utility.Text.Format("Rule (index '{0}') variant '{1}' is not match.", i, rule.Variant));
                        result = false;
                    }

                    if (!Directory.Exists(rule.AssetsDirectoryPath))
                    {
                        Debug.LogError(Utility.Text.Format("Rule (index '{0}') asset directory '{1}' is not exist.", i, rule.AssetsDirectoryPath));
                        result = false;
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return result;
        }

        private void Load()
        {
            RefreshConfigurationPaths();

            m_Configuration = string.IsNullOrEmpty(m_CurrentConfigPath) ? null : LoadAssetAtPath<ResourceRuleEditorData>(m_CurrentConfigPath);

            bool hasActiveConfiguration = false;
            bool isDirty = false;
            int count = m_AllConfigPaths.Count;
            for (int i = 0; i < count; i++)
            {
                int index = i + 1;
                EditorUtility.DisplayProgressBar("Load", Utility.Text.Format("{0}/{1} processing...", index, count), (float)index / count);

                ResourceRuleEditorData configuration = LoadAssetAtPath<ResourceRuleEditorData>(m_AllConfigPaths[i]);
                if (configuration == null || !configuration.IsActivate)
                {
                    continue;
                }

                if (hasActiveConfiguration)
                {
                    configuration.IsActivate = false;
                    EditorUtility.SetDirty(configuration);
                    isDirty = true;
                    continue;
                }

                hasActiveConfiguration = true;
                if (m_Configuration == null)
                {
                    m_CurrentConfigPath = m_AllConfigPaths[i];
                    m_Configuration = configuration;
                }
            }

            if (m_Configuration == null)
            {
                m_CurrentConfigIndex = 0;
                if (m_AllConfigPaths.Count <= 0)
                {
                    m_Configuration = CreateInstance<ResourceRuleEditorData>();
                    m_Configuration.IsActivate = true;
                    m_CurrentConfigPath = DefaultConfigurationPath;
                    m_AllConfigPaths = new List<string> { DefaultConfigurationPath };
                    m_ConfigNames = new[] { Path.GetFileNameWithoutExtension(DefaultConfigurationPath) };
                }
                else
                {
                    m_CurrentConfigPath = m_AllConfigPaths[m_CurrentConfigIndex];
                    m_Configuration = LoadAssetAtPath<ResourceRuleEditorData>(m_CurrentConfigPath);
                    if (!hasActiveConfiguration && !m_Configuration.IsActivate)
                    {
                        m_Configuration.IsActivate = true;
                        EditorUtility.SetDirty(m_Configuration);
                        isDirty = true;
                    }
                }
            }
            else
            {
                m_CurrentConfigIndex = m_AllConfigPaths.FindIndex(path => string.Equals(m_CurrentConfigPath, path, StringComparison.Ordinal));
                if (m_CurrentConfigIndex < 0)
                {
                    m_CurrentConfigIndex = 0;
                }
            }

            if (isDirty)
            {
                AssetDatabase.SaveAssets();
            }

            m_RuleList = null;
            EditorUtility.ClearProgressBar();
        }

        private void RefreshConfigurationPaths()
        {
            m_AllConfigPaths = AssetDatabase.FindAssets("t:ResourceRuleEditorData")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(Utility.Path.GetRegularPath)
                .ToList();
            m_ConfigNames = m_AllConfigPaths.Select(Path.GetFileNameWithoutExtension).ToArray();
        }

        private static T LoadAssetAtPath<T>(string path) where T : Object
        {
#if UNITY_5
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
            return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
#endif
        }

        private void InitializeRuleList()
        {
            m_RuleList = new ReorderableList(m_Configuration.Rules, typeof(ResourceRule))
            {
                draggable = true,
                elementHeight = RuleListElementHeight,
                headerHeight = 0f,
                drawElementCallback = DrawListElement,
                onAddCallback = AddRule,
            };
        }

        private void AddRule(ReorderableList list)
        {
            string path = SelectFolder();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            ResourceRule rule = new ResourceRule
            {
                AssetsDirectoryPath = path,
            };
            m_Configuration.Rules.Add(rule);
        }

        private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= m_Configuration.Rules.Count)
            {
                return;
            }

            ResourceRule rule = m_Configuration.Rules[index];
            rect.y++;

            Rect fieldRect = rect;
            fieldRect.width = 20f + Gap;
            fieldRect.height = RowHeight;
            rule.Valid = EditorGUI.Toggle(fieldRect, rule.Valid);

            fieldRect.xMin = fieldRect.xMax + Gap;
            fieldRect.xMax += NameWidth;
            float assetBundleNameWidth = fieldRect.width;
            rule.Name = EditorGUI.TextField(fieldRect, rule.Name);

            fieldRect.xMin = fieldRect.xMax + Gap;
            fieldRect.xMax = fieldRect.xMin + LoadTypeWidth;
            rule.LoadType = (LoadType)EditorGUI.EnumPopup(fieldRect, rule.LoadType);

            fieldRect.xMin = fieldRect.xMax + Gap + 20f;
            fieldRect.xMax = fieldRect.xMin + 30f;
            rule.Packed = EditorGUI.Toggle(fieldRect, rule.Packed);

            fieldRect.xMin = fieldRect.xMax + Gap;
            fieldRect.xMax = fieldRect.xMin + FileSystemWidth;
            rule.FileSystem = EditorGUI.TextField(fieldRect, rule.FileSystem);

            fieldRect.xMin = fieldRect.xMax + Gap;
            fieldRect.xMax = fieldRect.xMin + GroupsWidth;
            rule.Groups = EditorGUI.TextField(fieldRect, rule.Groups);

            fieldRect.xMin = fieldRect.xMax + Gap;
            fieldRect.xMax = fieldRect.xMin + VariantWidth;
            rule.Variant = EditorGUI.TextField(fieldRect, rule.Variant);
            if (!string.IsNullOrEmpty(rule.Variant))
            {
                rule.Variant = rule.Variant.ToLowerInvariant();
            }

            fieldRect.xMin = fieldRect.xMax + Gap;
            fieldRect.width = assetBundleNameWidth + 65f;
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUI.TextField(fieldRect, GetDisplayAssetDirectoryPath(rule.AssetsDirectoryPath));
            }
            EditorGUI.EndDisabledGroup();

            fieldRect.xMin = fieldRect.xMax + Gap;
            fieldRect.width = SelectButtonWidth;
            if (GUI.Button(fieldRect, "Select"))
            {
                string path = SelectFolder();
                if (!string.IsNullOrEmpty(path))
                {
                    rule.AssetsDirectoryPath = path;
                }
            }

            fieldRect.xMin = fieldRect.xMax + Gap;
            fieldRect.xMax = fieldRect.xMin + FilterTypeWidth;
            rule.FilterType = (ResourceFilterType)EditorGUI.EnumPopup(fieldRect, rule.FilterType);

            fieldRect.xMin = fieldRect.xMax + Gap;
            fieldRect.xMax = rect.xMax;
            rule.SearchPatterns = EditorGUI.TextField(fieldRect, rule.SearchPatterns);
        }

        private string SelectFolder()
        {
            string dataPath = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Res/"));
            string selectedPath = EditorUtility.OpenFolderPanel("Path", dataPath, string.Empty);
            if (string.IsNullOrEmpty(selectedPath))
            {
                return null;
            }

            selectedPath = Utility.Path.GetRegularPath(selectedPath);
            if (selectedPath.StartsWith(dataPath, StringComparison.Ordinal))
            {
                return Utility.Path.GetRegularPath(ResourceRootPath + selectedPath.Substring(dataPath.Length));
            }

#if UNITY_2019_1_OR_NEWER
            ShowNotification(new GUIContent("Can not be outside of 'Assets/Res/'!"), 2);
#else
            ShowNotification(new GUIContent("Can not be outside of 'Assets/Res/'!"));
#endif
            return null;
        }

        private void DrawListHeader()
        {
            Rect rect = new Rect(Gap, 3f, 40f, RowHeight);

            Rect configLabelRect = new Rect(rect.x, rect.y, 75f, rect.height);
            EditorGUI.LabelField(configLabelRect, "Rule Config:");

            Rect configsRect = new Rect(configLabelRect.xMax + Gap, rect.y, 200f, rect.height);
            int configIndex = EditorGUI.Popup(configsRect, m_CurrentConfigIndex, m_ConfigNames);
            if (configIndex != m_CurrentConfigIndex)
            {
                m_CurrentConfigIndex = configIndex;
                m_CurrentConfigPath = m_AllConfigPaths[m_CurrentConfigIndex];
                m_Configuration = LoadAssetAtPath<ResourceRuleEditorData>(m_CurrentConfigPath);
                m_RuleList = null;
            }

            Rect activeLabelRect = new Rect(configsRect.xMax + Gap, rect.y, 60f, rect.height);
            EditorGUI.LabelField(activeLabelRect, "Active:");

            Rect activeRect = new Rect(activeLabelRect.xMax + Gap, rect.y, 20f, rect.height);
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUI.Toggle(activeRect, m_Configuration.IsActivate);
            }
            EditorGUI.EndDisabledGroup();

            Rect activateRect = new Rect(activeRect.xMax + Gap, rect.y, 100f, rect.height);
            if (GUI.Button(activateRect, "Active This"))
            {
                ActivateCurrentConfiguration();
            }

            Rect reloadRect = new Rect(activateRect.xMax + Gap, rect.y, 100f, rect.height);
            if (GUI.Button(reloadRect, "Reload"))
            {
                Load();
            }

            Rect saveRect = new Rect(reloadRect.xMax + Gap, rect.y, 100f, rect.height);
            if (GUI.Button(saveRect, "Save"))
            {
                Save();
            }

            Rect refreshRect = new Rect(saveRect.xMax + Gap, rect.y, 200f, rect.height);
            if (GUI.Button(refreshRect, "Refresh ResourceCollection.xml"))
            {
                RefreshResourceCollection();
            }
        }

        private void DrawListElementLabels()
        {
            Rect rect = new Rect(Gap, 25f, 0f, RowHeight);
            EditorGUI.BeginDisabledGroup(true);
            {
                rect.width = ActiveWidth;
                EditorGUI.TextField(rect, "Active");

                rect.xMin = rect.xMax + Gap;
                rect.xMax += NameWidth;
                float assetBundleNameWidth = rect.width;
                EditorGUI.TextField(rect, "Name");

                rect.xMin = rect.xMax + Gap;
                rect.xMax = rect.xMin + LoadTypeWidth;
                EditorGUI.TextField(rect, "Load Type");

                rect.xMin = rect.xMax + Gap;
                rect.xMax = rect.xMin + PackedWidth;
                EditorGUI.TextField(rect, "Packed");

                rect.xMin = rect.xMax + Gap;
                rect.xMax = rect.xMin + FileSystemWidth;
                EditorGUI.TextField(rect, "File System");

                rect.xMin = rect.xMax + Gap;
                rect.xMax = rect.xMin + GroupsWidth;
                EditorGUI.TextField(rect, "Groups");

                rect.xMin = rect.xMax + Gap;
                rect.xMax = rect.xMin + VariantWidth;
                EditorGUI.TextField(rect, "Variant");

                rect.xMin = rect.xMax + Gap;
                rect.width = assetBundleNameWidth + AssetDirectoryWidthOffset;
                EditorGUI.TextField(rect, "AssetDirectory");

                rect.xMin = rect.xMax + Gap;
                rect.xMax = rect.xMin + FilterTypeWidth;
                EditorGUI.TextField(rect, "Filter Type");

                rect.xMin = rect.xMax + Gap;
                rect.xMax = rect.xMin + PatternsWidth;
                EditorGUI.TextField(rect, "Patterns");
            }
            EditorGUI.EndDisabledGroup();
        }

        private void ActivateCurrentConfiguration()
        {
            foreach (string configPath in m_AllConfigPaths)
            {
                ResourceRuleEditorData configuration = LoadAssetAtPath<ResourceRuleEditorData>(configPath);
                if (configuration == null)
                {
                    continue;
                }

                configuration.IsActivate = false;
                EditorUtility.SetDirty(configuration);
            }

            m_Configuration.IsActivate = true;
            EditorUtility.SetDirty(m_Configuration);
            AssetDatabase.SaveAssets();
        }

        private void Save()
        {
            if (!CheckRule())
            {
                return;
            }

            if (LoadAssetAtPath<ResourceRuleEditorData>(m_CurrentConfigPath) == null)
            {
                string directoryName = Path.GetDirectoryName(m_CurrentConfigPath);
                if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                AssetDatabase.CreateAsset(m_Configuration, m_CurrentConfigPath);
                RefreshConfigurationPaths();
                m_CurrentConfigIndex = m_AllConfigPaths.FindIndex(path => string.Equals(path, m_CurrentConfigPath, StringComparison.Ordinal));
            }

            EditorUtility.SetDirty(m_Configuration);
            AssetDatabase.SaveAssets();
        }

        private static string GetDisplayAssetDirectoryPath(string assetsDirectoryPath)
        {
            if (string.IsNullOrEmpty(assetsDirectoryPath))
            {
                return string.Empty;
            }

            return assetsDirectoryPath.StartsWith(ResourceRootPath, StringComparison.Ordinal) ? assetsDirectoryPath.Substring(ResourceRootPath.Length) : assetsDirectoryPath;
        }

        private static string[] GetSeparatedValues(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return EmptyStringArray;
            }

            string[] values = value.Split(SplitSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrEmpty(item))
                .ToArray();
            return values.Length > 0 ? values : EmptyStringArray;
        }

        private static string[] GetSearchPatterns(ResourceRule rule)
        {
            string[] searchPatterns = GetSeparatedValues(rule.SearchPatterns);
            return searchPatterns.Length > 0 ? searchPatterns : new[] { "*.*" };
        }

        private static string GetResourceVariant(ResourceRule rule)
        {
            return string.IsNullOrEmpty(rule.Variant) ? null : rule.Variant;
        }

        #region Refresh ResourceCollection.xml

        internal void RefreshResourceCollection()
        {
            if (m_Configuration == null)
            {
                Load();
            }

            RefreshResourceCollectionInternal();
        }

        internal void RefreshResourceCollectionWithOptimize()
        {
            RefreshResourceCollection();

            ResourceOptimize resourceOptimize = new ResourceOptimize();
            resourceOptimize.Optimize(m_ResourceCollection);
        }

        internal void RefreshResourceCollection(string configPath)
        {
            configPath = Utility.Path.GetRegularPath(configPath);
            if (m_Configuration == null || !string.Equals(m_CurrentConfigPath, configPath, StringComparison.Ordinal))
            {
                m_CurrentConfigPath = configPath;
                Load();
            }

            RefreshResourceCollectionInternal();
        }

        internal void RefreshResourceCollectionWithOptimize(string configPath)
        {
            RefreshResourceCollection(configPath);

            ResourceOptimize resourceOptimize = new ResourceOptimize();
            resourceOptimize.Optimize(m_ResourceCollection);
        }

        private void RefreshResourceCollectionInternal()
        {
            if (!CheckRule())
            {
                throw new GameFrameworkException("Refresh ResourceCollection.xml check rule fail.");
            }

            m_SourceAssetExceptTypeGuids = new HashSet<string>(AssetDatabase.FindAssets(SourceAssetExceptTypeFilter), StringComparer.Ordinal);
            m_SourceAssetExceptLabelGuids = new HashSet<string>(AssetDatabase.FindAssets(SourceAssetExceptLabelFilter), StringComparer.Ordinal);

            AnalyzeResourceFilters();
            CheckRemoveEmptyResource();
            if (!SaveCollection())
            {
                throw new GameFrameworkException("Refresh ResourceCollection.xml fail.");
            }

            Debug.Log("Refresh ResourceCollection.xml success.");
        }

        private GFResource[] GetResources()
        {
            return m_ResourceCollection.GetResources();
        }

        private bool HasResource(string name, string variant)
        {
            return m_ResourceCollection.HasResource(name, variant);
        }

        private bool AddResource(string name, string variant, string fileSystem, LoadType loadType, bool packed, string[] resourceGroups)
        {
            return m_ResourceCollection.AddResource(name, variant, fileSystem, loadType, packed, resourceGroups);
        }

        private bool RenameResource(string oldName, string oldVariant, string newName, string newVariant)
        {
            return m_ResourceCollection.RenameResource(oldName, oldVariant, newName, newVariant);
        }

        private bool AssignAsset(string assetGuid, string resourceName, string resourceVariant)
        {
            return m_ResourceCollection.AssignAsset(assetGuid, resourceName, resourceVariant);
        }

        private void CheckRemoveEmptyResource()
        {
            GFResource[] resources = GetResources();
            int count = resources.Length;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    int index = i + 1;
                    EditorUtility.DisplayProgressBar("OptimizeLoadType", Utility.Text.Format("{0}/{1} processing...", index, count), (float)index / count);

                    GFResource resource = resources[i];
                    if (resource.GetFirstAsset() == null)
                    {
                        m_ResourceCollection.RemoveResource(resource.Name, resource.Variant);
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void AnalyzeResourceFilters()
        {
            m_AssetGuidCache = new Dictionary<string, string>(StringComparer.Ordinal);
            m_ResourceCollection = new ResourceCollection();

            HashSet<string> signedResourceSet = new HashSet<string>(StringComparer.Ordinal);
            int count = m_Configuration.Rules.Count;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    int index = i + 1;
                    EditorUtility.DisplayProgressBar("AnalysisResourceFilters", Utility.Text.Format("{0}/{1} processing...", index, count), (float)index / count);

                    ResourceRule rule = m_Configuration.Rules[i];
                    if (!rule.Valid)
                    {
                        continue;
                    }

                    switch (rule.FilterType)
                    {
                        case ResourceFilterType.Root:
                            AnalyzeRootFilter(signedResourceSet, rule);
                            break;

                        case ResourceFilterType.Children:
                            AnalyzeChildrenFilter(signedResourceSet, rule);
                            break;

                        case ResourceFilterType.ChildrenFoldersOnly:
                            AnalyzeChildrenFoldersOnlyFilter(signedResourceSet, rule);
                            break;

                        case ResourceFilterType.ChildrenFilesOnly:
                            AnalyzeChildrenFilesOnlyFilter(signedResourceSet, rule);
                            break;
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void AnalyzeRootFilter(HashSet<string> signedResourceSet, ResourceRule rule)
        {
            string resourceName = string.IsNullOrEmpty(rule.Name) ? Utility.Path.GetRegularPath(rule.AssetsDirectoryPath.Replace("Assets/", string.Empty)) : rule.Name;
            ApplyResourceFilter(signedResourceSet, rule, resourceName);
        }

        private void AnalyzeChildrenFilter(HashSet<string> signedResourceSet, ResourceRule rule)
        {
            foreach (FileInfo file in GetAssetFiles(rule.AssetsDirectoryPath, SearchOption.AllDirectories, GetSearchPatterns(rule)))
            {
                string assetGuid = AssetPathToGUID(GetAssetName(file));
                if (!IsExceptAsset(assetGuid))
                {
                    ApplyResourceFilter(signedResourceSet, rule, GetResourceNameWithoutExtension(file), assetGuid);
                }
            }
        }

        private void AnalyzeChildrenFoldersOnlyFilter(HashSet<string> signedResourceSet, ResourceRule rule)
        {
            DirectoryInfo[] assetDirectories = new DirectoryInfo(rule.AssetsDirectoryPath).GetDirectories();
            foreach (DirectoryInfo directory in assetDirectories)
            {
                string relativeDirectoryName = directory.FullName.Substring(Application.dataPath.Length + 1);
                ApplyResourceFilter(signedResourceSet, rule, Utility.Path.GetRegularPath(relativeDirectoryName), string.Empty, directory.FullName);
            }
        }

        private void AnalyzeChildrenFilesOnlyFilter(HashSet<string> signedResourceSet, ResourceRule rule)
        {
            DirectoryInfo[] assetDirectories = new DirectoryInfo(rule.AssetsDirectoryPath).GetDirectories();
            foreach (DirectoryInfo directory in assetDirectories)
            {
                foreach (FileInfo file in GetAssetFiles(directory.FullName, SearchOption.AllDirectories, GetSearchPatterns(rule)))
                {
                    string assetGuid = AssetPathToGUID(GetAssetName(file));
                    if (!IsExceptAsset(assetGuid))
                    {
                        ApplyResourceFilter(signedResourceSet, rule, GetResourceNameWithoutExtension(file), assetGuid);
                    }
                }
            }
        }

        private void ApplyResourceFilter(HashSet<string> signedResourceSet, ResourceRule rule, string resourceName, string singleAssetGuid = "", string childDirectoryPath = "")
        {
            string resourcePath = Utility.Path.GetRegularPath(Path.Combine(rule.AssetsDirectoryPath, resourceName));
            if (!signedResourceSet.Add(resourcePath))
            {
                return;
            }

            string variant = GetResourceVariant(rule);
            foreach (GFResource oldResource in GetResources())
            {
                if (string.Equals(oldResource.Name, resourceName, StringComparison.Ordinal) && string.IsNullOrEmpty(oldResource.Variant))
                {
                    RenameResource(oldResource.Name, oldResource.Variant, resourceName, variant);
                    break;
                }
            }

            if (!HasResource(resourceName, variant))
            {
                string fileSystem = string.IsNullOrEmpty(rule.FileSystem) ? null : rule.FileSystem;
                AddResource(resourceName, variant, fileSystem, rule.LoadType, rule.Packed, GetSeparatedValues(rule.Groups));
            }

            switch (rule.FilterType)
            {
                case ResourceFilterType.Root:
                case ResourceFilterType.ChildrenFoldersOnly:
                    AssignFolderAssets(rule, resourceName, variant, childDirectoryPath);
                    break;

                case ResourceFilterType.Children:
                case ResourceFilterType.ChildrenFilesOnly:
                    AssignAsset(singleAssetGuid, resourceName, variant);
                    break;
            }
        }

        private void AssignFolderAssets(ResourceRule rule, string resourceName, string variant, string childDirectoryPath)
        {
            if (string.IsNullOrEmpty(childDirectoryPath))
            {
                childDirectoryPath = rule.AssetsDirectoryPath;
            }

            foreach (FileInfo file in GetAssetFiles(childDirectoryPath, SearchOption.AllDirectories, GetSearchPatterns(rule)))
            {
                string assetGuid = AssetPathToGUID(GetAssetName(file));
                if (!IsExceptAsset(assetGuid))
                {
                    AssignAsset(assetGuid, resourceName, variant);
                }
            }
        }

        private IEnumerable<FileInfo> GetAssetFiles(string directoryPath, SearchOption searchOption, string[] searchPatterns)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            foreach (string searchPattern in searchPatterns)
            {
                FileInfo[] assetFiles = directoryInfo.GetFiles(searchPattern, searchOption);
                foreach (FileInfo file in assetFiles)
                {
                    if (!string.Equals(file.Extension, ".meta", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return file;
                    }
                }
            }
        }

        private bool IsExceptAsset(string assetGuid)
        {
            return m_SourceAssetExceptTypeGuids.Contains(assetGuid) || m_SourceAssetExceptLabelGuids.Contains(assetGuid);
        }

        private bool SaveCollection()
        {
            return m_ResourceCollection.Save();
        }

        private string AssetPathToGUID(string assetName)
        {
            if (m_AssetGuidCache.TryGetValue(assetName, out string guid))
            {
                return guid;
            }

            guid = AssetDatabase.AssetPathToGUID(assetName);
            m_AssetGuidCache.Add(assetName, guid);
            return guid;
        }

        private static string GetAssetName(FileInfo file)
        {
            string relativeAssetName = file.FullName.Substring(Application.dataPath.Length + 1);
            return Utility.Path.GetRegularPath(Path.Combine("Assets", relativeAssetName));
        }

        private static string GetResourceNameWithoutExtension(FileInfo file)
        {
            string relativeAssetName = file.FullName.Substring(Application.dataPath.Length + 1);
            return Utility.Path.GetRegularPath(Path.ChangeExtension(relativeAssetName, null));
        }

        #endregion
    }
}
