using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ship_Condition : MonoBehaviour
{
    public float curValue;
    public float maxValue;

    public Image ValueBar;
    public TextMeshProUGUI conditionText;

    private void OnEnable()
    {
        ShipEvents.OnHpChanged += UpdateHpUI;
    }

    private void OnDisable()
    {
        ShipEvents.OnHpChanged -= UpdateHpUI;
    }

    private void UpdateHpUI(float currentHp, float maxHp)
    {
        ValueBar.fillAmount = currentHp / maxHp;
        conditionText.text = currentHp.ToString("N0");
    }
}
