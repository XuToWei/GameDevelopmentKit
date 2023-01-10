using System.Collections.Generic;

namespace ET.Server
{
    public static class RealmGateAddressHelper
    {
        public static DRStartSceneConfig GetGate(int zone)
        {
            List<DRStartSceneConfig> zoneGates = Tables.Instance.DTStartSceneConfig.Gates[zone];

            int n = RandomGenerator.RandomNumber(0, zoneGates.Count);

            return zoneGates[n];
        }
    }
}