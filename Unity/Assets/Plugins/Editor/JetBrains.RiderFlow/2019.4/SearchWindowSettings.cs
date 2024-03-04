using JetBrains.RiderFlow.Core.UI.SearchEverywhere;
using UnityEngine;

namespace JetBrains.RiderFlow.Since2019_4
{
    public class SearchWindowSettings : ISearchWindowSettings
    {
        private string mySearchValue = "";
        private int myTab = 0;
        private Rect myPosition = new Rect(0, 0, 800, 500);
        private bool myIsSettignsCached = false;
        
        public void SaveCache(string searchFieldValue, int chosenTab, Rect position)
        {
            mySearchValue = searchFieldValue;
            myTab = chosenTab;
            myPosition = position;
            myIsSettignsCached = true;
        }

        public Rect GetCachedSize => myPosition;
        public string GetCachedSearchQuery => mySearchValue;
        public bool IsSettingsCached => myIsSettignsCached;
        public int GetCachedTab => myTab;
    }
}