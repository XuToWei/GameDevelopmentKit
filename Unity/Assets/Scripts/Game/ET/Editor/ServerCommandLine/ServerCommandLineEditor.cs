using System.Collections.Generic;
using SimpleJSON;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    public enum DevelopMode
    {
        正式 = 0,
        开发 = 1,
        压测 = 2,
    }
    
    public class ServerCommandLineEditor: EditorWindow
    {
        [MenuItem("ET/Server Tool")]
        public static void ShowWindow()
        {
            GetWindow<ServerCommandLineEditor>("Server CommandLine", DockDefine.Types);
        }
        
        private int selectStartConfigIndex = 1;
        private string[] startConfigs;
        private string startConfig;
        private DevelopMode developMode;

        private void OnEnable()
        {
            JSONNode jsonNode = JSONNode.Parse(AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Res/Editor/ET/Luban/dtstartmachineconfig.json").text);
            List<string> configs = new List<string>();
            foreach (var childNode in jsonNode.Children)
            {
                DRStartMachineConfig config = DRStartMachineConfig.LoadJsonDRStartMachineConfig(childNode);
                if (!configs.Contains(config.StartConfig))
                {
                    configs.Add(config.StartConfig);
                }
            }
            this.startConfigs = configs.ToArray();
        }

        private void OnGUI()
        {
            selectStartConfigIndex = EditorGUILayout.Popup(selectStartConfigIndex, this.startConfigs);
            this.startConfig = this.startConfigs[this.selectStartConfigIndex];
            this.developMode = (DevelopMode) EditorGUILayout.EnumPopup("起服模式：", this.developMode);

            string dotnet = "dotnet.exe";
            
#if UNITY_EDITOR_OSX
            dotnet = "dotnet";
#endif
            
            if (GUILayout.Button("Start Server(Single Process)"))
            {
                string arguments = $"App.dll --Process=1 --StartConfig={this.startConfig} --Console=1";
                ProcessHelper.Run(dotnet, arguments, "../Bin/");
            }
            
            if (GUILayout.Button("Start Watcher"))
            {
                string arguments = $"App.dll --AppType=Watcher --StartConfig={this.startConfig} --Console=1";
                ProcessHelper.Run(dotnet, arguments, "../Bin/");
            }

            if (GUILayout.Button("Start Mongo"))
            {
                ProcessHelper.Run("mongod", @"--dbpath=db", "../Database/bin/");
            }

            if (GUILayout.Button("Build DotNet/DotNet.sln"))
            {
                ShellHelper.Run("dotnet build", "../DotNet");
            }
        }
    }
}
