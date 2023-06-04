using System.Collections.Generic;


namespace ET.Server
{
	public static partial class RealmGateAddressHelper
	{
		public static DRStartSceneConfig GetGate(int zone, string account)
		{
			long hash = account.GetLongHashCode();
			
			List<DRStartSceneConfig> zoneGates = Tables.Instance.DTStartSceneConfig.Gates[zone];
			
			return zoneGates[(int)(hash % zoneGates.Count)];
		}
	}
}
