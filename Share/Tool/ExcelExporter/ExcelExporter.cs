using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ET
{
    public static class ExcelExporter
    {
        private const string working_dir = "../Bin/";
        private const string luban_error_line = "=======================================================================";

        /// <summary>
        /// luban命令模板，不能带换行符
        /// </summary>
        private const string lubanCommandTemplate =
                "dotnet %GEN_CLIENT% --template_search_path %CUSTOM_TEMPLATE_DIR% -j cfg -- -d %INPUT_DATA_DIR%/Defines/__root__.xml --input_data_dir %INPUT_DATA_DIR%/Datas --output_code_dir %OUTPUT_CODE_DIR% --output_data_dir %OUTPUT_DATA_DIR% --gen_types %GEN_TYPE_CODE_DATA% -s %GEN_GROUP%";

        private const string gen_client = "../Tools/Luban/Tools/Luban.ClientServer/Luban.ClientServer.dll";
        private const string custom_template_dir = "../Tools/Luban/CustomTemplates/LoadAsync";
        private const string excel_dir = "../Design/Excel";
        private const string gen_config_name = "GenConfig.xml";

        private struct Input_Output_Gen_Info
        {
            public string Input_Data_Dir;
            public List<string> Output_Code_Dirs;
            public List<string> Output_Data_Dirs;
            public string Gen_Type_Code_Data;
            public string Gen_Group;
        }

        public static void Export()
        {
            string[] dirs = Directory.GetDirectories(excel_dir);
            if (dirs.Length < 1)
                return;
            bool useJson = Options.Instance.Custom.Split(" ").Contains("Json");
            List<Input_Output_Gen_Info> genInfos = new List<Input_Output_Gen_Info>();
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                string genConfigFile = Path.Combine(dir, gen_config_name).Replace('\\', '/');
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
                    info.Input_Data_Dir = dir;
                    info.Output_Code_Dirs = xmlGen.SelectSingleNode("Output_Code_Dirs").Attributes.GetNamedItem("Value").Value.Split(',').ToList();
                    info.Output_Data_Dirs = xmlGen.SelectSingleNode("Output_Data_Dirs").Attributes.GetNamedItem("Value").Value.Split(',').ToList();
                    info.Gen_Type_Code_Data = xmlGen.SelectSingleNode("Gen_Type_Code_Data").Attributes.GetNamedItem("Value").Value;
                    if (useJson)
                    {
                        info.Gen_Type_Code_Data = info.Gen_Type_Code_Data.
                                Replace("code_cs_unity_bin", "code_cs_unity_json")
                                .Replace("data_bin", "data_json");
                    }

                    info.Gen_Group = xmlGen.SelectSingleNode("Gen_Group").Attributes.GetNamedItem("Value").Value;
                    genInfos.Add(info);
                }
            }

            bool isSuccess = true;
            int maxParallelism = Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75 * 2.0));
            Parallel.ForEachAsync(genInfos,
                new ParallelOptions() { MaxDegreeOfParallelism = maxParallelism },
                async (info, _) =>
                {
                    await Task.CompletedTask;
                    string cmd = GetCommand(info);
                    if (!RunCommand(cmd))
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
                }).Wait();

            if (!isSuccess)
            {
                return;
            }

            GenerateUGFEntityId.GenerateCode();

            GenerateUGFUIFormId.GenerateCode();

            Log.Console("Export Excel Success!");
        }

        private static string GetCommand(Input_Output_Gen_Info info)
        {
            string cmd = lubanCommandTemplate;
            if (string.Equals(info.Gen_Type_Code_Data, "code_cs_unity_editor_json")) //editor,不用自定义模板,不用导出数据
            {
                cmd = cmd.Replace(" --template_search_path %CUSTOM_TEMPLATE_DIR%", "");
                cmd = cmd.Replace(" --output_data_dir %OUTPUT_DATA_DIR%", "");
            }

            cmd = cmd.Replace("%GEN_CLIENT%", gen_client)
                    .Replace("%CUSTOM_TEMPLATE_DIR%", custom_template_dir)
                    .Replace("%INPUT_DATA_DIR%", info.Input_Data_Dir)
                    .Replace("%OUTPUT_CODE_DIR%", info.Output_Code_Dirs[0])
                    .Replace("%OUTPUT_DATA_DIR%", info.Output_Data_Dirs[0])
                    .Replace("%GEN_TYPE_CODE_DATA%", info.Gen_Type_Code_Data)
                    .Replace("%GEN_GROUP%", info.Gen_Group);
            return cmd;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <returns>错误信息</returns>
        private static bool RunCommand(string cmd)
        {
            bool isSuccess = true;
            Process process = new();
            try
            {
                string app = "bash";
                string arguments = "-c";
                Encoding encoding = Encoding.UTF8;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    app = "cmd.exe";
                    arguments = "/c";
                    //luban在windows上编码为GB2312
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    encoding = Encoding.GetEncoding("GB2312");
                }

                ProcessStartInfo start = new ProcessStartInfo(app);

                process.StartInfo = start;
                start.Arguments = arguments + " \"" + cmd + "\"";
                start.CreateNoWindow = true;
                start.ErrorDialog = true;
                start.UseShellExecute = false;
                start.WorkingDirectory = working_dir;

                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.StandardOutputEncoding = encoding;
                start.StandardErrorEncoding = encoding;

                bool endOutput = false;
                bool endError = false;
                bool inError = false;
                
                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        //luban运行时候，配置错误不会抛出异常，所以需要从OutputData筛选错误信息
                        if (string.Equals(luban_error_line, args.Data))
                        {
                            inError = !inError;
                        }
                        else if (inError)
                        {
                            isSuccess = false;
                            Log.ConsoleError(args.Data);
                        }
                    }
                    else
                    {
                        endOutput = true;
                    }
                };
                
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        isSuccess = false;
                        Log.ConsoleError(args.Data);
                    }
                    else
                    {
                        endError = true;
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                while (!endOutput || !endError)
                {
                }

                process.CancelOutputRead();
                process.CancelErrorRead();
            }
            catch (Exception e)
            {
                isSuccess = false;
                Log.ConsoleError(e.ToString());
            }
            finally
            {
                process.Close();
            }

            return isSuccess;
        }
    }
}