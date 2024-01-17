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
            foreach (var startProcessConfig in Tables.Instance.DTStartProcessConfig.DataList)
            {
                if (startProcessConfig.StartConfig != Options.Instance.StartConfig)
                {
                    continue;
                }
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