using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ET
{
    public static partial class Proto2CS
    {
        public static class Proto2CS_UGF
        {
            private static readonly List<OpcodeInfo> s_MsgOpcode = new List<OpcodeInfo>();
            private static string s_CSName;
            private static List<string> s_CSOutDirs;
            private static int s_StartOpcode;
            private static StringBuilder s_StringBuilder;

            public static void Start(string codeName, List<string> outDirs, int opcode, string nameSpace)
            {
                s_CSName = codeName;
                s_CSOutDirs = outDirs;
                s_StartOpcode = opcode;

                s_StringBuilder = new StringBuilder();
                s_StringBuilder.Append("// This is an automatically generated class by Share.Tool. Please do not modify it.\n");
                s_StringBuilder.Append("\n");
                s_StringBuilder.Append("using ProtoBuf;\n");
                s_StringBuilder.Append("using System;\n");
                s_StringBuilder.Append("using System.Collections.Generic;\n");
                s_StringBuilder.Append("\n");
                s_StringBuilder.Append($"namespace {nameSpace}\n");
                s_StringBuilder.Append("{\n");
            }

            public static void Stop()
            {
                s_StringBuilder.Append("}\n");
                foreach (var csOutDir in s_CSOutDirs)
                {
                    GenerateCS(s_StringBuilder, csOutDir, s_CSName);
                }
            }

            public static void Proto2CS(string protoFile)
            {
                string s = File.ReadAllText(protoFile);
                
                bool isMsgStart = false;
                StringBuilder disposeSb = new StringBuilder();
                foreach (string line in s.Split('\n'))
                {
                    string newline = line.Trim();
                    
                    if (string.IsNullOrEmpty(newline))
                    {
                        continue;
                    }

                    if (!isMsgStart && newline.StartsWith("//"))
                    {
                        if (newline.StartsWith("///"))
                        {
                            s_StringBuilder.Append("\t/// <summary>\n");
                            s_StringBuilder.Append($"\t/// {newline.TrimStart('/', ' ')}\n");
                            s_StringBuilder.Append("\t/// </summary>\n");
                        }
                        else
                        {
                            s_StringBuilder.Append($"\t// {newline.TrimStart('/', ' ')}\n");
                        }
                        continue;
                    }

                    if (newline.StartsWith("message"))
                    {
                        string parentClass = "";
                        isMsgStart = true;

                        disposeSb.Clear();
                        disposeSb.Append($"\t\tpublic override void Clear()\n");
                        disposeSb.Append("\t\t{\n");

                        string msgName = newline.Split(s_SplitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                        string[] ss = newline.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

                        if (ss.Length == 2)
                        {
                            parentClass = ss[1].Trim();
                        }

                        s_StringBuilder.Append($"\t// protofile : {protoFile.Replace("\\", "/").Split("/")[^2]}/{Path.GetFileName(protoFile)}\n");
                        s_StringBuilder.Append($"\t[Serializable, ProtoContract(Name = @\"{msgName}\")]\n");
                        s_StringBuilder.Append($"\tpublic partial class {msgName}");
                        if (string.IsNullOrEmpty(parentClass))
                        {
                            if (msgName.StartsWith("CS", StringComparison.OrdinalIgnoreCase))
                            {
                                s_StringBuilder.Append(" : CSPacketBase\n");
                            }
                            else if (msgName.StartsWith("SC", StringComparison.OrdinalIgnoreCase))
                            {
                                s_StringBuilder.Append(" : SCPacketBase\n");
                            }
                            else
                            {
                                throw new Exception("\n");
                            }
                        }
                        else
                        {
                            s_StringBuilder.Append($" : {parentClass}\n");
                        }
                        
                        continue;
                    }

                    if (isMsgStart)
                    {
                        if (newline.StartsWith("{"))
                        {
                            s_StringBuilder.Append("\t{\n");
                            s_StringBuilder.Append($"\t\tpublic override int Id => {++s_StartOpcode};\n");
                            continue;
                        }

                        if (newline.StartsWith("}"))
                        {
                            isMsgStart = false;
                            disposeSb.Append("\t\t}\n");
                            s_StringBuilder.Append(disposeSb.ToString());
                            disposeSb.Clear();
                            s_StringBuilder.Append("\t}\n\n");
                            continue;
                        }

                        if (newline.Trim().StartsWith("//"))
                        {
                            s_StringBuilder.Append("\t\t/// <summary>\n");
                            s_StringBuilder.Append($"\t\t/// {newline.TrimStart('/', ' ')}\n");
                            s_StringBuilder.Append("\t\t/// </summary>\n");
                            continue;
                        }

                        string memberStr;
                        if (newline.Contains("//"))
                        {
                            string[] lineSplit = newline.Split("//");
                            memberStr = lineSplit[0].Trim();
                            s_StringBuilder.Append("\t\t/// <summary>\n");
                            s_StringBuilder.Append($"\t\t/// {lineSplit[1].Trim()}\n");
                            s_StringBuilder.Append("\t\t/// </summary>\n");
                        }
                        else
                        {
                            memberStr = newline;
                        }

                        if (newline.StartsWith("map<"))
                        {
                            Map(s_StringBuilder, newline, disposeSb);
                        }
                        else if (newline.StartsWith("repeated"))
                        {
                            Repeated(s_StringBuilder, newline, disposeSb);
                        }
                        else
                        {
                            Members(s_StringBuilder, newline, disposeSb);
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

                Log.Info($"proto2cs file : {csPath}");
            }

            private static void Map(StringBuilder sb, string newline, StringBuilder disposeSb)
            {
                try
                {
                    int start = newline.IndexOf("<", StringComparison.Ordinal) + 1;
                    int end = newline.IndexOf(">", StringComparison.Ordinal);
                    string types = newline.Substring(start, end - start);
                    string[] ss = types.Split(",");
                    string keyType = ConvertType(ss[0].Trim());
                    string valueType = ConvertType(ss[1].Trim());
                    string tail = newline.Substring(end + 1);
                    ss = tail.Trim().Replace(";", "").Split(" ");
                    string v = ss[0];
                    string n = ss[2];
                
                    sb.Append($"\t\t[ProtoMember({n})]\n");
                    sb.Append($"\t\tpublic Dictionary<{keyType}, {valueType}> {v} {{ get; set; }} = new Dictionary<{keyType}, {valueType}>();\n");

                    disposeSb.Append($"\t\t\tthis.{v}.Clear();\n");
                }
                catch (Exception)
                {
                    Log.Warning($"ErrorLine => \"{s_CSName}\" : \"{newline}\"\n");
                    throw;
                }
            }

            private static void Repeated(StringBuilder sb, string newline, StringBuilder disposeSb)
            {
                try
                {
                    int index = newline.IndexOf(";", StringComparison.Ordinal);
                    newline = newline.Remove(index);
                    string[] ss = newline.Split(s_SplitChars, StringSplitOptions.RemoveEmptyEntries);
                    string type = ss[1];
                    type = ConvertType(type);
                    string name = ss[2];
                    int n = int.Parse(ss[4]);

                    sb.Append($"\t\t[ProtoMember({n})]\n");
                    sb.Append($"\t\tpublic List<{type}> {name} {{ get; set; }} = new List<{type}>();\n");

                    disposeSb.Append($"\t\t\tthis.{name}.Clear();\n");
                }
                catch (Exception)
                {
                    Log.Warning($"ErrorLine => \"{s_CSName}\" : \"{newline}\"\n");
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

            private static void Members(StringBuilder sb, string newline, StringBuilder disposeSb)
            {
                try
                {
                    int index = newline.IndexOf(";", StringComparison.Ordinal);
                    newline = newline.Remove(index);
                    string[] ss = newline.Split(s_SplitChars, StringSplitOptions.RemoveEmptyEntries);
                    string type = ss[0];
                    string name = ss[1];
                    int n = int.Parse(ss[3]);
                    string typeCs = ConvertType(type);

                    sb.Append($"\t\t[ProtoMember({n})]\n");
                    sb.Append($"\t\tpublic {typeCs} {name} {{ get; set; }}\n");

                    switch (typeCs)
                    {
                        case "bytes":
                        {
                            break;
                        }
                        default:
                            disposeSb.Append($"\t\t\tthis.{name} = default;\n");
                            break;
                    }
                }
                catch (Exception)
                {
                    Log.Warning($"ErrorLine => \"{s_CSName}\" : \"{newline}\"\n");
                    throw;
                }
            }
        }
    }
}