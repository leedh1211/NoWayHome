using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCmenu : MonoBehaviour
{
    [SerializeField] private GameObject EscMenu;
    [SerializeField] private GameObject SettingMenu;
    [SerializeField] private GameObject Upgrademenu;
    [SerializeField] private GameObject tradeMenu;
    [SerializeField] private GameObject buildingMenu;
    [SerializeField] private GameObject CrossHair;
    public void ShowSetting() // setting 버튼
    {
        EscMenu.SetActive(false);
        SettingMenu.SetActive(true);
    }

    public void CloseSetting()
    {
        EscMenu.SetActive(true);
        SettingMenu.SetActive(false);
    }

    public void CloseMenu() // quit 버튼
    {
        CrossHair.SetActive(true);
        EscMenu.SetActive(false);
        if(tradeMenu.activeSelf || Upgrademenu.activeSelf || buildingMenu.activeSelf)
        {
            Time.timeScale = 1f;
        }
        else
        {
            EventBus.Raise(new EnablePlayerInputEvent());
            Time.timeScale = 1f;
        }
    }

    public void GoLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
