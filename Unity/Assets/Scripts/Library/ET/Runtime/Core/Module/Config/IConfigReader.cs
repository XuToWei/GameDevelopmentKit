using Cysharp.Threading.Tasks;

namespace ET
{
    public interface IConfigReader
    {
        UniTask<byte[]> ReadBytesAsync(string file);
        UniTask<string> ReadTextAsync(string file);
    }
}
