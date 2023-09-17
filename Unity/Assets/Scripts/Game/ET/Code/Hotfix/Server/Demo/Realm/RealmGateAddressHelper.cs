namespace ET.Server
{
	public static partial class RealmGateAddressHelper
	{
		public static DRStartSceneConfig GetGate(int zone, string account)
		{
			ulong hash = (ulong)account.GetLongHashCode();
			
			var zoneGates = Tables.Instance.DTStartSceneConfig.Gates[zone];
			
			return zoneGates[(int)(hash % (ulong)zoneGates.Count)];
		}
	}
}
