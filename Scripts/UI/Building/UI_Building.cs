using System.Collections;
using System.Collections.Generic;
using _02.Scripts.Resource;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class UI_Building : MonoBehaviour
{
    public UI_Building_Slot[] slots;
    public UI_Example ui_example;
    
    [SerializeField] private Transform _slotPanel;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _closeButton;
    
    [Header("Selected Object")]
    public UI_Building_Slot  selectedSlot;
    
    
    private void Start()
    {
        slots = new UI_Building_Slot[_slotPanel.childCount];
        
        // BuildingManager에서 만들 수 있는 건물 데이터 가져오기
        BuildingData[] buildingList = BuildingManager.Instance.GetBuildingObjects();

        for (int i = 0; i < buildingList.Length; i++)
        {
            slots[i] = _slotPanel.GetChild(i).GetComponent<UI_Building_Slot>();
            slots[i].index = i;
            slots[i].ui_building = this;
            
            // 건물 데이터가 있다면 데이터 넣어주기, 없다면 빈공간으로 설정
            if (i < buildingList.Length)
            {
                slots[i].data = buildingList[i];
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
        this.gameObject.SetActive(false);
        objectController(false);
        selectedSlot = null;
        
    }
    // 슬롯 클릭 시 SelectButton과 cancelButton 기능 활성화
    public void OnSlotSelected(int slotIndex)
    {
        selectedSlot = slots[slotIndex];
        if (selectedSlot != null)
        {
            ui_example.Set(selectedSlot.data);
            objectController(true);
        }
    }
    
    // 슬롯 선택 후, select 버튼을 누르면 UI를 끄고 오브젝트 프리뷰를 활성화
    public void OnSelectButtonClicked()
    {
        if (selectedSlot != null && selectedSlot.data != null)
        {
            // 재료가 충분한지 확인
            ingredientCostControll(0);
            if (selectedSlot.isEnoughCost)
            {
                // UI 끄고, 오브젝트 프리뷰 활성화
                this.gameObject.SetActive(false);
                // 오브젝트 프리뷰 활성화
                BuildingManager.Instance.showPreview = true;
                // 플레이어 인풋 다시 활성화
                EventBus.Raise(new EnablePlayerInputEvent());
                
            }
            // 재료가 부족할 경우, UI 유지, 재료가 부족하다는 알림
            else
            {
                
                Debug.Log("재료가 부족합니다!");
                // 팝업창으로 알림
            }
        }
    }
    
    // 슬롯 선택 후, cancel 버튼 누르면 UI 종료
    public void OnCancelButtonClicked()
    {
        objectController(false);
        selectedSlot = null;
    }

    public void OnCloseButtonClicked()
    {
        BuildingManager.Instance.playercontroller.offBuilding();
        BuildingManager.Instance.playercontroller.SwitchleftInputNormal();
        CloseBuildingWindow();
    }
    
    // PlayerManager에서 OnBuild(InputSystem)가 호출되면 UI_Building를 활성화/비활성화
    public void OpenBuildingWindow()
    {
        if (this.gameObject != null)
        {
            this.gameObject.SetActive(true);
            objectController(false);
            selectedSlot = null;
            BuildingManager.Instance.showPreview = false;
        }
    }

    public void CloseBuildingWindow()
    {
        if (this.gameObject != null)
        {
            this.gameObject.SetActive(false);
            selectedSlot = null;
            BuildingManager.Instance.showPreview = false;
        }
    }

    private void objectController(bool _bool)
    {
        ui_example.gameObject.SetActive(_bool);
        _selectButton.gameObject.SetActive(_bool);
        _cancelButton.gameObject.SetActive(_bool);
        _closeButton.gameObject.SetActive(!_bool);
    }

    // 인벤토리를 가져와 재료의 수를 확인하고 소비하거나 반환하는 메서드
    public void ingredientCostControll(int controllint)
    {
        Item item = null;
        foreach (var items in BuildingManager.Instance.resourceInventory.GetDict())
        {
            if (items.Key.displayName == "metal")
            {
                item = items.Key;
                break;
            }
        }

        // select 버튼을 누르면 아래 코드를 실행해 인벤토리의 아이템을 소비하도록 설계
        if (controllint == 0)
        {
            // 인벤토리에 아이템이 하나라도 존재하는 경우
            if (item != null)
            {
                int amount = selectedSlot.data.costs[0].cost;
                // 아래 코드가 실행될때 충분한 재료가 있다면 필요한만큼 인벤토리에서 제거, selectedSlot.isEnoughCost에 true 반환
                // 재료가 충분하지 않다면 selectedSlot.isEnoughCost에 false값 반환
                selectedSlot.isEnoughCost = BuildingManager.Instance.resourceInventory.Consume(item, amount);
            
            }
            // 인벤토리에 아이템이 아예 없는 경우
            else
            {
                selectedSlot.isEnoughCost = false;
                Debug.Log("item이 null입니다.");
            }
        }
        // 마우스 우클릭 시 아래 코드를 실행해 소비한 재료를 반환하도록 설계
        else if (controllint == 1)
        {
            // 설치를 취소할 경우 소비한 재료만큼 반환
            int amount = selectedSlot.data.costs[0].cost;
            BuildingManager.Instance.resourceInventory.Add(item, amount);
        }

        
    }

    


}
