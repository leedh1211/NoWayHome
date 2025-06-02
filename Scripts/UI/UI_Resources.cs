using TMPro;
using UnityEngine;

public class UI_Resources : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stoneText;
    public int resources = 0;
    private void Start()
    {
        stoneText.text = ":  0";
    }

    public void DownGradeStone(int value)
    {
        resources -= value;
        stoneText.text = ":  " + resources.ToString();
    }

    public void UpdateStone(int value)
    {
        resources += value;
        stoneText.text = ":  " + resources.ToString();
    }
}
