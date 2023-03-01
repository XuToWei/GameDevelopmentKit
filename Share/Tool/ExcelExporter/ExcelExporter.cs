using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Xml;

namespace ET
{
    public static class ExcelExporter
    {
        /// <summary>
        /// 是否使用json，需要测试使用json的时候更改此字段
        /// </summary>
        private static readonly bool UseJson = false;
        
        /// <summary>
        /// luban命令模板，不能带换行符
        /// </summary>
        private const string lubanCommandTemplate = "dotnet %GEN_CLIENT% --template_search_path %CUSTOM_TEMPLATE_DIR% -j cfg -- -d %INPUT_DATA_DIR%/Defines/__root__.xml --input_data_dir %INPUT_DATA_DIR%/Datas --output_code_dir %OUTPUT_CODE_DIR% --output_data_dir %OUTPUT_DATA_DIR% --gen_types %GEN_TYPE_CODE_DATA% -s %GEN_GROUP%";
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
        
        private static List<Input_Output_Gen_Info> input_data_dirs;
        public static void Export()
        {
            string[] dirs = Directory.GetDirectories(excel_dir);
            if (dirs.Length < 1)
                return;
            
            input_data_dirs = new List<Input_Output_Gen_Info>();
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                string genConfigFile = Path.Combine(dir, gen_config_name).Replace('\\', '/');;
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
                //string saveCmd = "";
                XmlNodeList xmlGens = xmlRoot.SelectNodes("Gen");
                for (int j = 0; j < xmlGens.Count; j++)
                {
                    XmlNode xmlGen = xmlGens.Item(j);
                    Input_Output_Gen_Info info = new Input_Output_Gen_Info();
                    info.Input_Data_Dir = dir;
                    info.Output_Code_Dirs = xmlGen.SelectSingleNode("Output_Code_Dirs").Attributes.GetNamedItem("Value").Value.Split(',').ToList();
                    info.Output_Data_Dirs = xmlGen.SelectSingleNode("Output_Data_Dirs").Attributes.GetNamedItem("Value").Value.Split(',').ToList();
                    info.Gen_Type_Code_Data = xmlGen.SelectSingleNode("Gen_Type_Code_Data").Attributes.GetNamedItem("Value").Value;
                    if (UseJson)
                    {
                        info.Gen_Type_Code_Data = info.Gen_Type_Code_Data.Replace("code_cs_unity_bin", "code_cs_unity_json")
                                .Replace("data_bin", "data_json");
                    }
                    info.Gen_Group = xmlGen.SelectSingleNode("Gen_Group").Attributes.GetNamedItem("Value").Value;
                    input_data_dirs.Add(info);
                    //saveCmd += GetCommand(info);
                }
                //Log.Info(saveCmd);
            }

            List<Action> genActions = new List<Action>();
            foreach (Input_Output_Gen_Info info in input_data_dirs)
            {
                genActions.Add(() =>
                {
                    string cmd = GetCommand(info);
                    if (!RunCommand(cmd, "../Bin/"))
                    {
                        Log.Error($"Run error! Cmd:{cmd}");
                        return;
                    }

                    if (info.Output_Code_Dirs.Count > 1)
                    {
                        for (int i = 1; i < info.Output_Code_Dirs.Count; i++)
                        {
                            FileHelper.CopyDirectory(info.Output_Code_Dirs[0], info.Output_Code_Dirs[i]);
                        }
                    }

                    if (info.Output_Data_Dirs.Count > 1)
                    {
                        for (int i = 1; i < info.Output_Data_Dirs.Count; i++)
                        {
                            FileHelper.CopyDirectory(info.Output_Data_Dirs[0], info.Output_Data_Dirs[i]);
                        }
                    }
                });
            }

            int maxParallelCount = Math.Max(1, Environment.ProcessorCount / 2 - 1);
            Parallel.ForEach(genActions, new ParallelOptions {MaxDegreeOfParallelism = maxParallelCount}, genAction => { genAction(); });

            try
            {
                GenerateUGFEntityId.GenerateCode();
            }
            catch (Exception e)
            {
                Log.Console($"GenerateUGFEntityId Code Fail! Msg:{e}");
            }
            
            try
            {
                GenerateUGFUIFormId.GenerateCode();
            }
            catch (Exception e)
            {
                Log.Console($"GenerateUGFUIFormId Code Fail! Msg:{e}");
            }
            
            Log.Console("Export Excel Finished!");
        }

        private static string GetCommand(Input_Output_Gen_Info info)
        {
            string cmd = lubanCommandTemplate;
            if (string.Equals(info.Gen_Type_Code_Data, "code_cs_unity_editor_json"))//editor,不用自定义模板,不用导出数据
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

        private static bool RunCommand(string cmd, string workDirectory)
        {
            bool isSuccess = false;
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
                start.WorkingDirectory = workDirectory;

                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.StandardOutputEncoding = System.Text.Encoding.UTF8;
                start.StandardErrorEncoding = System.Text.Encoding.UTF8;

                bool endOutput = false;
                bool endError = false;

                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        Log.Debug(args.Data);
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
                        Log.Error(args.Data);
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
                isSuccess = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                isSuccess = false;
            }
            finally
            {
                process.Close();
            }
            return isSuccess;
        }
    }
}