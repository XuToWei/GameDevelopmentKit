using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public static partial class Entry
    {
        public static void Start()
        {
            Log.Info("Game.Hot.Code Start!");
            StartAsync().Forget();
        }

        private static async UniTask StartAsync()
        {
            await Tables.Instance.LoadAllAsync();
            Log.Info("Game.Hot.Code Load Config!");
        }
    }
}