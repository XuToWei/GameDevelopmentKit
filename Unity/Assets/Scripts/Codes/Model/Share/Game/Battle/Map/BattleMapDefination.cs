using System;

namespace ET
{
    public enum LineType: byte
    {
        Undefine = 0,
        Vertical,
        Horizontal,
    }

    public enum DirectionType: byte
    {
        Undefine = 0,
        Up,
        Down,
        Left,
        Right,
    }

    public enum GridState: byte
    {
        Undefine = 0,
        Empty,
        Player1,
        Player2,
    }
}
