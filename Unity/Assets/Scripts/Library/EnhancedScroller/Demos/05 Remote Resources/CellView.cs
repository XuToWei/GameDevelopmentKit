using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
#if UNITY_2017_4_OR_NEWER
using UnityEngine.Networking;
#endif

namespace EnhancedScrollerDemos.RemoteResourcesDemo
{
    public class CellView : EnhancedScrollerCellView
    {
        public Image cellImage;
        public Sprite defaultSprite;

        private Coroutine _loadImageCoroutine;

        public void SetData(Data data)
        {
            _loadImageCoroutine = StartCoroutine(LoadRemoteImage(data));
        }

        public IEnumerator LoadRemoteImage(Data data)
        {
            string path = data.imageUrl;

            Texture2D texture = null;

            // Get the remote texture

            #if UNITY_2020_1_OR_NEWER
                        var webRequest = UnityWebRequestTexture.GetTexture(path);
                        yield return webRequest.SendWebRequest();
                        if (webRequest.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogError("Failed to download image [" + path + "]: " + webRequest.error);
                        }
                        else
                        {
                            texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                        }
            #else
                #if UNITY_2017_4_OR_NEWER
                            var webRequest = UnityWebRequestTexture.GetTexture(path);
                            yield return webRequest.SendWebRequest();
                            if (webRequest.isNetworkError || webRequest.isHttpError)
                            {
                                Debug.LogError("Failed to download image [" + path + "]: " + webRequest.error);
                            }
                            else
                            {
                                texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                            }
                #else
                            WWW www = new WWW(path);
                            yield return www;
                            texture = www.texture;
                #endif
            #endif

            if (texture != null)
            {
                cellImage.sprite = Sprite.Create(texture, new Rect(0, 0, data.imageDimensions.x, data.imageDimensions.y), new Vector2(0, 0), data.imageDimensions.x);
            }
            else
            {
                ClearImage();
            }
        }

        public void ClearImage()
        {
            cellImage.sprite = defaultSprite;
        }

        /// <summary>
        /// Stop the coroutine if the cell is going to be recycled
        /// </summary>
        public void WillRecycle()
        {
            if (_loadImageCoroutine != null)
            {
                StopCoroutine(_loadImageCoroutine);
            }
        }
    }
}
