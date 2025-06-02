using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingMenuUI;
    [SerializeField] private GameObject Corrshiar;

    void Update()
    {
        if (settingMenuUI.activeSelf) return;
        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenuUI.activeSelf)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        Corrshiar.SetActive(false);
        pauseMenuUI.SetActive(true);
        EventBus.Raise(new DisablePlayerInputEvent());
        Time.timeScale = 0f;
    }
}
