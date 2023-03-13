using System.Collections.Generic;

namespace QFSW.QC
{
    public interface ILogStorage
    {
        int MaxStoredLogs { get; set; }
        IReadOnlyList<ILog> Logs { get; }

        void AddLog(ILog log);
        void RemoveLog();
        void Clear();

        string GetLogString();
    }

}