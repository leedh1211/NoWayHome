using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "bullet/BulletData")]
public class BulletData : ScriptableObject
{
    public GameObject prefab;
    public float speed;                 // 총알 속력
    public float lifeTime;              //자체 파괴까지 걸리는 시간
    public BulletType bulletType;       
    public EffectType hitEffect;        // 총알 피격 효과
    public LayerMask hitLayerMask;      //피격 대상 레이어 마스크(닿으면 사라짐)
    public LayerMask damageLayerMask;      //데미지 처리 레이어 마스크

}