#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThunderFireUITool
{
    // TortoiseSVN Commands: https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-automation.html
    public static class TortoiseSVNLogic
    {
        private const string ClientCommand = "TortoiseProc.exe";

        private static IEnumerable<string> GetSelectedAssetPaths()
        {
            return Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath);
        }


        private static string AssetPathsToContextPaths(IEnumerable<string> assetPaths, bool includeMeta)
        {
            if (!assetPaths.Any())
                return string.Empty;

            return string.Join("*", assetPaths.Select(path => AssetPathToContextPaths(path, includeMeta)));
        }

        private static string AssetPathToContextPaths(string assetPath, bool includeMeta)
        {
            //如果是空地址就处理整个Assets目录
            if (string.IsNullOrEmpty(assetPath))
                return Path.GetDirectoryName(Application.dataPath);

            //是否要加上meta文件一起
            string paths = assetPath;

            if (includeMeta)
                paths += "*" + assetPath + ".meta";

            return paths;
        }

        public static void Update(IEnumerable<string> assetPaths, bool includeMeta, bool wait = false)
        {
            if (!assetPaths.Any())
                return;

            string pathsArg = AssetPathsToContextPaths(assetPaths, includeMeta);
            if (string.IsNullOrEmpty(pathsArg))
                return;

            Process.Start(ClientCommand, $"/command:update /path:\"{pathsArg}\"");
        }

        public static void CommitSelected()
        {
            Commit(GetSelectedAssetPaths(), true);
        }

        public static void CommitAll()
        {
            Commit(new List<string> { Application.dataPath }, true);
        }

        public static void Commit(IEnumerable<string> assetPaths, bool includeMeta = true, bool wait = false)
        {
            if (!assetPaths.Any())
                return;

            string pathsArg = AssetPathsToContextPaths(assetPaths, includeMeta);
            //UnityEngine.Debug.Log(pathsArg);
            if (string.IsNullOrEmpty(pathsArg))
                return;

            Process.Start(ClientCommand, $"/command:commit /path:\"{pathsArg}\"");

        }

        public static void Add(IEnumerable<string> assetPaths)
        {
            if (!assetPaths.Any())
                return;


            string pathsArg = AssetPathsToContextPaths(assetPaths, true);
            if (string.IsNullOrEmpty(pathsArg))
                return;

            Process.Start(ClientCommand, $"/command:add /path:\"{pathsArg}\"");
        }

        public static void Revert(IEnumerable<string> assetPaths)
        {
            if (!assetPaths.Any())
                return;

            string pathsArg = AssetPathsToContextPaths(assetPaths, false);
            if (string.IsNullOrEmpty(pathsArg))
                return;

            Process.Start(ClientCommand, $"/command:revert /path:\"{pathsArg}\"");
        }

        public static void Resolve(string assetPath, bool wait = false)
        {
            if (System.IO.Directory.Exists(assetPath))
            {
                string pathsArg = AssetPathToContextPaths(assetPath, false);
                if (string.IsNullOrEmpty(pathsArg))
                    return;
                Process.Start(ClientCommand, $"/command:resolve /path:\"{pathsArg}\"");
                return;
            }
            else
            {
                string pathsArg = AssetPathToContextPaths(assetPath, false);
                if (string.IsNullOrEmpty(pathsArg))
                    return;
                Process.Start(ClientCommand, $"/command:conflicteditor /path:\"{pathsArg}\"");
            }
        }

    }
}
#endif