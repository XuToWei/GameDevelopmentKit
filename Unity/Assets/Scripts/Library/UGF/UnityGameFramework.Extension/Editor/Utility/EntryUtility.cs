using System;
using System.IO;
using GameFramework;
using GameFramework.Resource;

namespace UnityGameFramework.Extension.Editor
{
    public static class EntryUtility
    {
        public const string EntryPrefabPath = "Assets/Res/GameEntry.prefab";
        public const string LauncherScenePath = "Assets/Launcher.unity";

        public static ResourceMode GetEntryResourceMode()
        {
            using var reader = new StreamReader(EntryPrefabPath);
            for (int i = 0; i < int.MaxValue; i++)
            {
                string line = reader.ReadLine();
                if(line == null)
                    break;
                if (line.Contains("m_ResourceMode", StringComparison.Ordinal))
                {
                    string valueLine = reader.ReadLine();
                    if (valueLine != null)
                    {
                        int idx = valueLine.IndexOf("value: ", StringComparison.Ordinal);
                        if (idx >= 0)
                        {
                            return (ResourceMode)int.Parse(valueLine.Substring(idx + 7).Trim());
                        }
                    }
                    break;
                }
            }
            throw new GameFrameworkException("Failed to parse resource mode from entry scene.");
        }
    }
}
