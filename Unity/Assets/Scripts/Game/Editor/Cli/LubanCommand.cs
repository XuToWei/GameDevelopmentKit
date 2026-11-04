using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Unity.Pipeline.Commands;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>Validate and export Luban workbooks using the repository's tool.</summary>
    // Pipeline uses Task return types to await commands and exclude them from synchronous job batches.
#pragma warning disable ET0501
    public static class LubanCommand
    {
        private const string ErrorCode = "LUBAN_ERROR";
        private const string Group = "game.luban";
        private const int MaxReturnedOutputLength = 60000;

        [CliGroupCommand(Group, "运行 Luban Check，只校验不写生成产物；需要先编译 Kit.sln。")]
        public static Task<object> Validate(
            [CliArg("data_format", "配置数据格式：bin 或 json。")] LubanDataFormat format = LubanDataFormat.Bin,
            [CliArg("timeout", "包含外部进程在内的超时秒数。")] double timeoutInSeconds = 600)
        {
            return ExecuteAsync("validate", format, timeoutInSeconds);
        }

        [CliGroupCommand(Group, "导出 Luban 代码与数据；成功后刷新 AssetDatabase 并清理本地化缓存，可能触发编译。需要先编译 Kit.sln。")]
        public static Task<object> Export(
            [CliArg("data_format", "配置数据格式：bin 或 json。")] LubanDataFormat format = LubanDataFormat.Bin,
            [CliArg("timeout", "包含外部进程在内的超时秒数。")] double timeoutInSeconds = 600)
        {
            return ExecuteAsync("export", format, timeoutInSeconds);
        }

        // Pipeline awaits Task results. Keep Unity API access on the captured Unity context.
        private static async Task<object> ExecuteAsync(string action, LubanDataFormat dataFormat, double timeoutInSeconds)
        {
            if (!Enum.IsDefined(typeof(LubanDataFormat), dataFormat))
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "data_format must be bin or json.");
            }
            if (double.IsNaN(timeoutInSeconds) || timeoutInSeconds <= 0)
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "timeout must be positive.");
            }
            string format = dataFormat == LubanDataFormat.Json ? "json" : "bin";
            string repositoryRoot = ResolveRepositoryRoot();
            string binDirectory = Path.Combine(repositoryRoot, "Bin");
            bool isWindows = Application.platform == RuntimePlatform.WindowsEditor;
            ProcessResult result = await Task.Run(() => RunTool(binDirectory, action, format, isWindows, timeoutInSeconds));
            return Complete(result, action, format);
        }

        private static object Complete(ProcessResult result, string action, string format)
        {
            string operation = action == "validate" ? "check" : "export";
            string combined = result.StandardOutput + Environment.NewLine + result.StandardError;
            bool successMarker = combined.IndexOf(
                $"Luban excel {operation} success!", StringComparison.OrdinalIgnoreCase) >= 0;
            bool failureMarker = combined.IndexOf(
                $"Luban excel {operation} fail!", StringComparison.OrdinalIgnoreCase) >= 0;

            if (result.ExitCode != 0 || failureMarker || !successMarker)
            {
                throw new GameCommandException(ErrorCode,
                    $"Luban {action} failed (exitCode={result.ExitCode}). {TruncateOutput(combined)}");
            }

            if (action == "export")
            {
                EditorApplication.delayCall += RefreshAfterExport;
            }

            return new
            {
                action,
                format,
                success = true,
                refreshScheduled = action == "export",
                exitCode = result.ExitCode,
                elapsedMilliseconds = result.ElapsedMilliseconds,
                output = TruncateOutput(combined)
            };
        }

        private static ProcessResult RunTool(
            string binDirectory, string action, string format, bool isWindows, double timeoutInSeconds)
        {
            string executable = isWindows
                ? Path.Combine(binDirectory, "Tool.exe")
                : "dotnet";
            string toolFile = isWindows
                ? executable
                : Path.Combine(binDirectory, "Tool.dll");
            if (!File.Exists(toolFile))
            {
                throw new GameCommandException(ErrorCode,
                    $"Luban tool not found: {toolFile}. Compile Kit.sln first.");
            }

            string customs = action == "validate" ? "Check" : string.Empty;
            if (format == "json")
            {
                customs = customs.Length == 0 ? "Json" : $"{customs},Json";
            }

            string toolArguments = "--AppType=ExcelExporter --Console=1";
            if (customs.Length > 0)
            {
                toolArguments += $" --Customs={customs}";
            }
            string arguments = isWindows
                ? toolArguments
                : $"\"{toolFile}\" {toolArguments}";

            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = arguments,
                WorkingDirectory = binDirectory,
                CreateNoWindow = true,
                ErrorDialog = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    if (!process.Start())
                    {
                        throw new GameCommandException(ErrorCode,
                            $"Failed to start Luban tool: {executable}");
                    }

                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();
                    bool exited = double.IsPositiveInfinity(timeoutInSeconds)
                        ? WaitForExit(process)
                        : process.WaitForExit((int)Math.Min(int.MaxValue, Math.Ceiling(timeoutInSeconds * 1000)));
                    if (!exited)
                    {
                        process.Kill();
                        process.WaitForExit();
                        throw new GameCommandException(ErrorCode, $"Luban {action} timed out after {timeoutInSeconds} seconds.");
                    }
                    Task.WaitAll(outputTask, errorTask);
                    stopwatch.Stop();

                    return new ProcessResult
                    {
                        ExitCode = process.ExitCode,
                        ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                        StandardOutput = outputTask.Result ?? string.Empty,
                        StandardError = errorTask.Result ?? string.Empty
                    };
                }
                catch (GameCommandException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new GameCommandException(ErrorCode,
                        $"Failed to run Luban tool '{executable}': {ex.Message}");
                }
            }
        }

        private static bool WaitForExit(Process process)
        {
            process.WaitForExit();
            return true;
        }

        private static string ResolveRepositoryRoot()
        {
            DirectoryInfo unityProject = Directory.GetParent(Path.GetFullPath(Application.dataPath));
            DirectoryInfo repository = unityProject?.Parent;
            if (repository == null)
            {
                throw new GameCommandException(ErrorCode,
                    $"Cannot resolve repository root from Unity data path '{Application.dataPath}'.");
            }
            return repository.FullName;
        }

        private static void RefreshAfterExport()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            EditorLocalizationTool.Clear();
        }

        private static string TruncateOutput(string output)
        {
            string normalized = (output ?? string.Empty).Trim();
            if (normalized.Length <= MaxReturnedOutputLength)
            {
                return normalized;
            }
            return $"[truncated to last {MaxReturnedOutputLength} characters]{Environment.NewLine}" +
                   normalized.Substring(normalized.Length - MaxReturnedOutputLength);
        }

        private sealed class ProcessResult
        {
            public int ExitCode;
            public long ElapsedMilliseconds;
            public string StandardOutput;
            public string StandardError;
        }
    }

#pragma warning restore ET0501

    public enum LubanDataFormat
    {
        Bin,
        Json
    }
}
