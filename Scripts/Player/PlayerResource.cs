using _02.Scripts.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResource : MonoBehaviour
{
    private Dictionary<ResourceType, int> resourceDict = new Dictionary<ResourceType, int>();       //종류별 리소스의 개수를 관리하는 딕셔너리

    public void AddResource(ResourceType resourceType, int count)
    {
        if (!resourceDict.ContainsKey(resourceType))
        {
            resourceDict.Add(resourceType, count);
        }
        else
        {
            
            resourceDict[resourceType] += count;
        }
        Debug.Log(resourceType + " : " + count + "개 획득!");
    }
}
