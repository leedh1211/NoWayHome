using _02.Scripts;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Mixer & AudioSources")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioMixerGroup sfxMixerGroup; 

    [Header("초기 BGM")]
    [SerializeField] private AudioClip audioClip;

    private void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        ApplySavedVolume(); // PlayerPrefs에서 볼륨 적용
    }

    private void ApplySavedVolume()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(master) * 20);

        float bgm = PlayerPrefs.GetFloat("BGMVolume", 1f);
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(bgm) * 20);

        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfx) * 20);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LobbyScene")
        {
            PlayBGM(audioClip);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1.0f) // SFX 변경 함수
    {
        if (clip == null) return;

        GameObject temp = new GameObject("TempSFX");
        temp.transform.position = position;

        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.spatialBlend = 1.0f; // 3D 효과음
        source.volume = volume;
        source.Play();

        Destroy(temp, clip.length); // 임시 오디오 믹서 그룹 삭제
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("BGMVolume");
        PlayerPrefs.DeleteKey("SFXVolume");
    }
}
