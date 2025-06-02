using _02.Scripts;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShowStageEffect : MonoBehaviour
{
    [SerializeField] private Image image; // 움직일 이미지
    [SerializeField] private RectTransform imageRect; // 위치 이동용
    [SerializeField] private TextMeshProUGUI text; // 텍스트

    private Vector2 originalPos;

    private void Start()
    {
        originalPos = imageRect.anchoredPosition;
    }
    private void OnEnable()
    {
        EventBus.Subscribe<FarmingPhaseStarted>(OnFarmingPhaseStart);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<FarmingPhaseStarted>(OnFarmingPhaseStart);
    }

    private void OnFarmingPhaseStart(FarmingPhaseStarted ent)
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        text.text = "STAGE " + GameManager.Instance.StageIndex.ToString();
        // 시작 시 완전 투명하게 설정
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);

        DG.Tweening.Sequence seq = DOTween.Sequence();

        Vector2 downPos = originalPos + new Vector2(0, -250f);

        // 점점 보이면서 아래로
        seq.Append(image.DOFade(1f, 0.5f));
        seq.Join(text.DOFade(1f, 0.5f));
        seq.Join(imageRect.DOAnchorPos(downPos, 1f).SetEase(Ease.OutQuad));

        seq.AppendInterval(1f); // 잠시 대기

        // 다시 위로 올라가면서 투명
        seq.Append(image.DOFade(0f, 0.5f));
        seq.Join(text.DOFade(0f, 0.5f));
        seq.Join(imageRect.DOAnchorPos(originalPos, 1f).SetEase(Ease.InQuad));
    }
}
