using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using JetBrains.RiderFlow.Core.Launchers;
using JetBrains.RiderFlow.Core.Logging;
using JetBrains.RiderFlow.Core.ReEditor.Notifications;
using JetBrains.RiderFlow.Core.Requirements;
using JetBrains.RiderFlow.Core.Services.Caches.RecentFiles;
using JetBrains.RiderFlow.Core.Threading;
using JetBrains.RiderFlow.Core.UI.SceneIntegration;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.SceneHighlighting;
using JetBrains.RiderFlow.Core.UI.SearchEverywhere;
using JetBrains.RiderFlow.Core.Utils;
using JetBrains.RiderFlow.Since2021_2.EnhancedHierarchyIntegration;
using JetBrains.RiderFlow.Since2021_2.SceneIntegration;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace JetBrains.RiderFlow.Since2021_2
{
    [InitializeOnLoad]
    public class DelayedEntryPoint
    {

        static DelayedEntryPoint()
        {
            LogManager.Instance.Initialize();
            
            SearchEverywhereWindow.Settings = SearchWindowSettings.instance;
            RecentFilesCacheController.Cache = RecentFilesCache.instance;
            ProgressManagerOwner.ProgressManager = ProgressManager.Instance;
            
            GameObjectUtils.GlobalObjectIdentifiersToInstanceIDsSlow = GlobalObjectId.GlobalObjectIdentifiersToInstanceIDsSlow;

            OpenedPrefabPreviewTrackerIntegration.Initialize();
            BackendInstallationProgress.Initialize();
            OnEnable();
            SceneIntegrationSettings.IsClassicToolboxEnabled = false;
        }

        protected static void OnEnable()
        {
            if (!IsPrimaryUnityProcess())
                return;
            
            ContainerReadyRequirement.Instance.IsReady.AdviseUntil(Lifetime.Eternal, v =>
            {
                if (v)
                {
                    MainThreadScheduler.Instance.Queue(StartToolbar);
                    return true;
                }

                return false;
            });

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

        private static void StartToolbar()
        {
            if (SceneView.lastActiveSceneView.TryGetOverlay(RiderFlowToolbarConstants.ToolbarId, out var toolbox))
            {
                SceneIntegrationActions.ChangeToolbarDisplay(new VisualElementToolbarDisplay(toolbox));
                
                if (!SceneIntegrationSettings.AutoShowToolbox)
                    return;
                
                if (toolbox.displayed)
                    return;
                
                toolbox.collapsed = false;
                toolbox.displayed = true;
                toolbox.Undock();
                toolbox.floatingPosition = new Vector2(150, 150);

                UpdateLayout(toolbox);
            }
        }

        private static void UpdateLayout(Overlay toolbox)
        {
            var layoutSetter = typeof(Overlay).GetProperty(nameof(Overlay.layout))?.GetSetMethod(true);
            if (layoutSetter == null)
                return;

            layoutSetter.Invoke(toolbox, new object[] { Layout.HorizontalToolbar });
        }
    }    
}
