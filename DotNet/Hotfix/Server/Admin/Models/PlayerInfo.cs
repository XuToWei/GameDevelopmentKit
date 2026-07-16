namespace ET
{
    public class PlayerInfo
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime LastLoginTime { get; set; }
        public bool IsBanned { get; set; }
        public string BanReason { get; set; } = string.Empty;
        public DateTime BanExpireTime { get; set; }
        public bool IsOnline { get; set; }
        public int CurrentServerId { get; set; }
    }
}
