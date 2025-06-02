using UnityEngine;

[CreateAssetMenu(fileName = "Turret", menuName = "New Turret")]
public class TurretData : ScriptableObject
{
    [Header("Turret Settings")]
    public float maxHp = 500f;
    public float currentHp = 500f;
    public float attackRange = 30f;
    public float fireDelay = 0.25f;
    public float attackCooldown = 1f;
    public GameObject bulletPrefab;
}
