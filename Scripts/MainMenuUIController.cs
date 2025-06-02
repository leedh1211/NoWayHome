using System;
using System.Collections;
using System.Collections.Generic;
using _02.Scripts;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField]
    private Canvas _mainMenuCanvas;
    [SerializeField] 
    private GameObject mainMenu;
    [SerializeField]
    private GameObject SettingsPanel;
    [SerializeField]
    private GameObject Discription;

    public void OnNewGameClicked()
    {
        PlayerPrefs.SetInt("ContinueFlag", 0); // 새 게임
        SceneLoader.Instance.LoadGameScene(); // 더 이상 이 스크립트는 책임지지 않음
    }

    public void OnContinueClicked()
    {
        PlayerPrefs.SetInt("ContinueFlag", 1); // 이어하기
        SceneLoader.Instance.LoadGameScene(); // 더 이상 이 스크립트는 책임지지 않음
    }

    public void OnClickDiscription()
    {
        Discription.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OnCloseDiscription()
    {
        Debug.LogError("dd");
        Discription.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnClickSettings()
    {
        SettingsPanel.SetActive(true);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
