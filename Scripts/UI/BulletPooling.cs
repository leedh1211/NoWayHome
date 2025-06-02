using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooling : MonoBehaviour
{
    [SerializeField] private GameObject bulletIconPrefab;
    [SerializeField] private Transform parent; // UI에 붙일 부모 (ex: HorizontalLayoutGroup)
    [SerializeField] private int maxBullet = 100;
    private int curBulletIndex = 0;

    private List<GameObject> bulletIcons = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < maxBullet; i++)
        {
            GameObject icon = Instantiate(bulletIconPrefab, parent);
            bulletIcons.Add(icon);
        }
        curBulletIndex = maxBullet - 1;
    }

    private void Start()
    {
        Reload(25, 25);
    }

    public void UseBullet()
    {
        //for (int i = bulletIcons.Count - 1; i >= 0; i--)
        //{
        //    if (bulletIcons[i].activeSelf)
        //    {
        //        bulletIcons[i].SetActive(false);
        //        break;
        //    }
        //}
        bulletIcons[curBulletIndex--].SetActive(false);
    }

    public void Reload(int maxBullet, int currentBullet)
    {
        foreach(GameObject icon in bulletIcons) 
        {
            icon.SetActive(false);
        }
        //탄창을 꽉 채울만큼 탄알이 충분하지 않을 수 있으므로 현재 총알 개수로 가져옴
        this.maxBullet = maxBullet;
        for(int i = 0; i < currentBullet; i++)
        {
            bulletIcons[i].SetActive(true);
        }
        curBulletIndex = currentBullet - 1;
        //foreach (var icon in bulletIcons)
        //{
        //    icon.SetActive(true);
        //}
    }
}
