using System.Collections;
using _02.Scripts;
using UnityEngine;

public class PlayerStat : MonoBehaviour, IDamagable
{
    private float baseWalkSpeed = 3.0f;
    private float baseRunSpeed = 6.0f;
    private float baseMaxHP = 100f;
    private float baseMaxStamina = 100f;
    private float baseMaxHunger = 100f;
    private float baseMaxWater = 100f;
    private float baseJumpHeight = 1.2f;

    public float currentHP { get; private set; }        // 체력
    public float currentStamina { get; private set; }   //스태미나
    public float currentHunger { get; private set; }    //배고픔
    public float currentWater { get; private set; }     //목마름

    [Header("추가 스탯")]
    [SerializeField] private float bonusSpeed;
    [SerializeField] private float bonusMaxHP;
    [SerializeField] private float bonusMaxStamina;
    [SerializeField] private float bonusMaxHunger;
    [SerializeField] private float bonusMaxWater;
    [SerializeField] private float bonusJumpHeight;

    public float CurrentWalkSpeed => baseWalkSpeed + bonusSpeed;
    public float CurrentRunSpeed => baseRunSpeed + bonusSpeed;
    public float CurrentMaxHP => baseMaxHP + bonusMaxHP;
    public float CurrentMaxStamina => baseMaxStamina + bonusMaxStamina;
    public float CurrentMaxHunger => baseMaxHunger + bonusMaxHunger;
    public float CurrentMaxWater => baseMaxWater + bonusMaxWater;
    public float CurrentJumpHeight => baseJumpHeight + bonusJumpHeight;


    [Header("기본 배고픔, 목마름 감소")]
    [SerializeField] private float reduceDelay;
    [SerializeField] private float hungerReducePerTic;
    [SerializeField] private float waterReducePerTic;
    private float lastReduceTime;

    [Header("배고픔 상태")]
    [SerializeField] private float hungryDamagePerTic;
    [SerializeField] private float hungryDamageDelay;
    private Coroutine hungryRoutine;

    [Header("목마름 상태")]
    [SerializeField] private float thirstySpeedReduce;
    private Coroutine thirstyRoutine;

    [Header("피격 처리")]
    [SerializeField] private float damageDelay = 2f;     //피격 후 무적 시간
    [SerializeField] private bool isInvincible = false;  //피격, 구르기 시 무적
    [SerializeField] private bool isDamaged = false;  //피해를 받은 상태
    [SerializeField] private AudioClip[] damageClips;   //피격 효과음 모음
    private float lastDamageTime;
    private Animator animator;
    private int animIDDamaged = Animator.StringToHash("Damaged");
    private int animIDead = Animator.StringToHash("Dead");

    private bool isDead = false;
    private float passiveStatInterval = 0.5f;   //기본 목마름, 배고픔 감소 언터벌
    private float passiveStatTimer;

