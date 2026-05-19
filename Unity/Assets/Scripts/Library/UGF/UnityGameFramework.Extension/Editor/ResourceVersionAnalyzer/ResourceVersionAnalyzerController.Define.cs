using GameFramework;

namespace UnityGameFramework.Extension.Editor
{
    public sealed partial class ResourceVersionAnalyzerController
    {
        public enum VersionListType : byte
        {
            Package,
            Updatable,
        }

        public sealed class ResourceInfo
        {
            private readonly string m_Name;
            private readonly string m_Variant;
            private readonly string m_Extension;
            private readonly string m_FullName;
            private readonly byte m_LoadType;
            private readonly int m_Length;
            private readonly int m_HashCode;
            private readonly int m_CompressedLength;
            private readonly int m_CompressedHashCode;
            private readonly int m_AssetCount;
            private readonly string m_FileSystem;
            private readonly string[] m_ResourceGroups;

            public string Name => m_Name;

            public string Variant => m_Variant;

            public string Extension => m_Extension;

            public string FullName => m_FullName;

            public byte LoadType => m_LoadType;

            public int Length => m_Length;

            public int HashCode => m_HashCode;

            public int CompressedLength => m_CompressedLength;

            public int CompressedHashCode => m_CompressedHashCode;

            public int AssetCount => m_AssetCount;

            public string FileSystem => m_FileSystem;

            public string[] ResourceGroups => m_ResourceGroups;

            public ResourceInfo(string name, string variant, string extension, byte loadType,
                int length, int hashCode, int compressedLength, int compressedHashCode,
                int assetCount, string fileSystem, string[] resourceGroups)
            {
                m_Name = name;
                m_Variant = variant;
                m_Extension = extension ?? DefaultExtension;
                m_FullName = variant != null ? Utility.Text.Format("{0}.{1}", name, variant) : name;
                m_LoadType = loadType;
                m_Length = length;
                m_HashCode = hashCode;
                m_CompressedLength = compressedLength;
                m_CompressedHashCode = compressedHashCode;
                m_AssetCount = assetCount;
                m_FileSystem = fileSystem;
                m_ResourceGroups = resourceGroups;
            }
        }

        public sealed class AssetInfo
        {
            private readonly string m_Name;
            private readonly string m_ResourceFullName;
            private readonly string[] m_DependencyAssetNames;
            private readonly string[] m_DependentAssetNames;

            public string Name => m_Name;

            public string ResourceFullName => m_ResourceFullName;

            public string[] DependencyAssetNames => m_DependencyAssetNames;

            public string[] DependentAssetNames => m_DependentAssetNames;

            public int DependencyCount => m_DependencyAssetNames.Length;

            public int DependentCount => m_DependentAssetNames.Length;

            public AssetInfo(string name, string resourceFullName, string[] dependencyAssetNames, string[] dependentAssetNames)
            {
                m_Name = name;
                m_ResourceFullName = resourceFullName;
                m_DependencyAssetNames = dependencyAssetNames;
                m_DependentAssetNames = dependentAssetNames;
            }
        }

        public sealed class VersionData
        {
            private readonly string m_FilePath;
            private readonly VersionListType m_VersionListType;
            private readonly string m_ApplicableGameVersion;
            private readonly int m_InternalResourceVersion;
            private readonly ResourceInfo[] m_Resources;
            private readonly AssetInfo[] m_AssetInfos;
            private readonly int m_FileSystemCount;
            private readonly int m_ResourceGroupCount;
            private readonly long m_TotalLength;
            private readonly long m_TotalCompressedLength;

            public string FilePath => m_FilePath;

            public VersionListType VersionListType => m_VersionListType;

            public string ApplicableGameVersion => m_ApplicableGameVersion;

            public int InternalResourceVersion => m_InternalResourceVersion;

            public ResourceInfo[] Resources => m_Resources;

            public int ResourceCount => m_Resources.Length;

            public AssetInfo[] AssetInfos => m_AssetInfos;

            public int AssetCount => m_AssetInfos.Length;

            public int FileSystemCount => m_FileSystemCount;

