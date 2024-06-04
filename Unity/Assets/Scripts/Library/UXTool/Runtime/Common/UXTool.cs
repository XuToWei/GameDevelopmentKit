using Cysharp.Threading.Tasks;

public static class UXTool
{
    public static async UniTask InitAsync()
    {
        await ResourceManager.InitAsync();
        await LocalizationHelper.InitAsync();
    }

    public static void Clear()
    {
        ResourceManager.Clear();
        LocalizationHelper.Clear();
    }
}