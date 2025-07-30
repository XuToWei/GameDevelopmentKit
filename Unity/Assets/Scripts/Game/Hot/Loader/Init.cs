using Cysharp.Threading.Tasks;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Extension;

namespace Game.Hot
{
    [DisallowMultipleComponent]
    public class Init : MonoBehaviour
    {
        public static Init Instance
        {
            get;
            private set;
        }

        private GameObject m_HotEntryAsset;
        private GameObject m_HotEntryGameObject;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartAsync().Forget();
        }

        private void OnDestroy()
        {
            if (this.m_HotEntryGameObject != null)
            {
                DestroyImmediate(this.m_HotEntryGameObject);
                this.m_HotEntryGameObject = null;
            }
            if (this.m_HotEntryAsset != null)
            {
                GameEntry.Resource.UnloadAsset(this.m_HotEntryAsset);
                this.m_HotEntryAsset = null;
            }
        }

        private async UniTaskVoid StartAsync()
        {
            //AppDomain.CurrentDomain.UnhandledException += (sender, e) => { Log.Error(e.ExceptionObject.ToString()); };
            if (Define.EnableHotfix && GameEntry.CodeRunner.EnableCodeBytesMode)
            {
                await LoadCodeBytesAsync("Game.Hot.Code.dll.bytes");
                await LoadCodeBytesAsync("Game.Hot.Code.pdb.bytes");
            }
            this.m_HotEntryAsset = await GameEntry.Resource.LoadAssetAsync<GameObject>(AssetUtility.GetGameHotAsset("HotEntry.prefab"));
            this.m_HotEntryGameObject = GameObject.Instantiate(this.m_HotEntryAsset, GameEntry.CodeRunner.transform);
        }
        
        private async UniTask<byte[]> LoadCodeBytesAsync(string fileName)
        {
            fileName = AssetUtility.GetGameHotAsset(Utility.Text.Format("Code/{0}", fileName));
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(fileName);
            byte[] bytes = textAsset.bytes;
            GameEntry.Resource.UnloadAsset(textAsset);
            return bytes;
        }
    }
}