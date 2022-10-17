namespace ET
{
    [FriendOf(typeof(PointComponent))]
    public static class PointComponentSystem
    {
       public class AwakeSystem : AwakeSystem<PointComponent, int, int>
       {
           protected override void Awake(PointComponent self, int x, int y)
           {
               self.X = x;
               self.Y = y;
           }
       }
    }
}
