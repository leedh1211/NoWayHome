using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Death : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TextMeshProUGUI lifeTime;
    [SerializeField] private TextMeshProUGUI countText;

    private void OnEnable()
    {
        EventBus.Subscribe<GameOverEvent>(ShowDeathPopup);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<GameOverEvent>(ShowDeathPopup);
    }

    private void ShowDeathPopup(GameOverEvent result)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        lifeTime.text = $"Life Time : {FormatTime(result.LifeTime)}";
        countText.text = $"KILLS : {result.CurrentKillCount} / {result.TotalSpawnCount}";
        deathPanel.SetActive(true);
    }

    public void OnClickCancelButton()
    {
        deathPanel.SetActive(false);
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes}M {seconds}S";
    }
}
