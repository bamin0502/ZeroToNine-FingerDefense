using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("오디오 믹서")]
    public AudioMixer masterMixer;

    [Header("오디오 소스")]
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    [Header("음량 조절 슬라이더")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("음소거 토글")]
    public Toggle masterToggle;
    public Toggle bgmToggle;
    public Toggle sfxToggle;

    [Header("진동 토글")]
    public Toggle vibrationToggle;

    
    
    private void Awake()
    {
        if (!AudioManager.masterMixer)
            AudioManager.masterMixer = masterMixer;

        AudioManager.InitAudio();
    }

    private void Start()
    {
        masterSlider.minValue = AudioManager.minVolume;
        masterSlider.maxValue = AudioManager.maxVolume;
        bgmSlider.minValue = AudioManager.minVolume;
        bgmSlider.maxValue = AudioManager.maxVolume;
        sfxSlider.minValue = AudioManager.minVolume;
        sfxSlider.maxValue = AudioManager.maxVolume;

        masterSlider.value = Mathf.Clamp(AudioManager.MasterVolume, AudioManager.minVolume, AudioManager.maxVolume);
        bgmSlider.value = Mathf.Clamp(AudioManager.BgmVolume, AudioManager.minVolume, AudioManager.maxVolume);
        sfxSlider.value = Mathf.Clamp(AudioManager.SfxVolume, AudioManager.minVolume, AudioManager.maxVolume);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);

        masterToggle.onValueChanged.AddListener(SetMasterMute);
        bgmToggle.onValueChanged.AddListener(SetBgmMute);
        sfxToggle.onValueChanged.AddListener(SetSfxMute);

        masterToggle.isOn = AudioManager.isMasterMute;
        SetMasterMute(AudioManager.isMasterMute);
        bgmToggle.isOn = AudioManager.isBgmMute;
        SetBgmMute(AudioManager.isBgmMute);
        sfxToggle.isOn = AudioManager.isSfxMute;
        SetSfxMute(AudioManager.isSfxMute);

        vibrationToggle.onValueChanged.AddListener(isVibration => AudioManager.isVibration = isVibration);
        vibrationToggle.isOn = AudioManager.isVibration;
    }

    public void SetMasterVolume(float volume)
    {
        if (AudioManager.isMasterMute)
            return;

        if (volume == AudioManager.minVolume)
            volume = AudioManager.muteVolume;
        AudioManager.MasterVolume = volume;

        AudioSettings.OnAudioConfigurationChanged += (change) =>
        {
            AudioManager.MasterVolume = volume;
        };
    }

    public void SetMasterMute(bool isMute)
    {
        AudioManager.isMasterMute = isMute;

        if (isMute)
            AudioManager.MasterVolume = AudioManager.muteVolume;
        else
            SetMasterVolume(masterSlider.value);
    }

    public void SetBgmVolume(float volume)
    {
        if (AudioManager.isBgmMute)
            return;

        if (volume == AudioManager.minVolume)
            volume = AudioManager.muteVolume;
        AudioManager.BgmVolume = volume;
        
        AudioSettings.OnAudioConfigurationChanged += (change) =>
        {
            AudioManager.BgmVolume = volume;
        };
    }

    public void SetBgmMute(bool isMute)
    {
        AudioManager.isBgmMute = isMute;

        if (isMute)
            AudioManager.BgmVolume = AudioManager.muteVolume;
        else
            SetBgmVolume(bgmSlider.value);
    }

    public void SetSfxVolume(float volume)
    {
        if (AudioManager.isSfxMute)
            return;

        if (volume == AudioManager.minVolume)
            volume = AudioManager.muteVolume;
        AudioManager.SfxVolume = volume;
        
        AudioSettings.OnAudioConfigurationChanged += (change) =>
        {
            AudioManager.SfxVolume = volume;
        };
    }

    public void SetSfxMute(bool isMute)
    {
        AudioManager.isSfxMute = isMute;

        if (isMute)
            AudioManager.SfxVolume = AudioManager.muteVolume;
        else
            SetSfxVolume(sfxSlider.value);
    }

    public void SaveAudioValue()
    {
        AudioManager.SaveAudioValue(masterSlider.value, bgmSlider.value, sfxSlider.value);
    }
}
