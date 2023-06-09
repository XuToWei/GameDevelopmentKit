using System;
using System.Reflection;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    public static class CodeRunnerUtility
    {
        public static bool IsEditorCodeBytesMode()
        {
            CodeRunnerComponent codeRunnerComponent = EntryUtility.GetEntrySceneComponent<CodeRunnerComponent>();
            Type type = typeof(CodeRunnerComponent);
            FieldInfo fieldInfo = type.GetField("m_EditorCodeBytesMode", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)fieldInfo.GetValue(codeRunnerComponent);
        }
    }
}
