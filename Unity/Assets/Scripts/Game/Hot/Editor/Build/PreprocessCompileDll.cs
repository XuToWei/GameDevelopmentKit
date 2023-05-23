#if UNITY_HOTFIX
using UnityGameFramework.Extension.Editor;

namespace Game.Hot.Editor
{
    public static class PreprocessCompileDll
    {
        [UGFBuildOnPreprocessAllPlatforms(1)]
        public static void CompileDll()
        {
            BuildAssemblyTool.Build();
        }
    }
}
#endif