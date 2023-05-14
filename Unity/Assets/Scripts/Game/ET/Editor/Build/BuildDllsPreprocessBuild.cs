#if UNITY_HOTFIX
using UnityEditor.Compilation;
using UnityGameFramework.Extension.Editor;

namespace ET.Editor
{
    public static class BuildDllsPreprocessBuild
    {
        [UGFPreprocessBuildEvent(1)]
        public static void BuildDlls()
        {
            BuildAssemblyHelper.BuildModel(CompilationPipeline.codeOptimization, Define.CodeMode);
            BuildAssemblyHelper.BuildHotfix(CompilationPipeline.codeOptimization, Define.CodeMode);
        }
    }
}
#endif
