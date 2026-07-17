namespace ET
{
    public class ServerInfo
    {
        public int ProcessId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ServerStatus Status { get; set; }
        public string InnerIP { get; set; } = string.Empty;
        public int InnerPort { get; set; }
        public string OuterIP { get; set; } = string.Empty;
        public int OuterPort { get; set; }
        public int PlayerCount { get; set; }
        public DateTime StartTime { get; set; }
        public double CpuUsage { get; set; }
        public long MemoryUsage { get; set; }
        public long MemoryTotal { get; set; }
    }

    public enum ServerStatus
    {
        Stopped,
        Starting,
        Running,
        Stopping,
        Error
    }
}
