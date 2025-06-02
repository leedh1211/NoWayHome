using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUpgradeButton : MonoBehaviour
{
    [SerializeField] private GameObject EscMenu;
    [SerializeField] private GameObject SettingMenu;

    public void Update()
    {
        if (EscMenu.activeSelf)
        {
            SettingMenu.SetActive(true);
        }
        else
        {
            SettingMenu.SetActive(false);
        }
    }
}
