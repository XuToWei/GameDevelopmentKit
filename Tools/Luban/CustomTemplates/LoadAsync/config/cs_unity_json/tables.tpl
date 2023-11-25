using Bright.Serialization;
using SimpleJSON;
using System.Collections.Generic;
using System.Threading.Tasks;
{{
    name = x.name
    namespace = x.namespace
    tables = x.tables
}}
{{cs_start_name_space_grace x.namespace}}
public sealed partial class {{name}} : ITables
{
    {{~for table in tables ~}}
    {{~if table.comment != '' ~}}
    /// <summary>
    /// {{table.escape_comment}}
    /// </summary>
    {{~end~}}
    public {{table.full_name}} {{table.name}} { private set; get; }
    {{~end~}}
    private Dictionary<string, IDataTable> _tables;
    public IEnumerable<IDataTable> DataTables => _tables.Values;
    public IDataTable GetDataTable(string tableName) => _tables.TryGetValue(tableName, out var v) ? v : null;

    public async Task LoadAsync(System.Func<string, Task<JSONNode>> loader)
    {
        TablesMemory.BeginRecord();

        _tables = new Dictionary<string, IDataTable>();
        List<Task> loadTasks = new List<Task>();
        {{~for table in tables ~}}
        {{table.name}} = new {{table.full_name}}(() => loader("{{table.output_data_file}}"));
        loadTasks.Add({{table.name}}.LoadAsync());
        _tables.Add("{{table.full_name}}", {{table.name}});
        {{~end~}}

        await Task.WhenAll(loadTasks);

        PostInit();
        {{~for table in tables ~}}
        {{table.name}}.Resolve(_tables); 
        {{~end~}}
        PostResolve();

        TablesMemory.EndRecord();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        {{~for table in tables ~}}
        {{table.name}}.TranslateText(translator); 
        {{~end~}}
    }

    partial void PostInit();
    partial void PostResolve();
}
{{cs_end_name_space_grace x.namespace}}