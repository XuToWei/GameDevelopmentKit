using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GameFramework;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityGameFramework.Editor.ResourceTools;
using GFResource = UnityGameFramework.Editor.ResourceTools.Resource;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Extension.Editor
{
    /// <summary>
    /// Resource 规则编辑器，支持按规则配置自动生成 ResourceCollection.xml
    /// </summary>
    internal class ResourceRuleEditor : EditorWindow
    {
        private static readonly Regex ResourceNameRegex = new Regex(@"^([A-Za-z0-9\._-]+/)*[A-Za-z0-9\._-]+$");
        private static readonly Regex ResourceVariantRegex = new Regex(@"^[a-z0-9_-]+$");

        private readonly string m_NormalConfigurationPath = "Assets/Res/Editor/Config/ResourceRuleEditor.asset";
        private ResourceRuleEditorData m_Configuration;
        private ResourceCollection m_ResourceCollection;

        private ReorderableList m_RuleList;
        private Vector2 m_ScrollPosition = Vector2.zero;

        private readonly string m_SourceAssetExceptTypeFilter = "t:Script t:SubGraphAsset t:Preset";
        private string[] m_SourceAssetExceptTypeFilterGUIDArray;

        private readonly string m_SourceAssetExceptLabelFilter = "l:ResourceExclusive";
        private string[] m_SourceAssetExceptLabelFilterGUIDArray;
        
        private Dictionary<string, string> m_AssetGuidCache;

        [MenuItem("Game Framework/Resource Tools/Resource Rule Editor", false, 50)]
        static void Open()
        {
            ResourceRuleEditor window = GetWindow<ResourceRuleEditor>(true, "Resource Rule Editor", true);
            window.minSize = new Vector2(1550f, 420f);
            window.maxSize = new Vector2(1550f, 1000f);
        }
        
        [MenuItem("Game Framework/Resource Tools/Refresh Activate Resource Collection", false, 51)]
        static void RefreshActivateResourceCollection()
        {
            ResourceRuleEditorUtility.RefreshResourceCollection();
        }
 
        [MenuItem("Game Framework/Resource Tools/Refresh Activate Resource Collection With Optimize", false, 53)]
        static void RefreshActivateResourceCollectionWithOptimize()
        {
            ResourceRuleEditorUtility.RefreshResourceCollection();
        }

        private bool CheckRule()
        {
            bool isSuccess = true;
            int count = m_Configuration.rules.Count;
            for (int i = 0; i < m_Configuration.rules.Count; i++)
            {
                int cur = i + 1;
                EditorUtility.DisplayProgressBar("CheckRule", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                ResourceRule rule = m_Configuration.rules[i];
                if(!rule.valid)
                    continue;
                if (string.IsNullOrEmpty(rule.name))
                {
                    Debug.LogError(Utility.Text.Format("Rule (index '{0}') name can't be empty.", i));
                    isSuccess = false;
                }
                if (!string.IsNullOrEmpty(rule.name) && !ResourceNameRegex.IsMatch(rule.name))
                {
                    Debug.LogError(Utility.Text.Format("Rule (index '{0}') name '{1}' is not math.", i, rule.name));
                    isSuccess = false;
                }
                if (!string.IsNullOrEmpty(rule.variant) && !ResourceVariantRegex.IsMatch(rule.variant))
                {
                    Debug.LogError(Utility.Text.Format("Rule (index '{0}') variant '{1}' is not math.", i, rule.variant));
                    isSuccess = false;
                }
                if (!Directory.Exists(rule.assetsDirectoryPath))
                {
                    Debug.LogError(Utility.Text.Format("Rule (index '{0}') asset directory '{1}' is not exist.", i, rule.assetsDirectoryPath));
                    isSuccess = false;
                }
            }
            EditorUtility.ClearProgressBar();
            return isSuccess;
        }

        [OnOpenAsset]
        internal static bool OnOpenAsset(int instanceID, int line)
        {
            var config = EditorUtility.InstanceIDToObject (instanceID) as ResourceRuleEditorData;
            if (config != null)
            {
                ResourceRuleEditor window = GetWindow<ResourceRuleEditor>(true, "Resource Rule Editor", true);
                window.minSize = new Vector2(1550f, 420f);
                window.maxSize = new Vector2(1550f, 1000f);
                window.m_CurrentConfigPath = AssetDatabase.GetAssetPath(config);
                window.Load();
                return true;
            }
            return false; // we did not handle the open
        }
        void OnSelectionChange()
        {
            var config = Selection.activeObject as ResourceRuleEditorData;
            if (config != null && config != m_Configuration)
            {
                m_CurrentConfigPath = AssetDatabase.GetAssetPath(config);
                Load();
                GetWindow<ResourceRuleEditor>().Focus();
            }
        }

        private void OnGUI()
        {
            if (m_Configuration == null)
            {
                Load();
            }
            
            GUILayout.BeginHorizontal();
            {
                OnListHeaderGUI();
            }
            GUILayout.EndHorizontal();
            
            if (m_RuleList == null)
            {
                InitRuleListDrawer();
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(23);
                OnListElementLabelGUI();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginVertical();
            {
                GUILayout.Space(50);
                m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
                {
                    m_RuleList.DoLayoutList();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();

            if (GUI.changed)
                EditorUtility.SetDirty(m_Configuration);
        }

        private void Load()
        {
            m_AllConfigPaths = AssetDatabase.FindAssets("t:ResourceRuleEditorData").Select(AssetDatabase.GUIDToAssetPath).ToList();
            m_ConfigNames = m_AllConfigPaths.Select(Path.GetFileNameWithoutExtension).ToArray();
            m_Configuration = LoadAssetAtPath<ResourceRuleEditorData>(m_CurrentConfigPath);
            
            bool hasActivate = false;
            int count = m_AllConfigPaths.Count;
            int cur = 0;
            foreach (var configPath in m_AllConfigPaths)
            {
                cur++;
                EditorUtility.DisplayProgressBar("Load", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                ResourceRuleEditorData ruleEditorData = LoadAssetAtPath<ResourceRuleEditorData>(configPath);
                if (hasActivate)
                {
                    if (ruleEditorData.isActivate)
                    {
                        ruleEditorData.isActivate = false;
                        EditorUtility.SetDirty(ruleEditorData);
                        AssetDatabase.SaveAssets();
                    }
                }
                else
                {
                    if (ruleEditorData.isActivate)
                    {
                        hasActivate = true;
                        if (m_Configuration == null)
                        {
                            m_CurrentConfigPath = configPath;
                            m_Configuration = ruleEditorData;
                        }
                    }
                }
            }
            
            if (m_Configuration == null)
            {
                m_CurrentConfigIndex = 0;
                if (m_AllConfigPaths.Count == 0)
                {
                    m_Configuration = CreateInstance<ResourceRuleEditorData>();
                    m_Configuration.isActivate = true;
                    m_CurrentConfigPath = m_NormalConfigurationPath;
                    m_AllConfigPaths = new List<string>() { m_NormalConfigurationPath };
                    m_ConfigNames = new [] { Path.GetFileNameWithoutExtension(m_NormalConfigurationPath) };
                }
                else
                {
                    m_Configuration = LoadAssetAtPath<ResourceRuleEditorData>(m_AllConfigPaths[m_CurrentConfigIndex]);
                    if (!hasActivate)
                    {
                        if (!m_Configuration.isActivate)
                        {
                            m_Configuration.isActivate = true;
                            EditorUtility.SetDirty(m_Configuration);
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
            }
            else
            {
                m_CurrentConfigIndex = m_AllConfigPaths.ToList().FindIndex(0, str => string.Equals(m_CurrentConfigPath, str, StringComparison.Ordinal));
            }
            m_RuleList = null;
            
            EditorUtility.ClearProgressBar();
        }

        private T LoadAssetAtPath<T>(string path) where T : Object
        {
#if UNITY_5
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
            return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
#endif
        }

        private void InitRuleListDrawer()
        {
            m_RuleList = new ReorderableList(m_Configuration.rules, typeof(ResourceRule));
            m_RuleList.drawElementCallback = OnListElementGUI;
            m_RuleList.draggable = true;
            m_RuleList.elementHeight = 22;
            m_RuleList.headerHeight = 0;
            m_RuleList.onAddCallback = (list) => Add();
        }

        private void Add()
        {
            string path = SelectFolder();
            if (!string.IsNullOrEmpty(path))
            {
                var rule = new ResourceRule();
                rule.assetsDirectoryPath = path;
                m_Configuration.rules.Add(rule);
            }
        }

        private void OnListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index >= m_Configuration.rules.Count)
            {
                return;
            }
            const float GAP = 5;

            ResourceRule rule = m_Configuration.rules[index];
            rect.y++;

            Rect r = rect;
            r.width = 20 + GAP;
            r.height = 18;
            rule.valid = EditorGUI.Toggle(r, rule.valid);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMax + 200;
            float assetBundleNameLength = r.width;
            rule.name = EditorGUI.TextField(r, rule.name);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 240;
            rule.loadType = (LoadType)EditorGUI.EnumPopup(r, rule.loadType);

            r.xMin = r.xMax + GAP + 20;
            r.xMax = r.xMin + 30;
            rule.packed = EditorGUI.Toggle(r, rule.packed);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            rule.fileSystem = EditorGUI.TextField(r, rule.fileSystem);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            rule.groups = EditorGUI.TextField(r, rule.groups);

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            rule.variant = EditorGUI.TextField(r, rule.variant);
            if (!string.IsNullOrEmpty(rule.variant))
            {
                rule.variant = rule.variant.ToLower();
            }

            r.xMin = r.xMax + GAP;
            r.width = assetBundleNameLength + 65;
            GUI.enabled = false;
            EditorGUI.TextField(r, rule.assetsDirectoryPath.Replace("Assets/Res/", ""));
            GUI.enabled = true;

            r.xMin = r.xMax + GAP;
            r.width = 50;
            if (GUI.Button(r, "Select"))
            {
                var path = SelectFolder();
                if (!string.IsNullOrEmpty(path))
                    rule.assetsDirectoryPath = path;
            }

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 150;
            rule.filterType = (ResourceFilterType)EditorGUI.EnumPopup(r, rule.filterType);

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax;
            rule.searchPatterns = EditorGUI.TextField(r, rule.searchPatterns);
        }

        private string SelectFolder()
        {
            string dataPath = $"{Application.dataPath}/Res/";
            string selectedPath = EditorUtility.OpenFolderPanel("Path", dataPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(dataPath))
                {
                    return Utility.Path.GetRegularPath("Assets/Res/" + selectedPath.Substring(dataPath.Length));
                }
                else
                {
#if UNITY_2019_1_OR_NEWER
                    ShowNotification(new GUIContent("Can not be outside of 'Assets/Res/'!"), 2);
#else
                    ShowNotification(new GUIContent("Can not be outside of 'Assets/Res/'!"));
#endif
                }
            }
            return null;
        }

        private int m_CurrentConfigIndex;
        private string m_CurrentConfigPath;
        private List<string> m_AllConfigPaths;
        private string[] m_ConfigNames;

        private void OnListHeaderGUI()
        {
            const float GAP = 5;
            Rect rect = new Rect(GAP, 3, 40, 18);
            Rect configLabel = new Rect(rect.x, rect.y, 75, rect.height);
            EditorGUI.LabelField(configLabel, "Rule Config:");
            Rect configs = new Rect(configLabel.xMax + GAP, rect.y, 200, rect.height);
            m_CurrentConfigIndex = EditorGUI.Popup(configs, m_CurrentConfigIndex, m_ConfigNames);
            if (m_CurrentConfigPath != m_AllConfigPaths[m_CurrentConfigIndex])
            {
                m_CurrentConfigPath = m_AllConfigPaths[m_CurrentConfigIndex];
                m_Configuration = LoadAssetAtPath<ResourceRuleEditorData>(m_CurrentConfigPath);
                m_RuleList = null;
            }
            
            Rect isActivateLabel = new Rect(configs.xMax + GAP, rect.y, 60, rect.height);
            EditorGUI.LabelField(isActivateLabel, "Active:");
            
            GUI.enabled = false;
            Rect isActivate = new Rect(isActivateLabel.xMax + GAP, rect.y, 20, rect.height);
            EditorGUI.Toggle(isActivate, m_Configuration.isActivate);
            GUI.enabled = true;
            
            Rect activate = new Rect(isActivate.xMax + GAP, rect.y, 100, rect.height);
            if (GUI.Button(activate, "Active This"))
            {
                foreach (var configPath in m_AllConfigPaths)
                {
                    var data = LoadAssetAtPath<ResourceRuleEditorData>(configPath);
                    data.isActivate = false;
                    EditorUtility.SetDirty(data);
                }
                m_Configuration.isActivate = true;
                EditorUtility.SetDirty(m_Configuration);
                AssetDatabase.SaveAssets();
            }
            
            Rect reload = new Rect(activate.xMax + GAP, rect.y, 100, rect.height);
            if (GUI.Button(reload, "Reload"))
            {
                Load();
            }
            Rect save = new Rect(reload.xMax + GAP, rect.y, 100, rect.height);
            if (GUI.Button(save, "Save"))
            {
                Save();
            }
            Rect refresh = new Rect(save.xMax + GAP, rect.y, 200, rect.height);
            if (GUI.Button(refresh, "Refresh ResourceCollection.xml"))
            {
                RefreshResourceCollection();
            }
        }

        private void OnListElementLabelGUI()
        {
            Rect rect = new Rect();
            const float GAP = 5;
            GUI.enabled = false;

            Rect r = new Rect(GAP, 25, rect.width, rect.height);
            r.width = 41;
            r.height = 18;
            EditorGUI.TextField(r, "Active");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMax + 200;
            float assetBundleNameLength = r.width;
            EditorGUI.TextField(r, "Name");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 240;
            EditorGUI.TextField(r, "Load Type");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 50;
            EditorGUI.TextField(r, "Packed");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            EditorGUI.TextField(r, "File System");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            EditorGUI.TextField(r, "Groups");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 85;
            EditorGUI.TextField(r, "Variant");

            r.xMin = r.xMax + GAP;
            r.width = assetBundleNameLength + 119;
            EditorGUI.TextField(r, "AssetDirectory");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 150;
            EditorGUI.TextField(r, "Filter Type");

            r.xMin = r.xMax + GAP;
            r.xMax = r.xMin + 250;
            EditorGUI.TextField(r, "Patterns");
            GUI.enabled = true;
        }

        private void Save()
        {
            if (LoadAssetAtPath<ResourceRuleEditorData>(m_CurrentConfigPath) == null)
            {
                AssetDatabase.CreateAsset(m_Configuration, m_CurrentConfigPath);
            }
            else
            {
                if (!CheckRule())
                {
                    return;
                }
                EditorUtility.SetDirty(m_Configuration);
                AssetDatabase.SaveAssets();
            }
        }

        #region Refresh ResourceCollection.xml

        internal void RefreshResourceCollection()
        {
            if (m_Configuration == null)
            {
                Load();
            }
            if (!CheckRule())
            {
                throw new Exception("Refresh ResourceCollection.xml check rule fail.");
            }
            m_SourceAssetExceptTypeFilterGUIDArray = AssetDatabase.FindAssets(m_SourceAssetExceptTypeFilter);
            m_SourceAssetExceptLabelFilterGUIDArray = AssetDatabase.FindAssets(m_SourceAssetExceptLabelFilter);
            AnalysisResourceFilters();
            CheckRemoveEmptyResource();
            if (SaveCollection())
            {
                Debug.Log("Refresh ResourceCollection.xml success");
            }
            else
            {
                throw new Exception("Refresh ResourceCollection.xml fail");
            }
        }

        internal void RefreshResourceCollectionWithOptimize()
        {
            RefreshResourceCollection();
            ResourceOptimize resourceOptimize = new ResourceOptimize();
            resourceOptimize.Optimize(m_ResourceCollection);
        }

        internal void RefreshResourceCollection(string configPath)
        {
            if (m_Configuration == null || !m_CurrentConfigPath.Equals(configPath))
            {
                m_CurrentConfigPath = configPath;
                Load();
            }
            if (!CheckRule())
            {
                throw new Exception("Refresh ResourceCollection.xml check rule fail.");
            }
            m_SourceAssetExceptTypeFilterGUIDArray = AssetDatabase.FindAssets(m_SourceAssetExceptTypeFilter);
            m_SourceAssetExceptLabelFilterGUIDArray = AssetDatabase.FindAssets(m_SourceAssetExceptLabelFilter);
            AnalysisResourceFilters();
            CheckRemoveEmptyResource();
            if (SaveCollection())
            {
                Debug.Log("Refresh ResourceCollection.xml success");
            }
            else
            {
                throw new Exception("Refresh ResourceCollection.xml fail");
            }
        }

        internal void RefreshResourceCollectionWithOptimize(string configPath)
        {
            RefreshResourceCollection(configPath);
            ResourceOptimize resourceOptimize = new ResourceOptimize();
            resourceOptimize.Optimize(m_ResourceCollection);
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
            if (m_ResourceCollection.AssignAsset(assetGuid, resourceName, resourceVariant))
            {
                return true;
            }
            return false;
        }

        private void CheckRemoveEmptyResource()
        {
            GFResource[] resources = GetResources();
            int count = resources.Length;
            int cur = 0;
            foreach (GFResource resource in resources)
            {
                cur++;
                EditorUtility.DisplayProgressBar("OptimizeLoadType", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                if (resource.GetFirstAsset() == null)
                {
                    m_ResourceCollection.RemoveResource(resource.Name, resource.Variant);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private void AnalysisResourceFilters()
        {
            m_AssetGuidCache = new Dictionary<string, string>(StringComparer.Ordinal);
            m_ResourceCollection = new ResourceCollection();
            List<string> signedAssetBundleList = new List<string>();
            int count = m_Configuration.rules.Count;
            int cur = 0;
            foreach (ResourceRule resourceRule in m_Configuration.rules)
            {
                cur++;
                EditorUtility.DisplayProgressBar("AnalysisResourceFilters", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                if (resourceRule.variant == "")
                    resourceRule.variant = null;
                if (resourceRule.valid)
                {
                    switch (resourceRule.filterType)
                    {
                        case ResourceFilterType.Root:
                        {
                            if (string.IsNullOrEmpty(resourceRule.name))
                            {
                                string relativeDirectoryName = resourceRule.assetsDirectoryPath.Replace("Assets/", "");
                                ApplyResourceFilter(ref signedAssetBundleList, resourceRule, GameFramework.Utility.Path.GetRegularPath(relativeDirectoryName));
                            }
                            else
                            {
                                ApplyResourceFilter(ref signedAssetBundleList, resourceRule, resourceRule.name);
                            }
                        }
                            break;
                        case ResourceFilterType.Children:
                        {
                            string[] patterns = resourceRule.searchPatterns.Split(';', ',', '|');
                            for (int i = 0; i < patterns.Length; i++)
                            {
                                FileInfo[] assetFiles = new DirectoryInfo(resourceRule.assetsDirectoryPath).GetFiles(patterns[i], SearchOption.AllDirectories);
                                foreach (FileInfo file in assetFiles)
                                {
                                    if (file.Extension.Contains("meta", StringComparison.Ordinal))
                                        continue;
                                    string relativeAssetName = file.FullName.Substring(Application.dataPath.Length + 1);
                                    string relativeAssetNameWithoutExtension = Utility.Path.GetRegularPath(relativeAssetName.Substring(0, relativeAssetName.LastIndexOf('.')));
                                    string assetName = Path.Combine("Assets", relativeAssetName);
                                    string assetGUID = AssetPathToGUID(assetName);
                                    if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID, StringComparer.Ordinal) && !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID, StringComparer.Ordinal))
                                    {
                                        ApplyResourceFilter(ref signedAssetBundleList, resourceRule, relativeAssetNameWithoutExtension, assetGUID);
                                    }
                                }
                            }
                        }
                            break;
                        case ResourceFilterType.ChildrenFoldersOnly:
                        {
                            DirectoryInfo[] assetDirectories = new DirectoryInfo(resourceRule.assetsDirectoryPath).GetDirectories();
                            foreach (DirectoryInfo directory in assetDirectories)
                            {
                                string relativeDirectoryName = directory.FullName.Substring(Application.dataPath.Length + 1);
                                ApplyResourceFilter(ref signedAssetBundleList, resourceRule, Utility.Path.GetRegularPath(relativeDirectoryName), string.Empty, directory.FullName);
                            }
                        }
                            break;
                        case ResourceFilterType.ChildrenFilesOnly:
                        {
                            DirectoryInfo[] assetDirectories = new DirectoryInfo(resourceRule.assetsDirectoryPath).GetDirectories();
                            foreach (DirectoryInfo directory in assetDirectories)
                            {
                                string[] patterns = resourceRule.searchPatterns.Split(';', ',', '|');
                                for (int i = 0; i < patterns.Length; i++)
                                {
                                    FileInfo[] assetFiles = new DirectoryInfo(directory.FullName).GetFiles(patterns[i], SearchOption.AllDirectories);
                                    foreach (FileInfo file in assetFiles)
                                    {
                                        if (file.Extension.Contains("meta", StringComparison.Ordinal))
                                            continue;
                                        string relativeAssetName = file.FullName.Substring(Application.dataPath.Length + 1);
                                        string relativeAssetNameWithoutExtension = Utility.Path.GetRegularPath(relativeAssetName.Substring(0, relativeAssetName.LastIndexOf('.')));
                                        string assetName = Path.Combine("Assets", relativeAssetName);
                                        string assetGUID = AssetPathToGUID(assetName);
                                        if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID, StringComparer.Ordinal) && !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID, StringComparer.Ordinal))
                                        {
                                            ApplyResourceFilter(ref signedAssetBundleList, resourceRule, relativeAssetNameWithoutExtension, assetGUID);
                                        }
                                    }
                                }
                            }
                        }
                            break;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private void ApplyResourceFilter(ref List<string> signedResourceList, ResourceRule resourceRule, string resourceName, string singleAssetGUID = "", string childDirectoryPath = "")
        {
            string resourcePath = Path.Combine(resourceRule.assetsDirectoryPath, resourceName);
            if (!signedResourceList.Contains(resourcePath, StringComparer.Ordinal))
            {
                signedResourceList.Add(resourcePath);
                foreach (GFResource oldResource in GetResources())
                {
                    if (string.Equals(oldResource.Name, resourceName, StringComparison.Ordinal) && string.IsNullOrEmpty(oldResource.Variant))
                    {
                        RenameResource(oldResource.Name, oldResource.Variant, resourceName, resourceRule.variant);
                        break;
                    }
                }
                if (!HasResource(resourceName, resourceRule.variant))
                {
                    if (string.IsNullOrEmpty(resourceRule.fileSystem))
                    {
                        resourceRule.fileSystem = null;
                    }
                    AddResource(resourceName, resourceRule.variant, resourceRule.fileSystem, resourceRule.loadType, resourceRule.packed, resourceRule.groups.Split(';', ',', '|'));
                }
                switch (resourceRule.filterType)
                {
                    case ResourceFilterType.Root:
                    case ResourceFilterType.ChildrenFoldersOnly:
                    {
                        string[] patterns = resourceRule.searchPatterns.Split(';', ',', '|');
                        if (childDirectoryPath == "")
                        {
                            childDirectoryPath = resourceRule.assetsDirectoryPath;
                        }

                        for (int i = 0; i < patterns.Length; i++)
                        {
                            FileInfo[] assetFiles = new DirectoryInfo(childDirectoryPath).GetFiles(patterns[i], SearchOption.AllDirectories);
                            foreach (FileInfo file in assetFiles)
                            {
                                if (file.Extension.Contains("meta", StringComparison.Ordinal))
                                    continue;
                                string assetName = Path.Combine("Assets", file.FullName.Substring(Application.dataPath.Length + 1));
                                string assetGUID = AssetPathToGUID(assetName);
                                if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID, StringComparer.Ordinal) && !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID, StringComparer.Ordinal))
                                {
                                    AssignAsset(assetGUID, resourceName, resourceRule.variant);
                                }
                            }
                        }
                    }
                        break;
                    case ResourceFilterType.Children:
                    case ResourceFilterType.ChildrenFilesOnly:
                    {
                        AssignAsset(singleAssetGUID, resourceName, resourceRule.variant);
                    }
                        break;
                }
            }
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

        #endregion
    }
}