using System.Collections;
using System.Collections.Generic;
using _02.Scripts.Resource;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    public UI_Building ui_building;
    public PlayerController playercontroller;
    public ResourceInventory resourceInventory;
    public bool showPreview = false;
        
    // building Object 데이터를 관리, 인스펙터에서 직접 할당 가능
    [SerializeField] private BuildingData[] buildingObjects;
    
    public BuildingData[] GetBuildingObjects()
    {
        return buildingObjects;
    }

    private void Awake()
    {
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        ui_building = GetComponent<UI_Building>();
        playercontroller = FindObjectOfType<PlayerController>();
        resourceInventory = playercontroller.GetComponent<ResourceInventory>();
    }
    
}