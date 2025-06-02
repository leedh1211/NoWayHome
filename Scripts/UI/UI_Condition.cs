using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Condition : MonoBehaviour
{
    [SerializeField] private PlayerConditionType conditionType;
    public float curValue;
    public float maxValue;

    public Image ValueBar;
    public TextMeshProUGUI conditionText;

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
        ChangeConditionText(curValue);
    }

    public void Minus(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
        ChangeConditionText(curValue);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerConditionChangedEvent>(OnConditionChangedHandler);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<PlayerConditionChangedEvent>(OnConditionChangedHandler);
    }

    public void ChangeConditionText(float value)
    {
        conditionText.text = value.ToString("N0");
    }

    private void OnConditionChangedHandler(PlayerConditionChangedEvent args)
    {
        if (args.ConditionType != conditionType) return;
    
        curValue = args.CurCondition;
        maxValue = args.MaxCondition;

        ValueBar.fillAmount = curValue / maxValue;
        conditionText.text = curValue.ToString("N0");
    }
}
