using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension.Editor
{
    /// <summary>
    /// 资源版本分析控制器。
    /// </summary>
    public sealed partial class ResourceVersionAnalyzerController
    {
        private const string DefaultExtension = "dat";
        private const int HeaderLength = 3;
        private static readonly byte[] PackageListHeader = new byte[] { (byte)'G', (byte)'F', (byte)'P' };
        private static readonly byte[] UpdatableListHeader = new byte[] { (byte)'G', (byte)'F', (byte)'U' };

        private readonly PackageVersionListSerializer m_PackageVersionListSerializer;
        private readonly UpdatableVersionListSerializer m_UpdatableVersionListSerializer;

        public ResourceVersionAnalyzerController()
        {
            m_PackageVersionListSerializer = new PackageVersionListSerializer();
            m_PackageVersionListSerializer.RegisterDeserializeCallback(0, BuiltinVersionListSerializer.PackageVersionListDeserializeCallback_V0);
            m_PackageVersionListSerializer.RegisterDeserializeCallback(1, BuiltinVersionListSerializer.PackageVersionListDeserializeCallback_V1);
            m_PackageVersionListSerializer.RegisterDeserializeCallback(2, BuiltinVersionListSerializer.PackageVersionListDeserializeCallback_V2);

            m_UpdatableVersionListSerializer = new UpdatableVersionListSerializer();
            m_UpdatableVersionListSerializer.RegisterDeserializeCallback(0, BuiltinVersionListSerializer.UpdatableVersionListDeserializeCallback_V0);
            m_UpdatableVersionListSerializer.RegisterDeserializeCallback(1, BuiltinVersionListSerializer.UpdatableVersionListDeserializeCallback_V1);
            m_UpdatableVersionListSerializer.RegisterDeserializeCallback(2, BuiltinVersionListSerializer.UpdatableVersionListDeserializeCallback_V2);
        }

        public VersionData Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new GameFrameworkException("Version list file path is invalid.");
            }

            if (!File.Exists(filePath))
            {
                throw new GameFrameworkException(Utility.Text.Format("Version list file '{0}' is not exist.", filePath));
            }

            byte[] bytes = File.ReadAllBytes(filePath);
            if (bytes.Length < HeaderLength + 1)
            {
                throw new GameFrameworkException(Utility.Text.Format("Version list file '{0}' is too small.", filePath));
            }

            if (CheckHeader(bytes, PackageListHeader))
            {
                return LoadPackageVersionList(filePath, bytes);
            }

            if (CheckHeader(bytes, UpdatableListHeader))
            {
                return LoadUpdatableVersionList(filePath, bytes);
            }

            throw new GameFrameworkException(Utility.Text.Format("Version list file '{0}' has unknown format. Only Package (GFP) and Updatable (GFU) formats are supported.", filePath));
        }

        public CompareResult Compare(VersionData versionA, VersionData versionB)
        {
            Dictionary<string, ResourceInfo> resourcesA = new Dictionary<string, ResourceInfo>();
            for (int i = 0; i < versionA.Resources.Length; i++)
            {
                resourcesA[versionA.Resources[i].FullName] = versionA.Resources[i];
            }

            Dictionary<string, ResourceInfo> resourcesB = new Dictionary<string, ResourceInfo>();
            for (int i = 0; i < versionB.Resources.Length; i++)
            {
                resourcesB[versionB.Resources[i].FullName] = versionB.Resources[i];
            }

            List<CompareItem> items = new List<CompareItem>();

            foreach (KeyValuePair<string, ResourceInfo> pair in resourcesA)
            {
                ResourceInfo infoB;
                if (resourcesB.TryGetValue(pair.Key, out infoB))
                {
                    if (pair.Value.Length != infoB.Length || pair.Value.HashCode != infoB.HashCode)
                    {
                        items.Add(new CompareItem(CompareStatus.Modified, pair.Value, infoB));
                    }
                    else
                    {
                        items.Add(new CompareItem(CompareStatus.Unchanged, pair.Value, infoB));
                    }
                }
                else
                {
                    items.Add(new CompareItem(CompareStatus.Removed, pair.Value, null));
                }
            }

            foreach (KeyValuePair<string, ResourceInfo> pair in resourcesB)
            {
                if (!resourcesA.ContainsKey(pair.Key))
                {
                    items.Add(new CompareItem(CompareStatus.Added, null, pair.Value));
                }
            }

            return new CompareResult(items.ToArray());
        }

        public DistributionInfo[] GetDistributionByExtension(VersionData data)
        {
            Dictionary<string, long[]> dict = new Dictionary<string, long[]>();
            for (int i = 0; i < data.Resources.Length; i++)
            {
                string ext = data.Resources[i].Extension;
                long[] values;
                if (!dict.TryGetValue(ext, out values))
                {
                    values = new long[] { 0, 0, 0 };
                    dict[ext] = values;
                }

                values[0]++;
                values[1] += data.Resources[i].Length;
                values[2] += data.Resources[i].CompressedLength;
            }

            return dict.OrderByDescending(p => p.Value[1])
                .Select(p => new DistributionInfo(p.Key, (int)p.Value[0], p.Value[1], p.Value[2]))
                .ToArray();
        }

        public DistributionInfo[] GetDistributionByFileSystem(VersionData data)
        {
            Dictionary<string, long[]> dict = new Dictionary<string, long[]>();
            for (int i = 0; i < data.Resources.Length; i++)
            {
                string fs = data.Resources[i].FileSystem ?? "<None>";
                long[] values;
                if (!dict.TryGetValue(fs, out values))
                {
                    values = new long[] { 0, 0, 0 };
                    dict[fs] = values;
                }

                values[0]++;
                values[1] += data.Resources[i].Length;
                values[2] += data.Resources[i].CompressedLength;
            }

            return dict.OrderByDescending(p => p.Value[1])
                .Select(p => new DistributionInfo(p.Key, (int)p.Value[0], p.Value[1], p.Value[2]))
                .ToArray();
        }

        public DistributionInfo[] GetDistributionByResourceGroup(VersionData data)
        {
            Dictionary<string, long[]> dict = new Dictionary<string, long[]>();
            for (int i = 0; i < data.Resources.Length; i++)
            {
                string[] groups = data.Resources[i].ResourceGroups;
                if (groups == null || groups.Length == 0)
                {
                    long[] noGroupValues;
                    if (!dict.TryGetValue("<None>", out noGroupValues))
                    {
                        noGroupValues = new long[] { 0, 0, 0 };
                        dict["<None>"] = noGroupValues;
                    }

                    noGroupValues[0]++;
                    noGroupValues[1] += data.Resources[i].Length;
                    noGroupValues[2] += data.Resources[i].CompressedLength;
                    continue;
                }

                for (int j = 0; j < groups.Length; j++)
                {
                    long[] values;
                    if (!dict.TryGetValue(groups[j], out values))
                    {
                        values = new long[] { 0, 0, 0 };
                        dict[groups[j]] = values;
                    }

                    values[0]++;
                    values[1] += data.Resources[i].Length;
                    values[2] += data.Resources[i].CompressedLength;
                }
            }

            return dict.OrderByDescending(p => p.Value[1])
                .Select(p => new DistributionInfo(p.Key, (int)p.Value[0], p.Value[1], p.Value[2]))
                .ToArray();
        }

        private static bool CheckHeader(byte[] bytes, byte[] header)
        {
            for (int i = 0; i < header.Length; i++)
            {
                if (bytes[i] != header[i])
                {
                    return false;
                }
            }

            return true;
        }

        private VersionData LoadPackageVersionList(string filePath, byte[] bytes)
        {
            PackageVersionList versionList;
            using (MemoryStream stream = new MemoryStream(bytes, false))
            {
                versionList = m_PackageVersionListSerializer.Deserialize(stream);
            }

            if (!versionList.IsValid)
            {
                throw new GameFrameworkException(Utility.Text.Format("Package version list file '{0}' is invalid.", filePath));
            }

            PackageVersionList.Resource[] resources = versionList.GetResources();
            PackageVersionList.Asset[] assets = versionList.GetAssets();
            PackageVersionList.FileSystem[] fileSystems = versionList.GetFileSystems();
            PackageVersionList.ResourceGroup[] resourceGroups = versionList.GetResourceGroups();

            Dictionary<int, string> resourceIndexToFileSystem = BuildFileSystemLookup(fileSystems);
            Dictionary<int, List<string>> resourceIndexToGroups = BuildResourceGroupLookup(resourceGroups);

            // Build asset index → resource full name mapping
            Dictionary<int, string> assetIndexToResourceFullName = new Dictionary<int, string>();
            long totalLength = 0;
            ResourceInfo[] resourceInfos = new ResourceInfo[resources.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                string fileSystem;
                resourceIndexToFileSystem.TryGetValue(i, out fileSystem);

                List<string> groups;
                string[] groupArray = null;
                if (resourceIndexToGroups.TryGetValue(i, out groups))
                {
                    groupArray = groups.ToArray();
                }

                int[] assetIndexes = resources[i].GetAssetIndexes();
                resourceInfos[i] = new ResourceInfo(
                    resources[i].Name, resources[i].Variant, resources[i].Extension,
                    resources[i].LoadType, resources[i].Length, resources[i].HashCode,
                    0, 0, assetIndexes.Length, fileSystem, groupArray);

                for (int j = 0; j < assetIndexes.Length; j++)
                {
                    assetIndexToResourceFullName[assetIndexes[j]] = resourceInfos[i].FullName;
                }

                totalLength += resources[i].Length;
            }

            AssetInfo[] assetInfos = BuildAssetInfos(assets, assetIndexToResourceFullName);

            return new VersionData(filePath, VersionListType.Package,
                versionList.ApplicableGameVersion, versionList.InternalResourceVersion,
                resourceInfos, assetInfos, fileSystems.Length, resourceGroups.Length,
                totalLength, 0);
        }

        private VersionData LoadUpdatableVersionList(string filePath, byte[] bytes)
        {
            UpdatableVersionList versionList;
            using (MemoryStream stream = new MemoryStream(bytes, false))
            {
                versionList = m_UpdatableVersionListSerializer.Deserialize(stream);
            }

            if (!versionList.IsValid)
            {
                throw new GameFrameworkException(Utility.Text.Format("Updatable version list file '{0}' is invalid.", filePath));
            }

            UpdatableVersionList.Resource[] resources = versionList.GetResources();
            UpdatableVersionList.Asset[] assets = versionList.GetAssets();
            UpdatableVersionList.FileSystem[] fileSystems = versionList.GetFileSystems();
            UpdatableVersionList.ResourceGroup[] resourceGroups = versionList.GetResourceGroups();

            Dictionary<int, string> resourceIndexToFileSystem = BuildFileSystemLookup(fileSystems);
            Dictionary<int, List<string>> resourceIndexToGroups = BuildResourceGroupLookup(resourceGroups);

            // Build asset index → resource full name mapping
            Dictionary<int, string> assetIndexToResourceFullName = new Dictionary<int, string>();
            long totalLength = 0;
            long totalCompressedLength = 0;
            ResourceInfo[] resourceInfos = new ResourceInfo[resources.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                string fileSystem;
                resourceIndexToFileSystem.TryGetValue(i, out fileSystem);

                List<string> groups;
                string[] groupArray = null;
                if (resourceIndexToGroups.TryGetValue(i, out groups))
                {
                    groupArray = groups.ToArray();
                }

                int[] assetIndexes = resources[i].GetAssetIndexes();
                resourceInfos[i] = new ResourceInfo(
                    resources[i].Name, resources[i].Variant, resources[i].Extension,
                    resources[i].LoadType, resources[i].Length, resources[i].HashCode,
                    resources[i].CompressedLength, resources[i].CompressedHashCode,
                    assetIndexes.Length, fileSystem, groupArray);

                for (int j = 0; j < assetIndexes.Length; j++)
                {
                    assetIndexToResourceFullName[assetIndexes[j]] = resourceInfos[i].FullName;
                }

                totalLength += resources[i].Length;
                totalCompressedLength += resources[i].CompressedLength;
            }

            AssetInfo[] assetInfos = BuildAssetInfos(assets, assetIndexToResourceFullName);

            return new VersionData(filePath, VersionListType.Updatable,
                versionList.ApplicableGameVersion, versionList.InternalResourceVersion,
                resourceInfos, assetInfos, fileSystems.Length, resourceGroups.Length,
                totalLength, totalCompressedLength);
        }

        private static AssetInfo[] BuildAssetInfos<TAsset>(TAsset[] assets, Dictionary<int, string> assetIndexToResourceFullName)
            where TAsset : struct
        {
            // Extract names and dependency indexes using interface-like access via dynamic dispatch
            string[] assetNames = new string[assets.Length];
            int[][] assetDependencyIndexes = new int[assets.Length][];

            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is PackageVersionList.Asset packageAsset)
                {
                    assetNames[i] = packageAsset.Name;
                    assetDependencyIndexes[i] = packageAsset.GetDependencyAssetIndexes();
                }
                else if (assets[i] is UpdatableVersionList.Asset updatableAsset)
                {
                    assetNames[i] = updatableAsset.Name;
                    assetDependencyIndexes[i] = updatableAsset.GetDependencyAssetIndexes();
                }
            }

            // Build reverse dependency lookup
            Dictionary<int, List<int>> reverseDeps = new Dictionary<int, List<int>>();
            for (int i = 0; i < assets.Length; i++)
            {
                int[] deps = assetDependencyIndexes[i];
                if (deps == null)
                {
                    continue;
                }

                for (int j = 0; j < deps.Length; j++)
                {
                    List<int> dependents;
                    if (!reverseDeps.TryGetValue(deps[j], out dependents))
                    {
                        dependents = new List<int>();
                        reverseDeps[deps[j]] = dependents;
                    }

                    dependents.Add(i);
                }
            }

            // Build AssetInfo array
            AssetInfo[] assetInfos = new AssetInfo[assets.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                string resourceFullName;
                assetIndexToResourceFullName.TryGetValue(i, out resourceFullName);

                // Resolve dependency names
                int[] depIndexes = assetDependencyIndexes[i];
                string[] depNames;
                if (depIndexes != null && depIndexes.Length > 0)
                {
                    depNames = new string[depIndexes.Length];
                    for (int j = 0; j < depIndexes.Length; j++)
                    {
                        depNames[j] = assetNames[depIndexes[j]];
                    }
                }
                else
                {
                    depNames = new string[0];
                }

                // Resolve reverse dependency names
                List<int> revDepIndexes;
                string[] revDepNames;
                if (reverseDeps.TryGetValue(i, out revDepIndexes))
                {
                    revDepNames = new string[revDepIndexes.Count];
                    for (int j = 0; j < revDepIndexes.Count; j++)
                    {
                        revDepNames[j] = assetNames[revDepIndexes[j]];
                    }
                }
                else
                {
                    revDepNames = new string[0];
                }

                assetInfos[i] = new AssetInfo(assetNames[i], resourceFullName, depNames, revDepNames);
            }

            return assetInfos;
        }

        private static Dictionary<int, string> BuildFileSystemLookup(PackageVersionList.FileSystem[] fileSystems)
        {
            Dictionary<int, string> lookup = new Dictionary<int, string>();
            for (int i = 0; i < fileSystems.Length; i++)
            {
                int[] resourceIndexes = fileSystems[i].GetResourceIndexes();
                for (int j = 0; j < resourceIndexes.Length; j++)
                {
                    lookup[resourceIndexes[j]] = fileSystems[i].Name;
                }
            }

            return lookup;
        }

        private static Dictionary<int, string> BuildFileSystemLookup(UpdatableVersionList.FileSystem[] fileSystems)
        {
            Dictionary<int, string> lookup = new Dictionary<int, string>();
            for (int i = 0; i < fileSystems.Length; i++)
            {
                int[] resourceIndexes = fileSystems[i].GetResourceIndexes();
                for (int j = 0; j < resourceIndexes.Length; j++)
                {
                    lookup[resourceIndexes[j]] = fileSystems[i].Name;
                }
            }

            return lookup;
        }

        private static Dictionary<int, List<string>> BuildResourceGroupLookup(PackageVersionList.ResourceGroup[] resourceGroups)
        {
            Dictionary<int, List<string>> lookup = new Dictionary<int, List<string>>();
            for (int i = 0; i < resourceGroups.Length; i++)
            {
                int[] resourceIndexes = resourceGroups[i].GetResourceIndexes();
                for (int j = 0; j < resourceIndexes.Length; j++)
                {
                    List<string> groups;
                    if (!lookup.TryGetValue(resourceIndexes[j], out groups))
                    {
                        groups = new List<string>();
                        lookup[resourceIndexes[j]] = groups;
                    }

                    groups.Add(resourceGroups[i].Name);
                }
            }

            return lookup;
        }

        private static Dictionary<int, List<string>> BuildResourceGroupLookup(UpdatableVersionList.ResourceGroup[] resourceGroups)
        {
            Dictionary<int, List<string>> lookup = new Dictionary<int, List<string>>();
            for (int i = 0; i < resourceGroups.Length; i++)
            {
                int[] resourceIndexes = resourceGroups[i].GetResourceIndexes();
                for (int j = 0; j < resourceIndexes.Length; j++)
                {
                    List<string> groups;
                    if (!lookup.TryGetValue(resourceIndexes[j], out groups))
                    {
                        groups = new List<string>();
                        lookup[resourceIndexes[j]] = groups;
                    }

                    groups.Add(resourceGroups[i].Name);
                }
            }

            return lookup;
        }
    }
}
