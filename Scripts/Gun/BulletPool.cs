using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : Singleton<BulletPool>
{
    [SerializeField]
    private BulletData[] _bulletPrefab;
    [SerializeField]
    private int _bulletCount = 10;
    private Dictionary<BulletType, GameObject> _bulletPrefabDictionary = new();
    private Dictionary<BulletType, Queue<GameObject>> _bulletQueue = new();
    private Transform _bulletParent;
    private bool IsInitialized;

    public static event Action OnPoolReady;

    protected override void Awake()
    {
        base.Awake();
        if (_bulletParent == null)
        {
            _bulletParent = new GameObject("BulletPoolParent").transform;
            DontDestroyOnLoad(_bulletParent);
        }

        _bulletPrefab = Resources.LoadAll<BulletData>("ScriptableObjects/Bullet");

        foreach (var bulletPrefab in _bulletPrefab)
        {
            if (!_bulletPrefabDictionary.ContainsKey(bulletPrefab.bulletType))
            {
                _bulletPrefabDictionary.Add(bulletPrefab.bulletType, bulletPrefab.prefab);
            }
        }

        bulletPoolAllType(_bulletCount);
        IsInitialized = true;
        OnPoolReady?.Invoke();
    }

    public void bulletPoolAllType(int amount)
    {
        Debug.Log("bulletPoolAllType");
        foreach (var bullet in _bulletPrefabDictionary)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject prefab = bullet.Value;
                BulletType bulletType = bullet.Key;
                var obj = Instantiate(prefab, _bulletParent);
                if(obj.TryGetComponent<BulletCtrl>(out BulletCtrl ctrl))
                obj.SetActive(false);
                if (!_bulletQueue.ContainsKey(bulletType))
                {
                    _bulletQueue[bulletType] = new Queue<GameObject>();
                }
                _bulletQueue[bulletType].Enqueue(obj);
            }
        }
    }

    public void bulletPoolTargetType(BulletType bulletType, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject prefab = _bulletPrefabDictionary[bulletType];
            var obj = Instantiate(prefab, _bulletParent);
            obj.SetActive(false);
            if (!_bulletQueue.ContainsKey(bulletType))
            {
                _bulletQueue[bulletType] = new Queue<GameObject>();
            }
            _bulletQueue[bulletType].Enqueue(obj);
        }
    }

public GameObject Spawn(BulletType bulletType, Vector3 position, Quaternion rotation)
{
    if (!_bulletQueue.TryGetValue(bulletType, out var queue))
    {
        bulletPoolTargetType(bulletType, 1);
        queue = _bulletQueue[bulletType];
    }
    else if (queue.Count == 0)
    {
        bulletPoolTargetType(bulletType, 1);
        queue = _bulletQueue[bulletType];
    }

    GameObject obj = queue.Dequeue();
        
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        rb.position = position;
        rb.rotation = rotation;
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        obj.SetActive(true);

        return obj;
    }

    public void Release(BulletType bulletType, GameObject obj)
    {
        if (!_bulletQueue.TryGetValue(bulletType, out var queue))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        queue.Enqueue(obj);
    }

    public void ReleaseAll()
    {
        _bulletQueue.Clear();
    }
}
