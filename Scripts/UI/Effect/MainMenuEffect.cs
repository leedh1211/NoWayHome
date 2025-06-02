using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuEffect : MonoBehaviour
{
    [SerializeField] private Vector2[] targetPositions; // 이동할 위치
    [SerializeField] private RectTransform[] MainRect; // 위치 이동용

    private Vector3[] originPos;

    [SerializeField] UIIntroEffect uIIntroEffect;

    private void Start()
    {
        originPos = new Vector3[MainRect.Length];
        for (int i = 0; i < MainRect.Length; i++)
        {
            originPos[i] = MainRect[i].anchoredPosition;
            Debug.Log(originPos[i]);
        }
    }

    public void OnclickSetting()
    {
        uIIntroEffect.PlayBackAnimation();
        MainRect[1].DOAnchorPos(targetPositions[0], 1f);
    }

    public void OnclickSettingExit()
    {
        MainRect[1].DOAnchorPos(originPos[1], 1f);
        uIIntroEffect.PlayIntroAnimation();
    }

    public void OnClickDecription()
    {
        uIIntroEffect.PlayBackAnimation();
        MainRect[2].DOAnchorPos(targetPositions[1], 1f);
    }

    public void OnclickDesExit()
    {
        MainRect[2].DOAnchorPos(originPos[2], 1f);
        uIIntroEffect.PlayIntroAnimation();
    }
}
