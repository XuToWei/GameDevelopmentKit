using System.Collections.Generic;

namespace ET
{
    [FriendOf(typeof(BattleMapComponent))]
    public static class MapComponentSystem
    {
        public class AwakeSystem : AwakeSystem<BattleMapComponent, int ,int>
        {
            protected override void Awake(BattleMapComponent self, int x, int y)
            {
                self.XNum = x;
                self.YNum = y;
                
                self.PointComponentDict.Clear();
                for (x = 0; x < self.XNum; x++)
                {
                    for (y = 0; y < self.YNum; y++)
                    {
                        PointComponent pointComponent = self.AddChild<PointComponent, int, int>(x, y);
                        self.PointComponentDict.Add((x, y), pointComponent);
                    }
                }
                self.GridComponentDict.Clear();
                for (x = 0; x < self.XNum; x++)
                {
                    for (y = 0; y < self.YNum; y++)
                    {
                        GridComponent gridComponent = self.AddChild<GridComponent, int, int>(x, y);
                        self.GridComponentDict.Add((x, y), gridComponent);
                    }
                }

                int lineNo = 0;
                self.LineComponentDict.Clear();

                void AddLineComponent(int x1, int y1, int x2, int y2)
                {
                    LineComponent lineComponent = self.AddChild<LineComponent, int, (int, int, int, int)>(lineNo, (x1, y1, x2, y2));
                    self.LineComponentDict.Add(lineNo, lineComponent);
                    if (lineComponent.LineType == LineType.Vertical)
                    {
                        GridComponent rightGrid = self.GetGridComponent(x1, y1);
                        if (rightGrid != null)
                        {
                            rightGrid.AroundLineNoDict.Add(DirectionType.Left , lineNo);
                        }
                        GridComponent leftGrid = self.GetGridComponent(x1 - 1, y1);
                        if (leftGrid != null)
                        {
                            leftGrid.AroundLineNoDict.Add(DirectionType.Right , lineNo);
                        }
                    }
                    lineNo++;
                }

                for (x = 0; x < self.XNum - 1; x++)
                {
                    for (y = 0; y < self.YNum - 1; y++)
                    {
                        AddLineComponent(x, y, x, y + 1);
                        AddLineComponent(x, y, x + 1, y);
                    }
                }
            }
        }

        public static LineComponent GetLineComponent(this BattleMapComponent component, int lineNo)
        {
            if (component.LineComponentDict.TryGetValue(lineNo, out LineComponent lineComponent))
            {
                return lineComponent;
            }
            return default;
        }

        public static PointComponent GetPointComponent(this BattleMapComponent component, int x, int y)
        {
            if (component.PointComponentDict.TryGetValue((x, y), out PointComponent pointComponent))
            {
                return pointComponent;
            }
            return default;
        }

        public static GridComponent GetGridComponent(this BattleMapComponent component, int x, int y)
        {
            if (component.GridComponentDict.TryGetValue((x, y), out GridComponent gridComponent))
            {
                return gridComponent;
            }
            return default;
        }

        public static bool CheckCanSurroundedGrid(this BattleMapComponent component, GridComponent gridComponent)
        {
            bool CheckLine(DirectionType directionType)
            {
                if (gridComponent.AroundLineNoDict.TryGetValue(directionType, out var lineNo))
                {
                    LineComponent lineComponent = component.GetLineComponent(lineNo);
                    if (lineComponent != null && lineComponent.IsConnected)
                        return true;
                }
                return false;
            }

            return CheckLine(DirectionType.Up) && CheckLine(DirectionType.Down) && CheckLine(DirectionType.Left) && CheckLine(DirectionType.Right);
        }

        public static (GridComponent, GridComponent) GetAroundGridComponents(this BattleMapComponent component, LineComponent lineComponent)
        {
            if (lineComponent.LineType == LineType.Vertical)
            {
                return (component.GetGridComponent(lineComponent.X1 - 1, lineComponent.Y1),
                    component.GetGridComponent(lineComponent.X1, lineComponent.Y1));
            }
            else if (lineComponent.LineType == LineType.Horizontal)
            {
                return (component.GetGridComponent(lineComponent.X1, lineComponent.Y1 - 1),
                    component.GetGridComponent(lineComponent.X1, lineComponent.Y1));
            }
            else
            {
                return default;
            }
        }
    }
}
