using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    internal sealed class DefaultComponentSystemCodeCreator : ICodeCreator
    {
        private enum CodePathType
        {
            Client,
            Server,
            Share,
            View,
        }

        private const string ComponentCodeTemplateFile           = "Assets/Res/Editor/ET/Config/DefaultCodeTemplate.txt";
        private const string ComponentSystemCodeTemplateFile     = "Assets/Res/Editor/ET/Config/DefaultSystemCodeTemplate.txt";

        private const string CodePath = "Assets/Scripts/Game/ET/Code";

        private CodePathType m_CodePathType;
        private string m_CodeSubPath;
        
        public void OnEnable()
        {
            this.m_CodePathType = CodePathType.Client;
            this.m_CodeSubPath = string.Empty;
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Code Model Path", GetModelPath());
            EditorGUILayout.LabelField("Code Hotfix Path", GetHotfixPath());
            
            this.m_CodePathType = (CodePathType)EditorGUILayout.EnumPopup("Create Type", this.m_CodePathType);
            this.m_CodeSubPath = EditorGUILayout.TextField("Code Sub Path", this.m_CodeSubPath);
        }

        public void GenerateCode(string codeName)
        {
            GenerateComponentSystemCode(codeName);
        }
        
        private void GenerateComponentSystemCode(string componentName)
        {
            void GenerateCS(string templateFile, string codeFile)
            {
                if (File.Exists(codeFile))
                {
                    throw new Exception($"{codeFile} already exist!");
                }
                if (!File.Exists(templateFile))
                {
                    throw new Exception($"{templateFile} does not exist!");
                }
                string dirName = Path.GetDirectoryName(codeFile);
                if (string.IsNullOrEmpty(dirName))
                {
                    throw new Exception($"{dirName} does not exist!");
                }
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                string template = File.ReadAllText(templateFile);
                string codeStr = template.Replace("#NAME#", componentName);
                string nameSpace = GetNameSpace();
                codeStr = codeStr.Replace("#NAMESPACE#", string.IsNullOrEmpty(nameSpace) ? String.Empty : $".{nameSpace}");
                File.WriteAllText(codeFile, codeStr);
                Debug.Log($"Generate cs:{codeFile}!");
            }
            
            GenerateCS(ComponentCodeTemplateFile,          $"{GetModelPath()}/{componentName}.cs");
            GenerateCS(ComponentSystemCodeTemplateFile,    $"{GetHotfixPath()}/{componentName}System.cs");
            Debug.Log("生成完毕！");
        }

        private string GetModelPath()
        {
            if (this.m_CodePathType == CodePathType.Client)
            {
                return $"{CodePath}/Model/Client/{this.m_CodeSubPath}";
            }
            if (this.m_CodePathType == CodePathType.Server)
            {
                return $"{CodePath}/Model/Server/{this.m_CodeSubPath}";
            }
            if (this.m_CodePathType == CodePathType.Share)
            {
                return $"{CodePath}/Model/Share/{this.m_CodeSubPath}";
            }
            if (this.m_CodePathType == CodePathType.View)
            {
                return $"{CodePath}/ModelView/Client/{this.m_CodeSubPath}";
            }
            return CodePath;
        }
        
        private string GetHotfixPath()
        {
            if (this.m_CodePathType == CodePathType.Client)
            {
                return $"{CodePath}/Hotfix/Client/{this.m_CodeSubPath}";
            }
            if (this.m_CodePathType == CodePathType.Server)
            {
                return $"{CodePath}/Hotfix/Server/{this.m_CodeSubPath}";
            }
            if (this.m_CodePathType == CodePathType.Share)
            {
                return $"{CodePath}/Hotfix/Share/{this.m_CodeSubPath}";
            }
            if (this.m_CodePathType == CodePathType.View)
            {
                return $"{CodePath}/HotfixView/Client/{this.m_CodeSubPath}";
            }
            return CodePath;
        }
        
        private string GetNameSpace()
        {
            if (this.m_CodePathType == CodePathType.Client)
            {
                return "Client";
            }
            if (this.m_CodePathType == CodePathType.Server)
            {
                return "Server";
            }
            if (this.m_CodePathType == CodePathType.Share)
            {
                return string.Empty;
            }
            if (this.m_CodePathType == CodePathType.View)
            {
                return "Client";
            }
            return string.Empty;
        }
    }
}
