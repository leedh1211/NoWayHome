using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectData" , menuName = "effect/EffectData")]
public class EffectData : ScriptableObject
{
    public GameObject prefab;
    public EffectType effectType;
    public float duration;           //지속시간
}
