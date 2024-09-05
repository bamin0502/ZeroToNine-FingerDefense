using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager
{
    public static bool isPlayerPrefsLoad = false;
    public static AudioMixer masterMixer;

    public static readonly float maxVolume = 0f;
    public static readonly float minVolume = -40f;
    public static readonly float muteVolume = -80f;

    public static void InitAudio()
    {
        if (isPlayerPrefsLoad)
            return;
        
        masterVolume = PlayerPrefs.GetFloat("Master");
        bgmVolume = PlayerPrefs.GetFloat("BGM");
        sfxVolume = PlayerPrefs.GetFloat("SFX");

        isMasterMute = PlayerPrefs.GetInt("MasterMute") == 0
                        ? isMasterMute : PlayerPrefs.GetInt("MasterMute") == 1;
        isBgmMute = PlayerPrefs.GetInt("BgmMute") == 0
                        ? isBgmMute : PlayerPrefs.GetInt("BgmMute") == 1;
        isSfxMute = PlayerPrefs.GetInt("SfxMute") == 0
                        ? isSfxMute : PlayerPrefs.GetInt("SfxMute") == 1;

        isVibration = PlayerPrefs.GetInt("Vibration") == 0
                        ? isVibration : PlayerPrefs.GetInt("Vibration") == 1;

        isPlayerPrefsLoad = true;
    }

    public static void SaveAudioValue(float masterVol, float bgmVol, float sfxVol)
    {
        PlayerPrefs.SetFloat("Master", masterVol);
        PlayerPrefs.SetFloat("BGM", bgmVol);
        PlayerPrefs.SetFloat("SFX", sfxVol);

        PlayerPrefs.SetInt("MasterMute", isMasterMute ? 1 : -1);
        PlayerPrefs.SetInt("BgmMute", isBgmMute ? 1 : -1);
        PlayerPrefs.SetInt("SfxMute", isSfxMute ? 1 : -1);

        PlayerPrefs.SetInt("Vibration", isVibration ? 1 : -1);
    }
    
    private static float masterVolume;
    public static float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = value;
            masterMixer.SetFloat("Master", masterVolume);
        }
    }
    public static bool isMasterMute = false;

    private static float bgmVolume;
    public static float BgmVolume
    {
        get => bgmVolume;
        set
        {
            bgmVolume = value;
            masterMixer.SetFloat("BGM", bgmVolume);
        }
    }
    public static bool isBgmMute = false;

    private static float sfxVolume;
    public static float SfxVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = value;
            masterMixer.SetFloat("SFX", sfxVolume);
        }
    }
    public static bool isSfxMute = false;

    public static bool isVibration = true;
    public static void Vibration()
    {
        if (isVibration)
        {
            Handheld.Vibrate();
        }
    }
}