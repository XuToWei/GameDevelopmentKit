using QFSW.QC.Suggestors.Tags;
using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFSW.QC.Extras
{
    public static class SceneCommands
    {
        private static async Task PollUntilAsync(int pollInterval, Func<bool> predicate)
        {
            while (!predicate())
            {
                await Task.Delay(pollInterval);
            }
        }

        [Command("load-scene", "loads a scene by name into the game")]
        private static async Task LoadScene(
            [SceneName]
            string sceneName,

            [CommandParameterDescription("'Single' mode replaces the current scene with the new scene, whereas 'Additive' merges them")]
            LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneUtilities.LoadSceneAsync(sceneName, loadMode);
            await PollUntilAsync(16, () => asyncOperation.isDone);
        }

        [Command("load-scene-index", "loads a scene by index into the game")]
        private static async Task LoadScene(int sceneIndex,
        [CommandParameterDescription("'Single' mode replaces the current scene with the new scene, whereas 'Additive' merges them")]LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex, loadMode);
            await PollUntilAsync(16, () => asyncOperation.isDone);
        }

        [Command("unload-scene", "unloads a scene by name")]
        private static async Task UnloadScene([SceneName(LoadedOnly = true)] string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            await PollUntilAsync(16, () => asyncOperation.isDone);
        }

        [Command("unload-scene-index", "unloads a scene by index")]
        private static async Task UnloadScene(int sceneIndex)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneIndex);
            await PollUntilAsync(16, () => asyncOperation.isDone);
        }

        [Command("all-scenes", "gets the name and index of every scene included in the build")]
        private static Dictionary<int, string> GetAllScenes()
        {
            Dictionary<int, string> sceneData = new Dictionary<int, string>();
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                int sceneIndex = i;
                string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                sceneData.Add(sceneIndex, sceneName);
            }

            return sceneData;
        }

        [Command("loaded-scenes", "gets the name and index of every scene currently loaded")]
        private static IEnumerable<KeyValuePair<int, string>> GetLoadedScenes()
        {
            return SceneUtilities.GetLoadedScenes()
                .OrderBy(x => x.buildIndex)
                .Select(x => new KeyValuePair<int, string>(x.buildIndex, x.name));
        }

        [Command("active-scene", "gets the name of the active primary scene")]
        private static string GetCurrentScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            return scene.name;
        }

        [Command("set-active-scene", "sets the active scene to the scene with name 'sceneName'")]
        private static void SetActiveScene([SceneName(LoadedOnly = true)] string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                throw new ArgumentException($"Scene {sceneName} must be loaded before it can be set active");
            }

            SceneManager.SetActiveScene(scene);
        }
    }
}
