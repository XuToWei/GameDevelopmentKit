using System;
using System.Reflection;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    public static class CodeRunnerUtility
    {
        public static bool IsEnableEditorCodeBytesMode()
        {
            CodeRunnerComponent codeRunnerComponent = EntryUtility.GetEntrySceneComponent<CodeRunnerComponent>();
            if (codeRunnerComponent == null)
                return false;
            Type type = typeof(CodeRunnerComponent);
            FieldInfo fieldInfo = type.GetField("m_EnableEditorCodeBytesMode", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)fieldInfo.GetValue(codeRunnerComponent);
        }
    }
}
