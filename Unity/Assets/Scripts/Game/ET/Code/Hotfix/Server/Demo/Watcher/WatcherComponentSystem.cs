namespace ET.Server
{
    [EntitySystemOf(typeof(WatcherComponent))]
    [FriendOf(typeof(WatcherComponent))]
    public static partial class WatcherComponentSystem
    {
        [EntitySystem]
        private static void Awake(this WatcherComponent self)
        {
            string[] localIP = NetworkHelper.GetAddressIPs();
            foreach (var key in Tables.Instance.DTStartProcessConfig.KeyList)
            {
                if (key.Item1 != Options.Instance.StartConfig)
                {
                    continue;
                }
                var startProcessConfig = Tables.Instance.DTStartProcessConfig.Get(key);
                if (!WatcherHelper.IsThisMachine(startProcessConfig.InnerIP, localIP))
                {
                    continue;
                }
                System.Diagnostics.Process process = WatcherHelper.StartProcess(startProcessConfig.Id);
                self.Processes.Add(startProcessConfig.Id, process);
            }
        }
    }
}