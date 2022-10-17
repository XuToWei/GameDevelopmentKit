namespace ET
{
    [FriendOf(typeof(GridComponent))]
    public static class GridComponentSystem
    {
        public class AwakeSystem : AwakeSystem<GridComponent, int ,int>
        {
            protected override void Awake(GridComponent self, int x, int y)
            {
                self.X = x;
                self.Y = y;
                self.AroundLineNoDict.Clear();
            }
        }
    }
}