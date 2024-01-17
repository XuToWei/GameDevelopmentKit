using Cysharp.Threading.Tasks;

namespace Luban
{
    public interface IDataTable
    {
        UniTask LoadAsync();
    }
}
