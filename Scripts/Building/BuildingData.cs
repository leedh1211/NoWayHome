using _02.Scripts.Resource;
using UnityEngine;

public enum BuildingType
{
    CreateInstallableField,
    InstallInField
}

[System.Serializable]
public class BuildingCost
{
    public ResourceType ingredient;    // 필요한 재료
    public int cost;                   // 필요한 수량
}

[System.Serializable]
public class TurretSetting
{
    public float attackRange;          // 터렛 사거리
    public float fireDelay;            // 터렛 연사 딜레이
    public float attackCooldown;       // 공격 쿨타임
    public GameObject bulletPrefab;    // 총알 프리팹
}
    
[CreateAssetMenu(fileName = "Building", menuName = "New Building")]
public class BuildingData : ScriptableObject
{
    [Header("Building Info")]
    public string objectName;           // 건물 오브젝트 이름
    public string description;          // 건물 오브젝트 설명
    public BuildingType buildingType;   // 건물 타입
    public Sprite image;                // UI_Building에서 나타날 이미지
    public GameObject prefab;           // Buildings Prefabs
    public GameObject preview;          // 프리뷰
    public float maxHp = 500f;
    public float currentHp = 500f;
        
    [Header("Building Cost")]
    public BuildingCost[] costs;         // 건설에 필요한 재료들
    
    [Header("Turret Settings")]
    public TurretSetting[] settings;     // 터렛 세팅
    
}