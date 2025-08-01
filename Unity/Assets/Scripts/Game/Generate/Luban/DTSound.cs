
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;

namespace Game
{
public partial class DTSound : IDataTable
{
    private System.Collections.Generic.Dictionary<int, DRSound> _dataMap;
    private System.Collections.Generic.List<DRSound> _dataList;
    private readonly System.Func<Cysharp.Threading.Tasks.UniTask<ByteBuf>> _loadFunc;

    public DTSound(System.Func<Cysharp.Threading.Tasks.UniTask<ByteBuf>> loadFunc)
    {
        _loadFunc = loadFunc;
    }

    public async Cysharp.Threading.Tasks.UniTask LoadAsync()
    {
        ByteBuf _buf = await _loadFunc();
        int n = _buf.ReadSize();
        if(_dataMap == null)
        {
            _dataMap = new System.Collections.Generic.Dictionary<int, DRSound>(n);
            _dataList = new System.Collections.Generic.List<DRSound>(n);
        }
        else
        {
            _dataMap.Clear();
            _dataList.Clear();
        }
        for(int i = n ; i > 0 ; --i)
        {
            DRSound _v;
            _v = global::Game.DRSound.DeserializeDRSound(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public System.Collections.Generic.Dictionary<int, DRSound> DataMap => _dataMap;
    public System.Collections.Generic.List<DRSound> DataList => _dataList;
    public DRSound GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public DRSound Get(int key) => _dataMap[key];
    public DRSound this[int key] => _dataMap[key];

    public void ResolveRef(TablesComponent tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
        PostResolveRef();
    }


    partial void PostInit();
    partial void PostResolveRef();
}
}
