using _02.Scripts.NPC;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _02.Scripts.Resource
{
    public class TradeSlot : MonoBehaviour
    {
        [SerializeField] private Image inputIcon;
        [SerializeField] private TMP_Text inputAmountText;
        [SerializeField] private Image outPutIcon;
        [SerializeField] private TMP_Text outPutAmountText;
        [SerializeField] private Image background;
        
        [SerializeField]
        private TradeUIManager tradeUIManager;

        private Color normalColor = Color.white;
        private Color selectedColor = Color.blue;

        public Item inputItem;
        public Item outPutItem;
        public int inputAmount;
        public int outPutAmount;

        public void Init(TradeRecipe recipe)
        {
            tradeUIManager = FindObjectOfType<TradeUIManager>();
            
            foreach (TradeEntry entry in recipe.inputRecipe)
            {
                inputItem = entry.item;
                inputIcon.sprite = entry.item.icon;
                inputAmountText.text = entry.amount.ToString();
                inputAmount = entry.amount;
            }
            
            foreach (TradeEntry entry in recipe.outputRecipe)
            {
                outPutItem = entry.item;
                outPutIcon.sprite = entry.item.icon;
                outPutAmountText.text = entry.amount.ToString();
                outPutAmount = entry.amount;
            }
            
            var inputTrigger = inputIcon.GetComponent<HoverTrigger>();
            inputTrigger.Initialize(inputItem.description);

            var outputTrigger = outPutIcon.GetComponent<HoverTrigger>();
            outputTrigger.Initialize(outPutItem.description);
            Deselect();
        }

        public void Select()
        {
            Debug.Log("Select");
            background.color = selectedColor;
        }

        public void Deselect()
        {
            background.color = normalColor;
        }

        public void OnClick()
        {
            Debug.Log("click");
            tradeUIManager.SelectSlot(this);
        }
    }
}