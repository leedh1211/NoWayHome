using System.Collections;
using System.Collections.Generic;
using _02.Scripts;
using TMPro;
using UnityEngine;

public class UI_GameStage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI Time;

    private TimerController timerController;

    private void OnEnable()
    {
        timerController = GameManager.Instance.timerController;

        if (timerController != null)
        {
            timerController.OnTimeChanged += UpdateTimeUI;
        }
    }

    private void OnDisable()
    {
        if (timerController != null)
        {
            timerController.OnTimeChanged -= UpdateTimeUI;
        }
    }

    private void SetTimeUI(GameState state)
    {
        string stateString = "";
        if (state == GameState.Lobby)
        {
            stateString = "Lobby";
        }else if (state == GameState.Farm)
        {
            stateString = "Farming Phase";
        }else if (state == GameState.Defense)
        {
            stateString = "Defense Phase";
        }
        Title.text = stateString;
    }

    private void UpdateTimeUI(GameState state,float time)
    {
        SetTimeUI(state);
        Time.text = FormatTime(time);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:D2}:{seconds:D2}";
    }
    
    
}
