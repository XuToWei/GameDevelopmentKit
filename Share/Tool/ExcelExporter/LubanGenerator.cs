using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ET
{
    public static class LubanGenerator
    {
        private const string CommandDir = "../Develop/Excel/";
        private const string CommandBinName = "gen";
        private const string CommandTestJsonName = "gen_json";

        /// <summary>
        /// 生成luban配置
        /// </summary>
        public static void Generate()
        {
            //如果要测试更换成CommandTestJsonName
            string commandName = CommandBinName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RunGenerateCommandNormal($"{commandName}.bat");
            }
            else
            {
                RunGenerateCommandNormal($"{commandName}.sh");
            }
        }
        
        private static void RunGenerateCommandNormal(string fileName)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            string workDirectory = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, CommandDir);
            psi.WorkingDirectory = workDirectory;
            psi.FileName = Path.Combine(workDirectory, fileName);
            psi.CreateNoWindow = false;
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            Process process = new Process();
            process.StartInfo = psi;
            process.Start();
        }
    }
}