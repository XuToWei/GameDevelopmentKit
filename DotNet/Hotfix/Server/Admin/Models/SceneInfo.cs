namespace ET
{
    public class SceneInfo
    {
        public int Id { get; set; }
        public string SceneType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Zone { get; set; }
        public string InnerAddress { get; set; } = string.Empty;
        public string OuterAddress { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public int FiberId { get; set; }
        public int ProcessId { get; set; }
    }
}
