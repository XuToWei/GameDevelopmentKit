using System.Threading.Tasks;

namespace Bright.Serialization
{
    public interface ITables
    {
        Task LoadAsync(System.Func<string, Task<ByteBuf>> loader);
    }
}