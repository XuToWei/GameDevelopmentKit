using QFSW.QC.Utilities;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFSW.QC.Editor
{
    public class QCInspectorBase : UnityEditor.Editor
    {
        const string ROOT_PATH = "Source";
        protected string BannerName => "Banner.png";
        protected Texture2D Banner { get; private set; }

        protected T LoadAssetInSource<T>(string assetName, string root) where T : UnityEngine.Object
        {
            MonoScript src = MonoScript.FromScriptableObject(this);
            string srcPath = AssetDatabase.GetAssetPath(src);
            string dirPath = Path.GetDirectoryName(srcPath);
            string[] pathParts = dirPath.Split(new string[] { root }, StringSplitOptions.None);
            string rootPath = string.Join(root, pathParts.SkipLast()) + root;
            string[] files = Directory.GetFiles(rootPath, assetName, SearchOption.AllDirectories);

            if (files.Length > 0)
            {
                string bannerPath = files[0];
                return AssetDatabase.LoadAssetAtPath<T>(bannerPath);
            }

            return null;
        }

        protected virtual void OnEnable()
        {
            if (!Banner)
            {
                Banner = LoadAssetInSource<Texture2D>(BannerName, ROOT_PATH);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorHelpers.DrawHeader(Banner);
            base.OnInspectorGUI();
        }
    }
}
