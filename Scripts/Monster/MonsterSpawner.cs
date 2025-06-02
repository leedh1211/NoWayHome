using System.Collections;
using System.Collections.Generic;
using _02.Scripts;
using _02.Scripts.Monster;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public List<StageMonsterListSO> spawnDataList;
    [SerializeField] private GameObject rocket;
    [SerializeField] private ObjectPooler objectPooler;

    [SerializeField] float maxX;
    [SerializeField] float maxZ;
    [SerializeField] float minX;
    [SerializeField] float minZ;
    // public Transform[] spawnPoints;

    private StageMonsterListSO currentData;
    private int currentLevel;
    private int[] totalSpawned;
    private int totalDead = 0;

    private void OnEnable()
    {
        EventBus.Subscribe<DefensePhaseStarted>(OnDefensePhaseStarted);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<DefensePhaseStarted>(OnDefensePhaseStarted);
    }

    private void OnDefensePhaseStarted(DefensePhaseStarted e)
    {
        Debug.Log("DefensePhaseStarted 이벤트 수신됨 → 몬스터 스폰 시작");
        currentLevel = e.Level;
        StartSpawn(currentLevel);
    }

    public void StartSpawn(int level)
    {
        currentData = spawnDataList.Find(data => data.level == level);
        totalSpawned = new int[currentData.spawnDataList.Count];

        if (currentData == null)
        {
            Debug.LogError($"SpawnData for level {level} not found!");
            return;
        }

        for (int i = 0; i < currentData.spawnDataList.Count; i++)
        {
            objectPooler.Initialize(currentData.spawnDataList[i]);
            StartCoroutine(SpawnRoutine(currentData.spawnDataList[i], i));
        }
    }

    IEnumerator SpawnRoutine(SpawnData spawnData, int index)
    {
        while (totalSpawned[index] < spawnData.maxSpawn)
        {
            for (int i = 0; i < spawnData.spawnCount; ++i)
            {
                if (totalSpawned[index] >= spawnData.maxSpawn) break;
                GameObject monster = objectPooler.GetFromPool(spawnData);
                if (monster != null)
                {
                    Vector3 spawnPos = GetRandomEdgePosition();
                    monster.transform.position = spawnPos;
                    var monsterComponent = monster.GetComponent<Monster>();

                    if (monsterComponent != null)
                    {
                        monsterComponent.Inject(rocket.transform, objectPooler, spawnData.monsterData);
                        monsterComponent.OnDeath += HandleMonsterDeath;

                        if (monster.TryGetComponent<MonsterFlySpawn>(out MonsterFlySpawn flySpawn))
                        {
                            Debug.Log("드롭시작");
                            flySpawn.StartDrop(GetRandomPosition());
                        }
                    }

                    ++totalSpawned[index];
                    ++GameManager.Instance.totalSpawned;
                }
            }

            yield return new WaitForSeconds(spawnData.spawnInterval);
        }

        Debug.Log($"Stage {currentData.level} 스폰 완료!");
    }

    private void HandleMonsterDeath()
    {
        ++totalDead;
        GameManager.Instance.currentKillCount = totalDead;
    }

    Vector3 GetRandomEdgePosition(float y = 5f)
    {
        int side = Random.Range(0, 4); // 0: Left, 1: Right, 2: Bottom, 3: Top

        return side switch
        {
            0 => new Vector3(minX, y, Random.Range(minZ, maxZ)), // Left
            1 => new Vector3(maxX, y, Random.Range(minZ, maxZ)), // Right
            2 => new Vector3(Random.Range(minX, maxX), y, minZ), // Bottom
            3 => new Vector3(Random.Range(minX, maxX), y, maxZ), // Top
            _ => Vector3.zero
        };
    }

    Vector3 GetRandomPosition(float y = 5f)
    {
        return new Vector3(Random.Range(minX, maxX), y, Random.Range(minZ, maxZ));
    }
}