using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GameFramework;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFramework.Extension.Editor
{
    public sealed partial class ResourceOptimize
    {
        private const long MAX_COMBINE_SHARE_AB_ITEM_SIZE = 500 * 1024 * 8; // 500K 文件体积小于这个数量才能合并
        private const long MAX_COMBINE_SHARE_AB_SIZE = 1024 * 1024 * 8; // 1M 最终合并的目标大小
        private const long MIN_NO_NAME_COMBINE_SIZE = 32 * 1024 * 8; // 32K 最终合并的目标大小
        // public const long MAX_COMBINE_SHARE_NO_NAME = 60 * 1024 * 8; // 60K 没有包名的最大体积 
        // public const int MAX_COMBINE_SHARE_NO_NAME_REFERENCE_COUNT = 7; // 没有包名的最多的引用计数
        //  public const int MIN_COMBINE_AB_SIZE_2 = 100 * 1024 * 8;//  100K 没有包名的最大体积 
        private const int MAX_COMBINE_SHARE_MIN_REFERENCE_COUNT = 3; //最大的引用计数

        //有需要改名，修改这里
        public static string GetNewCombineName(List<string> currentCombineBundle)
        {
            string newCombine = string.Join("@@", currentCombineBundle);
            return Utility.Text.Format("Auto/Combine/{0:x8}", Utility.Verifier.GetCrc32(Encoding.UTF8.GetBytes(newCombine)));
        }

        private ResourceCollection m_ResourceCollection;

        private readonly Dictionary<string, DependencyData> m_DependencyDatas = new();
        //key：冗余资源路径，value：引用该资源的主资源
        private readonly Dictionary<string, List<Asset>> m_ScatteredAssets = new();
        private readonly HashSet<Stamp> m_AnalyzedStamps = new();
        private readonly Dictionary<string, List<string>> m_CombineBundles = new();
        private readonly MethodInfo m_GetStorageMemorySizeLongMethod = typeof(EditorWindow).Assembly.GetType("UnityEditor.TextureUtil").GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Public);
        private readonly object[] m_ParamCache = new object[1];
        private readonly Dictionary<string, string[]> m_DependencyCachePool = new();
        private readonly Dictionary<string, long> m_AssetSizeCache = new();

        [MenuItem("Game Framework/Resource Tools/Resource Optimize", false, 52)]
        static void StartOptimize()
        {
            ResourceCollection resourceCollection = new ResourceCollection();
            resourceCollection.Load();
            ResourceOptimize optimize = new ResourceOptimize();
            optimize.Optimize(resourceCollection);
        }

        public void Optimize(ResourceCollection resourceCollection)
        {
            if (resourceCollection == null)
            {
                throw new GameFrameworkException("ResourceCollection is invalid.");
            }
            m_ResourceCollection = resourceCollection;
            RemoveOldCombineResources();
            AddLauncherResource();
            OptimizeLoadType();
            OptimizeSprite();
            Analyze();
            CalculateCombine();
            Save();
        }

        private void RemoveOldCombineResources()
        {
            Resource[] resources = m_ResourceCollection.GetResources();
            foreach (Resource resource in resources)
            {
                if (resource.Name.StartsWith("Auto/Combine/", StringComparison.Ordinal))
                {
                    m_ResourceCollection.RemoveResource(resource.Name, resource.Variant);
                }
            }
        }

        private void AddLauncherResource()
        {
            m_ResourceCollection.RemoveResource(GameEntryLoader.LauncherResourceName, null);
#if UNITY_WEBGL
            m_ResourceCollection.AddResource(GameEntryLoader.LauncherResourceName, null, null, LoadType.LoadFromMemory, true, new string[] { GameEntryLoader.LauncherResourceGroupName });
#else
            m_ResourceCollection.AddResource(GameEntryLoader.LauncherResourceName, null, null, LoadType.LoadFromFile, true, new string[] { GameEntryLoader.LauncherResourceGroupName });
#endif
            string guid = AssetDatabase.AssetPathToGUID(GameEntryLoader.GameEntryPrefabAssetPath);
            m_ResourceCollection.UnassignAsset(guid);
            if (!m_ResourceCollection.AssignAsset(guid, GameEntryLoader.LauncherResourceName, null))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not assign asset '{0}' to resource '{1}'.", GameEntryLoader.GameEntryPrefabAssetPath, GameEntryLoader.LauncherResourceName));
            }
        }

        private void OptimizeLoadType()
        {
#if UNITY_WEBGL
            Resource[] resources = m_ResourceCollection.GetResources();
            int count = resources.Length;
            for (int i = 0; i < count; i++)
            {
                int cur = i + 1;
                EditorUtility.DisplayProgressBar("OptimizeLoadType", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                Resource resource = resources[i];
                if(resource.LoadType != LoadType.LoadFromMemory &&
                   resource.LoadType != LoadType.LoadFromMemoryAndDecrypt &&
                   resource.LoadType != LoadType.LoadFromMemoryAndQuickDecrypt)
                {
                    resource.LoadType = LoadType.LoadFromMemory;
                    Debug.Log($"UNITY_WEBGL下修改资源\"{resource.Name}\"的加载方式为LoadFromMemory");
                }
                if (!string.IsNullOrEmpty(resource.FileSystem))
                {
                    resource.FileSystem = string.Empty;
                    Debug.Log($"UNITY_WEBGL下删除资源\"{resource.Name}\"的文件系统");
                }
            }
#endif
            EditorUtility.ClearProgressBar();
        }

        private void OptimizeSprite()
        {
            Dictionary<string, string> sprite2Atlas = new Dictionary<string, string>();
            string[] searchInFolders = new string[] { "Assets/Res" };
            List<string> saGuids = AssetDatabase.FindAssets("t:SpriteAtlas", searchInFolders).ToList();
            for (int i = saGuids.Count - 1; i >= 0; i--)
            {
                string saGuid = saGuids[i];
                if (m_ResourceCollection.HasAsset(saGuid))
                {
                    saGuids.RemoveAt(i);
                    continue;
                }
                string saPath = AssetDatabase.GUIDToAssetPath(saGuid);
                SpriteAtlas sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(saPath);
                foreach (string spritePath in GetSpriteAssetPaths(sa))
                {
                    string spriteGuid = AssetDatabase.AssetPathToGUID(spritePath);
                    if (!sprite2Atlas.TryGetValue(spriteGuid, out string atlasPath))
                    {
                        sprite2Atlas.Add(spriteGuid, saPath);
                    }
                    else if (atlasPath != saPath)
                    {
                        throw new GameFrameworkException($"[{spritePath}]被重复添加进不同的SpriteAtlas（[{atlasPath}]，[{saPath}]），请检查清理！");
                    }
                }
            }
            foreach (Asset asset in m_ResourceCollection.GetAssets())
            {
                if (sprite2Atlas.ContainsKey(asset.Guid) || saGuids.Contains(asset.Guid))
                {
                    asset.Resource.UnassignAsset(asset);
                }
            }
            saGuids.Sort((a, b) => String.Compare(a, b, StringComparison.Ordinal));
            foreach (string saGuid in saGuids)
            {
                string saPath = AssetDatabase.GUIDToAssetPath(saGuid);
                SpriteAtlas sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(saPath);
                SortedSet<string> assets = GetSpriteAssetPaths(sa);
                assets.Add(saPath);
                List<string> assetList = assets.ToList();
                string newBundleName = GetNewCombineName(assetList);
#if UNITY_WEBGL
                //WebGL下不能使用LoadFromFile
                m_ResourceCollection.AddResource(newBundleName, null, null, LoadType.LoadFromMemory, false);
#else
                m_ResourceCollection.AddResource(newBundleName, null, null, LoadType.LoadFromFile, false);
#endif
                foreach (string asset in assetList)
                {
                    m_ResourceCollection.AssignAsset(AssetDatabase.AssetPathToGUID(asset), newBundleName, null);
                }
            }
        }

        private static SortedSet<string> GetSpriteAssetPaths(SpriteAtlas spriteAtlas)
        {
            SortedSet<string> spriteAssetPaths = new SortedSet<string>(StringComparer.Ordinal);
            if (spriteAtlas == null)
            {
                return spriteAssetPaths;
            }

            int spriteCount = spriteAtlas.spriteCount;
            if (spriteCount > 0)
            {
                Sprite[] sprites = new Sprite[spriteCount];
                spriteAtlas.GetSprites(sprites);
                foreach (Sprite sprite in sprites)
                {
                    if (sprite == null)
                    {
                        continue;
                    }

                    string spritePath = AssetDatabase.GetAssetPath(sprite);
                    if (!string.IsNullOrEmpty(spritePath))
                    {
                        spriteAssetPaths.Add(spritePath);
                    }
                }
            }

            UnityEngine.Object[] packables = SpriteAtlasExtensions.GetPackables(spriteAtlas);
            foreach (UnityEngine.Object packable in packables)
            {
                if (packable == null)
                {
                    continue;
                }

                string packablePath = AssetDatabase.GetAssetPath(packable);
                if (string.IsNullOrEmpty(packablePath))
                {
                    continue;
                }

                if (AssetDatabase.IsValidFolder(packablePath))
                {
                    string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite", new string[] { packablePath });
                    foreach (string spriteGuid in spriteGuids)
                    {
                        string spritePath = AssetDatabase.GUIDToAssetPath(spriteGuid);
                        if (!string.IsNullOrEmpty(spritePath))
                        {
                            spriteAssetPaths.Add(spritePath);
                        }
                    }
                }
                else
                {
                    UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(packablePath);
                    foreach (UnityEngine.Object obj in objects)
                    {
                        if (obj is Sprite)
                        {
                            spriteAssetPaths.Add(packablePath);
                            break;
                        }
                    }
                }
            }

            return spriteAssetPaths;
        }

        private void Save()
        {
            int count = m_CombineBundles.Count;
            int cur = 0;
            foreach (KeyValuePair<string, List<string>> kv in m_CombineBundles)
            {
                cur++;
                EditorUtility.DisplayProgressBar("Save", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
#if UNITY_WEBGL
                //WebGL下不能使用LoadFromFile
                m_ResourceCollection.AddResource(kv.Key, null, null, LoadType.LoadFromMemory, false);
#else
                m_ResourceCollection.AddResource(kv.Key, null, null, LoadType.LoadFromFile, false);
#endif
                foreach (string name in kv.Value)
                {
                    m_ResourceCollection.AssignAsset(AssetDatabase.AssetPathToGUID(name), kv.Key, null);
                }
            }
            m_ResourceCollection.Save();
            EditorUtility.ClearProgressBar();
        }

        private void CalculateCombine()
        {
            m_CombineBundles.Clear();
            Dictionary<string, ABInfo> allCombines = new Dictionary<string, ABInfo>();
            int allShareCount = 0; //所有的share ab 的数量
            int allShareCanCombine = 0; // 所有size 合格的ab
            int allShareRemoveByNoName = 0; // 因为 size 太小，ref count 太少 放弃合并，放弃包名
            int allShareRemoveByReferenceCountTooFew = 0; // 因为 ref count 太少 放弃合并
            int allFinalCombine = 0; // 最终合并的数量

            int count = m_ScatteredAssets.Count;
            int cur = 0;
            foreach (KeyValuePair<string, List<Asset>> kv in m_ScatteredAssets)
            {
                cur++;
                EditorUtility.DisplayProgressBar("CalculateCombine (1/3)", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                string assetPath = kv.Key;
                allShareCount++;
                if (!File.Exists(assetPath))
                {
                    Debug.LogError(Utility.Text.Format("File do not exist :{0}", assetPath));
                    continue;
                }

                long byteSize = GetAssetSize(assetPath);
                if (byteSize < MAX_COMBINE_SHARE_AB_ITEM_SIZE)
                {
                    allCombines.Add(assetPath, new ABInfo(assetPath, byteSize, kv.Value.Count));
                    allShareCanCombine++;
                }
                else
                {
                    //太大的直接单个打包
                    if(kv.Value.Count > 1)
                    {
                        List<string> bundle = new List<string>()
                        {
                            assetPath
                        };
                        m_CombineBundles[GetNewCombineName(bundle)] = bundle;
                    }
                }
            }

            count = allCombines.Count;
            cur = 0;
            HashSet<string> removedKeys = new HashSet<string>();
            foreach (ABInfo abInfo in allCombines.Values)
            {
                cur++;
                EditorUtility.DisplayProgressBar("CalculateCombine (2/3)", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                string bundleName = abInfo.Name;
                if (abInfo.Size * abInfo.ReferenceCount < MIN_NO_NAME_COMBINE_SIZE)
                {
                    allShareRemoveByNoName++;
                    removedKeys.Add(bundleName);
                }
                else
                {
                    if (abInfo.ReferenceCount < MAX_COMBINE_SHARE_MIN_REFERENCE_COUNT)
                    {
                        allShareRemoveByReferenceCountTooFew++;
                        removedKeys.Add(bundleName);
                    }
                }
            }
            foreach (string key in removedKeys)
            {
                allCombines.Remove(key);
            }

            List<ABInfo> left =  allCombines.Values.ToList();
            //优先用名字排序，这样相同目录（往往是相同资源的依赖）的尽可能规划到一起
            left.Sort((a,b) =>
            {
                int c1 = string.Compare(a.Name, b.Name, StringComparison.Ordinal);
                if(c1 == 0)
                {
                    int c2 = a.ReferenceCount.CompareTo(b.ReferenceCount);
                    if (c2 == 0)
                    {
                        return a.Size.CompareTo(b.Size);
                    }
                    return c2;
                }
                return c1;
            });
            allFinalCombine = left.Count;
            List<string> currentCombineBundles = new List<string>();
            long currentCombineBundleSize = 0;
            count = left.Count;
            cur = 0;
            foreach (ABInfo abInfo in left)
            {
                cur++;
                EditorUtility.DisplayProgressBar("CalculateCombine (3/3)", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                if (currentCombineBundles.Count > 0 && currentCombineBundleSize + abInfo.Size > MAX_COMBINE_SHARE_AB_SIZE)
                {
                    m_CombineBundles[GetNewCombineName(currentCombineBundles)] = currentCombineBundles;
                    currentCombineBundles = new List<string>();
                    currentCombineBundleSize = 0;
                }
                currentCombineBundles.Add(abInfo.Name);
                currentCombineBundleSize += abInfo.Size;
            }
            if (currentCombineBundles.Count > 0)
            {
                m_CombineBundles[GetNewCombineName(currentCombineBundles)] = currentCombineBundles;
            }
            Debug.Log($"总共有share ab的数量{allShareCount}，大小合格的数量{allShareCanCombine}，" +
                      $"因为ab太小和引用计数太少而被取消包名的数量{allShareRemoveByNoName}，" +
                      $"因为引用过少被移除合并的数量{allShareRemoveByReferenceCountTooFew}，" +
                      $"最终{allFinalCombine}个share ab，" +
                      $"合并成{m_CombineBundles.Count}个share_combine，" +
                      $"因为这次合并操作，总共减少了{(m_CombineBundles.Count > 0 ? allShareRemoveByNoName + allFinalCombine - m_CombineBundles.Count : 0)}个share bundle");
            EditorUtility.ClearProgressBar();
        }

        private void Analyze()
        {
            m_DependencyDatas.Clear();
            m_ScatteredAssets.Clear();
            m_AnalyzedStamps.Clear();
            m_DependencyCachePool.Clear();

            HashSet<string> excludeAssetNames = GetFilteredAssetNames("t:Script t:SubGraphAsset t:Preset");
            Asset[] assets = m_ResourceCollection.GetAssets();
            int count = assets.Length;
            int cur = 0;
            for (int i = 0; i < count; i++)
            {
                cur++;
                EditorUtility.DisplayProgressBar("Analyze (1/2)", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                string assetName = assets[i].Name;
                if (string.IsNullOrEmpty(assetName))
                {
                    Debug.LogWarning(Utility.Text.Format("Can not find asset by guid '{0}'.", assets[i].Guid));
                    continue;
                }

                DependencyData dependencyData = new DependencyData();
                AnalyzeAsset(assetName, assets[i], excludeAssetNames, ref dependencyData);
                dependencyData.RefreshData();
                m_DependencyDatas.Add(assetName, dependencyData);
            }

            count = m_ScatteredAssets.Count;
            cur = 0;
            foreach (List<Asset> scatteredAsset in m_ScatteredAssets.Values)
            {
                cur++;
                EditorUtility.DisplayProgressBar("Analyze (2/2)", Utility.Text.Format("{0}/{1} processing...", cur, count), (float)cur / count);
                scatteredAsset.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            }
            EditorUtility.ClearProgressBar();
        }

        private void AnalyzeAsset(string assetName, Asset hostAsset, HashSet<string> excludeAssetNames, ref DependencyData dependencyData)
        {
            string[] dependencyAssetNames = GetDependencies(assetName);
            foreach (string dependencyAssetName in dependencyAssetNames)
            {
                if (excludeAssetNames.Contains(dependencyAssetName))
                {
                    continue;
                }

                if (dependencyAssetName == assetName)
                {
                    continue;
                }

                if (dependencyAssetName.EndsWith(".unity", StringComparison.Ordinal))
                {
                    // 忽略对场景的依赖
                    continue;
                }
                if (AssetDatabase.IsValidFolder(dependencyAssetName))
                {
                    // 忽略对目录的依赖（部分编辑数据）
                    continue;
                }

                Stamp stamp = new Stamp(hostAsset.Name, dependencyAssetName);
                if (!m_AnalyzedStamps.Add(stamp))
                {
                    continue;
                }

                string guid = AssetDatabase.AssetPathToGUID(dependencyAssetName);
                if (string.IsNullOrEmpty(guid))
                {
                    Debug.LogWarning(Utility.Text.Format("Can not find guid by asset '{0}'.", dependencyAssetName));
                    continue;
                }

                Asset asset = m_ResourceCollection.GetAsset(guid);
                if (asset != null)
                {
                    dependencyData.AddDependencyAsset(asset);
                }
                else
                {
                    dependencyData.AddScatteredDependencyAsset(dependencyAssetName);

                    List<Asset> scatteredAssets = null;
                    if (!m_ScatteredAssets.TryGetValue(dependencyAssetName, out scatteredAssets))
                    {
                        scatteredAssets = new List<Asset>();
                        m_ScatteredAssets.Add(dependencyAssetName, scatteredAssets);
                    }

                    scatteredAssets.Add(hostAsset);

                    AnalyzeAsset(dependencyAssetName, hostAsset, excludeAssetNames, ref dependencyData);
                }
            }
        }

        private HashSet<string> GetFilteredAssetNames(string filter)
        {
            string[] filterAssetGuids = AssetDatabase.FindAssets(filter);
            HashSet<string> filterAssetNames = new HashSet<string>();
            foreach (string filterAssetGuid in filterAssetGuids)
            {
                filterAssetNames.Add(AssetDatabase.GUIDToAssetPath(filterAssetGuid));
            }

            return filterAssetNames;
        }

        private long GetAssetSize(string assetPath)
        {
            if (m_AssetSizeCache.TryGetValue(assetPath, out long size))
            {
                return size;
            }
            Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (type == typeof(Texture2D))
            {
                //记录精准的贴图大小
                Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                m_ParamCache[0] = texture2D;
                size = (long)m_GetStorageMemorySizeLongMethod.Invoke(null, m_ParamCache);
                m_AssetSizeCache.Add(assetPath, size);
                Resources.UnloadAsset(texture2D);
                return size;
            }
            else
            {
                //直接使用文件长度
                size = new FileInfo(assetPath).Length;
                m_AssetSizeCache.Add(assetPath, size);
                return size;
            }
        }

        private string[] GetDependencies(string assetName)
        {
            if(m_DependencyCachePool.TryGetValue(assetName, out string[] dependencies))
            {
                return dependencies;
            }
            else
            {
                string[] dependencyAssetNames = AssetDatabase.GetDependencies(assetName, false);
                m_DependencyCachePool.Add(assetName, dependencyAssetNames);
                return dependencyAssetNames;
            }
        }
    }
}
