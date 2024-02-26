using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UnityGameFramework.Extension
{
    public class AssetCollectionPreprocessBuildHandle : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            AssetCollectionUtility.RefreshAssetCollection();
        }
    }
}