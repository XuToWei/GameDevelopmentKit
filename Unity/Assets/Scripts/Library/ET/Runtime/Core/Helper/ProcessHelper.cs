using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;

namespace ET
{
    public static class ProcessHelper
    {
        public static Process Run(string exe, string arguments, string workingDirectory = ".")
        {
            //Log.Debug($"Process Run exe:{exe} ,arguments:{arguments} ,workingDirectory:{workingDirectory}");
            try
            {
                bool redirectStandardOutput = true;
                bool redirectStandardError = true;
                bool useShellExecute = false;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    redirectStandardOutput = false;
                    redirectStandardError = false;
                    useShellExecute = true;
                }

                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = useShellExecute,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = redirectStandardOutput,
                    RedirectStandardError = redirectStandardError,
                };
                
                Process process = Process.Start(info);
                return process;
            }
            catch (Exception e)
            {
                throw new Exception($"dir: {Path.GetFullPath(workingDirectory)}, command: {exe} {arguments}", e);
            }
        }
        
        public static async UniTask<Process> RunAsync(string exe, string arguments, string workingDirectory = ".")
        {
            //Log.Debug($"Process Run exe:{exe} ,arguments:{arguments} ,workingDirectory:{workingDirectory}");
            try
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                Process process = Process.Start(info);
                if (!process.HasExited)
                {
                    return process;
                }

                try
                {
                    process.EnableRaisingEvents = true;
                }
                catch (InvalidOperationException)
                {
                    if (process.HasExited)
                    {
                        return process;
                    }
                    throw;
                }

                var tcs = AutoResetUniTaskCompletionSource<bool>.Create();

                void Handler(object s, EventArgs e) => tcs.TrySetResult(true);
            
                process.Exited += Handler;

                try
                {
                    if (process.HasExited)
                    {
                        return process;
                    }
                    await tcs.Task;
                }
                finally
                {
                    process.Exited -= Handler;
                }
                return process;
            }
            catch (Exception e)
            {
                throw new Exception($"dir: {Path.GetFullPath(workingDirectory)}, command: {exe} {arguments}", e);
            }
        }
    }
}