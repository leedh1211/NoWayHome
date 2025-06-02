using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts.Resource
{
    public class ResourceController : MonoBehaviour
    {
        [SerializeField]
        private StageRespawnObject[] stageRespawnObjects;
        [SerializeField]
        private Terrain terrain;
        [SerializeField]
        private ResourcePool resourcePool;

        private int currentStageIndex = 1;

        private Dictionary<ResourceType, List<GameObject>> spawnedResources = new();

        private void OnEnable()
        {
            EventBus.Subscribe<FarmingPhaseStarted>(HandleFarmingStart);
            EventBus.Subscribe<FarmingPhaseEnded>(HandleFarmingEnd);
        }

        private void OnDisable()
        {
            EventBus.UnSubscribe<FarmingPhaseStarted>(HandleFarmingStart);
            EventBus.UnSubscribe<FarmingPhaseEnded>(HandleFarmingEnd);
        }

        private void HandleFarmingStart(FarmingPhaseStarted _)
        {
            SpawnResource(currentStageIndex);
        }

        private void HandleFarmingEnd(FarmingPhaseEnded _)
        {
            ClearResource();
        }

        public void SpawnResource(int stage)
        {
            Debug.Log($"Spawning resource {stage}");
            List<ResourceSpawnInfo> spawnInfos = stageRespawnObjects[stage - 1].SpawnInfos;

            foreach (ResourceSpawnInfo spawnObj in spawnInfos)
            {
                for (int i = 0; i < spawnObj.amount; i++)
                {
                    Vector3 randomPosition = GetRandomPositionOnTerrain(terrain);
                    GameObject obj = resourcePool.Spawn(spawnObj.resource, randomPosition); // ✅ 변경
                    AddResource(spawnObj.resource, obj);
                }
            }
        }

        public void AddResource(ResourceType resourceType, GameObject obj)
        {
            if (spawnedResources.ContainsKey(resourceType))
            {
                spawnedResources[resourceType].Add(obj);
            }
            else
            {
                spawnedResources.Add(resourceType, new List<GameObject>() { obj });
            }
        }

        public void RemoveResource(ResourceType resourceType, GameObject obj)
        {
            if (spawnedResources.TryGetValue(resourceType, out var list))
            {
                list.Remove(obj);
                resourcePool.Release(resourceType, obj); // ✅ 변경
            }
        }

        public void ClearResource()
        {
            foreach (var resourceList in spawnedResources)
            {
                foreach (var obj in resourceList.Value.ToArray())
                {
                    RemoveResource(resourceList.Key, obj);
                }
            }
            spawnedResources.Clear();
        }

        Vector3 GetRandomPositionOnTerrain(Terrain terrain)
        {
            Vector3 terrainSize = terrain.terrainData.size;
            Vector3 terrainOrigin = terrain.GetPosition();

            float x = Random.Range(terrainOrigin.x, terrainOrigin.x + terrainSize.x);
            float z = Random.Range(terrainOrigin.z, terrainOrigin.z + terrainSize.z);
            float y = terrain.SampleHeight(new Vector3(x, 0f, z)) + terrainOrigin.y;

            return new Vector3(x, y, z);
        }
    }
}
