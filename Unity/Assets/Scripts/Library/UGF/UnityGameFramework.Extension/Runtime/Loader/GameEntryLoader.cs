using System.Collections;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public class GameEntryLoader : MonoBehaviour
    {
        public const string LauncherResourceGroupName = "Launcher";
        public const string LauncherResourceName = "Auto/Launcher";
        public const string GameEntryPrefabAssetPath = "Assets/Res/GameEntry.prefab";

        private const string LauncherResourceSavePathKey = "Setting.LauncherResourceSavePath";
        
        //注意这里需要和SettingComponent里的一致，不能后续修改
        [SerializeField]
        private SettingHelperBase m_SettingHelper;
        [SerializeField]
        private GameObject m_DefaultGameEntryPrefab;
        [SerializeField]
        private bool m_EditorResourceMode;
        [SerializeField]
        private bool m_EnableEditorCodeBytesMode;

        private IEnumerator Start()
        {
            m_EditorResourceMode &= Application.isEditor;
            if (m_EditorResourceMode)
            {
                Instantiate(m_DefaultGameEntryPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                string resourcePath = GetResourcePath();
                if (string.IsNullOrEmpty(resourcePath))
                {
                    Instantiate(m_DefaultGameEntryPrefab, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(resourcePath);
                    yield return assetBundleCreateRequest;
                    if (!assetBundleCreateRequest.isDone)
                    {
                        throw new GameFrameworkException($"GameEntryLoader load asset bundle '{resourcePath}' failure.");
                    }
                    AssetBundleRequest assetBundleRequest = assetBundleCreateRequest.assetBundle.LoadAssetAsync<GameObject>(GameEntryPrefabAssetPath);
                    yield return assetBundleRequest;
                    if (!assetBundleRequest.isDone)
                    {
                        throw new GameFrameworkException($"GameEntryLoader load asset '{GameEntryPrefabAssetPath}' failure.");
                    }
                    Instantiate(assetBundleRequest.asset, Vector3.zero, Quaternion.identity);
                    assetBundleCreateRequest.assetBundle.Unload(false);
                }
            }
#if UNITY_EDITOR
            GameEntry.GetComponent<BaseComponent>().EditorResourceMode = m_EditorResourceMode;
            GameEntry.GetComponent<CodeRunnerComponent>().EnableCodeBytesMode = m_EnableEditorCodeBytesMode;
#endif
            Destroy(this.gameObject);
        }

        private string GetResourcePath()
        {
            if (m_SettingHelper == null)
            {
                throw new GameFrameworkException("Setting helper is invalid.");
            }

            m_SettingHelper.Load();
            return m_SettingHelper.GetString(LauncherResourceSavePathKey);
        }

        public static void SaveResourcePath(string resourcePath)
        {
            SettingComponent settingComponent = GameEntry.GetComponent<SettingComponent>();
            if (settingComponent == null)
            {
                throw new GameFrameworkException("Setting component is invalid.");
            }

            settingComponent.SetString(LauncherResourceSavePathKey, resourcePath);
            settingComponent.Save();
        }
    }
}
