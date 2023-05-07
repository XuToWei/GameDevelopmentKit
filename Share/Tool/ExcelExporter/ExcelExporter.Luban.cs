using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ET
{
    public static partial class ExcelExporter
    {
        public static class ExcelExporter_Luban
        {
            private const string luban_error_line = "=======================================================================";
            private const string not_translated_text_file = "NotLocalization.txt";

            /// <summary>
            /// luban命令模板，不能带换行符
            /// </summary>
            private static readonly string lubanCommandTemplate =
                    $"dotnet %GEN_CLIENT% --template_search_path %CUSTOM_TEMPLATE_DIR% -j cfg -- -d %INPUT_DATA_DIR%/Defines/__root__.xml --input_data_dir %INPUT_DATA_DIR%/Datas --output_code_dir %OUTPUT_CODE_DIR% --output_data_dir %OUTPUT_DATA_DIR% --gen_types %GEN_TYPE_CODE_DATA% -s %GEN_GROUP%";

            /// <summary>
            /// luban本地化命令模板，不能带换行符
            /// </summary>
            private static readonly string lubanLocalizationCommandTemplate =
                    $" --l10n:input_text_files {LocalizationExcelFile} --l10n:text_field_name %TEXT_FIELD_NAME% --l10n:output_not_translated_text_file {not_translated_text_file}";

            private const string gen_client = "../Tools/Luban/Tools/Luban.ClientServer/Luban.ClientServer.dll";
            private const string custom_template_dir = "../Tools/Luban/CustomTemplates/LoadAsync";
            private const string excel_dir = "../Design/Excel";
            private const string gen_config_name = "GenConfig.xml";
            private const string luban_work_dir = "../Temp/Luban";

            private static Encoding s_Encoding;

            private class Input_Output_Gen_Info
            {
                public string Luban_Work_Dir;
                public string Input_Data_Dir;
                public List<string> Output_Code_Dirs;
                public List<string> Output_Data_Dirs;
                public string Gen_Type_Code_Data;
                public string Gen_Group;
                public string Text_Field_Name;
            }

            public static void Export()
            {
                Console.WriteLine("Start Export Luban Excel ...");
                if (Options.Instance.Customs.Contains("GB2312", StringComparison.OrdinalIgnoreCase))
                {
                    //luban在windows上编码为GB2312
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    s_Encoding = Encoding.GetEncoding("GB2312");
                }
                else
                {
                    s_Encoding = Encoding.UTF8;
                }
                
                string excelDir =  Path.GetFullPath($"{WorkDir}/{excel_dir}");
                string[] dirs = Directory.GetDirectories(excelDir);
                if (dirs.Length < 1)
                {
                    throw new Exception($"Directory {excelDir} is empty");
                }

                string lubanWorkDir = Path.GetFullPath($"{WorkDir}/{luban_work_dir}");
                if (!Directory.Exists(lubanWorkDir))
                {
                    Directory.CreateDirectory(lubanWorkDir);
                }

                bool useJson = Options.Instance.Customs.Contains("Json", StringComparison.OrdinalIgnoreCase);
                List<Input_Output_Gen_Info> genInfos = new List<Input_Output_Gen_Info>();
                for (int i = 0; i < dirs.Length; i++)
                {
                    string dir = Path.GetFullPath(dirs[i]).Replace('/', '\\');
                    string genConfigFile =  Path.GetFullPath(Path.Combine(dir, gen_config_name));
                    if (!File.Exists(genConfigFile))
                    {
                        continue;
                    }

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(File.ReadAllText(genConfigFile));
                    XmlNode xmlRoot = xmlDocument.SelectSingleNode("Config");
                    XmlNode openNode = xmlRoot.SelectSingleNode("Open");
                    if (!openNode.Attributes.GetNamedItem("Value").Value.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    XmlNodeList xmlGens = xmlRoot.SelectNodes("Gen");
                    for (int j = 0; j < xmlGens.Count; j++)
                    {
                        XmlNode xmlGen = xmlGens.Item(j);
                        Input_Output_Gen_Info info = new Input_Output_Gen_Info();
                        int lastIndex = dir.LastIndexOf('\\');
                        info.Luban_Work_Dir =  Path.GetFullPath($"{lubanWorkDir}/{dir.Substring(lastIndex, dir.Length - lastIndex)}_{j}");
                        info.Input_Data_Dir = dir;
                        string dirsStr = xmlGen.SelectSingleNode("Output_Code_Dirs").Attributes.GetNamedItem("Value").Value.Replace("\n", "");
                        info.Output_Code_Dirs = dirsStr.Split(',').ToList();
                        dirsStr = xmlGen.SelectSingleNode("Output_Data_Dirs").Attributes.GetNamedItem("Value").Value.Replace("\n", "");
                        info.Output_Data_Dirs = dirsStr.Split(',').ToList();
                        info.Gen_Type_Code_Data = xmlGen.SelectSingleNode("Gen_Type_Code_Data").Attributes.GetNamedItem("Value").Value;
                        if (useJson)
                        {
                            info.Gen_Type_Code_Data = info.Gen_Type_Code_Data.Replace("code_cs_unity_bin", "code_cs_unity_json")
                                    .Replace("data_bin", "data_json")
                                    .Replace("code_cs_bin", "code_cs_dotnet_json");
                        }
                        info.Gen_Group = xmlGen.SelectSingleNode("Gen_Group").Attributes.GetNamedItem("Value").Value;
                        XmlNode textFieldNameNode = xmlGen.SelectSingleNode("Text_Field_Name");
                        if (textFieldNameNode == null)
                        {
                            info.Text_Field_Name = string.Empty;
                        }
                        else
                        {
                            info.Text_Field_Name = textFieldNameNode.Attributes.GetNamedItem("Value").Value;
                        }
                        genInfos.Add(info);
                    }
                }
                
                foreach (var genInfo in genInfos)
                {
                    string file = Path.GetFullPath($"{genInfo.Luban_Work_Dir}/{not_translated_text_file}");
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }

                Console.WriteLine("Export Luban Excel Parallel ForEachAsync!");
                bool isSuccess = true;
                int maxParallelism = Math.Max(1, Environment.ProcessorCount / 2 - 1);
                int processCount = 0;
                Parallel.ForEachAsync(genInfos,
                    new ParallelOptions() { MaxDegreeOfParallelism = maxParallelism },
                    async (info, _) =>
                    {
                        if (info.Luban_Work_Dir != null && !Directory.Exists(info.Luban_Work_Dir))
                        {
                            Directory.CreateDirectory(info.Luban_Work_Dir);
                        }
                        string cmd = GetCommand(info);
                        if (!await RunCommand(cmd, info.Luban_Work_Dir))
                        {
                            isSuccess = false;
                            return;
                        }
                        if (info.Output_Code_Dirs.Count > 1)
                        {
                            for (int k = 1; k < info.Output_Code_Dirs.Count; k++)
                            {
                                FileHelper.CleanDirectory(info.Output_Code_Dirs[k]);
                                FileHelper.CopyDirectory(info.Output_Code_Dirs[0], info.Output_Code_Dirs[k]);
                            }
                        }
                        if (info.Output_Data_Dirs.Count > 1)
                        {
                            for (int k = 1; k < info.Output_Data_Dirs.Count; k++)
                            {
                                FileHelper.CleanDirectory(info.Output_Data_Dirs[k]);
                                FileHelper.CopyDirectory(info.Output_Data_Dirs[0], info.Output_Data_Dirs[k]);
                            }
                        }
                        Console.WriteLine($"Export Luban Process : {Interlocked.Add(ref processCount, 1)}/{genInfos.Count} ");
                    }).Wait();

                if (!isSuccess)
                {
                    throw new Exception("Export Luban Excel Fail!");
                }

                foreach (var genInfo in genInfos)
                {
                    string file = Path.GetFullPath($"{genInfo.Luban_Work_Dir}/{not_translated_text_file}");
                    if (File.Exists(file))
                    {
                        if (File.ReadAllText(file).Length > 0)
                        {
                            Console.WriteLine($"Please check {file} to solve not translated text!");
                        }
                    }
                }

                GenerateUGFEntityId.GenerateCode();
                GenerateUGFUIFormId.GenerateCode();
                Console.WriteLine("Export Luban Excel Success!");
            }

            private static string GetCommand(Input_Output_Gen_Info info)
            {
                string cmd = lubanCommandTemplate;
                if (string.Equals(info.Gen_Type_Code_Data, "code_cs_unity_editor_json")) //editor,不用自定义模板,不用导出数据
                {
                    cmd = cmd.Replace(" --template_search_path %CUSTOM_TEMPLATE_DIR%", "");
                    cmd = cmd.Replace(" --output_data_dir %OUTPUT_DATA_DIR%", "");
                }

                cmd = cmd.Replace("%GEN_CLIENT%",  Path.GetFullPath($"{WorkDir}/{gen_client}"))
                        .Replace("%CUSTOM_TEMPLATE_DIR%",  Path.GetFullPath($"{WorkDir}/{custom_template_dir}"))
                        .Replace("%INPUT_DATA_DIR%",  Path.GetFullPath(info.Input_Data_Dir))
                        .Replace("%OUTPUT_CODE_DIR%",  Path.GetFullPath($"{WorkDir}/{info.Output_Code_Dirs[0]}"))
                        .Replace("%OUTPUT_DATA_DIR%",  Path.GetFullPath($"{WorkDir}/{info.Output_Data_Dirs[0]}"))
                        .Replace("%GEN_TYPE_CODE_DATA%", info.Gen_Type_Code_Data)
                        .Replace("%GEN_GROUP%", info.Gen_Group);
                if (!string.IsNullOrEmpty(info.Text_Field_Name))//如果Text_Field_Name有配置就执行本地化
                {
                    cmd += lubanLocalizationCommandTemplate.Replace("%TEXT_FIELD_NAME%", info.Text_Field_Name);
                }
                return cmd;
            }

            /// <summary>
            /// 执行命令
            /// </summary>
            /// <param name="cmd">命令</param>
            /// <param name="workDir">工作目录</param>
            /// <returns>错误信息</returns>
            private static async Task<bool> RunCommand(string cmd, string workDir)
            {
                bool isSuccess = true;
                Process process = new();
                try
                {
                    string app = "bash";
                    string arguments = "-c";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        app = "cmd.exe";
                        arguments = "/c";
                    }

                    ProcessStartInfo start = new ProcessStartInfo(app);

                    process.StartInfo = start;
                    start.Arguments = arguments + " \"" + cmd + "\"";
                    start.CreateNoWindow = true;
                    start.ErrorDialog = true;
                    start.UseShellExecute = false;
                    start.WorkingDirectory = workDir;

                    start.RedirectStandardOutput = true;
                    start.RedirectStandardError = true;
                    start.RedirectStandardInput = true;
                    start.StandardOutputEncoding = s_Encoding;
                    start.StandardErrorEncoding = s_Encoding;

                    bool inError = false;
                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            //luban运行时候，配置错误不会抛出异常，所以需要从OutputData筛选错误信息
                            if (string.Equals(luban_error_line, args.Data))
                            {
                                isSuccess = false;
                                inError = !inError;
                                Console.Error.WriteLine(args.Data);
                            }
                            else if (inError)
                            {
                                isSuccess = false;
                                if (!string.IsNullOrEmpty(args.Data))
                                {
                                    Console.Error.WriteLine(args.Data);
                                }
                            }
                        }
                    };
                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            isSuccess = false;
                            Console.Error.WriteLine(args.Data);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    await process.WaitForExitAsync();
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    await Console.Error.WriteLineAsync(e.ToString());
                }
                finally
                {
                    process.Close();
                }

                return isSuccess;
            }
        }
    }
}