using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public class KKK : MonoBehaviour
    {
        // Start is called before the first frame update
        async UniTask OnEnable()
        {
            
            UniTaskCompletionSource<int> tcs = new UniTaskCompletionSource<int>();
            tcs.TrySetException(null);
            //StartCoroutine(SetDley(tcs));

            Debug.Log(await tcs.Task);
            
            
            Debug.Log("XXXXXXXXXXXXXX");
            
            
            //var asset2 = await Resources.LoadAsync<TextAsset>("bar").WithCancellation(this.GetCancellationTokenOnDestroy());
        }

        // Update is called once per frame
        IEnumerator SetDley(UniTaskCompletionSource<int> tcs)
        {
            CancellationTokenSource ct = new CancellationTokenSource();
            CancellationTokenRegistration pp = default;
            var kkk = ct.Token.Register(() =>
            {
                Debug.Log("1111111111111111111111111");
                pp.Dispose();
            });
            pp = kkk;
            yield return new WaitForSeconds(2);
            ct.Cancel();
            ct.Token.Register(() =>
            {
                Debug.Log("MMMMMMMMM");
            });
            ct.Cancel();
            //
            //tcs.TrySetException(new Exception());
            Debug.Log(ct.IsCancellationRequested);
        }
    }
}
