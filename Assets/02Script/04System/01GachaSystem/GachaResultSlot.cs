using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GachaResultSlot : MonoBehaviour
{
    public RectTransform gradeStarParent;
    public GameObject gradeStarPrefab;
    
    public Image duplicationIcon;
    public TextMeshProUGUI duplicationCountText;
    public TextMeshProUGUI firstGachaText;
    public RectTransform CharacterUIParent;

    public void Setup(GachaData data, AssetListTable assetListTable, StringTable stringTable, bool isNew)
    {
        // 등급 별로 별 아이콘 생성
        for (var i = 0; i < data.Grade+1; i++)
        {
            Instantiate(gradeStarPrefab, gradeStarParent);
        }

        var assetName = assetListTable.Get(data.AssetNo);
        if (!string.IsNullOrEmpty(assetName))
        {
            Addressables.LoadAssetAsync<GameObject>($"Prefab/06GachaCharacterUI/{assetName}").Completed += OnCharacterLoaded;
        }
        else
        {
            Logger.LogWarning($"AssetNo {data.AssetNo}에 해당하는 이름이 AssetListTable에 없습니다.");
        }

        if (isNew)
        {
            firstGachaText.text = "New";
            firstGachaText.gameObject.SetActive(true);
        }
        else
        {
            firstGachaText.gameObject.SetActive(false);
        }
    }

    private void OnCharacterLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(obj.Result, CharacterUIParent);
        }
        else
        {
            Logger.LogWarning($"Addressables 로드 실패: {obj.OperationException}");
        }
    }
}