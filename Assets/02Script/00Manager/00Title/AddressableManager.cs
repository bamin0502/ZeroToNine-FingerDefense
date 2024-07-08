using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    [SerializeField]
    private AssetReferenceGameObject[] assetReferenceGameObjects;
    [SerializeField]
    private AssetReference[] assetReferences;
    [SerializeField]
    private AssetReferenceT<AudioClip>[] audioClips;
    [SerializeField]
    private AssetReferenceSprite[] assetReferenceSprites;
    [SerializeField]
    private AssetReferenceT<TextAsset>[] textAssets;
    [SerializeField]
    private AssetReferenceT<PlayableAsset>[] playableAssets;
    [SerializeField]
    private AssetReferenceT<InputActionAsset>[] inputActionAssets;

    private List<GameObject> gameObjects = new List<GameObject>();

    void Start()
    {

    }

    public void ButtonClick()
    {
        foreach (var t in assetReferenceGameObjects)
        {
            if (t == null) continue;
            var load = t.LoadAssetAsync<GameObject>();
            load.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var asset = op.Result;
                    Logger.Log(asset.name);
                }
                else
                {
                    Logger.LogError($"Failed to load asset {t.RuntimeKey}");
                }
                Addressables.Release(load);
            };
        }

        foreach (var t in assetReferences)
        {
            if (t == null) continue;
            var load = t.LoadAssetAsync<AssetReference>();
            load.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var asset = op.Result;
                    var inst = asset.InstantiateAsync();
                    inst.Completed += (op2) =>
                    {
                        if (op2.Status == AsyncOperationStatus.Succeeded)
                        {
                            gameObjects.Add(op2.Result);
                        }
                        else
                        {
                            Logger.LogError($"Failed to instantiate asset {t.RuntimeKey}");
                        }
                    };
                }
                else
                {
                    Logger.LogError($"Failed to load asset {t.RuntimeKey}");
                }
                Addressables.Release(load);
            };
        }

        foreach (var t in audioClips)
        {
            if (t == null) continue;
            var load = t.LoadAssetAsync<AudioClip>();
            load.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var audio = op.Result;
                    Logger.Log(audio.name);
                }
                else
                {
                    Logger.LogError($"Failed to load asset {t.RuntimeKey}");
                }
                Addressables.Release(load);
            };
        }

        foreach (var t in assetReferenceSprites)
        {
            if (t == null) continue;
            var load = t.LoadAssetAsync<Sprite>();
            load.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var sprite = op.Result;
                    var image = gameObject.GetComponent<Image>();
                    image.sprite = sprite;
                }
                else
                {
                    Logger.LogError($"Failed to load asset {t.RuntimeKey}");
                }
                Addressables.Release(load);
            };
        }

        foreach (var t in textAssets)
        {
            if (t == null) continue;
            var load = t.LoadAssetAsync<TextAsset>();
            load.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var text = op.Result;
                    Debug.Log(text.text);
                }
                else
                {
                    Logger.LogError($"Failed to load asset {t.RuntimeKey}");
                }
                Addressables.Release(load);
            };
        }

        foreach (var t in playableAssets)
        {
            if (t == null) continue;
            var load = t.LoadAssetAsync<PlayableAsset>();
            load.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var playable = op.Result;
                    Logger.Log(playable.name);
                }
                else
                {
                    Logger.LogError($"Failed to load asset {t.RuntimeKey}");
                }
                Addressables.Release(load);
            };
        }

        foreach (var t in inputActionAssets)
        {
            if (t == null) continue;
            var load = t.LoadAssetAsync<InputActionAsset>();
            load.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var inputAction = op.Result;
                    var action = inputAction.FindAction("Click");
                    action.performed += (context) =>
                    {
                        Logger.Log("Click");
                    };
                }
                else
                {
                    Logger.LogError($"Failed to load asset {t.RuntimeKey}");
                }
                Addressables.Release(load);
            };
        }
    }
}
