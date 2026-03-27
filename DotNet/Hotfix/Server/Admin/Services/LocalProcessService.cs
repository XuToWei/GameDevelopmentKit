using System.Collections.Concurrent;
using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ET
{
    /// <summary>
    /// Manages game server processes on the local machine.
    /// </summary>
    public class LocalProcessService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LocalProcessService> _logger;
        private readonly ConcurrentDictionary<int, Process> _managedProcesses = new();

        public LocalProcessService(IConfiguration configuration, ILogger<LocalProcessService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool StartProcess(int processId)
        {
            var binPath = _configuration["Agent:BinPath"] ?? AppContext.BaseDirectory;
            var appPath = Path.Combine(binPath, "App.dll");

            if (!File.Exists(appPath))
            {
                _logger.LogError("App.dll not found at {Path}", appPath);
                return false;
            }

            var startConfig = Options.Instance.StartConfig;
            var processConfig = Tables.Instance.DTStartProcessConfig.Get(startConfig, processId);
            if (processConfig == null)
            {
                _logger.LogError("Process {ProcessId} not found in StartProcessConfig", processId);
                return false;
            }

            try
            {
                var process = ProcessHelper.Run("dotnet", $"App.dll --Process={processId} --StartConfig={startConfig} --Console=1", binPath);
                if (process != null)
                {
                    _managedProcesses[processId] = process;
                    _logger.LogInformation("Started process {ProcessId} (PID: {Pid})", processId, process.Id);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start process {ProcessId}", processId);
            }

            return false;
        }

        public bool StopProcess(int processId)
        {
            if (!_managedProcesses.TryRemove(processId, out var process))
            {
                _logger.LogWarning("Process {ProcessId} not managed by this Agent", processId);
                return false;
            }

            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                    process.WaitForExit(5000);
                    _logger.LogInformation("Stopped process {ProcessId}", processId);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop process {ProcessId}", processId);
                return false;
            }
        }
    }
}
