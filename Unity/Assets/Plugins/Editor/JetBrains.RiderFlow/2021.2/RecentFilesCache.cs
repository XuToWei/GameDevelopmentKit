using System.Collections.Generic;
using JetBrains.RiderFlow.Core.Services.Caches.RecentFiles;
using UnityEditor;
using UnityEngine;

namespace JetBrains.RiderFlow.Since2021_2
{
    [FilePath("Library/SearchCache/recentFiles.cache", FilePathAttribute.Location.ProjectFolder)]
    public class RecentFilesCache : ScriptableSingleton<RecentFilesCache>, IRecentFilePersistentCache
    {
        private const int ShowRecentFilesCount = 10;

        [SerializeField]
        private List<string> recentFilesPaths;
        
        public RecentFilesCache()
        {
            recentFilesPaths = new List<string>();
        }

        public void AddRecentFile(string path)
        {
            if (recentFilesPaths.Contains(path)) recentFilesPaths.Remove(path);
            recentFilesPaths.Add(path);
            if (recentFilesPaths.Count > ShowRecentFilesCount) recentFilesPaths.RemoveAt(0);
            Save(true);
        }

        public IEnumerable<string> RecentFiles
        {
            get
            {
                VerifyCache();
                return recentFilesPaths.ToArray();
            }
        }

        private void VerifyCache()
        {
            for (var i = 0; i < recentFilesPaths.Count; i++)
            {
                var guid = AssetDatabase.AssetPathToGUID(recentFilesPaths[i]);
                if (guid == string.Empty)
                {
                    recentFilesPaths.RemoveAt(i);
                }
            }
        }
    }
}