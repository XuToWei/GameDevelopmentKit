using System.IO;
using Game.Editor;
using UnityEditor;
using UnityGameFramework.Editor;
using UnityGameFramework.Extension.Editor;

namespace ET.Editor
{
    internal static class CodeModeDefineSymbolTool
    {
        private const string CodePath = "Assets/Scripts/Game/ET/Code";
        private static string s_IgnoreAsmdefTemplate;

        private static string IgnoreAsmdefTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(s_IgnoreAsmdefTemplate))
                {
                    s_IgnoreAsmdefTemplate = File.ReadAllText("Assets/Res/Editor/ET/Config/IgnoreAsmdefTemplate.txt");
                }
                return s_IgnoreAsmdefTemplate;
            }
        }

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
        
        static void AddAsmdefFile(string file, string asmdefName)
        {
            RemoveAsmdefFile(file);
            string content = IgnoreAsmdefTemplate.Replace("$asmdefName$", asmdefName);
            File.WriteAllText(file, content);
        }
        
        static void RemoveAsmdefFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
                string metaFile = $"{file}.meta";
                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                }
            }
        }
        
        static void EnableETClientCode()
        {
            RemoveAsmdefFile($"{CodePath}/Hotfix/Client/Ignore.asmdef");
            RemoveAsmdefFile($"{CodePath}/Model/Client/Ignore.asmdef");
        }
        
        static void DisableETClientCode()
        {
            AddAsmdefFile($"{CodePath}/Hotfix/Client/Ignore.asmdef", "Ignore.Hotfix.Client");
            AddAsmdefFile($"{CodePath}/Model/Client/Ignore.asmdef", "Ignore.Model.Client");
        }
        
        static void EnableETClientViewCode()
        {
            RemoveAsmdefFile($"{CodePath}/HotfixView/Client/Ignore.asmdef");
            RemoveAsmdefFile($"{CodePath}/ModelView/Client/Ignore.asmdef");
        }
        
        static void DisableETClientViewCode()
        {
            AddAsmdefFile($"{CodePath}/HotfixView/Client/Ignore.asmdef", "Ignore.HotfixView.Client");
            AddAsmdefFile($"{CodePath}/ModelView/Client/Ignore.asmdef", "Ignore.ModelView.Client");
        }
        
        static void EnableETServerCode()
        {
            RemoveAsmdefFile($"{CodePath}/Hotfix/Server/Ignore.asmdef");
            RemoveAsmdefFile($"{CodePath}/Model/Server/Ignore.asmdef");
        }
        
        static void DisableETServerCode()
        {
            AddAsmdefFile($"{CodePath}/Hotfix/Server/Ignore.asmdef", "Ignore.Hotfix.Server");
            AddAsmdefFile($"{CodePath}/Model/Server/Ignore.asmdef", "Ignore.Model.Server");
        }
        
        static void EnableModelGenerateClientCode()
        {
            RemoveAsmdefFile($"{CodePath}/Model/Generate/Client/Ignore.asmdef");
            AddAsmdefFile($"{CodePath}/Model/Generate/ClientServer/Ignore.asmdef", "Ignore.Model.Generate.ClientServer");
        }
        
        static void EnableModelGenerateClientServerCode()
        {
            RemoveAsmdefFile($"{CodePath}/Model/Generate/ClientServer/Ignore.asmdef");
            AddAsmdefFile($"{CodePath}/Model/Generate/Client/Ignore.asmdef", "Ignore.Model.Generate.Client");
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
