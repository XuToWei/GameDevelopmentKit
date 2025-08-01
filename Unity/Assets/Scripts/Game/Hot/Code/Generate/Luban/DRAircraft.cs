
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
public sealed partial class DRAircraft : Luban.BeanBase
{
    public DRAircraft(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        ThrusterId = _buf.ReadInt();
        {int n0 = _buf.ReadSize(); WeaponIds = new System.Collections.Generic.List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = _buf.ReadInt(); WeaponIds.Add(_e0);}}
        {int n0 = _buf.ReadSize(); ArmorIds = new System.Collections.Generic.List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = _buf.ReadInt(); ArmorIds.Add(_e0);}}
        DeadEffectId = _buf.ReadInt();
        DeadSoundId = _buf.ReadInt();
        PostInit();
    }

    public static DRAircraft DeserializeDRAircraft(ByteBuf _buf)
    {
        return new DRAircraft(_buf);
    }

    /// <summary>
    /// 战机编号
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 推进器编号
    /// </summary>
    public readonly int ThrusterId;
    /// <summary>
    /// 武器编号0
    /// </summary>
    public readonly System.Collections.Generic.List<int> WeaponIds;
    /// <summary>
    /// 装甲编号0
    /// </summary>
    public readonly System.Collections.Generic.List<int> ArmorIds;
    /// <summary>
    /// 死亡特效编号
    /// </summary>
    public readonly int DeadEffectId;
    /// <summary>
    /// 死亡声音编号
    /// </summary>
    public readonly int DeadSoundId;
    public const int __ID__ = 1010378180;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(TablesComponent tables)
    {
        PostResolveRef();
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "ThrusterId:" + ThrusterId + ","
        + "WeaponIds:" + Luban.StringUtil.CollectionToString(WeaponIds) + ","
        + "ArmorIds:" + Luban.StringUtil.CollectionToString(ArmorIds) + ","
        + "DeadEffectId:" + DeadEffectId + ","
        + "DeadSoundId:" + DeadSoundId + ","
        + "}";
    }

    partial void PostInit();
    partial void PostResolveRef();
}
}
