
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;

namespace Game.Hot
{
public partial struct vec2
{
    public vec2(ByteBuf _buf) 
    {
        X = _buf.ReadFloat();
        Y = _buf.ReadFloat();
        PostInit();
    }

    public static vec2 Deserializevec2(ByteBuf _buf)
    {
        return new vec2(_buf);
    }

    public readonly float X;
    public readonly float Y;

    public  void ResolveRef(TablesComponent tables)
    {
        PostResolveRef();
    }

    public override string ToString()
    {
        return "{ "
        + "x:" + X + ","
        + "y:" + Y + ","
        + "}";
    }

    partial void PostInit();
    partial void PostResolveRef();
}
}
