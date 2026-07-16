namespace ET
{
    public class FiberInfo
    {
        public int Id { get; set; }
        public int Zone { get; set; }
        public string SceneType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SchedulerType { get; set; } = string.Empty; // Main/Thread/ThreadPool
        public int EntityCount { get; set; }
        public int ProcessId { get; set; }
    }
}
