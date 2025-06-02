using _02.Scripts.Resource;
using System.Collections;
using TMPro;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private GameObject UpgradeWindow;
    [SerializeField] private GameObject tradeWindow;
    [SerializeField] private GameObject[] images;
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private TextMeshProUGUI[] showText;
    [SerializeField] private AudioClip[] soundClips;
    private AudioSource audioSource;
    public int weaponCount = 0;
    public int shipCount = 0;
    public int weaponResources = 1;
    public int shipResources = 1;
    public int weaponPercent = 100;
    public int shipPercent = 100;
    public int weaponAtteck = 10;
    public int shipHp = 100;
    private int whatKind = 1;
    private Coroutine coroutine;
    [SerializeField]private GunData RIfleData;
    [SerializeField] private GunData MucinData;
    [SerializeField] private Item itemToUse;
    private GunData runtimeRifleData;
    private GunData runtimeMucinData;
    [SerializeField] private BuildingData ShipHp;
    private BuildingData SpaceShipHP;
    private ResourceInventory resourceInventory;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        resourceInventory = FindObjectOfType<ResourceInventory>();

        int continueFlag = PlayerPrefs.GetInt("ContinueFlag", 0);
        if (continueFlag == 0)
        {
            runtimeRifleData = Instantiate(RIfleData);
            runtimeMucinData = Instantiate(MucinData);
            SpaceShipHP = Instantiate(ShipHp);
            ResetWeaponStats(); // 초기화 메서드 호출
        }
        ShowWeaponUp();
        ShowShipUp();
    }

    public void ForceInit()
    {
        if (runtimeRifleData == null)
            runtimeRifleData = Instantiate(RIfleData);
        if (runtimeMucinData == null)
            runtimeMucinData = Instantiate(MucinData);
        if(SpaceShipHP == null)
            SpaceShipHP = Instantiate(ShipHp);
    }

    public void SetRuntimeGunDamage(float rifleDamage, float mucinDamage, float shipHP)
    {
        runtimeRifleData.damage = rifleDamage;
        runtimeMucinData.damage = mucinDamage;
        SpaceShipHP.maxHp = shipHP;
        EventBus.Raise(new WeaponUpgradeEvent(runtimeRifleData, runtimeMucinData)); 
        EventBus.Raise(new ShipUpgradeEvent(SpaceShipHP));
    }

    private void ResetWeaponStats()
    {
        runtimeRifleData.damage = RIfleData.damage; // 원본 damage로 초기화
        runtimeMucinData.damage = MucinData.damage;
        SpaceShipHP.maxHp = ShipHp.maxHp;
    }

    public float GetRuntimeRifleDamage()
    {
        return runtimeRifleData.damage;
    }
    public float GetRuntimeShipHp()
    {
        return SpaceShipHP.maxHp;
    }
    public float GetRuntimeMucinDamage()
    {
        return runtimeMucinData.damage;
    }

    public void WeaponImages()
    {
        if (images[0].activeSelf) return;
        PlaySound(0);
        whatKind = 1;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].SetActive(i < 2);
        }
    }

    public void ShipImages()
    {
        if (images[2].activeSelf) return;
        PlaySound(0);
        whatKind = 2;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].SetActive(i >= 2);
        }
    }

    public void WeaponUpgradeSelet()
    {
        if (weaponPercent == 0) return;
        if (coroutine != null) return;
        coroutine =  StartCoroutine(ShowStartParticle());
    }

    public void ShipUpgradeSelet()
    {
        if (shipPercent == 0) return;
        if (coroutine != null) return;
        coroutine = StartCoroutine(ShowStartParticle());
    }

    private void ShowWeaponUp()
    {
        showText[0].text = "+ " + weaponCount.ToString();
        showText[1].text = " : " + weaponResources.ToString();
        showText[2].text = "강화 성공 확률 : " + weaponPercent.ToString() + "%";
        showText[6].text = "라이플 공격력 : " + runtimeRifleData.damage.ToString();
        showText[7].text = "머신건 공격력 : " + runtimeMucinData.damage.ToString();
    }

    private void ShowShipUp()
    {
        showText[3].text = "+ " + shipCount.ToString();
        showText[4].text = " : " + shipResources.ToString();
        showText[5].text = "강화 성공 확률 : " + shipPercent.ToString() + "%";
        showText[8].text = "우주선 체력 : " + SpaceShipHP.maxHp.ToString();
    }

    IEnumerator ShowStartParticle()
    {
        
        if (whatKind == 2)
        {
            bool cando = resourceInventory.Consume(itemToUse, shipResources);
            if (!cando)
            {
                PlaySound(4);
                yield break;
            }
            int value = shipPercent / 10;
            int result = Random.Range(value, 11);
            PlaySound(3);
            particleSystems[2].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystems[2].Play();
            yield return new WaitForSeconds(1f);
            if (result <= value)
            {
                PlaySound(1);
                shipCount++;
                shipResources += 1;
                shipPercent -= 10;
                SpaceShipHP.maxHp += shipHp;
                showText[3].text = "+ " + shipCount.ToString();
                showText[4].text = " : " + shipResources.ToString();
                showText[5].text = "강화 성공 확률 : " + shipPercent.ToString() + "%";
                showText[8].text = "우주선 체력 : " + SpaceShipHP.maxHp.ToString();
                particleSystems[0].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleSystems[0].Play();
                EventBus.Raise(new ShipUpgradeEvent(SpaceShipHP));
            }
            else
            {
                PlaySound(2);
                particleSystems[1].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleSystems[1].Play();
            }
        }
        else if (whatKind == 1)
        {
            bool cando = resourceInventory.Consume(itemToUse, weaponResources);
            if (!cando) 
            {
                PlaySound(4);
                yield break;
            }
            PlaySound(3);
            particleSystems[2].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystems[2].Play();
            yield return new WaitForSeconds(1f);
            int value = weaponPercent / 10;
            int result = Random.Range(value, 11);
            if (result <= value)
            {
                Debug.Log("강화 성공!");
                PlaySound(1);
                weaponCount++;
                weaponResources += 1;
                weaponPercent -= 10;
                runtimeRifleData.damage += weaponAtteck;
                runtimeMucinData.damage += weaponAtteck;
                showText[0].text = "+ " + weaponCount.ToString();
                showText[1].text = " : " + weaponResources.ToString();
                showText[2].text = "강화 성공 확률 : " + weaponPercent.ToString() + "%";
                showText[6].text = "라이플 공격력 : " + runtimeRifleData.damage.ToString();
                showText[7].text = "머신건 공격력 : " + runtimeMucinData.damage.ToString();
                particleSystems[0].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleSystems[0].Play();
                EventBus.Raise(new WeaponUpgradeEvent(runtimeRifleData, runtimeMucinData));
            }
            else
            {
                PlaySound(2);
                particleSystems[1].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleSystems[1].Play();
            }
        }
        coroutine = null;
    }

    public void ShowUpgradeWindow()
    {
        UpgradeWindow.SetActive(false);
        tradeWindow.SetActive(true);
    }

    public void PlaySound(int index)
    {
        if (index >= 0 && index < soundClips.Length)
        {
            audioSource.PlayOneShot(soundClips[index]);
        }
    }
}
