using _02.Scripts.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour, IInteractable
{
    [SerializeField] private  ResourceObject resourceData;

    public InteractableType GetInteractableType()
    {
        return InteractableType.PickUp;
    }
    public string GetInteractPrompt()
    {
        return "줍기";
    }

    public void OnInteract(PlayerInteract player)
    {
        //플레이어 리소스에 아이템 추가
        if(player.TryGetComponent<ResourceInventory>(out ResourceInventory resourceInventory))
        {
            resourceInventory.Add(resourceData.itemSO,resourceData.value);
        }
        Destroy(gameObject);
    }
}
