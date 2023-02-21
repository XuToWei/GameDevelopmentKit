using UnityEditor;
using UnityGameFramework.Editor;

namespace Game.Editor
{
    internal static class DefineSymbolTool
    {
#if UNITY_HOTFIX
        [MenuItem("Tools/Define Symbol/Remove UNITY_HOTFIX")]
        private static void Remove_UNITY_HOTFIX()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_HOTFIX");
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_HOTFIX")]
        private static void Add_UNITY_HOTFIX()
        {
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_HOTFIX");
        }
#endif
        
#if UNITY_ET
        [MenuItem("Tools/Define Symbol/Remove UNITY_ET")]
        private static void Remove_UNITY_ET()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET");
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_ET")]
        private static void Remove_UNITY_ET()
        {
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET");
        }
#endif
        
#if UNITY_UGFHOTFIX
        [MenuItem("Tools/Define Symbol/Remove UNITY_UGFHOTFIX")]
        private static void Remove_UNITY_UGFHOTFIX()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_UGFHOTFIX");
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_UGFHOTFIX")]
        private static void Remove_UNITY_UGFHOTFIX()
        {
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_UGFHOTFIX");
        }
#endif
    }
}
