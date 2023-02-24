using System.IO;
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
            RemoveLinkXML("UNITY_HOTFIX");
            RemoveLinkXML("UNITY_ET");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_HOTFIX");
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_HOTFIX")]
        private static void Add_UNITY_HOTFIX()
        {
            AddLinkXML("UNITY_HOTFIX");
#if UNITY_ET
            AddLinkXML("UNITY_ET");
#endif
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_HOTFIX");
        }
#endif
        
#if UNITY_ET
        [MenuItem("Tools/Define Symbol/Remove UNITY_ET")]
        private static void Remove_UNITY_ET()
        {
#if UNITY_HOTFIX
            RemoveLinkXML("UNITY_ET");
#endif
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET");
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_ET")]
        private static void Add_UNITY_ET()
        {
#if UNITY_HOTFIX
            AddLinkXML("UNITY_ET");
#endif
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET");
        }
#endif
        
#if UNITY_GAMEHOT
        [MenuItem("Tools/Define Symbol/Remove UNITY_GAMEHOT")]
        private static void Remove_UNITY_UGFHOTFIX()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_GAMEHOT");
        }
#else
        [MenuItem("Tools/Define Symbol/Add UNITY_GAMEHOT")]
        private static void Add_UNITY_UGFHOTFIX()
        {
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_GAMEHOT");
        }
#endif
        
        /// <summary>
        /// 开启link.xml，防止代码裁剪，导致热更缺少接口
        /// </summary>
        /// <param name="scriptingDefineSymbol"></param>
        private static void AddLinkXML(string scriptingDefineSymbol)
        {
            string content = File.ReadAllText("Assets/Link.xml");
            if (content.Contains($"<!--{scriptingDefineSymbol}_FIRST-->") && content.Contains($"<!--{scriptingDefineSymbol}_END-->"))
                return;
            content = content.Replace($"<!--{scriptingDefineSymbol}", $"<!--{scriptingDefineSymbol}_FIRST-->");
            content = content.Replace($"{scriptingDefineSymbol}-->", $"<!--{scriptingDefineSymbol}_END-->");
            File.WriteAllText("Assets/Link.xml", content);
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// 关闭link.xml，减少包体
        /// </summary>
        /// <param name="scriptingDefineSymbol"></param>
        private static void RemoveLinkXML(string scriptingDefineSymbol)
        {
            string content = File.ReadAllText("Assets/Link.xml");
            if (!content.Contains($"<!--{scriptingDefineSymbol}_FIRST-->") && !content.Contains($"<!--{scriptingDefineSymbol}_END-->"))
                return;
            content = content.Replace($"<!--{scriptingDefineSymbol}_FIRST-->", $"<!--{scriptingDefineSymbol}");
            content = content.Replace($"<!--{scriptingDefineSymbol}_END-->", $"{scriptingDefineSymbol}-->");
            File.WriteAllText("Assets/Link.xml", content);
            AssetDatabase.Refresh();
        }
    }
}
