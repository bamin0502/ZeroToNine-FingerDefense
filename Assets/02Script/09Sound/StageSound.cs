using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class StageSound : MonoBehaviour
{
    [SerializeField] private List<string> bgmAddresses; // Addressables 키 리스트로 변경
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        LoadAndPlayBgm(Variables.LoadTable.chapterId);
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }

    private void LoadAndPlayBgm(int chapterId)
    {
        if (chapterId >= 0 && chapterId < bgmAddresses.Count)
        {
            string bgmAddress = bgmAddresses[chapterId];
            Addressables.LoadAssetAsync<AudioClip>(bgmAddress).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    audioSource.clip = handle.Result;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning($"Failed to load BGM for chapterId: {chapterId} with address: {bgmAddress}");
                }
            };
        }
        else
        {
            Debug.LogWarning($"Invalid chapterId: {chapterId}. Unable to load BGM.");
        }
    }
}