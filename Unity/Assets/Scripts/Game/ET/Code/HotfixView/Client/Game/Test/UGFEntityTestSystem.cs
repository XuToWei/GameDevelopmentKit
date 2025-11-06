namespace ET.Client
{
    [FriendOf(typeof(UGFEntityTest))]
    [EntitySystemOf(typeof(UGFEntityTest))]
    public static partial class UGFEntityTestSystem
    {
        [UGFEntitySystem]
        private static void UGFEntityOnShow(this UGFEntityTest self)
        {
            
        }
    }
}