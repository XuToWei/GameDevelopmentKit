using System;

namespace ET.Server
{
    /// <summary>
    /// Helper to send status reports to Admin process.
    /// Discovers Admin process ID from Luban StartSceneConfig at runtime.
    /// </summary>
    public static class S2Admin_StatusReportHelper
    {
        /// <summary>
        /// Send a process status report to the Admin process.
        /// Silently fails if Admin is not configured or not reachable.
        /// </summary>
        public static void SendStatusReport(Scene mainScene, int status)
        {
            try
            {
                var adminConfig = Tables.Instance.DTStartSceneConfig.AdminConfig;
                if (adminConfig == null)
                {
                    return;
                }

                var report = S2Admin_ProcessStatusReport.Create();
                report.ProcessId = Options.Instance.Process;
                report.Status = status;
                report.MemoryUsage = GC.GetTotalMemory(false);
                report.FiberCount = FiberManager.Instance.Count();

                var adminActorId = new ActorId(adminConfig.Process, ConstFiberId.Admin);
                mainScene.GetComponent<ProcessInnerSender>().Send(adminActorId, report);
            }
            catch
            {
                // Admin not reachable, silently ignore
            }
        }
    }
}
