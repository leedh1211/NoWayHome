using _02.Scripts.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunCtrl : MonoBehaviour
{
    public enum GunState
    {
        Ready,      //발사 준비됨
        Empty,      //탄알창이 빔
        Reloading   //재장전 중
    }

    [Header("총 설정")]
    [SerializeField] private GameObject gun;                
    [SerializeField] private GunData gunData;
    [SerializeField] private BulletData bulletData;             //발사 할 총알 데이터
    [SerializeField] private Transform firePos;
    [SerializeField] private ParticleSystem muzzleFlash; //발사 시 총구에서 나오는 이팩트
    [SerializeField] private Item currentBulletData; // 사용하는 총알 데이터
    [SerializeField] private ResourceInventory playerInventory;

    // public int ammoRemain { get; private set; }      //남은 전체 탄알
    public int magAmmo { get; private set; }         //현재 탄알집에 남아 있는 탄알

    private float Damage => gunData.damage;     //발사하는 총알의 데미지

    private GunState state;
    private float lastFireTime;

    private void Awake()
    {
        magAmmo = gunData.magCapacity;
    }

    private void Start()
    {
        playerInventory.Add(currentBulletData,gunData.startAmmoRemain);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<WeaponUpgradeEvent>(WeaponUpgradeEventHandler);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<WeaponUpgradeEvent>(WeaponUpgradeEventHandler);
    }

    public void GunOn()
    {
        EventBus.Raise(new GunReloadEvent(magAmmo, gunData.magCapacity, GetAmmoRemain()));
        gun.SetActive(true);
        state = GunState.Ready;
        lastFireTime = 0;
    }

    public void GunOff()
    {
        gun.SetActive(false);
    }

    public bool Fire()
    {
        if (state == GunState.Ready && Time.time >= lastFireTime + gunData.timeBetFire)
        {
            lastFireTime = Time.time;
            Vector3 hitPosition = Vector3.zero;

            //RaycastHit hit;
            //if (Physics.Raycast(firePos.position, firePos.forward, out hit, attackDistance, enemyLayer))
            //{
            //    Debug.Log("Bullet hit");
            //    hitPosition = hit.point;
            //    if (hit.collider.TryGetComponent<IDamagable>(out IDamagable damagable))
            //    {   //IDamagable 객체를 가져와서
            //        Debug.Log("Enemy hit");
            //        damagable.TakeDamage(gunData.damage, hitPosition);  //데미지 처리
            //    }
            //}
            //데미지 처리를 RayCasting이 아닌 BulletCtrl 에서 직접 하기로 변경

            GameObject bullet =  BulletPool.Instance.Spawn(bulletData.bulletType, firePos.position , firePos.rotation);

            if(bullet.TryGetComponent<BulletCtrl>(out BulletCtrl bulletCtrl))
            {
                bulletCtrl.SetDamage(Damage);
            }

            StartCoroutine(ShotEffect(hitPosition));

            EventBus.Raise(new GunFiredEvent());
            if (--magAmmo <= 0)
            {
                state = GunState.Empty;     //탄창에 남은 탄알이 없음
            }

            return true;
        }
        return false;
    }

    public bool Reload()
    {
        if (state == GunState.Reloading || GetAmmoRemain() <= 0 || magAmmo >= gunData.magCapacity)
        {
            //이미 재장전 중이거나 남은 탄알이 없거나 탄창에 이미 탄알이 가득한 경우
            return false;
        }

        StartCoroutine(ReloadRoutine());
        return true;
    }

    private IEnumerator ReloadRoutine()
    {
        // 현재 상태를 재장전 중 상태로 전환
        state = GunState.Reloading;
        SoundManager.Instance.PlaySFX(gunData.reloadClip, transform.position);

        // 재장전 소요 시간 만큼 처리 쉬기
        yield return new WaitForSeconds(gunData.reloadTime);

        int ammoToFill = gunData.magCapacity - magAmmo;

        int ammoRemain = GetAmmoRemain();

        if (ammoRemain < ammoToFill)
        {
            //남은 탄알이 탄창에 채워야 하는 수 보다 부족한 경우
            ammoToFill = ammoRemain;
        }
        magAmmo += ammoToFill;
        playerInventory.Consume(currentBulletData, ammoToFill);
        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = GunState.Ready;
        EventBus.Raise(new GunReloadEvent(magAmmo, gunData.magCapacity, GetAmmoRemain()));
    }

    //발사 이펙트와 소리를 재생하고 탄알 궤적을 그림
    private IEnumerator ShotEffect(Vector3 hitPoint)
    {
        SoundManager.Instance.PlaySFX(gunData.shotClip, firePos.position);
        muzzleFlash.Play();
        yield return new WaitForSeconds(0.03f);
    }
    
    private void WeaponUpgradeEventHandler(WeaponUpgradeEvent evnt)
    {
        switch (gunData.GunType)
        {
            case GunType.Rifle:
                gunData = evnt.RifleData;
                break;
            case GunType.MachineGun:
                gunData = evnt.MachineData;
                break;
        }
    }

    private int GetAmmoRemain()
    {
        return playerInventory.GetItemAmount(currentBulletData);
    }
}
