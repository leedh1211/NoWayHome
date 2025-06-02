using System;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    None,
    Spark,          //스파크 효과
    Explosion,      //폭발 효과
    Bleed           //출혈 효과
}

public class EffectPool : Singleton<EffectPool>
 {
     [SerializeField]
     private EffectData[] _effectPrefab;
     [SerializeField]
     private int _effectCount = 5;
     private Dictionary<EffectType, GameObject> _effectPrefabDictionary = new();
     private Dictionary<EffectType, Queue<GameObject>> _effectQueue = new();
     private Transform _effectParent;
     private bool IsInitialized;

     public static event Action OnPoolReady;


     protected override void Awake()
     {
        base.Awake();
         if (_effectParent == null)
         {
             _effectParent = new GameObject("EffectPoolParent").transform;
            DontDestroyOnLoad(_effectParent);
         }

        _effectPrefab = Resources.LoadAll<EffectData>("ScriptableObjects/Effect");

         foreach (var effectPrefab in _effectPrefab)
         {
             if (!_effectPrefabDictionary.ContainsKey(effectPrefab.effectType))
             {
                 _effectPrefabDictionary.Add(effectPrefab.effectType, effectPrefab.prefab);
             }
         }

         EffectPoolAllType(_effectCount);
         IsInitialized = true;
         OnPoolReady?.Invoke();
     }

     public void EffectPoolAllType(int amount)
     {
         Debug.Log("EffectPoolAllType");
         foreach (var effect in _effectPrefabDictionary)
         {
             for (int i = 0; i < amount; i++)
             {
                 GameObject prefab = effect.Value;
                 EffectType effectType = effect.Key;
                 var obj = Instantiate(prefab, _effectParent);
                 obj.SetActive(false);
                 if (!_effectQueue.ContainsKey(effectType))
                 {
                     _effectQueue[effectType] = new Queue<GameObject>();
                 }
                 _effectQueue[effectType].Enqueue(obj);
             }
         }
     }

     public void EffectPoolTargetType(EffectType effectType, int amount)
     {
         for (int i = 0; i < amount; i++)
         {
             GameObject prefab = _effectPrefabDictionary[effectType];
             var obj = Instantiate(prefab, _effectParent);
             obj.SetActive(false);
             if (!_effectQueue.ContainsKey(effectType))
             {
                 _effectQueue[effectType] = new Queue<GameObject>();
             }
             _effectQueue[effectType].Enqueue(obj);
         }
     }

     public GameObject Spawn(EffectType effectType, Vector3 position, Quaternion rotation)
     {
        if (effectType == EffectType.None) return null;

         if (!_effectQueue.TryGetValue(effectType, out var queue))
         {
             EffectPoolTargetType(effectType, 1);
             queue = _effectQueue[effectType];
         }
         else if (queue.Count == 0)
         {
             EffectPoolTargetType(effectType, 1);
             queue = _effectQueue[effectType];
         }

         GameObject obj = queue.Dequeue();

         obj.transform.position = position;
         obj.transform.rotation = rotation;
         obj.SetActive(true);
         return obj;
     }

     public void Release(EffectType effectType, GameObject obj)
     {
         if (!_effectQueue.TryGetValue(effectType, out var queue))
         {
             Destroy(obj);
             return;
         }

         obj.SetActive(false);
         queue.Enqueue(obj);
     }

     public void ReleaseAll()
     {
         _effectQueue.Clear();
     }
 }
