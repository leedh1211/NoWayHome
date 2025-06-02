using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts.Monster
{
    [CreateAssetMenu(fileName = "StageMonsterList.asset", menuName = "Monster/StageMonsterList")]   
    public class StageMonsterListSO : ScriptableObject
    {
        public int level;
        public List<SpawnData> spawnDataList = new List<SpawnData>();
    }
}