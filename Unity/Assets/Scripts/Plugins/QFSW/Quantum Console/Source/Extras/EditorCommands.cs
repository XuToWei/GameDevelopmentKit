#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFSW.QC.Extras
{
    internal static class EditorCommands
    {
        private static IEnumerable<GameObject> LoadPrefabs(string prefabName, params PrefabAssetType[] prefabTypes)
        {
            string filter = $"{prefabName} t:GameObject";
            string[] guids = AssetDatabase.FindAssets(filter);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (obj.name == prefabName)
                {
                    if (prefabTypes.Contains(PrefabUtility.GetPrefabAssetType(obj)))
                    {
                        yield return obj;
                    }
                }
            }
        }

        private static T ForceSingle<T>(IEnumerable<T> stream, string errorMessage, string warningMessage)
        {
            bool singleFound = false;
            T single = default;
            foreach (T item in stream)
            {
                if (singleFound)
                {
                    Debug.LogWarning(warningMessage);
                    break;
                }

                single = item;
                singleFound = true;
            }

            if (singleFound)
            {
                return single;
            }
            else
            {
                throw new ArgumentException(errorMessage);
            }
        }

        private static GameObject LoadPrefab(string prefabName)
        {
            IEnumerable<GameObject> prefabs = LoadPrefabs(prefabName, PrefabAssetType.Regular, PrefabAssetType.Variant);
            return ForceSingle(prefabs, $"No prefab with the name {prefabName} could be found.", $"Multiple prefabs with the name {prefabName} were found");
        }

        private static GameObject LoadModel(string modelName)
        {
            IEnumerable<GameObject> models = LoadPrefabs(modelName, PrefabAssetType.Model);
            return ForceSingle(models, $"No model with the name {modelName} could be found.", $"Multiple models with the name {modelName} were found");
        }

        [Command("instantiate-prefab", "Instantiates a GameObject from the specified prefab", Platform.EditorPlatforms)]
        private static void InstantiateGOFromPrefab(
            [CommandParameterDescription("The name of the prefab to instantiate a copy of.")]string prefabName,
            [CommandParameterDescription("The position of the instantiated GameObject.")]Vector3 position,
            [CommandParameterDescription("The rotation of the instantiated GameObject.")]Quaternion rotation)
        {
            GameObject.Instantiate(LoadPrefab(prefabName), position, rotation);
        }

        [Command("instantiate-prefab", "Instantiates a GameObject from the specified prefab", Platform.EditorPlatforms)]
        private static void InstantiateGOFromPrefab(string prefabName, Vector3 position)
        {
            GameObject prefab = LoadPrefab(prefabName);
            GameObject.Instantiate(prefab, position, prefab.transform.rotation);
        }

        [Command("instantiate-prefab", "Instantiates a GameObject from the specified prefab", Platform.EditorPlatforms)]
        private static void InstantiateGOFromPrefab(string prefabName)
        {
            GameObject.Instantiate(LoadPrefab(prefabName));
        }

        [Command("instantiate-model", "Instantiates a GameObject from the specified model prefab", Platform.EditorPlatforms)]
        private static void InstantiateGOFromModelPrefab(
            [CommandParameterDescription("The name of the model to instantiate a copy of.")]string modelName,
            [CommandParameterDescription("The position of the instantiated GameObject.")]Vector3 position,
            [CommandParameterDescription("The rotation of the instantiated GameObject.")]Quaternion rotation)
        {
            GameObject.Instantiate(LoadModel(modelName), position, rotation);
        }

        [Command("instantiate-model", "Instantiates a GameObject from the specified model prefab", Platform.EditorPlatforms)]
        private static void InstantiateGOFromModelPrefab(string modelName, Vector3 position)
        {
            GameObject prefab = LoadModel(modelName);
            GameObject.Instantiate(prefab, position, prefab.transform.rotation);
        }

        [Command("instantiate-model", "Instantiates a GameObject from the specified model prefab", Platform.EditorPlatforms)]
        private static void InstantiateGOFromModelPrefab(string modelName)
        {
            GameObject.Instantiate(LoadModel(modelName));
        }
    }
}
#endif