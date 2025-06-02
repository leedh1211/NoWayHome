using _02.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Result : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI currentStage;
    [SerializeField] private TextMeshProUGUI countText;

    private void OnEnable()
    {
        EventBus.Subscribe<DefenseResultShowRequested>(goResultScene);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<DefenseResultShowRequested>(goResultScene);
    }

    private void goResultScene(DefenseResultShowRequested result)
    {
        EndingUIController.StageIndex = result.StageIndex - 1;
        EndingUIController.KillCount = result.CurrentKillCount;
        EndingUIController.totalSpawnCount = result.TotalSpawnCount;

        SceneManager.LoadScene("EndingScene");
    }

    // private void ShowResultPopup()
    // {
    //     Time.timeScale = 0f;
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    //     currentStage.text = $"STAGE {result.StageIndex - 1}";
    //     countText.text = $"KILLS : {result.CurrentKillCount} / {result.TotalSpawnCount}";
    //     resultPanel.SetActive(true);
    // }

    public void OnClickCancelButton()
    {
        Time.timeScale = 1f;
        resultPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
