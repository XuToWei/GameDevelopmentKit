using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AgentBridge;
using Newtonsoft.Json.Linq;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// Unity Agent Bridge command for validating and exporting Luban workbooks.
    /// </summary>
    public sealed class LubanCommand : ICommandHandler
    {
        private const string ErrorCode = "LUBAN_ERROR";
        private const int MaxReturnedOutputLength = 60000;

        public string Command => "luban";
        public string Description => "Luban 配置工具：action=validate/export，默认 validate。validate 使用 ExcelExporter Check 模式，只校验不写生成产物；export 生成代码和 bin/json 数据，成功后刷新 Unity AssetDatabase。返回进程输出、耗时和退出码；需要先编译 Kit.sln 生成 Bin/Tool.exe 或 Tool.dll。";
        public string Group => "Game";
        public bool CanDisable => true;
        public CommandBatchMode BatchMode => CommandBatchMode.NotAllowed;

        // Run the external tool off the Unity thread, then marshal result handling back to it.
        public Task<object> ExecuteAsync(JObject @params)
        {
            string action = @params?["action"]?.Value<string>() ?? "validate";
            string format = @params?["format"]?.Value<string>() ?? "bin";
            if (action != "validate" && action != "export")
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Unknown action: {action}. Supported: validate, export");
            }
            if (format != "bin" && format != "json")
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Unknown format: {format}. Supported: bin, json");
            }

            string repositoryRoot = ResolveRepositoryRoot();
            string binDirectory = Path.Combine(repositoryRoot, "Bin");
            TaskScheduler mainThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return Task.Run(() => RunTool(binDirectory, action, format)).ContinueWith<object>(task =>
                Complete(task.GetAwaiter().GetResult(), action, format),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                mainThreadScheduler);
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
                throw new CommandException(ErrorCode,
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

        public JObject ParamsSchema { get; } = JObject.Parse(@"{
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""properties"": {
    ""action"": {
      ""type"": ""string"",
      ""default"": ""validate"",
      ""enum"": [""validate"", ""export""],
      ""description"": ""默认 validate。validate 只运行 Check；export 会写生成代码和数据并刷新 AssetDatabase。""
    },
    ""format"": {
      ""type"": ""string"",
      ""default"": ""bin"",
      ""enum"": [""bin"", ""json""],
      ""description"": ""默认 bin。export=json 使用 cs-simple-json/json；validate 时保留该选项用于对应配置模式。""
    }
  }
}");

        private static ProcessResult RunTool(
            string binDirectory, string action, string format)
        {
            bool isWindows = Application.platform == RuntimePlatform.WindowsEditor;
            string executable = isWindows
                ? Path.Combine(binDirectory, "Tool.exe")
                : "dotnet";
            string toolFile = isWindows
                ? executable
                : Path.Combine(binDirectory, "Tool.dll");
            if (!File.Exists(toolFile))
            {
                throw new CommandException(ErrorCode,
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
                        throw new CommandException(ErrorCode,
                            $"Failed to start Luban tool: {executable}");
                    }

                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();
                    process.WaitForExit();
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
                catch (CommandException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CommandException(ErrorCode,
                        $"Failed to run Luban tool '{executable}': {ex.Message}");
                }
            }
        }

        private static string ResolveRepositoryRoot()
        {
            DirectoryInfo unityProject = Directory.GetParent(Path.GetFullPath(Application.dataPath));
            DirectoryInfo repository = unityProject?.Parent;
            if (repository == null)
            {
                throw new CommandException(ErrorCode,
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
}
