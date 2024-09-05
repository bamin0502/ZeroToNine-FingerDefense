using UnityEngine;
using UnityEngine.AddressableAssets;

public class TutorialMonsterFactory : MonoBehaviour
{
    public Transform monsterSpawnPoint;
    
    private MonsterTable monsterTable;
    private AssetListTable assetListTable;
    
    private void Awake()
    {
        monsterTable = DataTableManager.Get<MonsterTable>(DataTableIds.Monster);
        assetListTable = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
    }
    
    public MonsterController CreateMonster(int id)
    {
        var monsterData = monsterTable.Get(id);
        if (monsterData == null)
            return null;
       
        var assetId = monsterData.AssetNo;
        var assetName = assetListTable.Get(assetId);
        if (string.IsNullOrEmpty(assetName))
            return null;
        
        var prefab = Addressables.LoadAssetAsync<GameObject>($"Prefab/01Monster/{assetName}");
        MonsterController spawnedMonster = null;

        prefab.Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                var instance = Instantiate(handle.Result, monsterSpawnPoint.position, Quaternion.identity);
                instance.transform.SetParent(monsterSpawnPoint);
                spawnedMonster = instance.GetComponent<MonsterController>(); // Assuming MonsterController is attached to the prefab
            }
            else
            {
                Logger.LogError($"해당 경로의 몬스터 프리팹을 확인해주세요: {assetName}");
                return;
            }
        };

        return spawnedMonster;
    }
}
