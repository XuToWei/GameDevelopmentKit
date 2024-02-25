#if UNITY_EDITOR && ODIN_INSPECTOR
namespace TF_TableList
{
    /// <summary>
    /// 支持一下table一些组件的持久化，例如在一个Filter里输入了搜索词后，下次再打开，默认就填入之前的搜索词
    /// </summary>
    public interface ITablePersistentComponent
    {
        string PersistentContextKey { get; }
        void OnLoadPersistentValue(string value);
        string GetPersistentValue();
    }
}
#endif