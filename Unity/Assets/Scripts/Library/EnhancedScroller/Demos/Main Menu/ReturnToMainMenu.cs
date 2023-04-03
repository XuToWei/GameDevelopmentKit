using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace EnhancedScrollerDemos.MainMenu
{
    public class ReturnToMainMenu : MonoBehaviour
    {
        public void ReturnToMainMenuButton_OnClick()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}