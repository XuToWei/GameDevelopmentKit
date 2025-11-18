using System.Runtime.InteropServices;

namespace UnityGameFramework.Extension.Editor
{
    public sealed partial class ResourceOptimize
    {
        private class ABInfo {

            private readonly string m_Name;
            private readonly long m_Size;
            private readonly int m_ReferenceCount;

            public string Name => m_Name;
            public long Size => m_Size;
            public int ReferenceCount => m_ReferenceCount;
  
            public ABInfo(string name, long size, int referenceCount)
            {
                this.m_Name = name;
                this.m_Size = size;
                this.m_ReferenceCount = referenceCount;
            }
        }
        
        [StructLayout(LayoutKind.Auto)]
        private struct Stamp
        {
            private readonly string m_HostAssetName;
            private readonly string m_DependencyAssetName;

            public Stamp(string hostAssetName, string dependencyAssetName)
            {
                m_HostAssetName = hostAssetName;
                m_DependencyAssetName = dependencyAssetName;
            }

            public string HostAssetName => m_HostAssetName;
            public string DependencyAssetName => m_DependencyAssetName;
        }
    }
}
