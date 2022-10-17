using System.Collections.Generic;
using cfg;
using cfg.ET;

namespace ET.Server
{
	public static class RealmGateAddressHelper
	{
		public static DRStartSceneConfig GetGate(int zone)
		{
			List<DRStartSceneConfig> zoneGates = DataTables.Instance.DTStartSceneConfig.Gates[zone];
			
			int n = RandomGenerator.RandomNumber(0, zoneGates.Count);

			return zoneGates[n];
		}
	}
}
