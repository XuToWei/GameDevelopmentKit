using JetBrains.RiderFlow.Core.Launchers;
using JetBrains.RiderFlow.Core.Logging;
using JetBrains.RiderFlow.Core.ReEditor.Notifications;
using JetBrains.RiderFlow.Core.Services.Caches.RecentFiles;
using JetBrains.RiderFlow.Core.UI.SearchEverywhere;
using JetBrains.RiderFlow.Core.Utils;
using JetBrains.RiderFlow.Since2020_2.EnhancedHierarchyIntegration;
using UnityEditor;

namespace JetBrains.RiderFlow.Since2020_2
{
    [InitializeOnLoad]
    public class DelayedEntryPoint
    {

        static DelayedEntryPoint()
        {
            LogManager.Instance.Initialize();

            SearchEverywhereWindow.Settings = SearchWindowSettings.instance;
            RecentFilesCacheController.Cache = RecentFilesCache.instance;
            ProgressManagerOwner.ProgressManager = new ProgressManager();

            GameObjectUtils.GlobalObjectIdentifiersToInstanceIDsSlow = GlobalObjectId.GlobalObjectIdentifiersToInstanceIDsSlow;
            
            OpenedPrefabPreviewTrackerIntegration.Initialize();
            BackendInstallationProgress.Initialize();
            OnEnable();
        }

        protected static void OnEnable()
        {
            if (!IsPrimaryUnityProcess())
                return;

            Launcher.Run();
        }
        
        private static bool IsPrimaryUnityProcess()
        {
            if (AssetDatabase.IsAssetImportWorkerProcess())
                return false;

#if UNITY_2021_1_OR_NEWER
            if (UnityEditor.MPE.ProcessService.level == UnityEditor.MPE.ProcessLevel.Secondary)
                return false;
#elif UNITY_2020_2_OR_NEWER
      if (UnityEditor.MPE.ProcessService.level == UnityEditor.MPE.ProcessLevel.Slave)
        return false;
#elif UNITY_2020_1_OR_NEWER
      if (Unity.MPE.ProcessService.level == Unity.MPE.ProcessLevel.UMP_SLAVE)
        return false;
#endif
            return true;
        }
    }    
}
