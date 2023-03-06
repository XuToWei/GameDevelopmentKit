using System;
using System.Collections.Generic;
using System.Linq;
using Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts
{
    /// <summary>
    /// Instances of this class will manage and store individual preprocessor definition symbols.
    /// </summary>
    [CreateAssetMenu(menuName = "Preprocessor Definition", fileName = "Preprocessor-Definition", order = 89)]
    public sealed class PreprocessorSymbolDefinitionFile : ScriptableObject, IDisposable, ISerializationCallbackReceiver, IPreprocessorSymbolDefinitionFile
    {
        #region State

        [Tooltip("List containing script symbolData definitions")]
        [SerializeField] [HideInInspector] private List<PreprocessorSymbolData> scriptSymbolDefinitions;

        /// <summary>
        /// List contains symbols that will be removed when changes to the object are applied.
        /// </summary>
        [FormerlySerializedAs("symbolToRemove")]
        [SerializeField] [HideInInspector] private List<string> symbolsToRemoveOnSave = new List<string>();

        /// <summary>
        /// List contains symbols of the last applied state of the object. This list is used to determine which symbols
        /// will be added and which symbols will be removed.
        /// </summary>
        [FormerlySerializedAs("symbolCache")]
        [SerializeField] [HideInInspector] private List<string> symbolsToAddOnSave = new List<string>();

        #endregion

        #region Public API

        /// <summary>
        /// Get a collection of preprocessor symbols contained within data objects storing additional information.
        /// </summary>
        public IEnumerable<PreprocessorSymbolData> LocalSymbols =>
            scriptSymbolDefinitions?.Where(x => !string.IsNullOrWhiteSpace(x.Symbol)) ?? Array.Empty<PreprocessorSymbolData>();

        /// <summary>
        /// Returns a collection of 'valid' symbols that will be applied.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetValidPreprocessorDefines() =>
            from localSymbol in LocalSymbols
            where localSymbol.Enabled && localSymbol.IsValid && localSymbol.TargetGroup.HasFlag(PreprocessorDefineUtilities.FlagsBuildTargetCache)
            select localSymbol.Symbol;

        /// <summary>
        /// Get symbols stored in this object and apply them on a global scale.
        /// This will also remove symbols that were deactivated.
        /// </summary>
        public void ApplyPreprocessorDefines()
        {
            UpdateAndValidateSymbolCache();

            // Get a list of all global symbols.
            var oldDefines = PreprocessorDefineUtilities.GetCustomDefinesOfActiveTargetGroup().ToList();

            // Create a new list and add the valid symbols of this object.
            var newDefines = new List<string>(GetValidPreprocessorDefines());

            // Iterate old definitions and remove the once that need to be removed.
            foreach (var symbol in oldDefines)
            {
                if (!symbolsToRemoveOnSave.Contains(symbol))
                {
                    newDefines.Add(symbol);
                }
                else if (PreprocessorDefineUtilities.IsSymbolElevated(symbol))
                {
                    newDefines.Add(symbol);
                }
            }

            // Determine which symbols were added.
            var symbolsToAdd = newDefines.Where(newSymbol => !oldDefines.Contains(newSymbol));

            // ReSharper disable once PossibleMultipleEnumeration
            if (symbolsToAdd.Any() || symbolsToRemoveOnSave.Any())
            {
                // Apply the updated defines on a global scale.
                // ReSharper disable once PossibleMultipleEnumeration
                PreprocessorDefineUtilities.SetCustomDefinesOfActiveTargetGroup(newDefines.RemoveDuplicates(), symbolsToAdd, symbolsToRemoveOnSave);
            }

            // Clear the symbol cache
            symbolsToRemoveOnSave.Clear();
        }

        /// <summary>
        /// Remove all preprocessor defines managed and contained in this object.
        /// </summary>
        public void RemovePreprocessorDefines()
        {
            var currentSymbols = PreprocessorDefineUtilities.GetCustomDefinesOfActiveTargetGroup().ToList();
            var removedSymbols = currentSymbols.Where(symbol => GetValidPreprocessorDefines().Contains(symbol)).ToList();
            var updatedSymbols = currentSymbols.Where(symbol => !GetValidPreprocessorDefines().Contains(symbol)).ToList();

            scriptSymbolDefinitions = null;
            PreprocessorDefineUtilities.SetCustomDefinesOfActiveTargetGroup(updatedSymbols, null, removedSymbols);
        }

        public void RemovePreprocessorSymbol(PreprocessorSymbolData symbol)
        {
            scriptSymbolDefinitions.TryRemove(symbol);
        }

        public void RemovePreprocessorSymbol(string symbol)
        {
            for (short i = 0; i < scriptSymbolDefinitions.Count; i++)
            {
                if (scriptSymbolDefinitions[i].Symbol == symbol)
                {
                    scriptSymbolDefinitions.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Private Logic

        /// <summary>
        /// Remove symbols handled by this file if necessary before it is deleted.
        /// </summary>
        public void Dispose()
        {
            if (PreprocessorSymbolDefinitionSettings.RemoveSymbolsOnDelete)
            {
                RemovePreprocessorDefines();
            }

            PreprocessorSymbolDefinitionSettings.RemoveScriptDefineSymbolFile(this);
        }

        private void UpdateAndValidateSymbolCache()
        {
            foreach (var cachedSymbol in symbolsToAddOnSave)
            {
                if (!GetValidPreprocessorDefines().Contains(cachedSymbol) &&
                    !symbolsToRemoveOnSave.Contains(cachedSymbol))
                {
                    symbolsToRemoveOnSave.Add(cachedSymbol);
                }
            }

            symbolsToAddOnSave.Clear();
            symbolsToAddOnSave = new List<string>(GetValidPreprocessorDefines());
        }

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            PreprocessorSymbolDefinitionSettings.AddScriptDefineSymbolFile(this);
        }

        #endregion
    }
}
