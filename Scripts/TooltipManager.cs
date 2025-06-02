using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private TMP_Text tooltipText;

    private bool isMouseOverTooltip = false;
    private bool isHoverTarget = false;

    private void Start()
    {
        EventBus.Subscribe<EnablePlayerInputEvent>(EnableTooltip);
    }

    public void Show(string message, RectTransform target)
    {
        Debug.Log($"툴팁 호출됨: {message} at {target.name}");
        tooltipText.text = message;
        tooltipRect.gameObject.SetActive(true);

        float x = target.position.x + 170f;
        float y = target.position.y + 150f;
        tooltipRect.position = new Vector3(x, y, 0f); // 대상 이미지 위치로 이동

        isHoverTarget = true;
    }

    public void HideIfNotHovered()
    {
        if (!isMouseOverTooltip && tooltipRect)
        {
            tooltipRect.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverTooltip = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverTooltip = false;

        if (!isHoverTarget)
        {
            tooltipRect.gameObject.SetActive(false);
        }
    }

    public void ResetHover()
    {
        isHoverTarget = false;
        HideIfNotHovered();
    }

    private void EnableTooltip(EnablePlayerInputEvent args)
    {
        isHoverTarget = false;
        HideIfNotHovered();
    }
}