    private void Start()
    {
        currentHP = CurrentMaxHP;
        currentStamina = CurrentMaxStamina;
        currentHunger = CurrentMaxHunger;
        currentWater = CurrentMaxWater;
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Health, currentHP, CurrentMaxHP));
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Stamina, currentStamina, CurrentMaxStamina));
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Hunger, currentHunger, CurrentMaxHunger));
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Water, currentWater, CurrentMaxWater));

        if(!TryGetComponent<Animator>(out animator))
        {
            Debug.Log(this.name + " : Cannot Find Animator Component");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (isDead) return;

        //if(Time.time >= lastReduceTime + reduceDelay)
        //{
        //    lastReduceTime = Time.time;
        //    currentHunger -= hungerReducePerTic;
        //    currentWater -= waterReducePerTic;
        //    EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Hunger, currentHunger, CurrentMaxHunger));
        //    EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Water, currentWater, CurrentMaxWater));
        //}
        if(Time.time >= passiveStatTimer + passiveStatInterval)
        {
            passiveStatTimer = Time.time;
            Digest(0.25f);
            DeHydrate(0.25f);
        }
        

        if (isDamaged && Time.time >= lastDamageTime + damageDelay)
        {
            isDamaged = false;
            animator.SetBool(animIDDamaged, false);
        }
    }

    public void Heal(float value)
    {
        if (value < 0 || isDead) return;

        currentHP = Mathf.Min(currentHP + value, CurrentMaxHP);
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Health, currentHP, CurrentMaxHP));
    }

    public void TakeDamage(float damage, Vector3 hitPosition,  Vector3 hitDirection)
    {
        if(isInvincible || isDamaged || isDead) return;

        int index = Random.Range(0, damageClips.Length);
        SoundManager.Instance.PlaySFX(damageClips[index], transform.position);
        lastDamageTime = Time.time;
        isDamaged = true;
        animator.SetBool(animIDDamaged, true);

        currentHP -= damage;
        Debug.Log("Player Damaged! Currnet Health : " + currentHP + "/" + CurrentMaxHP);
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Health, currentHP, CurrentMaxHP));
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    public bool UseStamina(float value)
    {
        if (value < 0 || isDead) return false;

        if(value > currentStamina) return false;

        currentStamina = currentStamina - value;
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Stamina, currentStamina, CurrentMaxStamina));

        return true;
    }

    public void RecoverStamina(float value)
    {
        if (value < 0 || isDead) return;
        Debug.Log("stamina recover");
        currentStamina = Mathf.Min(currentStamina + value, CurrentMaxStamina);
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Stamina, currentStamina, CurrentMaxStamina));
    }

    public void Eat(float value)        
    {
        if (value < 0 || isDead) return;
        
        currentHunger = Mathf.Min(currentHunger + value, CurrentMaxHunger);
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Hunger, currentHunger, CurrentMaxHunger));
    }

    public void Digest(float value)
    {
        if (value < 0 || isDead) return;

        currentHunger -= value;
        if(currentHunger <= 0)
        {
            currentHunger = 0;
            if(hungryRoutine == null)
            {
                hungryRoutine = StartCoroutine(HungryRoutine());
            }
        }
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Hunger, currentHunger, CurrentMaxHunger));
    }

    public void Hydrate(float value)
    {
        if (value < 0 || isDead) return;

        currentWater = Mathf.Min(currentWater + value, CurrentMaxWater);
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Water, currentWater, CurrentMaxWater));
    }

    public void DeHydrate(float value)
    {
        if (value < 0 || isDead) return;

        currentWater -= value;
        if (currentWater <= 0)
        {
            currentWater = 0;
            if(thirstyRoutine == null)
            {
                thirstyRoutine = StartCoroutine(ThirstyRoutine());
            }
        }
        EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Water, currentWater, CurrentMaxWater));
    }

    public void Die()
    {
        GameManager gm = GameManager.Instance;
        //사망 관련 로직
        EventBus.Raise(new GameOverEvent(gm.DefenseDuration - gm.timerController.currentTime, gm.currentKillCount, gm.totalSpawned));
        animator.SetBool(animIDead, true);
        isDead = true;
        StopAllCoroutines();
    }

    IEnumerator HungryRoutine() //배고픔이 0이면 주기적으로 체력 감소
    {
        while (currentHunger == 0)
        {
            currentHP -= hungerReducePerTic;
            EventBus.Raise(new PlayerConditionChangedEvent(PlayerConditionType.Health, currentHP, CurrentMaxHP));
            if (currentHP <= 0)
            {
                currentHP = 0;
                Die();
            }
            yield return new WaitForSeconds(2.0f);
        }
    }
    IEnumerator ThirstyRoutine()    //목마름 수치가 0이면 이동속도 감소
    {
        baseWalkSpeed -= thirstySpeedReduce;
        baseRunSpeed -= thirstySpeedReduce;
        while (currentWater == 0)
        {
            yield return new WaitForSeconds(1f);
        }
        baseWalkSpeed += thirstySpeedReduce;
        baseRunSpeed += thirstySpeedReduce;
    }

    public void SetInvincible(bool invincible)      //무적 상태 설정
    {
        isInvincible = invincible;
    }
}
