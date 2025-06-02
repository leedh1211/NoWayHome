using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string tooltipText;
    private TooltipManager tooltipManager;

    public void Initialize(string text)
    {
        tooltipManager = FindObjectOfType<TooltipManager>();   
        tooltipText = text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipManager.Show(tooltipText, transform as RectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.ResetHover();
    }
}