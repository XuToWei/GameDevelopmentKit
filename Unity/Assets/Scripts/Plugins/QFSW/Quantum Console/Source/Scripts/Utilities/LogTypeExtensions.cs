using UnityEngine;

namespace QFSW.QC
{
    public static class LogTypeExtensions
    {
        public static LoggingThreshold ToLoggingThreshold(this LogType logType)
        {
            LoggingThreshold severity = LoggingThreshold.Always;
            switch (logType)
            {
                case LogType.Exception: severity = LoggingThreshold.Exception; break;
                case LogType.Error: severity = LoggingThreshold.Error; break;
                case LogType.Assert: severity = LoggingThreshold.Error; break;
                case LogType.Warning: severity = LoggingThreshold.Warning; break;
                case LogType.Log: severity = LoggingThreshold.Always; break;
            }

            return severity;
        }
    }
}
