using System.IO;
using UnityEditor;
using UnityGameFramework.Editor;

namespace ET.Editor
{
    internal static class CodeModeDefineSymbolTool
    {
#if UNITY_ET_CODEMODE_CLIENT
        [MenuItem("ET/Define Symbol/CodeMode(Client)/Enable UNITY_ET_CODEMODE_SERVER")]
        public static void UNITY_ET_CODEMODE_SERVER()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
        }
        
        [MenuItem("ET/Define Symbol/CodeMode(Client)/Enable UNITY_ET_CODEMODE_CLIENTSERVER")]
        public static void UNITY_ET_CODEMODE_CLIENTSERVER()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
        }
        
        [InitializeOnLoadMethod]
        public static void UNITY_ET_CODEMODE_CLIENT()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            EnableETClientCode();
            EnableETClientViewCode();
            DisableETServerCode();
            EnableModelGenerateClientCode();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
#elif UNITY_ET_CODEMODE_SERVER
        [MenuItem("ET/Define Symbol/CodeMode(Server)/Enable UNITY_ET_CODEMODE_CLIENT")]
        public static void UNITY_ET_CODEMODE_CLIENT()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
        }

        [MenuItem("ET/Define Symbol/CodeMode(Server)/Enable UNITY_ET_CODEMODE_CLIENTSERVER")]
        public static void UNITY_ET_CODEMODE_CLIENTSERVER()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
        }
        
        [InitializeOnLoadMethod]
        public static void UNITY_ET_CODEMODE_SERVER()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            EnableETClientCode();
            DisableETClientViewCode();
            EnableETServerCode();
            EnableModelGenerateClientServerCode();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
#elif UNITY_ET_CODEMODE_CLIENTSERVER
        [MenuItem("ET/Define Symbol/CodeMode(ClientServer)/Enable UNITY_ET_CODEMODE_CLIENT")]
        public static void UNITY_ET_CODEMODE_CLIENT()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
        }

        [MenuItem("ET/Define Symbol/CodeMode(ClientServer)/Enable UNITY_ET_CODEMODE_SERVER")]
        public static void UNITY_ET_CODEMODE_SERVER()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENTSERVER");
            ScriptingDefineSymbols.AddScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
        }

        [InitializeOnLoadMethod]
        public static void UNITY_ET_CODEMODE_CLIENTSERVER()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_CLIENT");
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol("UNITY_ET_CODEMODE_SERVER");
            EnableETClientCode();
            EnableETClientViewCode();
            EnableETServerCode();
            EnableModelGenerateClientServerCode();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
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
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
#endif
        
        static void EnableETClientCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Hotfix/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefFile))
            {
                File.Move(asmdefFile, asmdefDisableFile);
                File.Delete(asmdefFile);
                File.Delete($"{asmdefFile}.meta");
            }
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefFile))
            {
                File.Move(asmdefFile, asmdefDisableFile);
                File.Delete(asmdefFile);
                File.Delete($"{asmdefFile}.meta");
            }
        }
        
        static void DisableETClientCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Hotfix/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefDisableFile))
            {
                File.Move(asmdefDisableFile, asmdefFile);
                File.Delete(asmdefDisableFile);
                File.Delete($"{asmdefDisableFile}.meta");
            }
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefDisableFile))
            {
                File.Move(asmdefDisableFile, asmdefFile);
                File.Delete(asmdefDisableFile);
                File.Delete($"{asmdefDisableFile}.meta");
            }
        }
        static void EnableETClientViewCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/HotfixView/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefFile))
            {
                File.Move(asmdefFile, asmdefDisableFile);
                File.Delete(asmdefFile);
                File.Delete($"{asmdefFile}.meta");
            }
            asmdefFile = "Assets/Scripts/Game/ET/Code/ModelView/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefFile))
            {
                File.Move(asmdefFile, asmdefDisableFile);
                File.Delete(asmdefFile);
                File.Delete($"{asmdefFile}.meta");
            }
        }
        
        static void DisableETClientViewCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/HotfixView/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefDisableFile))
            {
                File.Move(asmdefDisableFile, asmdefFile);
                File.Delete(asmdefDisableFile);
                File.Delete($"{asmdefDisableFile}.meta");
            }
            asmdefFile = "Assets/Scripts/Game/ET/Code/ModelView/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefDisableFile))
            {
                File.Move(asmdefDisableFile, asmdefFile);
                File.Delete(asmdefDisableFile);
                File.Delete($"{asmdefDisableFile}.meta");
            }
        }
        
        static void EnableETServerCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Hotfix/Server/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefFile))
            {
                File.Move(asmdefFile, asmdefDisableFile);
                File.Delete(asmdefFile);
                File.Delete($"{asmdefFile}.meta");
            }
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Server/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefFile))
            {
                File.Move(asmdefFile, asmdefDisableFile);
                File.Delete(asmdefFile);
                File.Delete($"{asmdefFile}.meta");
            }
        }
        
        static void DisableETServerCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Hotfix/Server/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefDisableFile))
            {
                File.Move(asmdefDisableFile, asmdefFile);
                File.Delete(asmdefDisableFile);
                File.Delete($"{asmdefDisableFile}.meta");
            }
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Server/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefDisableFile))
            {
                File.Move(asmdefDisableFile, asmdefFile);
                File.Delete(asmdefDisableFile);
                File.Delete($"{asmdefDisableFile}.meta");
            }
        }
        
        static void EnableModelGenerateClientCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Generate/Client/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefFile))
            {
                File.Move(asmdefFile, asmdefDisableFile);
                File.Delete(asmdefFile);
                File.Delete($"{asmdefFile}.meta");
            }
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Generate/ClientServer/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefDisableFile))
            {
                File.Move(asmdefDisableFile, asmdefFile);
                File.Delete(asmdefDisableFile);
                File.Delete($"{asmdefDisableFile}.meta");
            }
        }
        
        static void EnableModelGenerateClientServerCode()
        {
            string asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Generate/ClientServer/Ignore.asmdef";
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefFile))
            {
                File.Move(asmdefFile, asmdefDisableFile);
                File.Delete(asmdefFile);
                File.Delete($"{asmdefFile}.meta");
            }
            asmdefFile = "Assets/Scripts/Game/ET/Code/Model/Generate/Client/Ignore.asmdef";
            asmdefDisableFile = $"{asmdefFile}.DISABLED";
            if (File.Exists(asmdefDisableFile))
            {
                File.Move(asmdefDisableFile, asmdefFile);
                File.Delete(asmdefDisableFile);
                File.Delete($"{asmdefDisableFile}.meta");
            }
        }
    }
}
