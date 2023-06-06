    using System;
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;

    namespace MultiPlay
{
    [InitializeOnLoad]
    public class SceneChangeDetector
    {
        private static FileSystemWatcher watcher;
        public static event Action<string> SceneChanged;

        static SceneChangeDetector()
        {
            string projectPath = Directory.GetParent(Application.dataPath)?.FullName;

            watcher = new FileSystemWatcher(projectPath);
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileChanged;
            watcher.Deleted += OnFileChanged;
            watcher.Renamed += OnFileRenamed;
        }

        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.EndsWith(".unity"))
            {
                SceneChanged?.Invoke(e.Name);
            }
        }

        private static void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (e.FullPath.EndsWith(".unity"))
            {
                SceneChanged?.Invoke(e.Name);
            }
        }
    }

}