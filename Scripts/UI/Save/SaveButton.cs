using _02.Scripts.Resource;
using _02.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core.Easing;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private GunData riDate;
    [SerializeField] private GunData muData;
    [SerializeField] private SaveShipData ship;
    [SerializeField] private ResourceInventory inventory;
    [SerializeField] private UpgradeButton upgrade;
    [SerializeField] private GameManager gameStageData;

    public void OnSaveButtonClicked()
    {
        if (riDate == null || muData == null || inventory == null || upgrade == null || gameStageData == null)
        {
            Debug.LogWarning("SaveButton: 저장에 필요한 참조가 하나 이상 없습니다.");
            return;
        }

        SaveManager.SaveGame(riDate, muData, ship, inventory, upgrade, gameStageData);
    }
}
