using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities
{
    internal static class Helper
    {
        public static List<T> RemoveDuplicates<T>(this IEnumerable<T> target)
        {
            var list = new List<T>();
            foreach (var item in target)
            {
                if(!list.Contains(item))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Add a value to a collection if it is not already contained.
        /// </summary>
        public static bool AddUnique<T>(this ICollection<T> collection, T value)
        {
            if (collection.Contains(value))
            {
                return false;
            }
            collection.Add(value);
            return true;
        }

        /// <summary>
        /// Remove a value from a collection if contained.
        /// </summary>
        public static bool TryRemove<T>(this ICollection<T> collection, T value)
        {
            if (!collection.Contains(value))
            {
                return false;
            }

            collection.Remove(value);
            return true;
        }

        /// <summary>
        /// Returns true if the collection is either null, empty or contains elements that are null.
        /// </summary>
        public static bool IsNullOrIncomplete<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count <= 0 || collection.Any(x => x == null);
        }

        /// <summary>
        /// Expensive method that will find every asset of a certain type that is located anywhere within the project.
        /// </summary>
        internal static List<T> FindAllAssetsOfType<T>() where T : UnityEngine.Object
        {
            var assets = new List<T>();
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null && !string.IsNullOrWhiteSpace(assetPath))
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
    }
}