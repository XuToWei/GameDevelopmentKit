using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace Game.Editor
{
    internal static class DefineSymbolTool
    {
        [MenuItem("Tools/Define Symbol/Refresh")]
        private static void Refresh()
        {
#if UNITY_HOTFIX
            HybridCLRTool.EnableHybridCLR();
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX");
#else
            HybridCLRTool.DisableHybridCLR();
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX");
#endif
#if UNITY_HOTFIX && UNITY_ET && (UNITY_ET_CODEMODE_CLIENT || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_CLIENT");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
#endif
#if UNITY_HOTFIX && UNITY_ET && (UNITY_ET_CODEMODE_SERVER || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_SERVER");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
#endif
#if !UNITY_HOTFIX && UNITY_ET && (UNITY_ET_CODEMODE_CLIENT || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_CLIENT");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
#endif
#if !UNITY_HOTFIX && UNITY_ET && (UNITY_ET_CODEMODE_SERVER || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_SERVER");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
#if UNITY_HOTFIX && UNITY_ET
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET");
#endif
#if !UNITY_HOTFIX && UNITY_ET
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET");
#endif
#if UNITY_ET
            LinkXMLHelper.AddLinkXML("UNITY_ET");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_ET");
#endif
#if UNITY_HOTFIX && UNITY_GAMEHOT
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_GAMEHOT");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_GAMEHOT");
#endif
#if !UNITY_HOTFIX && UNITY_GAMEHOT
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_GAMEHOT");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_GAMEHOT");
#endif
#if UNITY_GAMEHOT
            LinkXMLHelper.AddLinkXML("UNITY_GAMEHOT");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_GAMEHOT");
#endif
            HybridCLRTool.RefreshSettingsByLinkXML();
            AssetDatabase.SaveAssets();
            Debug.Log("Refresh!");
        }

#if UNITY_HOTFIX
        [MenuItem("Tools/Define Symbol/Remove UNITY_HOTFIX")]
        private static void Remove_UNITY_HOTFIX()
        {
            CompileAssemblyHelper.ClearBuildDir();
            HybridCLRTool.DisableHybridCLR();
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_GAMEHOT");
#if UNITY_ET && (UNITY_ET_CODEMODE_CLIENT || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_CLIENT");
#endif
#if UNITY_ET && (UNITY_ET_CODEMODE_SERVER || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
#if UNITY_ET
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET");
#endif
#if UNITY_GAMEHOT
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_GAMEHOT");
#endif
            HybridCLRTool.RefreshSettingsByLinkXML();
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_HOTFIX");
            AssetDatabase.SaveAssets();
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_HOTFIX")]
        private static void Add_UNITY_HOTFIX()
        {
            CompileAssemblyHelper.ClearBuildDir();
            HybridCLRTool.EnableHybridCLR();
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_GAMEHOT");
#if UNITY_ET && (UNITY_ET_CODEMODE_CLIENT || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_CLIENT");
#endif
#if UNITY_ET && (UNITY_ET_CODEMODE_SERVER || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_SERVER");
#endif
#if UNITY_ET
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET");
#endif
#if UNITY_GAMEHOT
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_GAMEHOT");
#endif
            HybridCLRTool.RefreshSettingsByLinkXML();
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_HOTFIX");
            AssetDatabase.SaveAssets();
        }
#endif

#if UNITY_ET
        [MenuItem("Tools/Define Symbol/Remove UNITY_ET")]
        private static void Remove_UNITY_ET()
        {
            CompileAssemblyHelper.ClearBuildDir();
            LinkXMLHelper.RemoveLinkXML("UNITY_ET");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");

            HybridCLRTool.RefreshSettingsByLinkXML();
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET");
            AssetDatabase.SaveAssets();
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_ET")]
        private static void Add_UNITY_ET()
        {
            CompileAssemblyHelper.ClearBuildDir();
            LinkXMLHelper.AddLinkXML("UNITY_ET");
#if UNITY_GAMEHOT
            Remove_UNITY_GAMEHOT();
#endif
#if UNITY_HOTFIX
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET");
#else
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET");
#endif
#if UNITY_HOTFIX && (UNITY_ET_CODEMODE_CLIENT || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_CLIENT");
#endif
#if UNITY_HOTFIX && (UNITY_ET_CODEMODE_SERVER || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_SERVER");
#endif
#if !UNITY_HOTFIX && (UNITY_ET_CODEMODE_CLIENT || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_CLIENT");
#endif
#if !UNITY_HOTFIX && (UNITY_ET_CODEMODE_SERVER || UNITY_ET_CODEMODE_CLIENTSERVER)
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
#if UNITY_HOTFIX
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET");
#else
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET");
#endif
            HybridCLRTool.RefreshSettingsByLinkXML();
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET");
            AssetDatabase.SaveAssets();
        }
#endif

#if UNITY_GAMEHOT
        [MenuItem("Tools/Define Symbol/Remove UNITY_GAMEHOT")]
        private static void Remove_UNITY_GAMEHOT()
        {
            CompileAssemblyHelper.ClearBuildDir();
            LinkXMLHelper.RemoveLinkXML("UNITY_GAMEHOT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_GAMEHOT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_GAMEHOT");

            HybridCLRTool.RefreshSettingsByLinkXML();
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_GAMEHOT");
            AssetDatabase.SaveAssets();
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_GAMEHOT")]
        private static void Add_UNITY_GAMEHOT()
        {
            CompileAssemblyHelper.ClearBuildDir();
            LinkXMLHelper.AddLinkXML("UNITY_GAMEHOT");
#if UNITY_ET
            Remove_UNITY_ET();
#endif
#if UNITY_HOTFIX
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_GAMEHOTT");
#else
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_GAMEHOT");
#endif
            HybridCLRTool.RefreshSettingsByLinkXML();
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_GAMEHOT");
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
