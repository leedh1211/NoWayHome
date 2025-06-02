using UnityEngine;

public class PhaseMusicController : MonoBehaviour
{
    [SerializeField] private AudioClip farmingBGM;
    [SerializeField] private AudioClip defenseBGM;

    private void OnEnable()
    {
        EventBus.Subscribe<FarmingPhaseStarted>(OnFarmingStart);
        EventBus.Subscribe<DefensePhaseStarted>(OnDefenseStart);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<FarmingPhaseStarted>(OnFarmingStart);
        EventBus.UnSubscribe<DefensePhaseStarted>(OnDefenseStart);
    }

    private void OnFarmingStart(FarmingPhaseStarted _)
    {
        SoundManager.Instance.PlayBGM(farmingBGM);
    }

    private void OnDefenseStart(DefensePhaseStarted _)
    {
        SoundManager.Instance.PlayBGM(defenseBGM);
    }
}
