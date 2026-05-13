using System;
using System.Diagnostics;
using System.IO;
using GameFramework;
using GameFramework.Resource;

namespace UnityGameFramework.Extension.Editor
{
    public static class CodeRunnerUtility
    {
        public static bool IsEnableEditorCodeBytesMode()
        {
            using var reader = new StreamReader(EntryUtility.LauncherScenePath);
            string targetString = "m_EnableEditorCodeBytesMode: ";
            for (int i = 0; i < int.MaxValue; i++)
            {
                string line = reader.ReadLine();
                if(line == null)
                    break;
                if (line.Contains(targetString, StringComparison.Ordinal))
                {
                    int index = line.IndexOf(targetString, StringComparison.Ordinal) + targetString.Length;
                    Debug.Assert(index >= 0);
                    return int.Parse(line.Substring(index, 1)) == 1;
                }
            }
            throw new GameFrameworkException("Failed to parse EnableEditorCodeBytesMode from entry scene.");
        }
    }
}