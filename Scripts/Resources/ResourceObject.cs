using UnityEngine;

namespace _02.Scripts.Resource
{
    [CreateAssetMenu(fileName = "ResourceObject", menuName = "resource/ResourceObject")]
    public class ResourceObject : ScriptableObject
    {
        public GameObject prefab;
        public GameObject dropPrefab;
        public int value;
        public int hitPerQuantity;   //채광 시 필요한 피격 횟수
        public int capacity = 5;
        public float size;
        public ResourceType resourceType;
        public Item itemSO;
    }
}