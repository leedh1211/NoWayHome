using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts
{
    public class TimerController : MonoBehaviour
    {
        public event Action<GameState,float> OnTimeChanged;
        public event Action<GameState,float> OnTimerEnd;

        public float currentTime;
        public GameState currentState;
        private bool timerActive;

        public void SetTime(GameState gameState,float time)
        {
            Debug.Log("타이머 세팅 : "+time);
            currentTime = time;
            currentState = gameState;
            timerActive = true;
        }

        private void Update()
        {
            if (!timerActive) return;
            string sceneName = SceneManager.GetActiveScene().name;
            if(sceneName == "LobbyScene")
            {
                currentState = GameState.Lobby;
            }
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0f; // 음수 방지
                timerActive = false;
                OnTimerEnd?.Invoke(currentState, currentTime);
            }
            if (OnTimeChanged != null)
            {
                OnTimeChanged.Invoke(currentState,currentTime);    
            }
        }
    }
}