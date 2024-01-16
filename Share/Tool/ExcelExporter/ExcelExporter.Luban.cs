using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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
            private static readonly string lubanCommandHeaderTemplate =
                    $"dotnet %GEN_CLIENT% --customTemplateDir %CUSTOM_TEMPLATE_DIR% --conf %CONF_ROOT%/luban.conf ";

            private const string gen_client = "../Tools/Luban/Tools/Luban/Luban.dll";
            private const string custom_template_dir = "../Tools/Luban/CustomTemplates/LoadAsync";
            private const string excel_dir = "../Design/Excel";
            private const string gen_config_name = "luban.conf";
            private const string luban_temp_dir = "../Temp/Luban";
            private const string unity_assets_path = "../Unity/Assets";

            private static Encoding s_Encoding;

            private class CmdInfo
            {
                public string cmd;
                public Action save;
                public string dirName;
            }

            public class GenConfig
            {
                public bool active { get; set; }
                public List<string> cmds { get; set; }
            }

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
                bool isCheck = Options.Instance.Customs.Contains("Check", StringComparison.OrdinalIgnoreCase);
                bool showCmd = Options.Instance.Customs.Contains("ShowCmd", StringComparison.OrdinalIgnoreCase);
                StringBuilder cmdStringBuilder = new();
                List<CmdInfo> cmdInfos = new List<CmdInfo>();
                for (int i = 0; i < dirs.Length; i++)
                {
                    string dir = Path.GetFullPath(dirs[i]);
                    string genConfigFile = Path.Combine(dir, gen_config_name);
                    if (!File.Exists(genConfigFile))
                    {
                        continue;
                    }

                    var genConfig = JsonSerializer.Deserialize<GenConfig>(
                        File.ReadAllText(genConfigFile, Encoding.UTF8).Replace("\r\n", " ").Replace("\n", " ").Replace("\u0009", " "),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true, });
                    if (!genConfig.active)
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
                    string changeTimeInfoFile = Path.Combine(lubanTempDir, $"temp_timeinfo_{dirName}.txt");
                    List<string> files = FileHelper.GetAllFiles(dir);
                    files.Sort();
                    StringBuilder timeStringBuilder = new();
                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new(file);
                        timeStringBuilder.AppendLine($"{fileInfo.Name}:{fileInfo.LastWriteTime} {fileInfo.LastWriteTime.Millisecond}");
                    }

                    string changeTimeInfo = timeStringBuilder.ToString();
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    if (File.Exists(changeTimeInfoFile) && string.Equals(File.ReadAllText(changeTimeInfoFile), changeTimeInfo))
                    {
                        Log.Info($"Luban ignore export directory : {dir}!");
                        continue;
                    }

                    cmdStringBuilder.Clear();

                    for (int j = 0; j < genConfig.cmds.Count; j++)
                    {
                        cmdStringBuilder.Append(lubanCommandHeaderTemplate);
                        var cmd = genConfig.cmds[j];
                        if (!cmd.Contains("-x l10n.textProviderFile"))
                        {
                            cmd += $" -x l10n.textProviderFile={LocalizationExcelFile.Replace("/", "\\")}";
                        }

                        if (isCheck)
                        {
                            cmd += " -f";
                        }

                        cmdStringBuilder.Append(cmd);
                        cmdStringBuilder.AppendLine();
                    }

                    var cmdInfo = new CmdInfo();
                    cmdInfo.cmd = cmdStringBuilder.ToString()
                            .Replace("%GEN_CLIENT%", Path.GetFullPath(Path.Combine(WorkDir, gen_client)))
                            .Replace("%CUSTOM_TEMPLATE_DIR%", Path.GetFullPath(Path.Combine(WorkDir, custom_template_dir)))
                            .Replace("%CONF_ROOT%", Path.GetFullPath(dir))
                            .Replace("%UNITY_ASSETS%", Path.GetFullPath(Path.Combine(WorkDir, unity_assets_path)))
                            .Replace("/", "\\");
                    Match match = Regex.Match(cmdInfo.cmd, "-(.*)");
                    if (match.Success)
                    {
                        string hyphenAndAfter = match.Value;
                        string replaced = Regex.Replace(hyphenAndAfter, @"(\s+)", " ");
                        cmdInfo.cmd = cmdInfo.cmd.Replace(hyphenAndAfter, replaced);
                    }

                    if (useJson)
                    {
                        cmdInfo.cmd = cmdInfo.cmd
                                .Replace("-c cs-bin", "-c cs-simple-json")
                                .Replace("-d bin", "-d json");
                    }

                    if (isCheck)
                    {
                        const string pattern1 = @"-x\s.*outputDataDir=\S*";
                        cmdInfo.cmd = Regex.Replace(cmdInfo.cmd, pattern1, "");
                        const string pattern2 = @"-x\s.*outputCodeDir=\S*";
                        cmdInfo.cmd = Regex.Replace(cmdInfo.cmd, pattern2, "");
                    }

                    cmdInfo.save = () => { File.WriteAllText(changeTimeInfoFile, changeTimeInfo); };
                    cmdInfos.Add(cmdInfo);
                    cmdInfo.dirName = dirName;
                }

                bool isSuccess = true;
                int processCount = 0;
                Parallel.ForEachAsync(cmdInfos,
                    async (cmdInfo, _) =>
                    {
                        Log.Info($"Export Luban directory : {cmdInfo.dirName}");
                        if (showCmd)
                        {
                            Log.Info(cmdInfo.cmd);
                        }

                        if (await RunCommand(cmdInfo.cmd, lubanTempDir))
                        {
                            cmdInfo.save();
                            Log.Info($"Export Luban process : {Interlocked.Add(ref processCount, 1)}/{cmdInfos.Count}");
                        }
                        else
                        {
                            isSuccess = false;
                            Log.Warning($"Export Luban process : {Interlocked.Add(ref processCount, 1)}/{cmdInfos.Count}");
                        }
                    }).Wait();

                if (isSuccess)
                {
                    Log.Info("Export Luban excel success!");
                }
                else
                {
                    Log.Warning($"Export Luban excel fail!");
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

                    // start.RedirectStandardOutput = true;
                    start.RedirectStandardError = true;
                    start.RedirectStandardInput = true;
                    // start.StandardOutputEncoding = s_Encoding;
                    start.StandardErrorEncoding = s_Encoding;

                    // process.OutputDataReceived += (sender, args) =>
                    // {
                    //     if (!string.IsNullOrEmpty(args.Data))
                    //     {
                    //         Log.Info(args.Data);
                    //     }
                    // };
                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            isSuccess = false;
                            Log.Warning(args.Data);
                        }
                    };

                    process.Start();
                    // process.BeginOutputReadLine();
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