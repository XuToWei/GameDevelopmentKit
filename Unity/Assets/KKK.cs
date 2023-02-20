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

            StartCoroutine(SetDley(tcs));

            Debug.Log(await tcs.Task);
            
            
            Debug.Log("XXXXXXXXXXXXXX");
            
            
            //var asset2 = await Resources.LoadAsync<TextAsset>("bar").WithCancellation(this.GetCancellationTokenOnDestroy());
        }

        // Update is called once per frame
        IEnumerator SetDley(UniTaskCompletionSource<int> tcs)
        {
            CancellationTokenSource ct = new CancellationTokenSource();
            var kkk = ct.Token.Register(() =>
            {
                Debug.Log("1111111111111111111111111");
            });
            yield return new WaitForSeconds(2);
            //ct.Cancel();
            //kkk.Dispose();
            tcs.TrySetException(new Exception());
            Debug.Log(ct.IsCancellationRequested);
        }
    }
}
