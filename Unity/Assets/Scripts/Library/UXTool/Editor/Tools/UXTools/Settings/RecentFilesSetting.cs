#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    [Serializable]
    public class RecentFilesSetting
    {
        public List<string> Paths = new List<string>();
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.RecentlySelected + "/RecentFilesSetting", false, -45)]
        public static void Create()
        {
            JsonAssetManager.CreateAssets<RecentFilesSetting>(ThunderFireUIToolConfig.FilesRecentSelectedPath);
        }

        public void Add(string path)
        {
            Paths.Insert(0, path);
            JsonAssetManager.SaveAssets(this);
            OnValueChanged();
        }

        public void Remove(int index)
        {
            if (index >= 0)
            {
                Paths.RemoveAt(index);
            }
            JsonAssetManager.SaveAssets(this);
            OnValueChanged();
        }

        public void Remove(string path)
        {
            var index = Paths.FindIndex(i => i == path);
            if (index >= 0)
            {
                Paths.RemoveAt(index);
            }
            JsonAssetManager.SaveAssets(this);
            OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (RecentFilesWindow.GetInstance() != null)
            {
                RecentFilesWindow.GetInstance().RefreshWindow();
            }
        }
    }
}

#endif
