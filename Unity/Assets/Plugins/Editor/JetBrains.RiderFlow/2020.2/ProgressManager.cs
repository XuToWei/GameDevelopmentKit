using JetBrains.RiderFlow.Core;
using JetBrains.RiderFlow.Core.ReEditor.Notifications;
using UnityEditor;

namespace JetBrains.RiderFlow.Since2020_2
{
    public class ProgressManager : IProgressManager
    {
        public static readonly ProgressManager Instance = new ProgressManager();
        private int myProgressesCount;
        private int myRootProgressId = -1;

        public int CreateProgress(string name, string description = null, bool isIndefinite = false)
        {
            var rootProgressId = CreateOrGetRootProgressId();
            var options = isIndefinite ? Progress.Options.Indefinite : Progress.Options.None;
            var id = Progress.Start(name, description, options, rootProgressId);
            myProgressesCount++;
            return id;
        }

        public void ReportProgress(int id, float progressValue, string description)
        {
            Progress.Report(id, progressValue, description);
        }

        public void FinishProgress(int id)
        {
            Progress.Finish(id);
            myProgressesCount--;
            if (myProgressesCount != 0) return;
            Progress.Finish(myRootProgressId);
        }

        private int CreateOrGetRootProgressId()
        {
            if (myProgressesCount == 0) myRootProgressId = Progress.Start(RiderFlowPaths_Generated.PACKAGE_NAME);
            return myRootProgressId;
        }
        
    }
}