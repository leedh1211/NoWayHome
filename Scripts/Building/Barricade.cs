using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour,  IDamagable
{
    [SerializeField] private BuildingData data;
    
    public void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitDirection)
    {
        data.currentHp = Mathf.Clamp(data.currentHp - damage, 0, data.maxHp);

        if (data.currentHp == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
