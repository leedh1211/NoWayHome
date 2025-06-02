using _02.Scripts.Monster;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Data/MonsterSpawn")]
public class SpawnData : ScriptableObject
{
    public MonsterData monsterData;
    public float spawnInterval;    // 스폰 간격
    public int spawnCount;         // 한번에 스폰되는 몬스터 개수
    public int maxSpawn;           // 이번 스테이지에서 스폰될 총 몬스터 개수
}
