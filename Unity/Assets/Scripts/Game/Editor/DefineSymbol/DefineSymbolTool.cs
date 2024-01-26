using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace Game.Editor
{
    internal static class DefineSymbolTool
    {
        [MenuItem("Game/Define Symbol/Refresh", false, 1)]
        public static void Refresh()
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
            Set_Luban_GenConfig("ET", true);
            ResourceRuleTool.ActivateRule_ET();
            LinkXMLHelper.AddLinkXML("UNITY_ET");
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.ET.Code.Model");
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.ET.Code.ModelView");
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.ET.Code.Hotfix");
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.ET.Code.HotfixView");
#else
            Set_Luban_GenConfig("ET", false);
            LinkXMLHelper.RemoveLinkXML("UNITY_ET");
            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.ET.Code.Model");
            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.ET.Code.ModelView");
            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.ET.Code.Hotfix");
            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.ET.Code.HotfixView");
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
            Set_Luban_GenConfig("GameHot", true);
            ResourceRuleTool.ActivateRule_GameHot();
            LinkXMLHelper.AddLinkXML("UNITY_GAMEHOT");
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.Hot.Code");
#else
            Set_Luban_GenConfig("GameHot", false);
            LinkXMLHelper.RemoveLinkXML("UNITY_GAMEHOT");
            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.Hot.Code");
#endif
            HybridCLRTool.RefreshSettings();
            AssetDatabase.SaveAssets();
            Debug.Log("Refresh!");
        }

#if UNITY_HOTFIX
        [MenuItem("Game/Define Symbol/Remove UNITY_HOTFIX", false, 2)]
        private static void Remove_UNITY_HOTFIX()
        {
            BuildAssemblyHelper.ClearBuildDir();
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
            HybridCLRTool.RefreshSettings();
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_HOTFIX");
            AssetDatabase.SaveAssets();
        }
#else
        [MenuItem("Game/Define Symbol/Add UNITY_HOTFIX", false, 2)]
        private static void Add_UNITY_HOTFIX()
        {
            BuildAssemblyHelper.ClearBuildDir();
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
            HybridCLRTool.RefreshSettings();
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_HOTFIX");
            AssetDatabase.SaveAssets();
        }
#endif

#if UNITY_ET
        [MenuItem("Game/Define Symbol/Remove UNITY_ET", false, 3)]
        private static void Remove_UNITY_ET()
        {
            Set_Luban_GenConfig("ET", false);
            BuildAssemblyHelper.ClearBuildDir();
            LinkXMLHelper.RemoveLinkXML("UNITY_ET");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");

            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.ET.Code.Model");
            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.ET.Code.ModelView");
            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.ET.Code.Hotfix");
            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.ET.Code.HotfixView");

            HybridCLRTool.RefreshSettings();
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET");
            AssetDatabase.SaveAssets();
        }
#else
        [MenuItem("Game/Define Symbol/Add UNITY_ET", false, 3)]
        private static void Add_UNITY_ET()
        {
            Set_Luban_GenConfig("ET", true);
            BuildAssemblyHelper.ClearBuildDir();
            ResourceRuleTool.ActivateRule_ET();
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
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.ET.Code.Model");
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.ET.Code.ModelView");
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.ET.Code.Hotfix");
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.ET.Code.HotfixView");

            HybridCLRTool.RefreshSettings();
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET");
            AssetDatabase.SaveAssets();
        }
#endif

#if UNITY_GAMEHOT
        [MenuItem("Game/Define Symbol/Remove UNITY_GAMEHOT", false, 4)]
        private static void Remove_UNITY_GAMEHOT()
        {
            Set_Luban_GenConfig("GameHot", false);
            BuildAssemblyHelper.ClearBuildDir();
            LinkXMLHelper.RemoveLinkXML("UNITY_GAMEHOT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_GAMEHOT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_GAMEHOT");

            HybridCLRTool.RemoveHotfixAssemblyDefinition("Game.Hot.Code");

            HybridCLRTool.RefreshSettings();
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_GAMEHOT");
            AssetDatabase.SaveAssets();
        }
#else
        [MenuItem("Game/Define Symbol/Add UNITY_GAMEHOT", false, 4)]
        private static void Add_UNITY_GAMEHOT()
        {
            Set_Luban_GenConfig("GameHot", true);
            BuildAssemblyHelper.ClearBuildDir();
            ResourceRuleTool.ActivateRule_GameHot();
            LinkXMLHelper.AddLinkXML("UNITY_GAMEHOT");
#if UNITY_ET
            Remove_UNITY_ET();
#endif
#if UNITY_HOTFIX
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_GAMEHOT");
#else
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_GAMEHOT");
#endif
            HybridCLRTool.AddHotfixAssemblyDefinition("Game.Hot.Code");

            HybridCLRTool.RefreshSettings();
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_GAMEHOT");
            AssetDatabase.SaveAssets();
        }
#endif

        private static void Set_Luban_GenConfig(string dirName, bool isOpen)
        {
            string[] dirs = Directory.GetDirectories("../Design/Excel");
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = Path.GetFullPath(dirs[i]);
                string genConfigFile = Path.Combine(dir, "luban.conf");
                if (!File.Exists(genConfigFile))
                {
                    continue;
                }
                if (string.Equals(Directory.GetParent(genConfigFile)?.Name, dirName, StringComparison.Ordinal))
                {
                    string json = File.ReadAllText(genConfigFile);
                    if (isOpen)
                    {
                        json = json.Replace(@"""active"":false", @"""active"":true");
                    }
                    else
                    {
                        json = json.Replace(@"""active"":true", @"""active"":false");
                    }
                    File.WriteAllText(genConfigFile, json);
                    break;
                }
            }
        }
    }
}
