#if UNITY_HOTFIX
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Extension.Editor;

namespace ET.Editor
{
    public static class PreprocessCompileDll
    {
        [UGFBuildOnPreprocessAllPlatforms(1)]
        public static void CompileDll(Platform platform)
        {
            BuildAssemblyTool.BuildModel(BuildAssemblyTool.DefaultCodeOptimization, Define.CodeMode);
            BuildAssemblyTool.BuildHotfix(BuildAssemblyTool.DefaultCodeOptimization, Define.CodeMode);
        }
    }
}
#endif
