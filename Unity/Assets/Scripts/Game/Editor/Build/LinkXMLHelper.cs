using System.IO;
using UnityEditor;

namespace Game.Editor
{
    public static class LinkXMLHelper
    {
        public static readonly string LinkXMLPath = "Assets/link.xml";
        
        /// <summary>
        /// 开启link.xml，防止代码裁剪，导致热更缺少接口
        /// </summary>
        /// <param name="scriptingDefineSymbol"></param>
        public static void AddLinkXML(string scriptingDefineSymbol)
        {
            string content = File.ReadAllText(LinkXMLPath);
            if (content.Contains($"<!--{scriptingDefineSymbol}_START-->") && content.Contains($"<!--{scriptingDefineSymbol}_END-->"))
                return;
            content = content.Replace($"<!--{scriptingDefineSymbol}-", $"<!--{scriptingDefineSymbol}_START-->");
            content = content.Replace($"-{scriptingDefineSymbol}-->", $"<!--{scriptingDefineSymbol}_END-->");
            File.WriteAllText(LinkXMLPath, content);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        
        /// <summary>
        /// 关闭link.xml，减少包体
        /// </summary>
        /// <param name="scriptingDefineSymbol"></param>
        public static void RemoveLinkXML(string scriptingDefineSymbol)
        {
            string content = File.ReadAllText(LinkXMLPath);
            if (!content.Contains($"<!--{scriptingDefineSymbol}_START-->") && !content.Contains($"<!--{scriptingDefineSymbol}_END-->"))
                return;
            content = content.Replace($"<!--{scriptingDefineSymbol}_START-->", $"<!--{scriptingDefineSymbol}-");
            content = content.Replace($"<!--{scriptingDefineSymbol}_END-->", $"-{scriptingDefineSymbol}-->");
            File.WriteAllText(LinkXMLPath, content);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
