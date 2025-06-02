using UnityEngine;

namespace _02.Scripts.Monster
{
    [CreateAssetMenu(fileName = "MonsterData.asset", menuName = "Monster/MonsterData")]
    public class MonsterData : ScriptableObject
    {
        public GameObject prefab;
        public int level;
        public float MaxHP;
        public float Speed;
        public float Damage;
        public float AttackCooldown;
        public float AttackRange;
    }
}