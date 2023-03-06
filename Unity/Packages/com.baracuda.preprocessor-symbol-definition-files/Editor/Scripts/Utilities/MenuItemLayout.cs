using UnityEditor;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities
{
    internal static class MenuItemLayout
    {
        [MenuItem("Tools/Preprocessor Symbol Definition Files", priority = 2360)]
        public static void SelectPreprocessorSymbolDefinitionSettings()
        {
            PreprocessorSymbolDefinitionSettings.Select();
        }
    }
}
