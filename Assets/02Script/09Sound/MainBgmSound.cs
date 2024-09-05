using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MainBgmSound : MonoBehaviour
{
    private AudioSource bgmAudioSource;
    
    public string mainBgmAddress;  // Addressables 키를 저장

    private void Awake()
    {
        bgmAudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(mainBgmAddress))
        {
            LoadAndPlayMainBgm();
        }
        else
        {
            Debug.LogWarning("mainBgmAddress is not set.");
        }
    }

    private void LoadAndPlayMainBgm()
    {
        Addressables.LoadAssetAsync<AudioClip>(mainBgmAddress).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                PlayAudioClip(handle.Result);
            }
            else
            {
                Debug.LogWarning($"Failed to load BGM with address: {mainBgmAddress}");
            }
        };
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (bgmAudioSource && clip)
        {
            bgmAudioSource.clip = clip;
            bgmAudioSource.Play();
        }
    }
    
    public void PlayAudioClip(string address)
    {
        Addressables.LoadAssetAsync<AudioClip>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                PlayAudioClip(handle.Result);
            }
            else
            {
                Debug.LogWarning($"Failed to load BGM with address: {address}");
            }
        };
    }

    public void StopAudioClip()
    {
        bgmAudioSource.Stop();
    }
}