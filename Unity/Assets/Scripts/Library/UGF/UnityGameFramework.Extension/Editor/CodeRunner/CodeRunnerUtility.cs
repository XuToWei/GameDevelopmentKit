using System;
using System.Diagnostics;

namespace UnityGameFramework.Extension.Editor
{
    public static class CodeRunnerUtility
    {
        public static bool IsEnableEditorCodeBytesMode()
        {
            var content = System.IO.File.ReadAllText(EntryUtility.EntryScenePath);
            string targetString = "  m_EnableEditorCodeBytesMode: ";
            int index = content.IndexOf(targetString, StringComparison.Ordinal) + targetString.Length;
            Debug.Assert(index >= 0);
            return int.Parse(content.Substring(index, 1)) == 1;
        }
    }
}