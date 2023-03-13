namespace QFSW.QC
{
    public interface ILogQueue
    {
        int MaxStoredLogs { get; set; }
        bool IsEmpty { get; }

        void QueueLog(ILog log);
        bool TryDequeue(out ILog log);
        void Clear();
    }
}