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
    public static partial class ExcelExporter
    {
        public static class ExcelExporter_Luban
        {
            private const string not_translated_text_file = "NotLocalization.txt";

            /// <summary>
            /// luban命令模板，不能带换行符
            /// </summary>
            private static readonly string lubanCommandTemplate =
                    $"dotnet %GEN_CLIENT% --customTemplateDir %CUSTOM_TEMPLATE_DIR% --conf %CONF_ROOT%/luban.conf ";

            private const string gen_client = "../Tools/Luban/Tools/Luban/Luban.dll";
            private const string custom_template_dir = "../Tools/Luban/CustomTemplates/LoadAsync";
            private const string excel_dir = "../Design/Excel";
            private const string gen_config_name = "GenConfig.xml";
            private const string luban_temp_dir = "../Temp/Luban";

            private static Encoding s_Encoding;

            // private class Input_Output_Gen_Info
            // {
            //     public string Luban_Work_Dir;
            //     public string Input_Data_Dir;
            //     public List<string> Output_Code_Dirs;
            //     public List<string> Output_Data_Dirs;
            //     public string Gen_Type_Code_Data;
            //     public string Gen_Group;
            //     public string Text_Field_Name;
            //     public string Extra_Command;
            // }

            public static bool IsEnableET { get; private set; }
            public static bool IsEnableGameHot { get; private set; }

            public static void DoExport()
            {
                Log.Info("Start export Luban excel ...");
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

                string excelDir = Path.GetFullPath(Path.Combine(WorkDir, excel_dir));
                string[] dirs = Directory.GetDirectories(excelDir);
                if (dirs.Length < 1)
                {
                    throw new Exception($"Directory {excelDir} is empty");
                }

                var dirList = dirs.ToList();
                dirList.Sort();
                dirs = dirList.ToArray();

                string lubanTempDir = Path.GetFullPath(Path.Combine(WorkDir, luban_temp_dir));
                if (!Directory.Exists(lubanTempDir))
                {
                    Directory.CreateDirectory(lubanTempDir);
                }

                bool useJson = Options.Instance.Customs.Contains("Json", StringComparison.OrdinalIgnoreCase);
                StringBuilder cmdStringBuilder = new ();
                List<Action> saveTimeActions = new List<Action>();
                for (int i = 0; i < dirs.Length; i++)
                {
                    string dir = Path.GetFullPath(dirs[i]);
                    string genConfigFile = Path.Combine(dir, gen_config_name);
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
                    if (string.Equals(Directory.GetParent(genConfigFile).Name, "ET", StringComparison.Ordinal))
                    {
                        IsEnableET = true;
                    }
                    else if (string.Equals(Directory.GetParent(genConfigFile).Name, "GameHot", StringComparison.Ordinal))
                    {
                        IsEnableGameHot = true;
                    }
                    
                    int lastIndex = dir.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                    string dirName = dir.Substring(lastIndex, dir.Length - lastIndex);
                    string changeTimeInfoFile = Path.Combine(lubanTempDir, $"ChangeTimeInfo_{dirName}.txt");
                    List<string> files = FileHelper.GetAllFiles(dir);
                    files.Sort();
                    StringBuilder timeStringBuilder = new ();
                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new (file);
                        timeStringBuilder.AppendLine($"{fileInfo.Name}:{fileInfo.LastWriteTime} {fileInfo.LastWriteTime.Millisecond}");
                    }
                    string changeTimeInfo = timeStringBuilder.ToString();
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    if (File.Exists(changeTimeInfoFile) && string.Equals(File.ReadAllText(changeTimeInfoFile), changeTimeInfo))
                    {
                        Log.Info($"Luban directory {dir} has no change and ignore export !");
                        continue;
                    }
                    saveTimeActions.Add(() =>
                    {
                        File.WriteAllText(changeTimeInfoFile, changeTimeInfo);
                    });
                    
                    string cmdHeader = lubanCommandTemplate.Replace("%GEN_CLIENT%", Path.GetFullPath(Path.Combine(WorkDir, gen_client)).Replace("/", "\\"))
                            .Replace("%CUSTOM_TEMPLATE_DIR%", Path.GetFullPath(Path.Combine(WorkDir, custom_template_dir)).Replace("/", "\\"))
                            .Replace("%CONF_ROOT%", Path.GetFullPath(dir).Replace("/", "\\"));
                    XmlNodeList xmlGens = xmlRoot.SelectNodes("Gen");
                    Log.Info(cmdHeader);
                    for (int j = 0; j < xmlGens.Count; j++)
                    {
                        cmdStringBuilder.Append(cmdHeader);
                        XmlNode xmlGen = xmlGens.Item(j);
                        string genCmd = xmlGen.InnerText.Replace("\r\n", " ")
                                .Replace("  ", " ")
                                .Replace("   ", " ")
                                .Replace("    ", " ")
                                .Replace("     ", " ")
                                .Replace("      ", " ")
                                .Replace("       ", " ")
                                .Replace("        ", " ");
                        if (useJson)
                        {
                            genCmd = genCmd.Replace("-c cs-bin", "-c cs-simple-json")
                                    .Replace("-c cs-bin", "-c cs-simple-json")
                                    .Replace("-d bin", "-d json");
                        }
                        Log.Info(xmlGen.InnerText);
                        Log.Info(genCmd);
                        cmdStringBuilder.Append(genCmd);
                        cmdStringBuilder.AppendLine();
                    }
                }

                bool isSuccess = true;
                async Task ExportAsync()
                { 
                    if (!await RunCommand(cmdStringBuilder.ToString(), lubanTempDir))
                    {
                        isSuccess = false;
                    }
                }
                ExportAsync().Wait();
                
                //
                // foreach (var genInfo in genInfos)
                // {
                //     string file = Path.Combine(genInfo.Luban_Work_Dir, not_translated_text_file);
                //     if (File.Exists(file))
                //     {
                //         if (File.ReadAllText(file).Length > 0)
                //         {
                //             Log.Warning($"Please check {file} to solve not translated text!");
                //         }
                //     }
                // }
                //
                // GenerateUGFEntityId.GenerateCode();
                // GenerateUGFUIFormId.GenerateCode();
                // GenerateUGFAllSoundId.GenerateCode();

                if (isSuccess)
                {
                    Log.Info("Export Luban excel success!");
                }
                else
                {
                    Log.Error($"Export Luban excel fail!");
                }
            }

            /// <summary>
            /// 执行命令
            /// </summary>
            /// <param name="cmd">命令</param>
            /// <param name="workDir">工作目录</param>
            /// <returns>是否成功</returns>
            private static async Task<bool> RunCommand(string cmd, string workDir)
            {
                Log.Info(cmd);
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

                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            Log.Info(args.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            isSuccess = false;
                            Log.Warning(args.Data);
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
                    Log.Error(e);
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