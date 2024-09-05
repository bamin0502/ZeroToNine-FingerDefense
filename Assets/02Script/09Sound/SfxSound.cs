using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SfxSound : MonoBehaviour
{
    private enum SfxSoundType
    {
        ACTIVATE_BUTTON_CLICK,
        ENTER_BUTTON_CLICK,
        NON_ACTIVATE_BUTTON_CLICK
    }

    private AudioSource sfxAduioSource;
    // public AudioClip activateButtonClickAudioClip;
    // public AudioClip enterButtonClickAudioClip;
    // public AudioClip nonActivateButtonClickAudioClip;

    private Dictionary<SfxSoundType, AudioClip> sounds = new();

    private void Awake()
    {
        sfxAduioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        LoadSfxSounds();
    }
    
    private void LoadClip(SfxSoundType soundType, string address)
    {
        Addressables.LoadAssetAsync<AudioClip>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                sounds[soundType] = handle.Result;
            }
        };
    }
    
    private void LoadSfxSounds()
    {
        LoadClip(SfxSoundType.ACTIVATE_BUTTON_CLICK, "ActivateButtonClickAudioClipAddress");
        LoadClip(SfxSoundType.ENTER_BUTTON_CLICK, "EnterButtonClickAudioClipAddress");
        LoadClip(SfxSoundType.NON_ACTIVATE_BUTTON_CLICK, "NonActivateButtonClickAudioClipAddress");
    }
    
    [VisibleEnum(typeof(SfxSoundType))]
    public void PlaySfxSound(int soundType)
    {
        if (sfxAduioSource && sounds[(SfxSoundType)soundType])
        {
            sfxAduioSource.PlayOneShot(sounds[(SfxSoundType)soundType]);
        }
    }
}
