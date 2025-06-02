using System.Collections.Generic;
using UnityEngine;
using _02.Scripts.Resource;

namespace _02.Scripts
{
    [CreateAssetMenu(fileName = "StageResourceSpawnData", menuName = "Stage/SpawnData")]
    public class StageRespawnObject : ScriptableObject
    {
        public List<ResourceSpawnInfo> SpawnInfos;
    }

    [System.Serializable]
    public class ResourceSpawnInfo
    {
        public ResourceType resource;
        public int amount;
    }
}