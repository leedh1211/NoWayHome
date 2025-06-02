using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts
{
    public class EndingUIController : MonoBehaviour
    {
        [SerializeField]
        private Canvas _EndingUICanvas;
        public RectTransform[] panels; // BackGround, Lobby, NextStage를 이 배열에 등록
        public TextMeshProUGUI text;
        public float startX = 1500f;
        public float targetX = 500f;
        public float duration = 0.5f; // 이동 시간
        public AnimationCurve easing; // 타이밍 곡선(자연스러운 감속)
        public static int StageIndex = 0;
        public static int KillCount = 0;
        public static int totalSpawnCount = 0;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            Cursor.lockState = CursorLockMode.None;
            SetText();
            foreach (var panel in panels)
            {
                Vector3 pos = panel.anchoredPosition;
                pos.x = startX;
                panel.anchoredPosition = pos;
            }

            StartCoroutine(SlideInAll());
        }

        private System.Collections.IEnumerator SlideInAll()
        {
            float delay = 0.2f;
    
            foreach (var panel in panels)
            {
                StartCoroutine(SlideIn(panel));
                yield return new WaitForSeconds(delay); // 다음 패널 실행까지 딜레이
            }
        }

        private System.Collections.IEnumerator SlideIn(RectTransform panel)
        {
            float elapsed = 0f;
            Vector3 start = panel.anchoredPosition;
            Vector3 end = new Vector3(targetX, start.y, start.z);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = easing.Evaluate(t);
                panel.anchoredPosition = Vector3.Lerp(start, end, easedT);
                yield return null;
            }

            panel.anchoredPosition = end;
        }
        public void OnGoingLobby()
        {
            Debug.Log("OnGoingLobby");
            SceneManager.LoadScene("LobbyScene"); // 더 이상 이 스크립트는 책임지지 않음
        }

        public void OnClickNextStage()
        {
            Debug.Log("OnClickNextStage");
            PlayerPrefs.SetInt("ContinueFlag", 1); // 이어하기
            SceneLoader.Instance.LoadGameScene(); // 더 이상 이 스크립트는 책임지지 않음
        }

        private void SetText()
        {
            string ClearText = "";
            ClearText += "Stage "+StageIndex.ToString()+" Clear \n";
            ClearText += "처치한 적 "+KillCount.ToString()+"/"+totalSpawnCount+" \n";
            text.text = ClearText;
        }
    }

}