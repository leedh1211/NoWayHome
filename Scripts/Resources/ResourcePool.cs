using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts.Resource
{
    public class ResourcePool : MonoBehaviour
    {
        [SerializeField] private List<ResourceObject> _resourcePrefab;
        [SerializeField] private int _resourceCount;

        private Dictionary<ResourceType, GameObject> _resourcePrefabDictionary = new();
        private Dictionary<ResourceType, Queue<GameObject>> _resourceQueue = new();
        private Transform _resourceParent;

        public bool IsInitialized { get; private set; } = false;

        public event Action OnPoolReady;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            if (IsInitialized) return;

            Debug.Log("[ResourcePool] 초기화 시작");

            if (_resourceParent == null)
            {
                GameObject parentObj = new GameObject("ResourcePoolParent");
                _resourceParent = parentObj.transform;
                _resourceParent.SetParent(transform);
            }

            _resourcePrefabDictionary.Clear();
            foreach (var resourcePrefab in _resourcePrefab)
            {
                if (!_resourcePrefabDictionary.ContainsKey(resourcePrefab.resourceType))
                {
                    _resourcePrefabDictionary.Add(resourcePrefab.resourceType, resourcePrefab.prefab);
                }
            }

            _resourceQueue.Clear();
            ResourcePoolAllType(_resourceCount);

            IsInitialized = true;
            Debug.Log("[ResourcePool] 초기화 완료");

            OnPoolReady?.Invoke();
        }

        public void ResourcePoolAllType(int amount)
        {
            foreach (var kvp in _resourcePrefabDictionary)
            {
                ResourceType type = kvp.Key;
                GameObject prefab = kvp.Value;

                if (!_resourceQueue.ContainsKey(type))
                {
                    _resourceQueue[type] = new Queue<GameObject>();
                }

                for (int i = 0; i < amount; i++)
                {
                    GameObject obj = Instantiate(prefab, _resourceParent);
                    obj.SetActive(false);
                    obj.GetComponent<HarvestableOre>()._resourcePool = this;
                    _resourceQueue[type].Enqueue(obj);
                }
            }
        }

        public void ResourcePoolTargetType(ResourceType resourceType, int amount)
        {
            if (!_resourcePrefabDictionary.ContainsKey(resourceType)) return;
            if (!_resourceQueue.ContainsKey(resourceType))
                _resourceQueue[resourceType] = new Queue<GameObject>();

            GameObject prefab = _resourcePrefabDictionary[resourceType];

            for (int i = 0; i < amount; i++)
            {
                GameObject obj = Instantiate(prefab, _resourceParent);
                obj.SetActive(false);
                _resourceQueue[resourceType].Enqueue(obj);
            }
        }

        public GameObject Spawn(ResourceType resourceType, Vector3 position)
        {
            if (!_resourceQueue.TryGetValue(resourceType, out var queue) || queue.Count == 0)
            {
                ResourcePoolTargetType(resourceType, 1);
                queue = _resourceQueue[resourceType];
            }

            GameObject obj = queue.Dequeue();
            obj.transform.position = position;
            obj.SetActive(true);
            return obj;
        }

        public void Release(ResourceType resourceType, GameObject obj)
        {
            if (!_resourceQueue.TryGetValue(resourceType, out var queue))
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            queue.Enqueue(obj);
        }

        public void ReleaseAll()
        {
            _resourceQueue.Clear();
        }

        private void OnDisable()
        {
            IsInitialized = false;
        }
    }
}
