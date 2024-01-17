using System;
using System.Collections;

namespace ET.Server
{
    public static partial class WatcherHelper
    {
        public static DRStartMachineConfig GetThisMachineConfig()
        {
            string[] localIP = NetworkHelper.GetAddressIPs();
            DRStartMachineConfig startMachineConfig = null;
            foreach (var config in Tables.Instance.DTStartMachineConfig.DataList)
            {
                if (config.StartConfig != Options.Instance.StartConfig)
                {
                    continue;
                }
                if (!WatcherHelper.IsThisMachine(config.InnerIP, localIP))
                {
                    continue;
                }
                startMachineConfig = config;
                break;
            }

            if (startMachineConfig == null)
            {
                throw new Exception("not found this machine ip config!");
            }

            return startMachineConfig;
        }
        
        public static bool IsThisMachine(string ip, string[] localIPs)
        {
            if (ip != "127.0.0.1" && ip != "0.0.0.0" && !((IList) localIPs).Contains(ip))
            {
                return false;
            }
            return true;
        }
        
        public static System.Diagnostics.Process StartProcess(int processId, int createScenes = 0)
        {
            var startProcessConfig = Tables.Instance.DTStartProcessConfig.Get(Options.Instance.StartConfig, processId);
            const string exe = "dotnet";
            string arguments = $"App.dll" + 
                    $" --Process={startProcessConfig.Id}" +
                    $" --AppType=Server" +  
                    $" --StartConfig={Options.Instance.StartConfig}" +
                    $" --Develop={Options.Instance.Develop}" +
                    $" --LogLevel={Options.Instance.LogLevel}" +
                    $" --Console={Options.Instance.Console}";
            Log.Debug($"{exe} {arguments}");
            System.Diagnostics.Process process = ProcessHelper.Run(exe, arguments);
            return process;
        }
    }
}