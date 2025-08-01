
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
public partial class DTAsteroid : IDataTable
{
    private System.Collections.Generic.Dictionary<int, DRAsteroid> _dataMap;
    private System.Collections.Generic.List<DRAsteroid> _dataList;
    private readonly System.Func<Cysharp.Threading.Tasks.UniTask<ByteBuf>> _loadFunc;

    public DTAsteroid(System.Func<Cysharp.Threading.Tasks.UniTask<ByteBuf>> loadFunc)
    {
        _loadFunc = loadFunc;
    }

    public async Cysharp.Threading.Tasks.UniTask LoadAsync()
    {
        ByteBuf _buf = await _loadFunc();
        int n = _buf.ReadSize();
        if(_dataMap == null)
        {
            _dataMap = new System.Collections.Generic.Dictionary<int, DRAsteroid>(n);
            _dataList = new System.Collections.Generic.List<DRAsteroid>(n);
        }
        else
        {
            _dataMap.Clear();
            _dataList.Clear();
        }
        for(int i = n ; i > 0 ; --i)
        {
            DRAsteroid _v;
            _v = global::Game.Hot.DRAsteroid.DeserializeDRAsteroid(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public System.Collections.Generic.Dictionary<int, DRAsteroid> DataMap => _dataMap;
    public System.Collections.Generic.List<DRAsteroid> DataList => _dataList;
    public DRAsteroid GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public DRAsteroid Get(int key) => _dataMap[key];
    public DRAsteroid this[int key] => _dataMap[key];

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
