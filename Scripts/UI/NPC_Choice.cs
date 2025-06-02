using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Choice : MonoBehaviour
{
    public GameObject choice;
    public GameObject Upgrade;

    public void ShowUp()
    {
        choice.SetActive(false);
        Upgrade.SetActive(true);
    }
}
