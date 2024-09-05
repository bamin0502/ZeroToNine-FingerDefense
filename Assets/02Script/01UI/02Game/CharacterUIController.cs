using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterUIController : MonoBehaviour
{
    public CharacterUIButton characterUISlotPrefab;
    public RectTransform characterUIParent;
    
    private AssetListTable assetListTable;
    
    private void Awake()
    {
        assetListTable ??= DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
    }
    
    private void Start()
    {
        CreateCharacterSlots();
    }
    
    private void CreateCharacterSlots()
    {
        for (int i = 0; i < Variables.LoadTable.characterIds.Length; i++)
        {
            var assetName = assetListTable.Get(Variables.LoadTable.characterIds[i]);
            if (!string.IsNullOrEmpty(assetName))
            {
                string assetPath = $"Prefab/00CharacterUI/{assetName}";
                Addressables.LoadAssetAsync<GameObject>(assetPath).Completed += OnCharacterAssetLoaded;
            }
            else
            {
                Logger.LogError($"Asset name not found for AssetNo {Variables.LoadTable.characterIds[i]}");
            }
        }
    }

    private void OnCharacterAssetLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var prefab = handle.Result;
            var characterUISlotInstance = Instantiate(characterUISlotPrefab, characterUIParent);
            Instantiate(prefab, characterUISlotInstance.transform);
        }
        else
        {
            Logger.LogError("Failed to load character UI prefab via Addressables.");
        }
    }
}