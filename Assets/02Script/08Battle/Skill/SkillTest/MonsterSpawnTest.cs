using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

public class MonsterSpawnTest : MonoBehaviour
{
    public Transform monsterSpawnPos;
    [SerializeField] private float monsterSpawnRadius = 2.5f;
    [SerializeField] private Transform monsterTargetPos;

    public bool IsAutoSpawn{ get; set; } = false;
    [SerializeField] private float spawnInterval = 0.25f;
    private float spawnTimer = 0f;

    private MonsterTable monsterTable;
    private AssetListTable assetListTable;
    public TMP_InputField monsterIdText;

    private void Awake()
    {
        monsterTable = DataTableManager.Get<MonsterTable>(DataTableIds.Monster);
        assetListTable = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
    }

    private void Update()
    {
        if (!IsAutoSpawn)
            return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnInterval)
        {
            SpawnMonster();
            spawnTimer = 0f;
        }
    }

    public void SpawnMonster()
    {
        if (!int.TryParse(monsterIdText.text, out var id))
            return;

        CreateMonster(id).Forget();
    }

    public void RemoveAllMonster()
    {
        var monsters = GameObject.FindGameObjectsWithTag(Defines.Tags.MONSTER_TAG);
        foreach (var monster in monsters)
        {
            Destroy(monster);
        }
    }

    private async UniTask<MonsterController> CreateMonster(int id)
    {
        var monsterData = monsterTable.Get(id);
        if (monsterData == null)
        {
            Logger.LogError($"해당 아이디에 해당하는 몬스터가 몬스터 테이블에 존재하지 않습니다: {id}");
            return null;
        }

        var assetId = monsterData.AssetNo;
        var assetFileName = assetListTable.Get(assetId);
        if (string.IsNullOrEmpty(assetFileName))
        {
            Logger.LogError($"해당 아이디에 해당하는 몬스터 에셋이 에셋 리스트에 존재하지 않습니다: {id}");
            return null;
        }
        var assetPath = $"Prefab/03MonsterGame/{assetFileName}";

        // var monsterPrefab = Resources.Load<MonsterController>(assetPath);
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(assetPath);
        await handle.Task; // 비동기 로드 완료를 기다림

        if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
        {
            throw new InvalidOperationException($"경로에서 GameObject를 로드하지 못했습니다.: {assetPath}");
        }

        GameObject monsterGameObject = handle.Result;
        MonsterController monsterPrefab = monsterGameObject.GetComponent<MonsterController>();

        if (monsterPrefab == null)
        {
            Logger.LogError($"해당 경로의 몬스터 프리팹을 확인해주세요: {assetPath}");
            return null;
        }

        var spawnPos = (Vector2)monsterSpawnPos.position + UnityEngine.Random.insideUnitCircle * monsterSpawnRadius;
        var instantiatedMonster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        instantiatedMonster.Status.Data = monsterData;

        var deathSkill = SkillFactory.CreateSkill(monsterData.Skill, instantiatedMonster.gameObject);
        instantiatedMonster.deathSkill = deathSkill;
        var dragDeathSkill = SkillFactory.CreateSkill(monsterData.DragSkill, instantiatedMonster.gameObject);
        instantiatedMonster.dragDeathSkill = dragDeathSkill;

        instantiatedMonster.moveTargetPos = monsterTargetPos.position;

        return instantiatedMonster;
    }
}