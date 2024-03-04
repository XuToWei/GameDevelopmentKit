using JetBrains.RiderFlow.Core.Launchers;
using JetBrains.RiderFlow.Core.Logging;
using JetBrains.RiderFlow.Core.ReEditor.Notifications;
using JetBrains.RiderFlow.Core.UI.SearchEverywhere;
using JetBrains.RiderFlow.Core.Utils;
using JetBrains.RiderFlow.Since2019_4.EnhancedHierarchyIntegration;
using UnityEditor;
using UnityEngine;

namespace JetBrains.RiderFlow.Since2019_4
{
    [InitializeOnLoad]
    public class DelayedEntryPoint
    {

        static DelayedEntryPoint()
        {
            LogManager.Instance.Initialize();

            SearchEverywhereWindow.Settings = new SearchWindowSettings();
            ProgressManagerOwner.ProgressManager = new LogProgressManager();
            GameObjectUtils.GlobalObjectIdentifiersToInstanceIDsSlow = GlobalObjectIdsToInstanceIds;

            
            OpenedPrefabPreviewTrackerIntegration.Initialize();
            BackendInstallationProgress.Initialize();
            OnEnable();
        }

        static void GlobalObjectIdsToInstanceIds(GlobalObjectId[] ids, int[] instanceIds)
        {
            var objects = new UnityEngine.Object[ids.Length];
            GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(ids, objects);
            for (int i = 0; i < objects.Length; i++)
            {
                var gameObject = objects[i] as GameObject;
                if (gameObject == null)
                {
                    instanceIds[i] = 0;
                }
                else
                {
                    instanceIds[i] = gameObject.GetInstanceID();
                }
            }
        }
        
        protected static void OnEnable()
        {
            if (!IsPrimaryUnityProcess())
                return;

            Launcher.Run();
        }
        
        private static bool IsPrimaryUnityProcess()
        {
            return true;
        }
    }    
}
