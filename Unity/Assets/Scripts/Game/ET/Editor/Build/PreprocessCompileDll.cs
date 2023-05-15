#if UNITY_HOTFIX
using UnityGameFramework.Extension.Editor;

namespace ET.Editor
{
    public static class PreprocessCompileDll
    {
        [UGFPreprocessBuildEvent(1)]
        public static void CompileDll()
        {
            BuildAssemblyTool.BuildModel(BuildAssemblyTool.CodeOptimization, Define.CodeMode);
            BuildAssemblyTool.BuildHotfix(BuildAssemblyTool.CodeOptimization, Define.CodeMode);
        }
    }
}
#endif
