using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour, IDamagable
{
    [SerializeField] private BuildingData data;
    [SerializeField] private BulletData bulletData;
    private Transform currentTarget;
    private int firePointIndex = 0;
    private float cooldownTimer = 0f;

    [SerializeField] private Transform headPivot;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private int _turretSettingIndex = 0;

    [SerializeField] private AudioClip fireSound;

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            AcquireTarget();

            if (currentTarget != null)
            {
                RotateHeadToTarget(); // 타겟 방향 바라보기
                StartCoroutine(ShootBurst());
                cooldownTimer = data.settings[_turretSettingIndex].attackCooldown + data.settings[_turretSettingIndex].fireDelay;
            }
        }
    }

    void AcquireTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, data.settings[_turretSettingIndex].attackRange, LayerMask.GetMask("Enemy"));

        float minDistance = float.MaxValue;
        Transform nearest = null;

        foreach (var enemy in enemies)
        {
            Monster monster = enemy.GetComponent<Monster>();
            if (monster == null || monster.isDead) continue;

            Vector3 targetPos = monster.transform.position + Vector3.up * 0.5f; // 타겟 중앙 약간 위
            Vector3 direction = targetPos - transform.position;
            float distance = direction.magnitude;

            // 장애물에 Raycast 쏘기
            if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, LayerMask.GetMask("Building")))
            {
                // 만약 Ray가 부딪힌 대상이 자기 자신이 아니면 => 장애물로 간주하고 스킵
                if (hit.collider.gameObject != gameObject)
                {
                    continue;
                }
            }

            // 시야에 장애물 없을 경우 거리 비교 후 갱신
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = monster.transform;
            }
        }

        currentTarget = nearest;
    }

    void RotateHeadToTarget()
    {
        if (headPivot == null || currentTarget == null) return;

        Vector3 direction = currentTarget.position - headPivot.position;
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        Vector3 fixedEuler = lookRotation.eulerAngles;
        headPivot.rotation = Quaternion.Euler(fixedEuler);
    }

    IEnumerator ShootBurst()
    {
        if (currentTarget == null) yield break;

        for (int i = 0; i < 2; i++)
        {
            Transform firePoint = firePoints[firePointIndex];
            firePointIndex = (firePointIndex + 1) % firePoints.Length;
            Vector3 targetPos = currentTarget.position + Vector3.up * 1.0f;
            Vector3 dir = (targetPos - firePoint.position).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);

            BulletPool.Instance.Spawn(bulletData.bulletType, firePoint.position, rot);

            if (fireSound != null)
            {

                Debug.Log("터렛 발사");
                SoundManager.Instance.PlaySFX(fireSound, firePoint.position);
            }

            yield return new WaitForSeconds(data.settings[_turretSettingIndex].fireDelay);
        }
    }

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
