using System.Collections.Generic;
using _02.Scripts.Resource;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.NPC
{
    public class TradeUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject TradePanel;
        [SerializeField] private Transform slotParent;
        [SerializeField] private TradeSlot slotPrefab;
        [SerializeField] private Button tradeButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TradeRecipe[] recipes;
        [SerializeField] private ResourceInventory inventory;

        private List<TradeSlot> currentSlots = new();
        private TradeSlot selectedSlot;

        private void Start()
        {
            foreach (TradeRecipe recipe in recipes)
            {
                CreateSlots(recipe);    
            }

            tradeButton.onClick.RemoveAllListeners();
            tradeButton.onClick.AddListener(Trade);

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseTradeUI);
        }

        public void OpenTradeUI()
        {
            EventBus.Raise(new DisablePlayerInputEvent());
            TradePanel.SetActive(true);
        }

        private void CreateSlots(TradeRecipe recipe)
        {
            var slot = Instantiate(slotPrefab, slotParent);
            slot.Init(recipe);
            currentSlots.Add(slot);
        }

        public void SelectSlot(TradeSlot slot)
        {
            if (selectedSlot != null) selectedSlot.Deselect();
            selectedSlot = slot;
            selectedSlot.Select();
        }

        public void Trade()
        {
            if (selectedSlot)
            {
                if (inventory.Consume(selectedSlot.inputItem, selectedSlot.inputAmount))
                {
                    inventory.Add(selectedSlot.outPutItem, selectedSlot.outPutAmount);    
                }
                else
                {
                    Debug.Log("교환 실패");
                }
            }
        }

        public void CloseTradeUI()
        {
            EventBus.Raise(new EnablePlayerInputEvent());
            TradePanel.SetActive(false);
        }
    }
}
