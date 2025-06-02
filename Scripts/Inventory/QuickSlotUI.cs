using System.Collections.Generic;
using _02.Scripts.Resource;
using TMPro;
using UnityEngine;

namespace _02.Scripts.Inventory
{
    public class QuickSlotUI : MonoBehaviour
    {
        [SerializeField] private Transform slotParent;
        [SerializeField] private InventorySlotUI slotPrefab;
        [SerializeField] private int SlotAmount;

        public void UpdateSlots(Dictionary<Item, int> items)
        {
            foreach (Transform child in slotParent)
                Destroy(child.gameObject);
            
            InventorySlotUI[] Slots = new InventorySlotUI[SlotAmount];
            for (int i = 0; i < SlotAmount; i++)
            {
                InventorySlotUI slot = Instantiate(slotPrefab, slotParent);
                Slots[i] = slot;
            }

            int count = 0;
            foreach (var pair in items)
            {
                if (pair.Key.isConsumable)
                {
                    if (count >= SlotAmount) break;
                    Slots[count].SetQuickSlot(pair.Key, pair.Value, count+1);
                    count++;    
                }
            }
        }
    }
}