using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletCtrl : MonoBehaviour
{
    [SerializeField] private BulletData bulletData;
    private float damage = 10f;   //총알이 주는 데미지, 발사체에서 설정해야됨
    private bool hasTrailRenderer;
    private TrailRenderer trailRenderer;
    private Rigidbody rb;


    private void OnEnable()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        hasTrailRenderer = TryGetComponent<TrailRenderer>(out trailRenderer);

        rb.AddForce(transform.forward * bulletData.speed);
        Invoke(nameof(SelfDestruct), bulletData.lifeTime);
        if (hasTrailRenderer)
        {
            trailRenderer.enabled = true;
        }

        SceneManager.sceneLoaded += OnSceneLoadedEvent;
    }

    private void OnDisable()
    {
        Reset();
        CancelInvoke(nameof(SelfDestruct));

        SceneManager.sceneLoaded -= OnSceneLoadedEvent;
    }

    public void Reset()
    {
        rb.position = Vector3.zero;
        rb.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (hasTrailRenderer)
        {
            trailRenderer.Clear();
            trailRenderer.enabled = false;
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    ContactPoint contactPoint = collision.GetContact(0);

    //    Quaternion rot = Quaternion.LookRotation(-contactPoint.normal);

    //    if(bulletData.hitEffect != EffectType.None)
    //    {   //이팩트 효과가 존재할 경우 
    //        EffectPool.Instance.Spawn(bulletData.hitEffect, contactPoint.point, rot);
    //        //이팩트 풀에서 활성화
    //    }

    //    if((1 << collision.gameObject.layer & bulletData.hitLayerMask) != 0)
    //    {   //데미지 피격 대상일 경우
    //        if(collision.collider.TryGetComponent<IDamagable>(out IDamagable damagable))
    //        {
    //            damagable.TakeDamage(bulletData.damage, collision.transform.position);
    //        }
    //    }

    //    BulletPool.Instance.Release(bulletData.bulletType, this.gameObject);
    //}

    private void OnTriggerEnter(Collider other) 
    {
        if ((1 << other.gameObject.layer & bulletData.hitLayerMask) != 0)
        {   // 피격 대상일 경우, 총알 비활성화
            Vector3 hitPoint = transform.position;
            Quaternion rot = Quaternion.LookRotation(-transform.forward);
            if ((1 << other.gameObject.layer & bulletData.damageLayerMask) != 0)
            {   //데미지 처리 대상인 경우, 데미지 처리까지
                if (other.TryGetComponent<IDamagable>(out IDamagable damagable))
                {   //데미지 피격 대상일 경우 데미지 처리만
                    damagable.TakeDamage(damage, hitPoint, -transform.forward);
                }

            }
            else
            {
                EffectPool.Instance.Spawn(bulletData.hitEffect, hitPoint, rot); //피격 이팩트 표시
            }
            BulletPool.Instance.Release(bulletData.bulletType, this.gameObject);
        }
    }

    private void SelfDestruct()        //특정 시간이 지나면 자체 파괴(반환)
    {
        if (this.enabled)
        {
            BulletPool.Instance.Release(bulletData.bulletType, this.gameObject);
        }
    }

    private void OnSceneLoadedEvent(Scene scene, LoadSceneMode mode)
    {   //Scene 전환 시 자동으로 비활성화
        CancelInvoke(nameof(SelfDestruct));
        SelfDestruct();
    }
}
