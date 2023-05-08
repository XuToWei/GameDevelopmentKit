using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class TestUI : MonoBehaviour
    {
        [Button]
        public async UniTaskVoid TestCan()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            GameEntry.UI.OpenUIFormAsync(999, cancellationToken : cts.Token).Forget();
            await UniTask.Delay(1000);
            cts.Cancel();
            Debug.Log("XXX" + cts.Token.IsCancellationRequested);
        }
    }
}
