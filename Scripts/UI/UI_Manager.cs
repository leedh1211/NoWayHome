using _02.Scripts;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    public GameObject warningUI;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<DefensePhaseStarted>(DefensePhaseStartHandler);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<DefensePhaseStarted>(DefensePhaseStartHandler);
    }

    private void DefensePhaseStartHandler(DefensePhaseStarted evnt)
    {
        warningUI.SetActive(true);
    }
}
