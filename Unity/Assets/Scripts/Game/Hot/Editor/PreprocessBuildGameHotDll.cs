#if UNITY_HOTFIX
using UnityGameFramework.Extension.Editor;

namespace Game.Hot.Editor
{
    public static class PreprocessBuildGameHotDll
    {
        [UGFPreprocessBuildEvent(1)]
        public static void BuildDlls()
        {
            BuildGameHotDllTool.Build();
        }
    }
}
#endif