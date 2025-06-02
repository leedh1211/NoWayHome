using _02.Scripts.Resource;
using _02.Scripts;
using UnityEngine;

public class LoadGameSceneInitializer : MonoBehaviour
{
    [SerializeField] private GunData rifleGunData;
    [SerializeField] private GunData mucinGunData;
    [SerializeField] private BuildingData SpaceShipData;
    [SerializeField] private ResourceInventory inventory;
    [SerializeField] private UpgradeButton upgradeButton;

    void Start()
    {
        // 0. 이어하기 여부 체크
        bool shouldContinue = PlayerPrefs.GetInt("ContinueFlag", 0) == 1;
        if (!shouldContinue)
        {
            Debug.Log("새 게임 시작 - 저장 데이터 불러오지 않음");
            return;
        }

        // 1. GameManager에 필요한 참조 연결
        GameManager.Instance.InitGameSceneObjects(inventory, upgradeButton);

        // 2. 저장된 데이터 불러오기
        GameData data = SaveManager.LoadGame();
        if (data == null)
        {
            Debug.Log("저장된 데이터가 없어 기본값으로 시작합니다.");
            return;
        }

        // 3. GameManager에 스테이지 설정
        GameManager.Instance.SetStageIndex(data.gameStageData.stage);

        // 4. GunData 데미지 복원
        upgradeButton.ForceInit();
        upgradeButton.SetRuntimeGunDamage(data.damageData.rifleDamage, data.damageData.mucinDamage, data.shipData.maxHP);

        var spaceship = FindObjectOfType<SpaceShip>();
        if (spaceship != null)
        {
            spaceship.SetHp(data.shipData.maxHP);
            ShipEvents.InvokeHpChanged(spaceship.GetMaxHp(), spaceship.GetMaxHp());
        }
        else
        {
            Debug.LogWarning("SpaceShip 객체를 찾을 수 없습니다.");
        }

        // 5. 강화 정보 복원
        upgradeButton.weaponCount = data.upgradData.weaponCount;
        upgradeButton.shipCount = data.upgradData.shipCount;
        upgradeButton.weaponResources = data.upgradData.weaponResources;
        upgradeButton.shipResources = data.upgradData.shipResources;
        upgradeButton.weaponPercent = data.upgradData.weaponPercent;
        upgradeButton.shipPercent = data.upgradData.shipPercent;

        // 6. 인벤토리 복원
        inventory.ClearAll();
        foreach (var entry in data.inventoryData.entries)
        {
            Item item = Resources.Load<Item>($"ItemPath/{entry.itemId}");
            if (item != null)
            {

                inventory.Add(item, entry.count);
                Debug.Log($"아이템 {entry.itemId}.");
            }
            else
                Debug.Log($"아이템 {entry.itemId} 를 불러올 수 없습니다.");
        }

        Debug.Log(GameManager.Instance.StageIndex);
    }
}
