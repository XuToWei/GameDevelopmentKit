using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ET
{
    public static partial class Proto2CS
    {
        public static class Proto2CS_ET
        {
            private static readonly List<OpcodeInfo> msgOpcode = new List<OpcodeInfo>();
            private static string csName;
            private static List<string> csOutDirs;
            private static int startOpcode;
            private static StringBuilder sb;
            
            public static void Start(string codeName, List<string> outDirs, int opcode, string nameSpace)
            {
                csName = codeName;
                csOutDirs = outDirs;
                startOpcode = opcode;
                
                msgOpcode.Clear();
                sb = new StringBuilder();
                sb.Append("// This is an automatically generated class by Share.Tool. Please do not modify it.\n");
                sb.Append("\n");
                sb.Append("using MemoryPack;\n");
                sb.Append("using System.Collections.Generic;\n");
                sb.Append("\n");
                sb.Append($"namespace {nameSpace}\n");
                sb.Append("{\n");
            }

            public static void Stop()
            {
                sb.Append("\tpublic static partial class " + csName + "\n\t{\n");
                foreach (OpcodeInfo info in msgOpcode)
                {
                    sb.Append($"\t\t public const ushort {info.Name} = {info.Opcode};\n");
                }
                sb.Append("\t}\n");
                sb.Append("}\n");
                foreach (var csOutDir in csOutDirs)
                {
                    GenerateCS(sb, csOutDir, csName);
                }
            }

            public static void Proto2CS(string protoFile)
            {
                string s = File.ReadAllText(protoFile);
                
                bool isMsgStart = false;
                string msgName = string.Empty;
                StringBuilder sbDispose = new StringBuilder();
                foreach (string line in s.Split('\n'))
                {
                    string newline = line.Trim();
                    if (newline.StartsWith("package"))
                    {
                        continue;
                    }

                    if (newline == "")
                    {
                        continue;
                    }

                    if (newline.StartsWith("//ResponseType"))
                    {
                        string responseType = line.Split(" ")[1].TrimEnd('\r', '\n');
                        sb.Append($"\t[ResponseType(nameof({responseType}))]\n");
                        continue;
                    }

                    if (newline.StartsWith("//"))
                    {
                        sb.Append($"{newline}\n");
                        continue;
                    }

                    if (newline.StartsWith("message"))
                    {
                        string parentClass = "";
                        isMsgStart = true;

                        msgName = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                        string[] ss = newline.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

                        if (ss.Length == 2)
                        {
                            parentClass = ss[1].Trim();
                        }

                        msgOpcode.Add(new OpcodeInfo() { Name = msgName, Opcode = ++startOpcode });
                        
                        sb.Append($"\t[Message({csName}.{msgName})]\n");
                        sb.Append($"\t[MemoryPackable]\n");
                        sb.Append($"\t//protofile : {protoFile.Replace("\\", "/").Split("/")[^2]}/{Path.GetFileName(protoFile)}\n");
                        sb.Append($"\tpublic partial class {msgName}: MessageObject");
                        if (parentClass == "IActorMessage" || parentClass == "IActorRequest" || parentClass == "IActorResponse")
                        {
                            sb.Append($", {parentClass}\n");
                        }
                        else if (parentClass != "")
                        {
                            sb.Append($", {parentClass}\n");
                        }
                        else
                        {
                            sb.Append("\n");
                        }

                        continue;
                    }

                    if (isMsgStart)
                    {
                        if (newline.StartsWith("{"))
                        {
                            sbDispose.Clear();
                            sb.Append("\t{\n");
                            sb.Append($"\t\tpublic static {msgName} Create(bool isFromPool = true) \n\t\t{{ \n\t\t\treturn !isFromPool? new {msgName}() : ObjectPool.Instance.Fetch(typeof({msgName})) as {msgName}; \n\t\t}}\n");
                            continue;
                        }

                        if (newline.StartsWith("}"))
                        {
                            isMsgStart = false;
                            
                            // 加了no dispose则自己去定义dispose函数，不要自动生成
                            if (!newline.Contains("// no dispose"))
                            {
                                sb.Append(
                                    $"\t\tpublic override void Dispose() \n\t\t{{\n\t\t\tif (!this.IsFromPool) return;\n{sbDispose.ToString()}\t\t\tObjectPool.Instance.Recycle(this); \n\t\t}}\n");
                            }

                            sb.Append("\t}\n\n");
                            continue;
                        }

                        if (newline.Trim().StartsWith("//"))
                        {
                            sb.Append($"{newline}\n");
                            continue;
                        }

                        if (newline.Trim() != "" && newline != "}")
                        {
                            if (newline.StartsWith("map<"))
                            {
                                Map(sb, newline, sbDispose);
                            }
                            else if (newline.StartsWith("repeated"))
                            {
                                Repeated(sb, newline, sbDispose);
                            }
                            else
                            {
                                Members(sb, newline, sbDispose);
                            }
                        }
                    }
                }
            }

            private static void GenerateCS(StringBuilder sb, string path, string csName)
            {
                if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string csPath = $"{path}/{csName}.cs";
                using FileStream txt = new FileStream(csPath, FileMode.Create, FileAccess.ReadWrite);
                using StreamWriter sw = new StreamWriter(txt);
                sw.Write(sb.ToString().Replace("\t", "    "));

                Console.WriteLine($"proto2cs file : {csPath}");
            }

            private static void Map(StringBuilder sb, string newline, StringBuilder sbDispose)
            {
                try
                {
                    int start = newline.IndexOf("<") + 1;
                    int end = newline.IndexOf(">");
                    string types = newline.Substring(start, end - start);
                    string[] ss = types.Split(",");
                    string keyType = ConvertType(ss[0].Trim());
                    string valueType = ConvertType(ss[1].Trim());
                    string tail = newline.Substring(end + 1);
                    ss = tail.Trim().Replace(";", "").Split(" ");
                    string v = ss[0];
                    int n = int.Parse(ss[2]);

                    sb.Append(
                        "\t\t[MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]\n");
                    sb.Append($"\t\t[MemoryPackOrder({n - 1})]\n");
                    sb.Append($"\t\tpublic Dictionary<{keyType}, {valueType}> {v} {{ get; set; }} = new();\n");

                    sbDispose.Append($"\t\t\tthis.{v}.Clear();\n");
                }
                catch (Exception)
                {
                    ConsoleHelper.WriteErrorLine($"ErrorLine => \"{csName}\" : \"{newline}\"\n");
                    throw;
                }
            }

            private static void Repeated(StringBuilder sb, string newline, StringBuilder sbDispose)
            {
                try
                {
                    int index = newline.IndexOf(";");
                    newline = newline.Remove(index);
                    string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                    string type = ss[1];
                    type = ConvertType(type);
                    string name = ss[2];
                    int n = int.Parse(ss[4]);

                    sb.Append($"\t\t[MemoryPackOrder({n - 1})]\n");
                    sb.Append($"\t\tpublic List<{type}> {name} {{ get; set; }} = new List<{type}>();\n");

                    sbDispose.Append($"\t\t\tthis.{name}.Clear();\n");
                }
                catch (Exception)
                {
                    ConsoleHelper.WriteErrorLine($"ErrorLine => \"{csName}\" : \"{newline}\"\n");
                    throw;
                }
            }

            private static string ConvertType(string type)
            {
                string typeCs = "";
                switch (type)
                {
                    case "int16":
                        typeCs = "short";
                        break;
                    case "int32":
                        typeCs = "int";
                        break;
                    case "bytes":
                        typeCs = "byte[]";
                        break;
                    case "uint32":
                        typeCs = "uint";
                        break;
                    case "long":
                        typeCs = "long";
                        break;
                    case "int64":
                        typeCs = "long";
                        break;
                    case "uint64":
                        typeCs = "ulong";
                        break;
                    case "uint16":
                        typeCs = "ushort";
                        break;
                    default:
                        typeCs = type;
                        break;
                }

                return typeCs;
            }

            private static void Members(StringBuilder sb, string newline, StringBuilder sbDispose)
            {
                try
                {
                    int index = newline.IndexOf(";");
                    newline = newline.Remove(index);
                    string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                    string type = ss[0];
                    string name = ss[1];
                    int n = int.Parse(ss[3]);
                    string typeCs = ConvertType(type);

                    sb.Append($"\t\t[MemoryPackOrder({n - 1})]\n");
                    sb.Append($"\t\tpublic {typeCs} {name} {{ get; set; }}\n");
                    
                    switch (typeCs)
                    {
                        case "bytes":
                        {
                            break;
                        }
                        default:
                            sbDispose.Append($"\t\t\tthis.{name} = default;\n");
                            break;
                    }
                }
                catch (Exception)
                {
                    ConsoleHelper.WriteErrorLine($"ErrorLine => \"{csName}\" : \"{newline}\"\n");
                    throw;
                }
            }
        }
    }
}