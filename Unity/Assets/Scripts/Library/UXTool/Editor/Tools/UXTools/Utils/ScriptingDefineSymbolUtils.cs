using UnityEditor;
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
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            if (!defineSymbols.Contains(defineSymbol))
            {
                defineSymbols += ";" + defineSymbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.PS4, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.PS5, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.GameCoreXboxOne, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.GameCoreXboxSeries, defineSymbols);
            }
        }

        private static void DisableDefineSymbol(string defineSymbol)
        {
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            if (defineSymbols.Contains(defineSymbol))
            {
                defineSymbols = defineSymbols.Replace(defineSymbol + ";", "");
                defineSymbols = defineSymbols.Replace(defineSymbol, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.PS4, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.PS5, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.GameCoreXboxOne, defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.GameCoreXboxSeries, defineSymbols);
            }
        }
    }
}