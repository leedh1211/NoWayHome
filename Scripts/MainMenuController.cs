using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts
{
    public class MainMenuController : MonoBehaviour
    {
        public void OnClickStartGame()
        {
            SceneManager.LoadScene("StageTestScene");
        }

        public void OnClickLoadGame()
        {
            // 예: SaveManager.Load();
        }

        public void OnClickSettings()
        {
            // 예: SettingsPanel.SetActive(true);
        }

        public void OnClickQuit()
        {
            Application.Quit();
        }
    }
}