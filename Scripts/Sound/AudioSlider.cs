using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    public AudioMixer mixer;
    public string exposedParam; // "BGMVolume" 또는 "SFXVolume"
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("Slider 컴포넌트를 찾을 수 없습니다!", this);
            return;
        }
        slider.onValueChanged.AddListener(SetVolume);
    }

    private void Start()
    {
        float savedValue = PlayerPrefs.GetFloat(exposedParam, 1f);
        slider.value = savedValue;
        SetVolume(savedValue);
    }

    private void SetVolume(float value)
    {
        mixer.SetFloat(exposedParam, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(exposedParam, value);
    }
}
