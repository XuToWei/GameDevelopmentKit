using UnityEditor;
using UnityEditor.Build;

namespace ThunderFireUITool
{
    public class ScriptingDefineSymbolUtils
    {
#if UXTOOLS_DEV
        [MenuItem("Tools/Enable My Define Symbol")]
#endif
        public static void EnableInputSystemDefineSymbol()
        {
            EnableDefineSymbol("USE_InputSystem");
        }

#if UXTOOLS_DEV
        [MenuItem("Tools/Disable My Define Symbol")]
#endif
        public static void DisableInputSystemDefineSymbol()
        {
            DisableDefineSymbol("USE_InputSystem");
        }

        private static void EnableDefineSymbol(string defineSymbol)
        {
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone);
            if (!defineSymbols.Contains(defineSymbol))
            {
                defineSymbols += ";" + defineSymbol;
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.iOS, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.PS4, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.PS5, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.GameCoreXboxOne), defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.GameCoreXboxSeries), defineSymbols);
            }
        }

        private static void DisableDefineSymbol(string defineSymbol)
        {
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.Standalone));
            if (defineSymbols.Contains(defineSymbol))
            {
                defineSymbols = defineSymbols.Replace(defineSymbol + ";", "");
                defineSymbols = defineSymbols.Replace(defineSymbol, "");
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.iOS, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.PS4, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.PS5, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.GameCoreXboxOne), defineSymbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.GameCoreXboxSeries), defineSymbols);
            }
        }
    }
}