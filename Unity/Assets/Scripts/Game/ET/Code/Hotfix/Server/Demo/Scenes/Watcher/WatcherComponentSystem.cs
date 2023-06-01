using System.Diagnostics;

namespace ET.Server
{
    [FriendOf(typeof(WatcherComponent))]
    public static class WatcherComponentSystem
    {
        public class WatcherComponentAwakeSystem: AwakeSystem<WatcherComponent>
        {
            protected override void Awake(WatcherComponent self)
            {
                WatcherComponent.Instance = self;
            }
        }
    
        public class WatcherComponentDestroySystem: DestroySystem<WatcherComponent>
        {
            protected override void Destroy(WatcherComponent self)
            {
                WatcherComponent.Instance = null;
            }
        }
        
        public static void Start(this WatcherComponent self, int createScenes = 0)
        {
            string[] localIP = NetworkHelper.GetAddressIPs();
            foreach (DRStartProcessConfig startProcessConfig in Tables.Instance.DTStartProcessConfig.DataList)
            {
                if (!WatcherHelper.IsThisMachine(startProcessConfig.InnerIP, localIP))
                {
                    continue;
                }
                Process process = WatcherHelper.StartProcess(startProcessConfig.Id, createScenes);
                self.Processes.Add(startProcessConfig.Id, process);
            }
        }
    }
}