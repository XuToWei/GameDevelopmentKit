using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework.Resource;
using UnityEngine;

namespace ET
{
    public class KKK : MonoBehaviour
    {
        private int nn = 0;
        private async void OnEnable()
        {
            nn += 100;
            int o = nn + 1;
            Debug.Log(o.GetHashCode());
            
            LoadAssetCallbacks ll = new LoadAssetCallbacks((tempAssetName, asset, duration, userdata) =>
            {
                var cc = o;
                Debug.Log(cc);
            });

            o = nn + 3;
            Debug.Log(o.GetHashCode());
            LoadAssetCallbacks bb = new LoadAssetCallbacks((tempAssetName, asset, duration, userdata) =>
            {
                Debug.Log(o);
            });

            await UniTask.Delay(5000);
            ll.LoadAssetSuccessCallback(null,null,default,default);
            bb.LoadAssetSuccessCallback(null,null,default,default);
        }

        // Start is called before the first frame update
        async void OnEnable2()
        {
            
            UniTaskCompletionSource<int> tcs = new UniTaskCompletionSource<int>();
            tcs.TrySetCanceled();
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
