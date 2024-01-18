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
            private const string unity_assets_path = "../Unity/Assets";
            private const string root_path = "..";

            private static Encoding s_Encoding;

            private class CmdInfo
            {
                public string cmd;
                public string dirName;
                public string sourceCodePath;
                public List<string> copyCodePath;
                public string sourceDataPath;
                public List<string> copyDataPath;
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
                bool isGB2312 = Options.Instance.Customs.Contains("GB2312", StringComparison.OrdinalIgnoreCase);
                bool useJson = Options.Instance.Customs.Contains("Json", StringComparison.OrdinalIgnoreCase);
                bool isCheck = Options.Instance.Customs.Contains("Check", StringComparison.OrdinalIgnoreCase);
                bool showCmd = Options.Instance.Customs.Contains("ShowCmd", StringComparison.OrdinalIgnoreCase);
                bool showInfo = Options.Instance.Customs.Contains("ShowInfo", StringComparison.OrdinalIgnoreCase);

                string actionStr = isCheck? "check" : "export";
                Log.Info($"Start {actionStr} Luban excel ...");
                if (isGB2312)
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
                    for (int j = 0; j < genConfig.cmds.Count; j++)
                    {
                        var cmdInfo = new CmdInfo();
                        var cmd = lubanCommandHeaderTemplate + genConfig.cmds[j];
                        cmd = cmd
                                .Replace("%GEN_CLIENT%", Path.GetFullPath(Path.Combine(WorkDir, gen_client)))
                                .Replace("%CUSTOM_TEMPLATE_DIR%", Path.GetFullPath(Path.Combine(WorkDir, custom_template_dir)))
                                .Replace("%CONF_ROOT%", dir)
                                .Replace("%UNITY_ASSETS%", Path.GetFullPath(Path.Combine(WorkDir, unity_assets_path)))
                                .Replace("%ROOT%", Path.GetFullPath(Path.Combine(WorkDir, root_path)))
                                .Replace('\\', '/');

                        //去掉连续多个空格
                        Match match = Regex.Match(cmd, "-(.*)");
                        if (match.Success)
                        {
                            string hyphenAndAfter = match.Value;
                            string replaced = Regex.Replace(hyphenAndAfter, @"(\s+)", " ");
                            cmd = cmd.Replace(hyphenAndAfter, replaced);
                        }

                        if (useJson)
                        {
                            cmd = cmd
                                    .Replace("-c cs-bin", "-c cs-simple-json")
                                    .Replace("-d bin", "-d json");
                        }

                        if (isCheck)
                        {
                            cmd = Regex.Replace(cmd, @"-x\s.*outputCodeDir\s*=\S*\s", "");
                            cmd = Regex.Replace(cmd, @"-x\s.*outputDataDir\s*=\S*\s", "");
                            cmd = Regex.Replace(cmd, @"-c\s\S+\s", "");
                            cmd = Regex.Replace(cmd, @"-d\s\S+\s", "");
                            cmd += " -f";
                        }

                        if (!cmd.Contains("-x l10n.textProviderFile"))
                        {
                            cmd += $" -x l10n.textProviderFile={LocalizationExcelFile.Replace('\\', '/')}";
                        }

                        cmd = Regex.Replace(cmd, @"\s+(?=-)", " ");

                        match = Regex.Match(cmd, @"-x\s.*outputCodeDir\s*=([^\s]*)");
                        if (match.Success)
                        {
                            string pathStr = match.Groups[1].Value.Trim();
                            string[] paths = pathStr.Split(',');
                            if (paths.Length > 1)
                            {
                                cmdInfo.sourceCodePath = paths[0];
                                cmd = cmd.Replace(pathStr, cmdInfo.sourceCodePath);
                                cmdInfo.copyCodePath = new List<string>();
                                for (int k = 1; k < paths.Length; k++)
                                {
                                    cmdInfo.copyCodePath.Add(paths[k]);
                                }
                            }
                        }

                        match = Regex.Match(cmd, @"-x\s.*outputDataDir\s*=([^\s]*)");
                        if (match.Success)
                        {
                            string pathStr = match.Groups[1].Value.Trim();
                            string[] paths = pathStr.Split(',');
                            if (paths.Length > 1)
                            {
                                cmdInfo.sourceDataPath = paths[0];
                                cmd = cmd.Replace(pathStr, cmdInfo.sourceDataPath);
                                cmdInfo.copyDataPath = new List<string>();
                                for (int k = 1; k < paths.Length; k++)
                                {
                                    cmdInfo.copyDataPath.Add(paths[k]);
                                }
                            }
                        }

                        cmdInfo.cmd = cmd;
                        cmdInfo.dirName = dirName;
                        cmdInfos.Add(cmdInfo);
                    }
                }

                bool isSuccess = true;
                int processCount = 0;
                Parallel.ForEachAsync(cmdInfos,
                    async (cmdInfo, _) =>
                    {
                        if (showCmd)
                        {
                            Log.Info($"{cmdInfo.dirName} : {cmdInfo.cmd}");
                        }

                        if (await RunCommand(cmdInfo.cmd, WorkDir, cmdInfo.dirName, showInfo))
                        {
                            Log.Info($"Luban {actionStr} process : {Interlocked.Add(ref processCount, 1)}/{cmdInfos.Count}");
                        }
                        else
                        {
                            isSuccess = false;
                            Log.Warning($"Luban {actionStr} process : {Interlocked.Add(ref processCount, 1)}/{cmdInfos.Count}");
                        }
                    }).Wait();

                if (!isCheck)
                {
                    foreach (var cmdInfo in cmdInfos)
                    {
                        LubanFileHelper.ClearSubEmptyDirectory(cmdInfo.sourceCodePath);
                        LubanFileHelper.ClearSubEmptyDirectory(cmdInfo.sourceDataPath);
                        if (cmdInfo.copyCodePath != null)
                        {
                            foreach (var copyPath in cmdInfo.copyCodePath)
                            {
                                LubanFileHelper.CopyDirectory(cmdInfo.sourceCodePath, copyPath);
                                LubanFileHelper.ClearSubEmptyDirectory(copyPath);
                            }
                        }

                        if (cmdInfo.copyDataPath != null)
                        {
                            foreach (var copyPath in cmdInfo.copyDataPath)
                            {
                                LubanFileHelper.CopyDirectory(cmdInfo.sourceDataPath, copyPath);
                                LubanFileHelper.ClearSubEmptyDirectory(copyPath);
                            }
                        }
                    }

                    GenerateUGFAllSoundId.GenerateCode();
                    GenerateUGFEntityId.GenerateCode();
                    GenerateUGFUIFormId.GenerateCode();
                }

                if (isSuccess)
                {
                    Log.Info($"Luban excel {actionStr} success!");
                }
                else
                {
                    Log.Warning($"Luban excel {actionStr} fail!");
                }
            }

            private static async Task<bool> RunCommand(string cmd, string workDir, string logHeader, bool showInfo)
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

                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (showInfo && !string.IsNullOrEmpty(args.Data))
                        {
                            Log.Info(args.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            isSuccess = false;
                            Log.Warning($"{logHeader} : {args.Data}");
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