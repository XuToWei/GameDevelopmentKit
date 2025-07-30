namespace Game
{
    public static class GLog
    {
        public static LogWithTag Entity { get; } = new LogWithTag("<color=#ff0000>Entity:</color>"); // 红色
        public static LogWithTag Game { get; } = new LogWithTag("<color=#0066ff>Game:</color>"); // 蓝色
        public static LogWithTag Procedure { get; } = new LogWithTag("<color=#00ff00>Procedure:</color>"); // 绿色
        public static LogWithTag Resource { get; } = new LogWithTag("<color=#ff6600>Resource:</color>"); // 橙色
        public static LogWithTag Scene { get; } = new LogWithTag("<color=#9900ff>Scene:</color>"); // 紫色
        public static LogWithTag Sound { get; } = new LogWithTag("<color=#00ffff>Sound:</color>"); // 青色
        public static LogWithTag UI { get; } = new LogWithTag("<color=#ffff00>UI:</color>"); // 黄色
    }
}
