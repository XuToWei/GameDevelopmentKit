using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ET
{
    internal class OpcodeInfo
    {
        public string Name;
        public int Opcode;
    }

    public static partial class Proto2CS
    {
        private const string ProtoDir = "../Design/Proto";
        private static readonly char[] splitChars = { ' ', '\t' };
        
        private class Gen_Info
        {
            public string Proto_Dir;
            public int Start_Opcode;
            public string Code_Name;
            public List<string> Code_Output_Dirs;
            public GenCodeType Code_Type;
            public string Name_Space;
        }
        
        private enum GenCodeType
        {
            ET,
            UGF
        }
        
        public static void Export()
        {
            Log.Info("proto2cs start!");
            string[] childDirs = Directory.GetDirectories(ProtoDir);
            if (childDirs.Length < 1)
            {
                Log.Error($"{ProtoDir} doesn't exist child directory!");
                return;
            }
            List<Gen_Info> genInfos = new List<Gen_Info>();
            foreach (var childDir in childDirs)
            {
                string genConfigFile = $"{childDir}/GenConfig.xml";
                if (!File.Exists(genConfigFile))
                {
                    continue;
                }
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(File.ReadAllText(genConfigFile));
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("Config");
                XmlNodeList xmlGens = xmlRoot.SelectNodes("Gen");
                for (int i = 0; i < xmlGens.Count; i++)
                {
                    XmlNode xmlGen = xmlGens.Item(i);
                    XmlNode openNode = xmlGen.SelectSingleNode("Open");
                    if (!openNode.Attributes.GetNamedItem("Value").Value.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    Gen_Info info = new Gen_Info();
                    info.Proto_Dir = childDir;
                    info.Start_Opcode = int.Parse(xmlGen.SelectSingleNode("Start_Opcode").Attributes.GetNamedItem("Value").Value);
                    info.Code_Name = xmlGen.SelectSingleNode("Code_Name").Attributes.GetNamedItem("Value").Value;
                    string dirsStr = xmlGen.SelectSingleNode("Code_Output_Dirs").Attributes.GetNamedItem("Value").Value;
                    info.Code_Output_Dirs = dirsStr.Split(',').ToList();
                    info.Code_Type = Enum.Parse<GenCodeType>(xmlGen.SelectSingleNode("Code_Type").Attributes.GetNamedItem("Value").Value);
                    info.Name_Space = xmlGen.SelectSingleNode("Name_Space").Attributes.GetNamedItem("Value").Value;
                    genInfos.Add(info);
                }
            }

            if (genInfos.Count < 1)
            {
                Log.Error($"{ProtoDir} doesn't exist Open GenConfig.xml!");
                return;
            }

            foreach (var info in genInfos)
            {
                foreach (var dir in info.Code_Output_Dirs)
                {
                    if (Directory.Exists(dir))
                    {
                        Directory.Delete(dir, true);
                    }
                }
            }

            foreach (var info in genInfos)
            {
                if (info.Code_Type == GenCodeType.ET)
                {
                    //MemoryPack
                    List<string> protoFiles = Directory.GetFiles(info.Proto_Dir).ToList();
                    if (protoFiles.Count < 1)
                    {
                        continue;
                    }
                    protoFiles.Sort((a, b)=> String.Compare(a, b, StringComparison.Ordinal));
                    Proto2CS_ET.Start(info.Code_Name, info.Code_Output_Dirs, info.Start_Opcode, info.Name_Space);
                    foreach (var protoFile in protoFiles)
                    {
                        Proto2CS_ET.Proto2CS(protoFile);
                    }
                    Proto2CS_ET.Stop();
                }
                else if (info.Code_Type == GenCodeType.UGF)
                {
                    //Protobuf，为了通用
                    List<string> protoFiles = Directory.GetFiles(info.Proto_Dir).ToList();
                    if (protoFiles.Count < 1)
                    {
                        continue;
                    }
                    protoFiles.Sort((a, b)=> String.Compare(a, b, StringComparison.Ordinal));
                    Proto2CS_UGF.Start(info.Code_Name, info.Code_Output_Dirs, info.Start_Opcode, info.Name_Space);
                    foreach (var protoFile in protoFiles)
                    {
                        Proto2CS_UGF.Proto2CS(protoFile);
                    }
                    Proto2CS_UGF.Stop();
                }
            }
            
            Log.Info("proto2cs succeed!");
        }
    }
}