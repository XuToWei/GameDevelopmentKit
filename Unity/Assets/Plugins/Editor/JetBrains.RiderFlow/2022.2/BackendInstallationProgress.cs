using JetBrains.RiderFlow.Core.Infrastructure;
using JetBrains.RiderFlow.Core.Infrastructure.Tools;
using JetBrains.RiderFlow.Core.ReEditor.Notifications;
using JetBrains.RiderFlow.Core.Threading;

namespace JetBrains.RiderFlow.Since2022_2
{
    public static class BackendInstallationProgress
    {
        public static void Initialize()
        {
            BackendDownloadProgressManager.Create = (name) => new BackendDownloadProgress(name);
            BackendExtractProgressManager.Create = (name) => new BackendExtractProgress(name);
        }

        private class BackendDownloadProgress : DownloadTools.IDownloadProgress
        {
            private readonly string myName;
            private ProgressCookie myProgressCookie;

            public BackendDownloadProgress(string name)
            {
                myName = name;
            }

            public void Start()
            {
                MainThreadScheduler.Instance.Queue(() =>
                    myProgressCookie = new ProgressCookie($"Downloading {myName}", ""));
            }

            public void UpdateOnEachDownloadedMegabyte(int downloadedMegabytes, int totalMegabytes,
                                                       int downloadSpeedInMegabytesPerSecond)
            {
                MainThreadScheduler.Instance.Queue(() =>
                    UpdateProgress(downloadedMegabytes, totalMegabytes, downloadSpeedInMegabytesPerSecond));
            }

            public void Finish()
            {
                MainThreadScheduler.Instance.Queue(() => myProgressCookie.Dispose());
            }

            private void UpdateProgress(int downloadedMegabytes, int totalMegabytes, int downloadSpeedInMegabytes)
            {
                myProgressCookie?.UpdateProgress(
                    $"Downloaded {downloadedMegabytes}/{totalMegabytes} MB. Speed is {downloadSpeedInMegabytes} MB/s.",
                    (float)downloadedMegabytes / totalMegabytes);
            }
        }

        private class BackendExtractProgress : ExtractTools.IExtractProgress
        {
            private readonly string myName;
            private ProgressCookie myProgressCookie;

            public BackendExtractProgress(string name)
            {
                myName = name;
            }

            public void Start()
            {
                MainThreadScheduler.Instance.Queue(() =>
                    myProgressCookie = new ProgressCookie($"Installing {myName}", ""));
            }

            public void OnEachExtractedFile(int extractedFilesCount, int totalExtractedFiles)
            {
                MainThreadScheduler.Instance.Queue(() => UpdateProgress(extractedFilesCount, totalExtractedFiles));
            }

            public void Finish()
            {
                MainThreadScheduler.Instance.Queue(() => myProgressCookie.Dispose());
            }

            private void UpdateProgress(int extractedFilesCount, int totalExtractedFiles)
            {
                myProgressCookie?.UpdateProgress($"Extracted {extractedFilesCount}/{totalExtractedFiles} files",
                    (float)extractedFilesCount / totalExtractedFiles);
            }
        }
    }
}