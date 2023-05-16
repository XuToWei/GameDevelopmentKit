using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace Game.Editor
{
    internal static class DefineSymbolTool
    {
        public static readonly string LinkXML = "Assets/link.xml";

        [MenuItem("Tools/Define Symbol/Refresh")]
        private static void Refresh()
        {
#if UNITY_HOTFIX
            HybridCLRTool.EnableHybridCLR();
            AddLinkXML("UNITY_HOTFIX");
            RemoveLinkXML("UNITY_!HOTFIX");
#else
            DisableHybridCLR();
            AddLinkXML("UNITY_!HOTFIX");
            RemoveLinkXML("UNITY_HOTFIX");
#endif
            HybridCLRTool.Save();
            Debug.Log("Refresh!");
        }
        
#if UNITY_HOTFIX
        [MenuItem("Tools/Define Symbol/Remove UNITY_HOTFIX")]
        private static void Remove_UNITY_HOTFIX()
        {
            BuildAssemblyHelper.ClearBuildOutputDir();
            HybridCLRTool.DisableHybridCLR();
            AddLinkXML("UNITY_!HOTFIX");
            RemoveLinkXML("UNITY_HOTFIX");
#if UNITY_ET
            AddLinkXML("UNITY_!HOTFIX_ET");
            RemoveLinkXML("UNITY_HOTFIX_ET");
#else
            RemoveLinkXML("UNITY_HOTFIX_ET");
            RemoveLinkXML("UNITY_!HOTFIX_ET");
#endif
#if UNITY_GAMEHOT
            AddLinkXML("UNITY_!HOTFIX_GAMEHOT");
            RemoveLinkXML("UNITY_HOTFIX_GAMEHOT");
#else
            RemoveLinkXML("UNITY_HOTFIX_GAMEHOT");
            RemoveLinkXML("UNITY_!HOTFIX_GAMEHOT");
#endif
            HybridCLRTool.Save();
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_HOTFIX");
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_HOTFIX")]
        private static void Add_UNITY_HOTFIX()
        {
            BuildAssemblyHelper.ClearBuildOutputDir();
            HybridCLRTool.EnableHybridCLR();
            AddLinkXML("UNITY_HOTFIX");
            RemoveLinkXML("UNITY_!HOTFIX");
#if UNITY_ET
            AddLinkXML("UNITY_HOTFIX_ET");
            RemoveLinkXML("UNITY_!HOTFIX_ET");
#else
            RemoveLinkXML("UNITY_HOTFIX_ET");
            RemoveLinkXML("UNITY_!HOTFIX_ET");
#endif
#if UNITY_GAMEHOT
            AddLinkXML("UNITY_HOTFIX_GAMEHOT");
            RemoveLinkXML("UNITY_!HOTFIX_GAMEHOT");
#else
            RemoveLinkXML("UNITY_HOTFIX_GAMEHOT");
            RemoveLinkXML("UNITY_!HOTFIX_GAMEHOT");
#endif
            HybridCLRTool.Save();
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_HOTFIX");
        }
#endif
        
#if UNITY_ET
        [MenuItem("Tools/Define Symbol/Remove UNITY_ET")]
        private static void Remove_UNITY_ET()
        {
            BuildAssemblyHelper.ClearBuildOutputDir();
            RemoveLinkXML("UNITY_HOTFIX_ET");
            RemoveLinkXML("UNITY_!HOTFIX_ET");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET");
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_ET")]
        private static void Add_UNITY_ET()
        {
            BuildAssemblyHelper.ClearBuildOutputDir();
#if UNITY_GAMEHOT
            Remove_UNITY_GAMEHOT();
#endif
#if UNITY_HOTFIX
            AddLinkXML("UNITY_HOTFIX_ET");
            RemoveLinkXML("UNITY_!HOTFIX_ET");
#else
            AddLinkXML("UNITY_!HOTFIX_ET");
            RemoveLinkXML("UNITY_HOTFIX_ET");
#endif
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET");
        }
#endif
        
#if UNITY_GAMEHOT
        [MenuItem("Tools/Define Symbol/Remove UNITY_GAMEHOT")]
        private static void Remove_UNITY_GAMEHOT()
        {
            BuildAssemblyHelper.ClearBuildOutputDir();
            RemoveLinkXML("UNITY_HOTFIX_GAMEHOT");
            RemoveLinkXML("UNITY_!HOTFIX_GAMEHOT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_GAMEHOT");
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_GAMEHOT")]
        private static void Add_UNITY_GAMEHOT()
        {
#if UNITY_ET
            Remove_UNITY_ET();
#endif
#if UNITY_HOTFIX
            AddLinkXML("UNITY_HOTFIX_GAMEHOTT");
            RemoveLinkXML("UNITY_!HOTFIX_GAMEHOT");
#else
            AddLinkXML("UNITY_!HOTFIX_GAMEHOT");
            RemoveLinkXML("UNITY_HOTFIX_GAMEHOT");
#endif
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_GAMEHOT");
        }
#endif
        
        /// <summary>
        /// 开启link.xml，防止代码裁剪，导致热更缺少接口
        /// </summary>
        /// <param name="scriptingDefineSymbol"></param>
        private static void AddLinkXML(string scriptingDefineSymbol)
        {
            string content = File.ReadAllText(LinkXML);
            if (content.Contains($"<!--{scriptingDefineSymbol}_FIRST-->") && content.Contains($"<!--{scriptingDefineSymbol}_END-->"))
                return;
            content = content.Replace($"<!--{scriptingDefineSymbol}", $"<!--{scriptingDefineSymbol}_FIRST-->");
            content = content.Replace($"{scriptingDefineSymbol}-->", $"<!--{scriptingDefineSymbol}_END-->");
            File.WriteAllText(LinkXML, content);
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// 关闭link.xml，减少包体
        /// </summary>
        /// <param name="scriptingDefineSymbol"></param>
        private static void RemoveLinkXML(string scriptingDefineSymbol)
        {
            string content = File.ReadAllText(LinkXML);
            if (!content.Contains($"<!--{scriptingDefineSymbol}_FIRST-->") && !content.Contains($"<!--{scriptingDefineSymbol}_END-->"))
                return;
            content = content.Replace($"<!--{scriptingDefineSymbol}_FIRST-->", $"<!--{scriptingDefineSymbol}");
            content = content.Replace($"<!--{scriptingDefineSymbol}_END-->", $"{scriptingDefineSymbol}-->");
            File.WriteAllText(LinkXML, content);
            AssetDatabase.Refresh();
        }
    }
}
