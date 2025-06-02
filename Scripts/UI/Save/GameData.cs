using _02.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public DamageData damageData;
    public SaveShipData shipData;
    public GameStageData gameStageData;
    public InventoryData inventoryData;
    public UpgradData upgradData;
}

[Serializable]
public class DamageData
{
    public float rifleDamage;
    public float mucinDamage;
}

[Serializable]
public class SaveShipData
{
    public float maxHP;
}

[Serializable]
public class GameStageData
{
    public int stage;
}

[Serializable]
public class InventoryData
{
    public List<ItemEntry> entries = new();
}

[Serializable]
public class ItemEntry
{
    public string itemId;
    public int count;
}

[Serializable]
public class UpgradData
{
    public int weaponCount; // 강화 등급
    public int shipCount;
    public int weaponResources; // 강화 필요 등급
    public int shipResources;
    public int weaponPercent; // 강화 확률
    public int shipPercent;
}
