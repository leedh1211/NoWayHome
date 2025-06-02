using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BulletUIController : MonoBehaviour
{
    public BulletPooling bulletUI;
    private int currentBullet;
    private int maxBullet = 100;
    private bool firedThisFrame = false;
    public TextMeshProUGUI totalBulletText;

    private void Start()
    {
        currentBullet = maxBullet;
    }


    private void Update()
    { 

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    StartCoroutine(Reload());
        //}
    }

    private void OnEnable()
    {
        EventBus.Subscribe<GunFiredEvent>(OnFired);
        EventBus.Subscribe<GunReloadEvent>(OnGunReload);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<GunFiredEvent>(OnFired);
        EventBus.UnSubscribe<GunReloadEvent>(OnGunReload);
    }

    private void OnFired(GunFiredEvent e)
    {
        Fire();
    }

    private void OnGunReload(GunReloadEvent e)
    {
        maxBullet = e.maxBullet;
        currentBullet = e.currentBullet;
        bulletUI.Reload(currentBullet, maxBullet);
        totalBulletText.text = e.totalBullet.ToString();
    }
    private void Fire()
    {
        currentBullet--;
        bulletUI.UseBullet();
    }

    IEnumerator Reload()
    {
        currentBullet = maxBullet;
        yield return new WaitForSeconds(0.5f);
        //bulletUI.Reload();
    }
}