            public int ResourceGroupCount => m_ResourceGroupCount;

            public long TotalLength => m_TotalLength;

            public long TotalCompressedLength => m_TotalCompressedLength;

            public VersionData(string filePath, VersionListType versionListType,
                string applicableGameVersion, int internalResourceVersion,
                ResourceInfo[] resources, AssetInfo[] assetInfos, int fileSystemCount,
                int resourceGroupCount, long totalLength, long totalCompressedLength)
            {
                m_FilePath = filePath;
                m_VersionListType = versionListType;
                m_ApplicableGameVersion = applicableGameVersion;
                m_InternalResourceVersion = internalResourceVersion;
                m_Resources = resources;
                m_AssetInfos = assetInfos;
                m_FileSystemCount = fileSystemCount;
                m_ResourceGroupCount = resourceGroupCount;
                m_TotalLength = totalLength;
                m_TotalCompressedLength = totalCompressedLength;
            }
        }

        public sealed class DistributionInfo
        {
            private readonly string m_Name;
            private readonly int m_Count;
            private readonly long m_TotalLength;
            private readonly long m_TotalCompressedLength;

            public string Name => m_Name;

            public int Count => m_Count;

            public long TotalLength => m_TotalLength;

            public long TotalCompressedLength => m_TotalCompressedLength;

            public DistributionInfo(string name, int count, long totalLength, long totalCompressedLength)
            {
                m_Name = name;
                m_Count = count;
                m_TotalLength = totalLength;
                m_TotalCompressedLength = totalCompressedLength;
            }
        }

        public enum CompareStatus : byte
        {
            Added,
            Removed,
            Modified,
            Unchanged,
        }

        public sealed class CompareItem
        {
            private readonly CompareStatus m_Status;
            private readonly ResourceInfo m_ResourceA;
            private readonly ResourceInfo m_ResourceB;
            private readonly long m_SizeDifference;

            public CompareStatus Status => m_Status;

            public ResourceInfo ResourceA => m_ResourceA;

            public ResourceInfo ResourceB => m_ResourceB;

            public long SizeDifference => m_SizeDifference;

            public string FullName => m_ResourceA != null ? m_ResourceA.FullName : m_ResourceB.FullName;

            public CompareItem(CompareStatus status, ResourceInfo resourceA, ResourceInfo resourceB)
            {
                m_Status = status;
                m_ResourceA = resourceA;
                m_ResourceB = resourceB;

                int lengthA = resourceA != null ? resourceA.Length : 0;
                int lengthB = resourceB != null ? resourceB.Length : 0;
                m_SizeDifference = (long)lengthB - lengthA;
            }
        }

        public sealed class CompareResult
        {
            private readonly CompareItem[] m_Items;
            private readonly int m_AddedCount;
            private readonly int m_RemovedCount;
            private readonly int m_ModifiedCount;
            private readonly int m_UnchangedCount;
            private readonly long m_TotalSizeDifference;

            public CompareItem[] Items => m_Items;

            public int AddedCount => m_AddedCount;

            public int RemovedCount => m_RemovedCount;

            public int ModifiedCount => m_ModifiedCount;

            public int UnchangedCount => m_UnchangedCount;

            public long TotalSizeDifference => m_TotalSizeDifference;

            public CompareResult(CompareItem[] items)
            {
                m_Items = items;
                m_AddedCount = 0;
                m_RemovedCount = 0;
                m_ModifiedCount = 0;
                m_UnchangedCount = 0;
                m_TotalSizeDifference = 0;

                for (int i = 0; i < items.Length; i++)
                {
                    switch (items[i].Status)
                    {
                        case CompareStatus.Added:
                            m_AddedCount++;
                            break;
                        case CompareStatus.Removed:
                            m_RemovedCount++;
                            break;
                        case CompareStatus.Modified:
                            m_ModifiedCount++;
                            break;
                        case CompareStatus.Unchanged:
                            m_UnchangedCount++;
                            break;
                    }

                    m_TotalSizeDifference += items[i].SizeDifference;
                }
            }
        }
    }
}
