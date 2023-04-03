using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace EnhancedScollerDemos.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void SceneButton_OnClick(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}