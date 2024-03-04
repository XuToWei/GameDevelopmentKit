using JetBrains.RiderFlow.Core.UI.SearchEverywhere;
using UnityEditor;
using UnityEngine;

namespace JetBrains.RiderFlow.Since2020_2
{
    [FilePath("Library/SearchCache/searchWindow.cache", FilePathAttribute.Location.ProjectFolder)]
    public class SearchWindowSettings : ScriptableSingleton<SearchWindowSettings>, ISearchWindowSettings
    {
        public bool IsSettingsCached => isSettingsCached;
        [SerializeField] private bool isSettingsCached;
        
        [SerializeField] private string cachedSearchQuery = "";
        [SerializeField] private int cachedTab;
        [SerializeField] private float cacheWidth = 800;
        [SerializeField] private float cacheHeight = 500;
        [SerializeField] private float cacheX;
        [SerializeField] private float cacheY;
        
        public Rect GetCachedSize => new Rect(cacheX, cacheY, cacheWidth, cacheHeight);
        public string GetCachedSearchQuery => cachedSearchQuery ?? "";
        public int GetCachedTab => cachedTab;

        private void SaveCache(string query, int tab, float x, float y, float width, float height)
        {
            cachedSearchQuery = query;
            cachedTab = tab;
            cacheX = x;
            cacheY = y;
            cacheWidth = width;
            cacheHeight = height;
            isSettingsCached = true;
            Save(true);
        }

        public void SaveCache(string query, int tab, Rect size) => SaveCache(query, tab, size.x, size.y, size.width, size.height);
    }
}
