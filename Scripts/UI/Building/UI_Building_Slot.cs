using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_Building_Slot : MonoBehaviour
{
    public BuildingData data;
    
    public UI_Building ui_building;
    public Button button;
    public Image image;
    public TextMeshProUGUI objectName;
    public TextMeshProUGUI description;

    public int index;
    public bool isEnoughCost = true;
    private void Start()
    {
        ui_building = BuildingManager.Instance.ui_building;
    }

    public void Set()
    {
        if (data == null)
        {
            Clear();
        }
        else
        {
            image.gameObject.SetActive(true);
            image.sprite = data.image;
            objectName.text = data.objectName;
            description.text = data.description;
        }
        
    }

    public void Clear()
    {
        data = null;
        image.gameObject.SetActive(false);
        objectName.text = "";
        description.text = "";
    }
    
    public void OnClickButton()
    {
        // 슬롯 클릭시 UI_Building의 Cancel과 Select 버튼 활성화
        if (ui_building != null)
        {
            ui_building.OnSlotSelected(index);
        }
    }


}
