using _02.Scripts.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;

public class HarvestableOre : MonoBehaviour, IInteractable
{
    //[SerializeField] private ItemData dropItemData;
    [SerializeField] private ResourceObject resourceData;       //채집 가능한 자원 데이터
    private int capacity => resourceData.capacity;                  //총 자원량
    private int remainingCapacity;
    [SerializeField] private float maxDropDistance = 3.0f;
    [SerializeField] private float minDropDistance = 2.0f;
    [SerializeField] private AudioClip miningClip; //광맥 채집 시 효과음 
    public ResourcePool _resourcePool;
    private PlayerInteract player;
    private Vector3 dropPosition; 
    private int currentHit = 0;

    private void Start()
    {
        remainingCapacity = capacity;
    }
    public string GetInteractPrompt()
    {
        return "채집";
    }

    public void OnInteract(PlayerInteract player)
    {
        this.player = player;
        Harvest();
    }

    private void Harvest()
    {
        SoundManager.Instance.PlaySFX(miningClip, transform.position);
        if (++currentHit < resourceData.hitPerQuantity) 
        {
            return;
        }
       
        currentHit = 0;
        remainingCapacity--;
        if (player != null)
        {
            DropItem();
        }
   
        if (remainingCapacity <= 0)
        {
            _resourcePool.Release(resourceData.resourceType,gameObject);        //자원이 다 떨어졌으면 파괴
        }
        _resourcePool.ResourcePoolTargetType(resourceData.resourceType, 1);
    }
    private void DropItem()
    {
        //플레이어 근처 랜덤 위치에 아이템 드랍
        Vector3 directionToPlayer = player.transform.position - transform.position;

        directionToPlayer.Normalize();

        float randomAngle;

        if(Random.Range(0,2)  == 0)
        {
            randomAngle = Random.Range(10f, 30f);
        }
        else
        {
            randomAngle = Random.Range(-30f, -10f);
        }

        Quaternion angleRotation = Quaternion.Euler(0f, randomAngle, 0f);
        Vector3 dropDirection = angleRotation * directionToPlayer;

        float randomDistance = Random.Range(minDropDistance, maxDropDistance);

        dropPosition = transform.position + dropDirection * randomDistance + Vector3.up * 2f;

        Instantiate(resourceData.dropPrefab, dropPosition, Quaternion.identity);
    }

    public InteractableType GetInteractableType()
    {
        return InteractableType.Harvest;
    }
}
