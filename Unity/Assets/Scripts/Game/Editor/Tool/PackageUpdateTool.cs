using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using EditorProgress = UnityEditor.Progress;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Game.Editor
{
    public static class PackageUpdateTool
    {
        private const string MENU_PATH = "Game/Tool/UpdateAllPackages";
        private const int MAX_PREVIEW_COUNT = 20;

        private static ListRequest s_ListRequest;
        private static AddAndRemoveRequest s_UpdateRequest;
        private static int s_ProgressId = -1;

        [MenuItem(MENU_PATH)]
        public static void UpdateAllPackages()
        {
            if (s_ListRequest != null || s_UpdateRequest != null)
            {
                Debug.LogWarning("Package update is already running.");
                return;
            }

            s_ProgressId = EditorProgress.Start("Update All Packages", "Checking installed packages...", EditorProgress.Options.Indefinite);
            try
            {
                s_ListRequest = Client.List(false, false);
                EditorApplication.update += PollRequests;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                Finish(EditorProgress.Status.Failed);
            }
        }

        [MenuItem(MENU_PATH, true)]
        private static bool ValidateUpdateAllPackages()
        {
            return !Application.isPlaying && s_ListRequest == null && s_UpdateRequest == null;
        }

        private static void PollRequests()
        {
            if (s_ListRequest != null)
            {
                PollListRequest();
                return;
            }

            if (s_UpdateRequest != null)
            {
                PollUpdateRequest();
            }
        }

        private static void PollListRequest()
        {
            if (!s_ListRequest.IsCompleted)
            {
                return;
            }

            if (s_ListRequest.Status != StatusCode.Success)
            {
                FinishWithError("Can not list packages", s_ListRequest.Error);
                return;
            }

            List<PackageUpdate> updates;
            try
            {
                ReportProgress("Analyzing available updates...");
                updates = CollectUpdates(s_ListRequest.Result);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                Finish(EditorProgress.Status.Failed);
                return;
            }
            s_ListRequest = null;

            if (updates.Count == 0)
            {
                EditorUtility.DisplayDialog("Update All Packages", "All supported packages are up to date.", "OK");
                Finish(EditorProgress.Status.Succeeded);
                return;
            }

            ReportProgress($"Found {updates.Count} package updates. Waiting for confirmation...");
            if (!EditorUtility.DisplayDialog("Update All Packages", BuildPreview(updates), "Update", "Cancel"))
            {
                Finish(EditorProgress.Status.Canceled);
                return;
            }

            string[] packagesToAdd = new string[updates.Count];
            for (int i = 0; i < updates.Count; i++)
            {
                packagesToAdd[i] = updates[i].Identifier;
            }

            try
            {
                ReportProgress($"Resolving and updating {updates.Count} packages...");
                s_UpdateRequest = Client.AddAndRemove(packagesToAdd, Array.Empty<string>());
                Debug.Log($"Start updating {updates.Count} packages. Unity may reload scripts during the update.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                Finish(EditorProgress.Status.Failed);
            }
        }

        private static void PollUpdateRequest()
        {
            if (!s_UpdateRequest.IsCompleted)
            {
                return;
            }

            if (s_UpdateRequest.Status == StatusCode.Success)
            {
                Debug.Log("Update all packages complete.");
                Finish(EditorProgress.Status.Succeeded);
                return;
            }

            FinishWithError("Update packages failed", s_UpdateRequest.Error);
        }

        private static List<PackageUpdate> CollectUpdates(PackageCollection packages)
        {
            Dictionary<string, string> manifestDependencies = LoadManifestDependencies();
            List<PackageUpdate> updates = new List<PackageUpdate>();
            foreach (PackageInfo package in packages)
            {
                if (!package.isDirectDependency)
                {
                    continue;
                }

                if (package.source == PackageSource.Registry)
                {
                    string targetVersion = package.versions?.latestCompatible;
                    if (!string.IsNullOrEmpty(targetVersion) && targetVersion == package.version)
                    {
                        continue;
                    }

                    string identifier = string.IsNullOrEmpty(targetVersion)
                        ? package.name
                        : $"{package.name}@{targetVersion}";
                    string target = string.IsNullOrEmpty(targetVersion) ? "latest compatible" : targetVersion;
                    updates.Add(new PackageUpdate(package.name, identifier, $"{package.name}: {package.version} -> {target}"));
                    continue;
                }

                if (package.source == PackageSource.Git && manifestDependencies.TryGetValue(package.name, out string gitUrl))
                {
                    updates.Add(new PackageUpdate(package.name, gitUrl, $"{package.name}: refresh Git revision"));
                }
            }

            updates.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            return updates;
        }

        private static Dictionary<string, string> LoadManifestDependencies()
        {
            string manifestPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Packages/manifest.json"));
            JObject manifest = JObject.Parse(File.ReadAllText(manifestPath));
            JObject dependencies = manifest["dependencies"] as JObject;
            if (dependencies == null)
            {
                throw new InvalidDataException($"Can not find dependencies in '{manifestPath}'.");
            }

            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (JProperty dependency in dependencies.Properties())
            {
                result.Add(dependency.Name, dependency.Value.Value<string>());
            }
            return result;
        }

        private static string BuildPreview(List<PackageUpdate> updates)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"The following {updates.Count} packages will be updated:");
            builder.AppendLine();
            int previewCount = Math.Min(updates.Count, MAX_PREVIEW_COUNT);
            for (int i = 0; i < previewCount; i++)
            {
                builder.Append("• ").AppendLine(updates[i].Description);
            }
            if (updates.Count > previewCount)
            {
                builder.AppendLine($"... and {updates.Count - previewCount} more packages");
            }
            builder.AppendLine();
            builder.Append("Built-in, local and embedded packages are skipped. Ensure the project is under version control before continuing.");
            return builder.ToString();
        }

        private static void FinishWithError(string message, Error error)
        {
            Debug.LogError($"{message}: {error?.message}");
            Finish(EditorProgress.Status.Failed);
        }

        private static void ReportProgress(string description)
        {
            if (s_ProgressId >= 0 && EditorProgress.Exists(s_ProgressId))
            {
                EditorProgress.SetDescription(s_ProgressId, description);
            }
        }

        private static void Finish(EditorProgress.Status status)
        {
            EditorApplication.update -= PollRequests;
            s_ListRequest = null;
            s_UpdateRequest = null;
            if (s_ProgressId >= 0 && EditorProgress.Exists(s_ProgressId))
            {
                EditorProgress.Finish(s_ProgressId, status);
            }
            s_ProgressId = -1;
        }

        private sealed class PackageUpdate
        {
            public PackageUpdate(string name, string identifier, string description)
            {
                Name = name;
                Identifier = identifier;
                Description = description;
            }

            public string Name { get; }
            public string Identifier { get; }
            public string Description { get; }
        }
    }
}
