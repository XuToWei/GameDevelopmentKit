﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ET
{
    internal class OpcodeInfo
    {
        public string name;
        public int opcode;
    }

    public static partial class Proto2CS
    {
        private static readonly string WorkDir = Path.GetFullPath("../Bin");
        private const string PROTO_ROOT_DIR = "../Design/Proto";
        private static readonly char[] s_SplitChars = [' ', '\t'];
        private static readonly string[] s_SplitStrings = ["//"];

        private class GenConfig
        {
            public string protoDir { get; set; }
            public bool active { get; set; }
            public int startOpcode { get; set; }
            public string codeName { get; set; }
            public List<string> codeOutputDirs { get; set; }
            public string codeType { get; set; }
            public string nameSpace { get; set; }
        }

        public static void Export()
        {
            Log.Info("proto2cs start!");
            string[] childDirs = Directory.GetDirectories(PROTO_ROOT_DIR);
            if (childDirs.Length < 1)
            {
                throw new Exception($"{PROTO_ROOT_DIR} doesn't exist child directory!");
            }
            List<GenConfig> genConfigs = new List<GenConfig>();
            foreach (var childDir in childDirs)
            {
                string genConfigFile = $"{childDir}/proto.conf";
                if (!File.Exists(genConfigFile))
                {
                    continue;
                }
                var genConfig = JsonSerializer.Deserialize<GenConfig>(
                    File.ReadAllText(genConfigFile, Encoding.UTF8),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (!genConfig.active)
                {
                    continue;
                }
                for (int i = 0; i < genConfig.codeOutputDirs.Count; i++)
                {
                    genConfig.codeOutputDirs[i] = genConfig.codeOutputDirs[i]
                            .Replace("%CONF_ROOT%", childDir)
                            .Replace("%UNITY_ASSETS%", Path.GetFullPath(Path.Combine(WorkDir, Define.UNITY_ASSETS_PATH)))
                            .Replace("%ROOT%", Path.GetFullPath(Path.Combine(WorkDir, Define.ROOT_PATH)))
                            .Replace('\\', '/');
                }
                genConfig.protoDir = childDir;
                genConfigs.Add(genConfig);
            }

            if (genConfigs.Count < 1)
            {
                throw new Exception($"{PROTO_ROOT_DIR} doesn't exist any proto.conf file!");
            }

            // foreach (var genConfig in genConfigs)
            // {
            //     foreach (var dir in genConfig.codeOutputDirs)
            //     {
            //         if (Directory.Exists(dir))
            //         {
            //             //只删除cs文件，保留meta，避免提交反复生成meta
            //             var csFiles = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);
            //             foreach (var csFile in csFiles)
            //             {
            //                 Log.Info(csFile);
            //                 File.Delete(csFile);
            //             }
            //         }
            //     }
            // }

            foreach (var genConfig in genConfigs)
            {
                if (string.Equals(genConfig.codeType, "ET", StringComparison.CurrentCultureIgnoreCase))
                {
                    //MemoryPack
                    List<string> protoFiles = Directory.GetFiles(genConfig.protoDir, "*.proto", SearchOption.AllDirectories).ToList();
                    if (protoFiles.Count < 1)
                    {
                        continue;
                    }
                    protoFiles.Sort((a, b)=> String.Compare(a, b, StringComparison.Ordinal));
                    Proto2CS_ET.Start(genConfig.codeName, genConfig.codeOutputDirs, genConfig.startOpcode, genConfig.nameSpace);
                    foreach (var protoFile in protoFiles)
                    {
                        Proto2CS_ET.Proto2CS(protoFile);
                    }
                    Proto2CS_ET.Stop();
                }
                else if (string.Equals(genConfig.codeType, "UGF", StringComparison.CurrentCultureIgnoreCase))
                {
                    //Protobuf，为了通用
                    List<string> protoFiles = Directory.GetFiles(genConfig.protoDir, "*.proto", SearchOption.AllDirectories).ToList();
                    if (protoFiles.Count < 1)
                    {
                        continue;
                    }
                    protoFiles.Sort((a, b)=> String.Compare(a, b, StringComparison.Ordinal));
                    Proto2CS_UGF.Start(genConfig.codeName, genConfig.codeOutputDirs, genConfig.startOpcode, genConfig.nameSpace);
                    foreach (var protoFile in protoFiles)
                    {
                        Proto2CS_UGF.Proto2CS(protoFile);
                    }
                    Proto2CS_UGF.Stop();
                }
                else
                {
                    throw new Exception($"{genConfig.protoDir} error codeType : {genConfig.codeType} !");
                }
            }

            //删除空目录和空meta文件
            foreach (var genConfig in genConfigs)
            {
                foreach (var dir in genConfig.codeOutputDirs)
                {
                    if (Directory.Exists(dir))
                    {
                        var metaFiles = Directory.GetFiles(dir, "*.meta", SearchOption.AllDirectories);
                        foreach (var metaFile in metaFiles)
                        {
                            string name = metaFile.Substring(0, metaFile.Length - 5);
                            if(File.Exists(name))
                                continue;
                            if (Directory.Exists(name))
                            {
                                if (Directory.GetFiles(name, "*.cs", SearchOption.AllDirectories).Length > 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    Directory.Delete(name, true);
                                }
                            }
                            if (File.Exists(metaFile))
                            {
                                File.Delete(metaFile);
                            }
                        }
                    }
                }
            }

            Log.Info("proto2cs success!");
        }
    }
}