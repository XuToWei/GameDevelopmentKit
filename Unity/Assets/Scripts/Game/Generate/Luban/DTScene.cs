
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
public partial class DTScene : IDataTable
{
    private System.Collections.Generic.Dictionary<int, DRScene> _dataMap;
    private System.Collections.Generic.List<DRScene> _dataList;
    private readonly System.Func<Cysharp.Threading.Tasks.UniTask<ByteBuf>> _loadFunc;

    public DTScene(System.Func<Cysharp.Threading.Tasks.UniTask<ByteBuf>> loadFunc)
    {
        _loadFunc = loadFunc;
    }

    public async Cysharp.Threading.Tasks.UniTask LoadAsync()
    {
        ByteBuf _buf = await _loadFunc();
        int n = _buf.ReadSize();
        if(_dataMap == null)
        {
            _dataMap = new System.Collections.Generic.Dictionary<int, DRScene>(n);
            _dataList = new System.Collections.Generic.List<DRScene>(n);
        }
        else
        {
            _dataMap.Clear();
            _dataList.Clear();
        }
        for(int i = n ; i > 0 ; --i)
        {
            DRScene _v;
            _v = global::Game.DRScene.DeserializeDRScene(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public System.Collections.Generic.Dictionary<int, DRScene> DataMap => _dataMap;
    public System.Collections.Generic.List<DRScene> DataList => _dataList;
    public DRScene GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public DRScene Get(int key) => _dataMap[key];
    public DRScene this[int key] => _dataMap[key];

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
