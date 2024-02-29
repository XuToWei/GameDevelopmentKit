using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ET
{
    public static partial class Proto2CS
    {
        public static class Proto2CS_ET
        {
            private static readonly Regex s_ResponseTypeRegex = new Regex(@"//\s*ResponseType");

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
                
                s_MsgOpcode.Clear();
                s_StringBuilder = new StringBuilder();
                s_StringBuilder.Append("// This is an automatically generated class by Share.Tool. Please do not modify it.\n");
                s_StringBuilder.Append("\n");
                s_StringBuilder.Append("using MemoryPack;\n");
                s_StringBuilder.Append("using System.Collections.Generic;\n");
                s_StringBuilder.Append("\n");
                s_StringBuilder.Append($"namespace {nameSpace}\n");
                s_StringBuilder.Append("{\n");
            }

            public static void Stop()
            {
                s_StringBuilder.Append("\tpublic static partial class " + s_CSName + "\n\t{\n");
                foreach (OpcodeInfo info in s_MsgOpcode)
                {
                    s_StringBuilder.Append($"\t\t public const ushort {info.name} = {info.opcode};\n");
                }
                s_StringBuilder.Append("\t}\n");
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
                string msgName = string.Empty;
                string responseType = "";
                StringBuilder sbDispose = new StringBuilder();
                foreach (string line in s.Split('\n'))
                {
                    string newline = line.Trim();

                    if (string.IsNullOrEmpty(newline))
                    {
                        continue;
                    }

                    if (s_ResponseTypeRegex.IsMatch(newline))
                    {
                        responseType = s_ResponseTypeRegex.Replace(newline, string.Empty);
                        responseType = responseType.Trim().Split(' ')[0].TrimEnd('\r', '\n');
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

                        msgName = newline.Split(s_SplitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                        string[] ss = newline.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

                        if (ss.Length == 2)
                        {
                            parentClass = ss[1].Trim();
                        }

                        if (s_StartOpcode - 1 >= OpcodeRangeDefine.MaxOpcode)
                        {
                            throw new Exception($"Proto_ET error : {protoFile}'s opcode is larger then max opcode:{OpcodeRangeDefine.MaxOpcode}!");
                        }
                        s_MsgOpcode.Add(new OpcodeInfo() { name = msgName, opcode = ++s_StartOpcode });

                        s_StringBuilder.Append($"\t// protofile : {protoFile.Replace("\\", "/").Split("/")[^2]}/{Path.GetFileName(protoFile)}\n");
                        s_StringBuilder.Append($"\t[MemoryPackable]\n");
                        s_StringBuilder.Append($"\t[Message({s_CSName}.{msgName})]\n");
                        if (!string.IsNullOrEmpty(responseType))
                        {
                            s_StringBuilder.Append($"\t[ResponseType(nameof({responseType}))]\n");
                        }

                        s_StringBuilder.Append($"\tpublic partial class {msgName}: MessageObject");

                        if (parentClass is "IActorMessage" or "IActorRequest" or "IActorResponse")
                        {
                            s_StringBuilder.Append($", {parentClass}\n");
                        }
                        else if (parentClass != "")
                        {
                            s_StringBuilder.Append($", {parentClass}\n");
                        }
                        else
                        {
                            s_StringBuilder.Append("\n");
                        }

                        continue;
                    }

                    if (isMsgStart)
                    {
                        if (newline.StartsWith("{"))
                        {
                            sbDispose.Clear();
                            s_StringBuilder.Append("\t{\n");
                            s_StringBuilder.Append($"\t\tpublic static {msgName} Create(bool isFromPool = false) \n\t\t{{ \n\t\t\treturn ObjectPool.Instance.Fetch(typeof({msgName}), isFromPool) as {msgName}; \n\t\t}}\n\n");
                            continue;
                        }

                        if (newline.StartsWith("}"))
                        {
                            isMsgStart = false;
                            responseType = "";

                            // 加了no dispose则自己去定义dispose函数，不要自动生成
                            if (!newline.Contains("// no dispose"))
                            {
                                s_StringBuilder.Append($"\t\tpublic override void Dispose() \n\t\t{{\n\t\t\tif (!this.IsFromPool) {{ return; }}\n{sbDispose.ToString()}\t\t\tObjectPool.Instance.Recycle(this); \n\t\t}}\n");
                            }

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

                        if (memberStr.StartsWith("map<"))
                        {
                            Map(s_StringBuilder, memberStr, sbDispose);
                        }
                        else if (memberStr.StartsWith("repeated"))
                        {
                            Repeated(s_StringBuilder, memberStr, sbDispose);
                        }
                        else
                        {
                            Members(s_StringBuilder, memberStr, sbDispose);
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
                sb.Replace("\t", "    ");
                string result = sb.ToString().ReplaceLineEndings("\r\n");
                sw.Write(result);

                Log.Info($"proto2cs file : {csPath}");
            }

            private static void Map(StringBuilder sb, string newline, StringBuilder sbDispose)
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
                    int n = int.Parse(ss[2]);

                    sb.Append("\t\t[MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]\n");
                    sb.Append($"\t\t[MemoryPackOrder({n - 1})]\n");
                    sb.Append($"\t\tpublic Dictionary<{keyType}, {valueType}> {v} {{ get; set; }} = new();\n");

                    sbDispose.Append($"\t\t\tthis.{v}.Clear();\n");
                }
                catch (Exception)
                {
                    Log.Warning($"ErrorLine => \"{s_CSName}\" : \"{newline}\"\n");
                    throw;
                }
            }

            private static void Repeated(StringBuilder sb, string newline, StringBuilder sbDispose)
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

                    sb.Append($"\t\t[MemoryPackOrder({n - 1})]\n");
                    sb.Append($"\t\tpublic List<{type}> {name} {{ get; set; }} = new List<{type}>();\n");

                    sbDispose.Append($"\t\t\tthis.{name}.Clear();\n");
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

            private static void Members(StringBuilder sb, string newline, StringBuilder sbDispose)
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
                    Log.Warning($"ErrorLine => \"{s_CSName}\" : \"{newline}\"\n");
                    throw;
                }
            }
        }
    }
}