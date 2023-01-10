using System.Threading.Tasks;

namespace Bright.Serialization
{
    public interface IDataTable
    {
        Task LoadAsync();
    }
}
