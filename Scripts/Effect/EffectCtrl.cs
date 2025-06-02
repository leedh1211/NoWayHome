using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EffectCtrl : MonoBehaviour
{
    [SerializeField] private EffectData effectData;
    private void OnEnable()
    {
        Invoke(nameof(StopEffect), effectData.duration);
        SceneManager.sceneLoaded += OnSceneLoadedEvent;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedEvent;
    }

    private void StopEffect()
    {
        EffectPool.Instance.Release(effectData.effectType, this.gameObject);
    }

    private void OnSceneLoadedEvent(Scene scene, LoadSceneMode mode)
    {   //Scene 전환 시 자동으로 비활성화
        CancelInvoke(nameof(StopEffect));
        StopEffect();
    }
}
