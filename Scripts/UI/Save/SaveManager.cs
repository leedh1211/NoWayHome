using System.IO;
using System.Linq;
using UnityEngine;
using _02.Scripts.Resource;
using _02.Scripts;

public static class SaveManager
{
    private static string SavePath => Application.persistentDataPath + "/save.json";

    public static void SaveGame(GunData rifleDamage, GunData mucinDamage,SaveShipData ship,
        ResourceInventory inventory, UpgradeButton upgrade, GameManager Stage)
    {
        GameData data = new GameData
        {
            damageData = new DamageData
            {
                rifleDamage = upgrade.GetRuntimeRifleDamage(),
                mucinDamage = upgrade.GetRuntimeMucinDamage()
            },
            shipData = new SaveShipData
            {
                maxHP = upgrade.GetRuntimeShipHp()
            },
            gameStageData = new GameStageData
            {
                stage = Stage.StageIndex
            },
            upgradData = new UpgradData
            {
                weaponCount = upgrade.weaponCount,
                shipCount = upgrade.shipCount,
                weaponResources = upgrade.weaponResources,
                shipResources = upgrade.shipResources,
                weaponPercent = upgrade.weaponPercent,
                shipPercent = upgrade.shipPercent
            }
        };
        // 인벤토리 데이터 저장
        InventoryData inventoryData = new InventoryData();
        foreach (var pair in inventory.GetDict())
        {
            inventoryData.entries.Add(new ItemEntry
            {
                itemId = pair.Key.id, // 에셋 이름으로 저장
                count = pair.Value
            });
        }

        data.inventoryData = inventoryData;

        // 저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"게임 저장 완료: {SavePath}");
    }

    public static GameData LoadGame()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<GameData>(json);
        }

        Debug.LogWarning("저장 파일이 없습니다.");
        return null;
    }
}
