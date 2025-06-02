using _02.Scripts;
using UnityEngine;

public class SpaceShip : MonoBehaviour, IInstallable, IDamagable
{
    [Header("Installable Field")]
    public GameObject Installablefield;
    private GameObject _currentInstallablefield;
    [SerializeField] private BuildingData data;
    [SerializeField] private float _size;

    public float GetCurrentHp() => currentHP;
    public float GetMaxHp() => maxHP;

    private float currentHP;
    public float maxHP;

  
    public void Start()
    {
        maxHP = data.maxHp;
        currentHP = maxHP;
        ShipEvents.InvokeHpChanged(currentHP, maxHP);
        CreateInstallableField();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ShipUpgradeEvent>(ShipUpgradeEventHandler);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<ShipUpgradeEvent>(ShipUpgradeEventHandler);
    }

    public void CreateInstallableField()
    {
        if (_currentInstallablefield != null) return;
        
        _currentInstallablefield = Instantiate(Installablefield, transform.position, Quaternion.identity, transform);
        _currentInstallablefield.transform.localScale = Vector3.one * _size;
    }

    public void SetHp(float newMaxHp)
    {
        maxHP = newMaxHp;
        currentHP = maxHP;

        // UI에 초기 체력 반영
        ShipEvents.InvokeHpChanged(currentHP, maxHP);
    }

    public void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitDirection)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);

        ShipEvents.InvokeHpChanged(currentHP, maxHP);
        if (currentHP == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameManager gm = GameManager.Instance;
        //사망 관련 로직
        EventBus.Raise(new GameOverEvent(gm.DefenseDuration - gm.timerController.currentTime, gm.currentKillCount, gm.totalSpawned));
        Destroy(gameObject);
    }

    private void ShipUpgradeEventHandler(ShipUpgradeEvent evnt)
    {
        float tempMaxHP = maxHP;
        data = evnt.ShipData;
        Debug.Log(maxHP - tempMaxHP);
        currentHP += (maxHP - tempMaxHP);
        ShipEvents.InvokeHpChanged(currentHP, maxHP);
    }
}

