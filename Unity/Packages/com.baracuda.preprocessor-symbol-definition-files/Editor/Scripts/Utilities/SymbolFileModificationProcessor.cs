using UnityEditor;
using UnityEngine;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.AssetProcessor
{
    /// <summary>
    /// AssetModificationProcessor responsible to call a the 'Dispose' method on deleted Preprocessor-Symbol-Definition-Files
    /// </summary>
    internal class SymbolFileModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (options != RemoveAssetOptions.MoveAssetToTrash)
            {
                return AssetDeleteResult.DidNotDelete;
            }

            AssetDatabase.Refresh();

            if (!PreprocessorSymbolDefinitionSettings.RemoveSymbolsOnDelete)
            {
                return AssetDeleteResult.DidNotDelete;
            }

            if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) is PreprocessorSymbolDefinitionFile scriptDefinitionSymbolFile)
            {
                // Call the custom Dispose method on the object.
                scriptDefinitionSymbolFile.Dispose();
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}