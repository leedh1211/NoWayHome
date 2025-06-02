using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIIntroEffect : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private RectTransform[] uiElements; // 이동시킬 UI
    [SerializeField] private Vector2[] targetPositions;  // 각 UI의 최종 위치
    [SerializeField] private float duration = 0.8f;       // 이동 시간
    [SerializeField] private float delayInterval = 0.1f;  // 순차 딜레이

    private Vector2[] originPos;

    private void Start()
    {
        originPos = new Vector2[uiElements.Length];
        for (int i = 0; i < uiElements.Length; i++)
        {
            originPos[i] = uiElements[i].anchoredPosition;
        }
        PlayIntroAnimation();
        Invoke("Fadein", 1f);
    }

    public void PlayIntroAnimation()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            RectTransform ui = uiElements[i];
            Vector2 targetPos = targetPositions[i];

            // DOTween으로 목표 위치까지 이동 (Back 곡선 사용)
            ui.DOAnchorPos(targetPos, duration)
              .SetDelay(i * delayInterval)
              .SetEase(Ease.OutBack); // 튕기듯 자연스럽게
        }
    }

    public void PlayBackAnimation()
    {
        for(int i = 0; i < uiElements.Length; i++)
        {
            RectTransform ui = uiElements[i];
            Vector2 targetPos = targetPositions[i];
            ui.DOAnchorPos(originPos[i],0.5f);
        }
    }

    private void Fadein()
    {
        targetImage.DOFade(1f, 1f);
    }
}