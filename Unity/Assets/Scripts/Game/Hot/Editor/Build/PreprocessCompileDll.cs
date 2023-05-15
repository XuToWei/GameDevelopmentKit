#if UNITY_HOTFIX
using UnityGameFramework.Extension.Editor;

namespace Game.Hot.Editor
{
    public static class PreprocessCompileDll
    {
        [UGFPreprocessBuildEvent(1)]
        public static void CompileDll()
        {
            BuildAssemblyTool.Build();
        }
    }
}
#endif