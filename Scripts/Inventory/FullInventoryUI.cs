using System.Collections.Generic;
using _02.Scripts.Resource;
using UnityEngine;

namespace _02.Scripts.Inventory
{
    public class FullInventoryUI : MonoBehaviour
    {
        [SerializeField] private Transform gridParent;
        [SerializeField] private InventorySlotUI slotPrefab;

        public void UpdateInventory(Dictionary<Item, int> items)
        {
            foreach (Transform child in gridParent)
                Destroy(child.gameObject);

            foreach (var pair in items)
            {
                var slot = Instantiate(slotPrefab, gridParent);
                slot.SetSlot(pair.Key, pair.Value);
            }
        }
    }
}