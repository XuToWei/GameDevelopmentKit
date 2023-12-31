using System.IO;
using Game.Editor;
using UnityEditor;
using UnityGameFramework.Editor;
using UnityGameFramework.Extension.Editor;

namespace ET.Editor
{
    internal static class CodeModeDefineSymbolTool
    {
#if UNITY_ET_CODEMODE_CLIENT
        [MenuItem("ET/Define Symbol/Refresh", false, -1)]
        public static void Refresh()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            EnableETClientCode();
            EnableETClientViewCode();
            DisableETServerCode();
            EnableModelGenerateClientCode();
            RefreshETResourceRule(true);
#if UNITY_HOTFIX
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }
#elif UNITY_ET_CODEMODE_SERVER
        [MenuItem("ET/Define Symbol/Refresh", false, -1)]
        public static void Refresh()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            EnableETClientCode();
            DisableETClientViewCode();
            EnableETServerCode();
            EnableModelGenerateClientServerCode();
            RefreshETResourceRule(false);
#if UNITY_HOTFIX
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }
#elif UNITY_ET_CODEMODE_CLIENTSERVER
        [MenuItem("ET/Define Symbol/Refresh", false, -1)]
        public static void Refresh()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            EnableETClientCode();
            EnableETClientViewCode();
            EnableETServerCode();
            EnableModelGenerateClientServerCode();
            RefreshETResourceRule(false);
#if UNITY_HOTFIX
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }
#else
        [InitializeOnLoadMethod]
        static void UNITY_ET_CODEMODE_CLIENT()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            EnableETClientCode();
            DisableETServerCode();
            EnableModelGenerateClientCode();
            RefreshETResourceRule(true);
#if UNITY_HOTFIX
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }
#endif

#if UNITY_ET_CODEMODE_SERVER || UNITY_ET_CODEMODE_CLIENTSERVER
#if UNITY_ET_CODEMODE_SERVER
        [MenuItem("ET/Define Symbol/CodeMode(Server)/Enable UNITY_ET_CODEMODE_CLIENT")]
#endif
#if UNITY_ET_CODEMODE_CLIENTSERVER
        [MenuItem("ET/Define Symbol/CodeMode(ClientServer)/Enable UNITY_ET_CODEMODE_CLIENT")]
#endif
        public static void UNITY_ET_CODEMODE_CLIENT()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            EnableETClientCode();
            EnableETClientViewCode();
            DisableETServerCode();
            EnableModelGenerateClientCode();
            RefreshETResourceRule(true);
#if UNITY_HOTFIX
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }
#endif

#if UNITY_ET_CODEMODE_CLIENT || UNITY_ET_CODEMODE_CLIENTSERVER
#if UNITY_ET_CODEMODE_CLIENT
        [MenuItem("ET/Define Symbol/CodeMode(Client)/Enable UNITY_ET_CODEMODE_SERVER")]
#endif
#if UNITY_ET_CODEMODE_CLIENTSERVER
        [MenuItem("ET/Define Symbol/CodeMode(ClientServer)/Enable UNITY_ET_CODEMODE_SERVER")]
#endif
        public static void UNITY_ET_CODEMODE_SERVER()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            EnableETClientCode();
            DisableETClientViewCode();
            EnableETServerCode();
            EnableModelGenerateClientServerCode();
            RefreshETResourceRule(false);
#if UNITY_HOTFIX
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }
#endif

#if UNITY_ET_CODEMODE_CLIENT || UNITY_ET_CODEMODE_SERVER
#if UNITY_ET_CODEMODE_CLIENT
        [MenuItem("ET/Define Symbol/CodeMode(Client)/Enable UNITY_ET_CODEMODE_CLIENTSERVER")]
#endif
#if UNITY_ET_CODEMODE_SERVER
        [MenuItem("ET/Define Symbol/CodeMode(Server)/Enable UNITY_ET_CODEMODE_CLIENTSERVER")]
#endif
        public static void UNITY_ET_CODEMODE_CLIENTSERVER()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            EnableETClientCode();
            EnableETClientViewCode();
            EnableETServerCode();
            EnableModelGenerateClientServerCode();
            RefreshETResourceRule(false);
#if UNITY_HOTFIX
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.AddLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_!HOTFIX_ET_SERVER");
#else
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_CLIENT");
            LinkXMLHelper.RemoveLinkXML("UNITY_HOTFIX_ET_SERVER");
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_CLIENT");
            LinkXMLHelper.AddLinkXML("UNITY_!HOTFIX_ET_SERVER");
#endif
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
        }
#endif

        static void RenameFile(string sourceFile, string destFile)
        {
            if (File.Exists(sourceFile))
            {
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                    File.Delete($"{destFile}.meta");
                }
                File.Move(sourceFile, destFile);
                File.Delete(sourceFile);
                File.Delete($"{sourceFile}.meta");
            }
        }
        
        static void EnableETClientCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Hotfix/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefFile, asmdefDisableFile);
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefFile, asmdefDisableFile);
        }
        
        static void DisableETClientCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Hotfix/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefDisableFile, asmdefFile);
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefDisableFile, asmdefFile);
        }
        static void EnableETClientViewCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/HotfixView/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefFile, asmdefDisableFile);
            asmdefFile = "Assets/Scripts/Game/ET/Code/ModelView/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefFile, asmdefDisableFile);
        }
        
        static void DisableETClientViewCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/HotfixView/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefDisableFile, asmdefFile);
            asmdefFile = "Assets/Scripts/Game/ET/Code/ModelView/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefDisableFile, asmdefFile);
        }
        
        static void EnableETServerCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Hotfix/Server/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefFile, asmdefDisableFile);
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Server/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefFile, asmdefDisableFile);
        }
        
        static void DisableETServerCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Hotfix/Server/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefDisableFile, asmdefFile);
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Server/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefDisableFile, asmdefFile);
        }
        
        static void EnableModelGenerateClientCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Generate/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefFile, asmdefDisableFile);
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Generate/ClientServer/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefDisableFile, asmdefFile);
        }
        
        static void EnableModelGenerateClientServerCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Generate/ClientServer/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefFile, asmdefDisableFile);
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Generate/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            RenameFile(asmdefDisableFile, asmdefFile);
        }
        
        static void RefreshETResourceRule(bool isClient)
        {
            ResourceRuleEditorData ruleEditorData = AssetDatabase.LoadAssetAtPath<ResourceRuleEditorData>(ResourceRuleTool.ResourceRuleAsset_ET);
            foreach (var rule in ruleEditorData.rules)
            {
                if (rule.name == "ET.Client")
                {
                    rule.valid = isClient;
                }
                else if(rule.name == "ET.ClientServer")
                {
                    rule.valid = !isClient;
                }
            }
        }
    }
}
