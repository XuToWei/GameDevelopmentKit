using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Game.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace ET.Editor
{
    public static class BuildAssemblyTool
    {
        /// <summary>
        /// 编译输出的目录
        /// </summary>
        public const string CodeDir = "Assets/Res/ET/Code";
        
        /// <summary>
        /// ET Code的目录
        /// </summary>
        public const string ETSourceCodeDir = "Assets/Scripts/Game/ET/Code";
        
        public static CodeOptimization DefaultCodeOptimization => CodeOptimization.Debug;

        public static void BuildModel(CodeOptimization codeOptimization, CodeMode codeMode)
        {
            List<string> codes;

            switch (codeMode)
            {
                case CodeMode.Client:
                    codes = new List<string>()
                    {
                        $"{ETSourceCodeDir}/Model/Generate/Client/",
                        $"{ETSourceCodeDir}/Model/Share/",
                        $"{ETSourceCodeDir}/Model/Client/",
                        $"{ETSourceCodeDir}/ModelView/Client/",
                    };
                    break;
                case CodeMode.Server:
                    codes = new List<string>()
                    {
                        $"{ETSourceCodeDir}/Model/Generate/ClientServer/",
                        $"{ETSourceCodeDir}/Model/Share/",
                        $"{ETSourceCodeDir}/Model/Server/",
                        $"{ETSourceCodeDir}/Model/Client/",
                    };
                    break;
                case CodeMode.ClientServer:
                    codes = new List<string>()
                    {
                        $"{ETSourceCodeDir}/Model/Share/",
                        $"{ETSourceCodeDir}/Model/Client/",
                        $"{ETSourceCodeDir}/ModelView/Client/",
                        $"{ETSourceCodeDir}/Model/Generate/ClientServer/",
                        $"{ETSourceCodeDir}/Model/Server/",
                    };
                    break;
                default:
                    throw new Exception("not found enum");
            }

            BuildAssemblyHelper.Build("Model", codes, Array.Empty<string>(), GetExcludeReferences(codeMode), codeOptimization);
            File.Copy(Path.Combine(BuildAssemblyHelper.BuildOutputDir, "Model.dll"), Path.Combine(CodeDir, "Model.dll.bytes"), true);
            File.Copy(Path.Combine(BuildAssemblyHelper.BuildOutputDir, "Model.pdb"), Path.Combine(CodeDir, "Model.pdb.bytes"), true);
            Debug.Log("copy Model.dll to Res/ET/Code success!");
        }

        public static void BuildHotfix(CodeOptimization codeOptimization, CodeMode codeMode)
        {
            string[] logicFiles = Directory.GetFiles(BuildAssemblyHelper.BuildOutputDir, "Hotfix_*");
            foreach (string file in logicFiles)
            {
                File.Delete(file);
            }

            int random = RandomGenerator.RandomNumber(100000000, 999999999);
            string logicFile = $"Hotfix_{random}";
            
            List<string> codes;
            switch (codeMode)
            {
                case CodeMode.Client:
                    codes = new List<string>()
                    {
                        $"{ETSourceCodeDir}/Hotfix/Share/",
                        $"{ETSourceCodeDir}/Hotfix/Client/",
                        $"{ETSourceCodeDir}/HotfixView/Client/",
                    };
                    break;
                case CodeMode.Server:
                    codes = new List<string>()
                    {
                        $"{ETSourceCodeDir}/Hotfix/Share/",
                        $"{ETSourceCodeDir}/Hotfix/Server/",
                        $"{ETSourceCodeDir}/Hotfix/Client/",
                    };
                    break;
                case CodeMode.ClientServer:
                    codes = new List<string>()
                    {
                        $"{ETSourceCodeDir}/Hotfix/Share/",
                        $"{ETSourceCodeDir}/Hotfix/Client/",
                        $"{ETSourceCodeDir}/HotfixView/Client/",
                        $"{ETSourceCodeDir}/Hotfix/Server/",
                    };
                    break;
                default:
                    throw new Exception("not found enum");
            }
            
            BuildAssemblyHelper.Build("Hotfix", codes,  new []{Path.Combine(BuildAssemblyHelper.BuildOutputDir, "Model.dll")}, GetExcludeReferences(codeMode), codeOptimization);
            File.Copy(Path.Combine(BuildAssemblyHelper.BuildOutputDir, "Hotfix.dll"), Path.Combine(CodeDir, "Hotfix.dll.bytes"), true);
            File.Copy(Path.Combine(BuildAssemblyHelper.BuildOutputDir, "Hotfix.pdb"), Path.Combine(CodeDir, "Hotfix.pdb.bytes"), true);
            File.Copy(Path.Combine(BuildAssemblyHelper.BuildOutputDir, "Hotfix.dll"), Path.Combine(BuildAssemblyHelper.BuildOutputDir, $"{logicFile}.dll"), true);
            File.Copy(Path.Combine(BuildAssemblyHelper.BuildOutputDir, "Hotfix.pdb"), Path.Combine(BuildAssemblyHelper.BuildOutputDir, $"{logicFile}.pdb"), true);
            Debug.Log("copy Hotfix.dll to Res/ET/Code success!");
        }
        
        private static string[] GetExcludeReferences(CodeMode codeMode)
        {
            if (codeMode == CodeMode.Client)
            {
                return new string[]
                {
                    "MongoDB.Driver.Core.dll",
                    "MongoDB.Driver.dll",
                    "MongoDB.Driver.Legacy.dll",
                    "MongoDB.Libmongocrypt.dll",
                    "SharpCompress.dll",
                    "System.Buffers.dll",
                    "System.Runtime.CompilerServices.Unsafe.dll",
                    "System.Text.Encoding.CodePages.dll",
                };
            }
            return null;
        }
    }
}