using Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities;
using System.Collections.Generic;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts
{
    public interface IPreprocessorSymbolDefinitionFile
    {
        /// <summary>
        /// Get a collection of preprocessor symbols contained within data objects storing additional information.
        /// </summary>
        IEnumerable<PreprocessorSymbolData> LocalSymbols { get; }

        /// <summary>
        /// Returns a collection of 'valid' symbols that will be applied.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetValidPreprocessorDefines();

        /// <summary>
        /// Get symbols stored in this object and apply them on a global scale.
        /// This will also remove symbols that were deactivated.
        /// </summary>
        void ApplyPreprocessorDefines();

        /// <summary>
        /// Remove all preprocessor defines managed and contained in this object.
        /// </summary>
        void RemovePreprocessorDefines();

        /// <summary>
        /// Remove the specified symbol.
        /// </summary>
        void RemovePreprocessorSymbol(PreprocessorSymbolData symbol);

        /// <summary>
        /// Remove the specified symbol.
        /// </summary>
        void RemovePreprocessorSymbol(string symbol);
    }
}