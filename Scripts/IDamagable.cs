using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitDirection);
    void Die();
}
