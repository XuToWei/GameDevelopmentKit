using System;

namespace ET.Server
{
    [FriendOf(typeof(DBManagerComponent))]
    public static partial class DBManagerComponentSystem
    {
        [EntitySystem]
        private class DBManagerComponentAwakeSystem : AwakeSystem<DBManagerComponent>
        {
            protected override void Awake(DBManagerComponent self)
            {
                DBManagerComponent.Instance = self;
            }
        }

        [EntitySystem]
        private class DBManagerComponentDestroySystem : DestroySystem<DBManagerComponent>
        {
            protected override void Destroy(DBManagerComponent self)
            {
                DBManagerComponent.Instance = null;
            }
        }

        public static DBComponent GetZoneDB(this DBManagerComponent self, int zone)
        {
            DBComponent dbComponent = self.DBComponents[zone];
            if (dbComponent != null)
            {
                return dbComponent;
            }

            DRStartZoneConfig startZoneConfig = Tables.Instance.DTStartZoneConfig.Get(Options.Instance.StartConfig, zone);
            if (startZoneConfig.DBConnection == "")
            {
                throw new Exception($"zone: {zone} not found mongo connect string");
            }

            dbComponent = self.AddChild<DBComponent, string, string, int>(startZoneConfig.DBConnection, startZoneConfig.DBName, zone);
            self.DBComponents[zone] = dbComponent;
            return dbComponent;
        }
    }
}