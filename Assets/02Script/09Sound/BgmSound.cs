using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BgmSound : MonoBehaviour
{
    private MainBgmSound bgmManager;
    public string bgmAddress; 
    public bool isAutoPlay;

    private void OnEnable()
    {
        if (!bgmManager)
            bgmManager = GameObject.FindWithTag(Defines.Tags.SOUND_MANAGER_TAG)?.GetComponentInChildren<MainBgmSound>();

        if (isAutoPlay)
        {
            LoadAndPlayBgm();
        }
    }

    private void OnDisable()
    {
        bgmManager?.StopAudioClip();
    }

    public void PlayBgm()
    {
        LoadAndPlayBgm();
    }

    private void LoadAndPlayBgm()
    {
        if (!string.IsNullOrEmpty(bgmAddress))
        {
            Addressables.LoadAssetAsync<AudioClip>(bgmAddress).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    bgmManager?.PlayAudioClip(handle.Result);
                }
                else
                {
                    Debug.LogWarning($"Failed to load BGM with address: {bgmAddress}");
                }
            };
        }
        else
        {
            Debug.LogWarning("BGM Address is not set.");
        }
    }
}