using System;
using System.Collections;
using _02.Scripts.Monster;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour, IDamagable
{
    private float maxHp = 100f;
    private float currentHp;
    private float lastAttackTime = 0f;
    private bool isMoving;
    public bool isDead;
    public BulletType bulletType;
    public EffectType hitEffectType;
    public Transform firePoint;                    // 발사 위치
    private float attackRange;    // 공격 사거리
    [SerializeField] private GameObject originPrefab;
    [SerializeField] private NavMeshAgent agent;
    public Action OnDeath;
    private bool isSpawning = false;

    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deadSound;

    public float AttackRange
    {
        get { return attackRange; }
        set => attackRange = value;
    }

    private float attackCooldown; // 공격 속도
    public float AttackCooldown
    {
        get { return attackCooldown; }
        set => attackCooldown = value;
    }

    private ObjectPooler pooler;
    [SerializeField] private Transform rocket;
    private Transform currentTarget;
    [SerializeField] private Animator animator;

    public void Init()
    {
        currentHp = maxHp;
        isMoving = false;
        isDead = false;
        lastAttackTime = 0f;
        currentTarget = rocket;
        agent.SetDestination(rocket.position);
    }

    void Update()
    {
        if (isSpawning || isDead) return;
        SearchAndAct();
    }

    private void SearchAndAct()
    {
        if (isDead) return;

        IDamagable closestTarget = FindClosestTargetInRange();

        if (closestTarget != null)
        {
            MonoBehaviour mb = closestTarget as MonoBehaviour;
            if (mb != null)
            {
                currentTarget = mb.transform;
            }
            Move(false);
            Attack();
        }
        else
        {
            currentTarget = rocket;
            Move(true);
        }
    }

    private IDamagable FindClosestTargetInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, AttackRange);
        IDamagable closestTarget = null;
        float closestDistance = Mathf.Infinity;

        int monsterLayer = LayerMask.NameToLayer("Enemy");

        foreach (var col in hitColliders)
        {
            if (col.gameObject == gameObject || col.gameObject.layer == monsterLayer) continue;

            IDamagable damagable = col.GetComponent<IDamagable>();
            if (damagable != null)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestTarget = damagable;
                }
            }
        }

        return closestTarget;
    }

    public void Move(bool isMove)
    {
        if (isMoving == isMove) return;
        isMoving = isMove;
        agent.isStopped = !isMove;
        if (isMove)
        {
            agent.SetDestination(rocket.position);
        }
        animator.SetBool("isMoving", isMove);
    }

    public void Attack()
    {
        if (Time.time - lastAttackTime >= AttackCooldown && currentTarget != null)
        {
            Vector3 lookDirection = (currentTarget.position - transform.position).normalized;
            lookDirection.y = 0f;
            transform.forward = lookDirection;

            animator?.SetTrigger("Shot");
            Shoot();

            lastAttackTime = Time.time;
        }
    }

    void Shoot()
    {
        if (currentTarget == null) return;

        Vector3 targetPos = currentTarget.position + Vector3.up * 1f;
        Vector3 dir = (targetPos - firePoint.position).normalized;

        GameObject bullet = BulletPool.Instance.Spawn(bulletType, firePoint.position, Quaternion.LookRotation(dir));
    }

    public void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitDirection)
    {
        if (isDead) return;

        if (hitSound != null)
        {
            SoundManager.Instance.PlaySFX(hitSound, hitPosition);

        }
        animator.SetTrigger("Hit");
        currentHp = Mathf.Clamp(currentHp - damage, 0, maxHp);
        Quaternion direction = Quaternion.LookRotation(-hitDirection);
        EffectPool.Instance.Spawn(hitEffectType, hitPosition, direction);
        if (currentHp > 0)
        {
            PlayHitReaction(hitDirection);
        }
        else
        {
            Die();
        }
    }

    private void PlayHitReaction(Vector3 hitDirection)
    {
        Vector3 monsterRight = transform.right;
        float dot = Vector3.Dot(monsterRight, hitDirection.normalized);

        if (dot > 0)
        {
            animator.SetTrigger("Hit_R");
        }
        else
        {
            animator.SetTrigger("Hit_L");
        }
    }

    public void Die()
    {
        agent.isStopped = true;
        isDead = true;
        agent.velocity = Vector3.zero;
        animator.SetBool("isMoving", false);
        animator.SetTrigger("Die");
        OnDeath?.Invoke();
        OnDeath = null;
        if (deadSound != null)
        {
            SoundManager.Instance.PlaySFX(deadSound, firePoint.position);
        }
        StartCoroutine(DelayReturnToPool());
    }

    private IEnumerator DelayReturnToPool()
    {
        yield return new WaitForSeconds(5f);
        pooler.ReturnToPool(originPrefab, gameObject);
    }

    public void Inject(Transform rocket, ObjectPooler pooler, MonsterData data)
    {
        this.rocket = rocket;
        this.pooler = pooler;
        originPrefab = data.prefab;
        maxHp = data.MaxHP;
        agent.speed = data.Speed;
        AttackCooldown = data.AttackCooldown;
        AttackRange = data.AttackRange;

        Init();
    }

    public Vector3 GetInitialTargetPosition()
    {
        return currentTarget != null ? currentTarget.position : transform.position;
    }

    public void SetSpawning(bool value)
    {
        isSpawning = value;
    }
}
