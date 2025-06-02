using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Example : MonoBehaviour
{
    public UI_Building ui_building;
    public Image image;
    public TextMeshProUGUI ingredient;
    public TextMeshProUGUI cost;

    private void Start()
    {
        ui_building = BuildingManager.Instance.ui_building;
    }
    
    public void Set(BuildingData data)
    {
        if (data == null)
        {
            Clear(data);
            return;
        }
        
        image.gameObject.SetActive(true);
        image.sprite = data.image;
        
        string ingredientStr = "";
        string costStr = "";
        
        // 만약 들어가는 재료의 종류가 여러가지라면 줄바꿈을 통해 나타내기
        for (int i = 0; i < data.costs.Length; i++)
        {
            ingredientStr += data.costs[i].ingredient.ToString();
            costStr += data.costs[i].cost.ToString();

            // 마지막이 아니면 줄바꿈 또는 구분자 추가
            if (i < data.costs.Length - 1)
            {
                ingredientStr += "\n";
                costStr += "\n";
            }
        }

        ingredient.text = ingredientStr;
        cost.text = costStr;
    }
    
    public void Clear(BuildingData data)
    {
        data = null;
        image.gameObject.SetActive(false);
        ingredient.text = "";
        cost.text = "";
    }
    
}
