using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Monster))]
public class MonsterFlySpawn : MonoBehaviour
{
    [SerializeField] private float dropHeight = 30f;
    [SerializeField] private float dropDuration = 4.0f;
    [SerializeField] private GameObject landingEffectPrefab;
    [SerializeField] private GameObject flyEffectPrefab;
    [SerializeField] private float dropRadius = 1f; // 랜덤 낙하 위치 반경
    

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Monster monster;
    [SerializeField] private Animator animator;
    
    private GameObject flyEffect;

    public void StartDrop(Vector3 targetGroundPosition)
    {
        // 1. 착지 위치를 NavMesh 위로 보정
        if (NavMesh.SamplePosition(targetGroundPosition, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            targetGroundPosition = hit.position; // 보정된 위치로 대체
        }

        // 2. NavMesh 위 보정 완료 → 낙하 시작
        if (agent != null) agent.enabled = false;
        monster.SetSpawning(true);

        Vector3 startPos = targetGroundPosition + Vector3.up * dropHeight;
        transform.position = startPos;

        StartCoroutine(DropToGround(targetGroundPosition));
        animator.SetTrigger("isJump");
    }


    private IEnumerator DropToGround(Vector3 groundPosition)
    {
        float timer = 0f;
        Vector3 start = transform.position;
        GameObject effect = null;
        
        if (flyEffectPrefab != null)
        {
            flyEffect = Instantiate(flyEffectPrefab, transform.position, Quaternion.identity);
            flyEffect.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        while (timer < dropDuration)
        {
            float t = timer / dropDuration;
            Vector3 pos = Vector3.Lerp(start, groundPosition, t);
            pos.y += Mathf.Sin((1 - t) * Mathf.PI) * 2f; // 곡선 낙하
            transform.position = pos;
            if (flyEffect != null)
            {
                flyEffect.transform.position = pos+Vector3.up*3;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = groundPosition;
        OnLanded();
    }

    private void OnLanded()
    {
        Destroy(flyEffect);
        if (landingEffectPrefab != null)
        {
            Instantiate(landingEffectPrefab, transform.position, Quaternion.identity);
        }

        animator?.SetTrigger("isJumpEnd");
        
        agent.enabled = true;
        agent.SetDestination(monster.GetInitialTargetPosition());
        monster.SetSpawning(false);
    }


}