using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Game.Editor
{
    public static class ShellTool
    {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        private static readonly List<string> s_DefaultUnixEnvironmentVars = new List<string>() { "/usr/local/share/dotnet" };
#endif

        public static void Run(string cmd, string workDirectory, string encodingName = null, List<string> environmentVars = null)
        {
            Process process = new();
            try
            {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                string app = "bash";
                string splitChar = ":";
                string arguments = "-c";
                if (environmentVars == null)
                {
                    environmentVars = s_DefaultUnixEnvironmentVars;
                }
                if (string.IsNullOrEmpty(encodingName))
                {
                    encodingName = "UTF-8";
                }
#elif UNITY_EDITOR_WIN
                string app = "cmd.exe";
                string splitChar = ";";
                string arguments = "/c";
                if (string.IsNullOrEmpty(encodingName))
                {
                    encodingName = "GB2312";
                }
#endif
                ProcessStartInfo start = new ProcessStartInfo(app);

                if (environmentVars != null)
                {
                    foreach (string var in environmentVars)
                    {
                        start.EnvironmentVariables["PATH"] += (splitChar + var);
                    }
                }

                process.StartInfo = start;
                start.Arguments = arguments + " \"" + cmd + "\"";
                start.CreateNoWindow = true;
                start.ErrorDialog = true;
                start.UseShellExecute = false;
                start.WorkingDirectory = workDirectory;

                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = false;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = Encoding.GetEncoding(encodingName);
                start.StandardOutputEncoding = encoding;
                start.StandardErrorEncoding = encoding;
                
                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        UnityEngine.Debug.Log(args.Data);
                    }
                };
                
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        UnityEngine.Debug.LogError(args.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.ToString());
            }
            finally
            {
                process.Close();
            }
        }
        
        public static async UniTask RunAsync(string cmd, string workDirectory, string encodingName = null, List<string> environmentVars = null)
        {
            Process process = new();
            try
            {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                string app = "bash";
                string splitChar = ":";
                string arguments = "-c";
                if (environmentVars == null)
                {
                    environmentVars = s_DefaultUnixEnvironmentVars;
                }
                if (string.IsNullOrEmpty(encodingName))
                {
                    encodingName = "UTF-8";
                }
#elif UNITY_EDITOR_WIN
                string app = "cmd.exe";
                string splitChar = ";";
                string arguments = "/c";
                if (string.IsNullOrEmpty(encodingName))
                {
                    encodingName = "GB2312";
                }
#endif
                ProcessStartInfo start = new ProcessStartInfo(app);

                if (environmentVars != null)
                {
                    foreach (string var in environmentVars)
                    {
                        start.EnvironmentVariables["PATH"] += (splitChar + var);
                    }
                }

                process.StartInfo = start;
                start.Arguments = arguments + " \"" + cmd + "\"";
                start.CreateNoWindow = true;
                start.ErrorDialog = true;
                start.UseShellExecute = false;
                start.WorkingDirectory = workDirectory;

                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = Encoding.GetEncoding(encodingName);
                start.StandardOutputEncoding = encoding;
                start.StandardErrorEncoding = encoding;

                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        UnityEngine.Debug.Log(args.Data);
                    }
                };
                
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        UnityEngine.Debug.LogError(args.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await Task.Factory.StartNew(() =>
                {
                    process.WaitForExit();
                });
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.ToString());
            }
            finally
            {
                process.Close();
            }
        }
    }
}
