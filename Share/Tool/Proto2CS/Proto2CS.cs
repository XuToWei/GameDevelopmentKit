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
            public string Proto_File;
            public int Start_Opcode;
            public string Code_Name;
            public List<string> Code_Output_Dirs;
            public GenCodeType Code_Type;
        }
        
        private enum GenCodeType
        {
            ET,
            UGF
        }
        
        public static void Export()
        {
            Console.WriteLine("proto2cs start!");
            
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(File.ReadAllText($"{ProtoDir}/GenConfig.xml"));
            XmlNode xmlRoot = xmlDocument.SelectSingleNode("Config");
            XmlNodeList xmlGens = xmlRoot.SelectNodes("Gen");
            List<Gen_Info> genInfos = new List<Gen_Info>();
            for (int i = 0; i < xmlGens.Count; i++)
            {
                XmlNode xmlGen = xmlGens.Item(i);
                XmlNode openNode = xmlGen.SelectSingleNode("Open");
                if (!openNode.Attributes.GetNamedItem("Value").Value.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                Gen_Info info = new Gen_Info();
                info.Proto_File = xmlGen.SelectSingleNode("Proto_File").Attributes.GetNamedItem("Value").Value;
                info.Start_Opcode = int.Parse(xmlGen.SelectSingleNode("Start_Opcode").Attributes.GetNamedItem("Value").Value);
                info.Code_Name = xmlGen.SelectSingleNode("Code_Name").Attributes.GetNamedItem("Value").Value;
                string dirsStr = xmlGen.SelectSingleNode("Code_Output_Dirs").Attributes.GetNamedItem("Value").Value;
                info.Code_Output_Dirs = dirsStr.Split(',').ToList();
                info.Code_Type = Enum.Parse<GenCodeType>(xmlGen.SelectSingleNode("Code_Type").Attributes.GetNamedItem("Value").Value);
                genInfos.Add(info);
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
                    Proto2CS_ET.Proto2CS(info.Proto_File, info.Code_Name, info.Code_Output_Dirs, info.Start_Opcode);
                }
                else if (info.Code_Type == GenCodeType.UGF)
                {
                    //Protobuf，为了通用
                    Proto2CS_UGF.Proto2CS(info.Proto_File, info.Code_Name, info.Code_Output_Dirs, info.Start_Opcode);
                }
            }
            
            Console.WriteLine("proto2cs succeed!");
        }
    }
}