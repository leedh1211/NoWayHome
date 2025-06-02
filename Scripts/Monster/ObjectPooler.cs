using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // 비활성화 오브젝트 관리
    private Dictionary<GameObject, Queue<GameObject>> pool = new();
    // 활성화 오브젝트 관리
    private Dictionary<GameObject, List<GameObject>> activeObjects = new();

    private void OnEnable()
    {
        EventBus.Subscribe<DefensePhaseEnded>(OnDefensePhaseEnded);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<DefensePhaseEnded>(OnDefensePhaseEnded);
    }

    private void OnDefensePhaseEnded(DefensePhaseEnded e)
    {
        ClearAllPools();
    }

    public GameObject GetFromPool(SpawnData data)
    {
        GameObject prefab = data.monsterData.prefab;

        if (!pool.ContainsKey(prefab))
        {
            Debug.Log("몬스터가 풀에 존재하지 않습니다.");
            return null;
        }

        Queue<GameObject> queue = pool[prefab];

        if (queue.Count <= 0)
        {
            CreateObject(data, queue);
        }

        GameObject obj = queue.Dequeue();
        obj.SetActive(true);

        if (!activeObjects.ContainsKey(prefab))
        {
            activeObjects[prefab] = new List<GameObject>();
        }

        activeObjects[prefab].Add(obj);

        return obj;
    }

    public void ReturnToPool(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        pool[prefab].Enqueue(obj);
        activeObjects[prefab].Remove(obj);
    }

    public void Initialize(SpawnData data)
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        CreateObject(data, queue);
        pool[data.monsterData.prefab] = queue;
    }

    private void CreateObject(SpawnData data, Queue<GameObject> q)
    {
        GameObject prefab = data.monsterData.prefab;

        for (int i = 0; i < data.spawnCount; ++i)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            q.Enqueue(obj);
        }
    }

    public void ClearAllPools()
    {
        foreach (var kvp in activeObjects)
        {
            foreach (var obj in kvp.Value)
            {
                Destroy(obj);
            }
        }

        activeObjects.Clear();

        foreach (var kvp in pool)
        {
            while (kvp.Value.Count > 0)
            {
                Destroy(kvp.Value.Dequeue());
            }
        }

        pool.Clear();
    }
}
