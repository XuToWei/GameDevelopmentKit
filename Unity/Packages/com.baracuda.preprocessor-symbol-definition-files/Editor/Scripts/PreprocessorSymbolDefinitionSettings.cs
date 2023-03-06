using System.Collections.Generic;
using System.Linq;
using Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts
{
    /// <summary>
    /// Settings file managing and elevated symbols and options regarding definition files.
    /// </summary>
    public sealed class PreprocessorSymbolDefinitionSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Data

        [SerializeField] private bool removeSymbolsOnDelete = true;
        [SerializeField] private bool logMessages = true;
        [SerializeField] private bool showAllDefinedSymbols = true;

        [SerializeField] private List<string> elevatedSymbols = new List<string>();


        private const string FilenameAsset = "Preprocessor-Definition-Settings.asset";
        private static readonly List<PreprocessorSymbolDefinitionFile> scriptDefineSymbolFiles = new List<PreprocessorSymbolDefinitionFile>(8);
        private static readonly string defaultPath = $"Assets/{FilenameAsset}";
        private static readonly string[] preferredPaths =
        {
            "Assets/Settings",
            "Assets/Config",
            "Assets/Configurations",
            "Assets/Plugins/Settings",
            "Assets/Plugins/Config",
            "Assets/Plugins/Configurations",
        };

        /*
         *  Properties
         */

        /// <summary>
        /// Get a list of currently elevated symbols.
        /// </summary>
        public static List<string> ElevatedSymbols => Singleton.elevatedSymbols;

        /// <summary>
        /// Removes the content of a Preprocessor Symbol Definition File when it is deleted.
        /// If this option is not enabled the symbols of a deleted file will be elevated and must be removed manually
        /// </summary>
        public static bool RemoveSymbolsOnDelete
        {
            get => Singleton.removeSymbolsOnDelete;
            set => Singleton.removeSymbolsOnDelete = value;
        }

        /// <summary>
        /// When enabled, lists of all defined symbols will be displayed in the inspector of the settings file as well as
        /// the inspector of Preprocessor Symbol Definition Files
        /// </summary>
        public static bool ShowAllDefinedSymbols
        {
            get => Singleton.showAllDefinedSymbols;
            set => Singleton.showAllDefinedSymbols = value;
        }

        /// <summary>
        /// When enabled, messages will be logged when symbols are removed, added or elevated.
        /// </summary>
        public static bool LogMessages
        {
            get => Singleton.logMessages;
            set => Singleton.logMessages = value;
        }

        /// <summary>
        /// Get a list of all ScriptDefineSymbolFiles located in the project.
        /// </summary>
        public static IList<PreprocessorSymbolDefinitionFile> ScriptDefineSymbolFiles => scriptDefineSymbolFiles;

        #endregion

        #region Singleton

        public static PreprocessorSymbolDefinitionSettings Singleton => instance ? instance : instance = Helper.FindAllAssetsOfType<PreprocessorSymbolDefinitionSettings>().FirstOrDefault() ?? CreateInstanceAsset();

        private static PreprocessorSymbolDefinitionSettings instance = null;

        private static PreprocessorSymbolDefinitionSettings CreateInstanceAsset()
        {
            var asset = CreateInstance<PreprocessorSymbolDefinitionSettings>();
            AssetDatabase.CreateAsset(asset, CreateFilePath());
            AssetDatabase.SaveAssets();
            return asset;
        }

        private static string CreateFilePath()
        {
            foreach (var path in preferredPaths)
            {
                if (Directory.Exists(path))
                {
                    return $"{path}/{FilenameAsset}";
                }
            }

            return defaultPath;
        }

        #endregion

        /*
         *  Symbol Handling
         */

        public static void RemoveElevatedSymbol(string symbol)
        {
            Singleton.elevatedSymbols.TryRemove(symbol);
        }

        public static void FindAllPreprocessorSymbolDefinitionFiles()
        {
            scriptDefineSymbolFiles?.Clear();

            foreach (var file in Helper.FindAllAssetsOfType<PreprocessorSymbolDefinitionFile>())
            {
                if (file == null)
                {
                    continue;
                }

                AddScriptDefineSymbolFile(file);
            }
        }

        public static void AddScriptDefineSymbolFile(PreprocessorSymbolDefinitionFile file)
        {
            scriptDefineSymbolFiles.AddUnique(file);
        }

        public static void RemoveScriptDefineSymbolFile(PreprocessorSymbolDefinitionFile file)
        {
            scriptDefineSymbolFiles.TryRemove(file);
        }

        /*
         *  Before Auto Save
         */

        private void OnEnable()
        {
            if (AssetDatabase.GetAssetPath(this) == defaultPath)
            {
                AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(this), CreateFilePath());
            }
        }


        /*
         *  Misc
         */

        public static void Select()
        {
            Selection.activeObject = Singleton;
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            instance = this;
        }

        #region Obsolete

        #endregion
    }
}