#if UNITY_HOTFIX
using Game.Editor;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Extension.Editor;

namespace ET.Editor
{
    public static class PreprocessCompileDll
    {
        [UGFBuildOnPreprocessPlatform(1)]
        public static void CompileDll(Platform platform)
        {
            BuildAssemblyTool.Build(BuildAssemblyHelper.GetBuildTarget(platform));
        }
    }
}
#endif
