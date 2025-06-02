using _02.Scripts.Resource;
using _02.Scripts;
using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    [SerializeField] private UpgradeButton upgradeButton;
    [SerializeField] private ResourceInventory inventory;

    void Start()
    {
        var inventory = FindObjectOfType<ResourceInventory>();
        var upgrade = FindObjectOfType<UpgradeButton>();

        if (inventory == null)
        {
            Debug.LogError("초기화 대상이 씬에 존재하지 않습니다.");
            return;
        }

        GameManager.Instance.InitGameSceneObjects(inventory,upgradeButton);
    }
}

