using System.Collections.Generic;
using _02.Scripts.Resource;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02.Scripts.Inventory
{
    using UnityEngine;

    public class InventoryUIManager : MonoBehaviour
    {
        [SerializeField] private ResourceInventory inventory;
        [SerializeField] private QuickSlotUI quickSlotUI;
        [SerializeField] private FullInventoryUI fullInventoryUI;

        [SerializeField] private GameObject fullInventoryPanel;
        private bool isInventoryOpen = false;
        [SerializeField]
        private TooltipManager tooltipManager;

        private void Start()
        {
            fullInventoryPanel.SetActive(false);
            quickSlotUI.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            if (inventory != null)
            {
                inventory.OnInventoryChanged += RefreshUI;
            }
        }

        private void OnDisable()
        {
            if (inventory != null)
            {
                inventory.OnInventoryChanged -= RefreshUI;
            }
        }

        public void InventoryOnOff(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;
            Debug.Log("InventoryOnOff");
            isInventoryOpen = !isInventoryOpen;
            fullInventoryPanel.SetActive(isInventoryOpen);
            if (isInventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                tooltipManager.ResetHover();
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void RefreshUI(Dictionary<Item, int> inventoryDict)
        {
            var items = inventoryDict ?? new Dictionary<Item, int>(); // 첫 시작 대비
            quickSlotUI.UpdateSlots(items);
            fullInventoryUI.UpdateInventory(items);
        }
    }
}