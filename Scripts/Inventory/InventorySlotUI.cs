using _02.Scripts.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.Inventory
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private TextMeshProUGUI slotText;

        public void SetSlot(Item item, int amount)
        {
            icon.sprite = item.icon;
            amountText.text = amount.ToString();
            
            var inputTrigger = icon.GetComponent<HoverTrigger>();
            inputTrigger.Initialize(item.description);
        }
        
        public void SetQuickSlot(Item item, int amount, int slotNumber)
        {
            icon.sprite = item.icon;
            amountText.text = amount.ToString();
            slotText.text = slotNumber.ToString();
        }
    }
}