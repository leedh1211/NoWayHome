using UnityEngine;
using UnityEngine.UI;

public class MouseSensitiveUI : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;

    private void Start()
    {
        float saved = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        sensitivitySlider.value = saved;
        EventBus.Raise(new MouseSensitivityChangedEvent(saved)); // 시작 시 초기 감도 적용
    }

    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        EventBus.Raise(new MouseSensitivityChangedEvent(value));
    }
}
