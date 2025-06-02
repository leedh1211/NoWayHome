using UnityEngine;

namespace _02.Scripts.NPC
{
    [RequireComponent(typeof(Collider))]
    public class NpcController : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private TradeUIManager tradeUIManager;
        public InteractableType GetInteractableType()
        {
            return InteractableType.NPC;
        }

        public string GetInteractPrompt()
        {
            return "아이템 구입";
        }

        public void OnInteract(PlayerInteract player)
        {
            tradeUIManager.OpenTradeUI();
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    Debug.Log(other.name);
        //    if (other.CompareTag("Player"))
        //    {
        //        TradeUIManager.Instance?.OpenTradeUI();
        //    }
        //}
    }
}