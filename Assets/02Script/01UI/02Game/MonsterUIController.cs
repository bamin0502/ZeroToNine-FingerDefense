using System.Collections.Generic;
using UnityEngine;

public class MonsterUIController : MonoBehaviour
{
    public GameObject monsterUISlotPrefab;
    public RectTransform monsterUIParent;
    public MonsterSpawner monsterSpawner;
    private AssetListTable assetListTable;
    
    private void Awake()
    {
        assetListTable ??= DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
    }

    private void Start()
    {
        if (monsterSpawner == null)
        {
            Debug.LogError("MonsterSpawner가 설정되지 않았습니다.");
            return;
        }
        CreateMonsterSlots(monsterSpawner.monsters);
    }

    private void CreateMonsterSlots(HashSet<int> monsters)
    {
        foreach (var monster in monsters)
        {
            var assetName = assetListTable.Get(monster);
            if (!string.IsNullOrEmpty(assetName))
            {
                GameObject prefab = Resources.Load<GameObject>($"Prefab/01MonsterUI/{assetName}");
                if (prefab != null)
                {
                    var monsterUISlotInstance = Instantiate(monsterUISlotPrefab, monsterUIParent);
                    var prefabInstance = Instantiate(prefab, monsterUISlotInstance.transform);
                }
            }
        }
    }

    
}
