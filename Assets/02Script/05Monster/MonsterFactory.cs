using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;


public class MonsterFactory
{
    public Transform poolTransform;
    private AssetListTable assetList;
    private MonsterTable monsterTable;
    private SkillTable skillTable;

    private Dictionary<int, IObjectPool<MonsterController>> monsterPool =
        new Dictionary<int, IObjectPool<MonsterController>>();

    public async UniTask Init(HashSet<int> ids)
    {
        assetList = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
        monsterTable = DataTableManager.Get<MonsterTable>(DataTableIds.Monster);
        skillTable = DataTableManager.Get<SkillTable>(DataTableIds.Skill);

        foreach (var id in ids)
        {
            monsterPool[id] = new ObjectPool<MonsterController>(
                () => CreatedPooledMonsterAsync(id).GetAwaiter().GetResult(),
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                true, 10, 1000
            );
            
            var monster = await CreatedPooledMonsterAsync(id);
            monsterPool[id].Release(monster);
        }
        await UniTask.Yield();
    }

    private async UniTask<MonsterController> CreatedPooledMonsterAsync(int id)
    {
        if (assetList == null)
        {
            throw new InvalidOperationException("AssetList가 초기화되지 않았습니다.");
        }

        var assetId = monsterTable.Get(id).AssetNo;
        var assetFileName = assetList.Get(assetId);
        if (string.IsNullOrEmpty(assetFileName))
        {
            throw new InvalidOperationException($"ID :{assetId}에 대한 자산 경로를 찾을 수 없습니다.");
        }

        var assetPath = $"Prefab/03MonsterGame/{assetFileName}";
        Logger.Log($"경로에서 MonsterController 로드하기: {assetPath}");

        // Addressables를 사용하여 GameObject를 비동기 로드
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(assetPath);
        await handle.Task; // 비동기 로드 완료를 기다림

        if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
        {
            throw new InvalidOperationException($"경로에서 GameObject를 로드하지 못했습니다.: {assetPath}");
        }

        GameObject monsterGameObject = handle.Result;

        // GameObject에서 MonsterController 컴포넌트를 가져옴
        MonsterController monster = monsterGameObject.GetComponent<MonsterController>();
        if (monster == null)
        {
            throw new InvalidOperationException($"로드된 GameObject에서 MonsterController를 찾을 수 없습니다.: {assetPath}");
        }

        var instantiatedMonster = Object.Instantiate(monster);
        if (poolTransform != null)
            instantiatedMonster.transform.SetParent(poolTransform);

        var monsterData = monsterTable.Get(id);
        instantiatedMonster.Status.Data = monsterData;

        var deathSkill = SkillFactory.CreateSkill(monsterData.Skill, instantiatedMonster.gameObject);
        instantiatedMonster.deathSkill = deathSkill;

        var dragSkillData = skillTable.Get(monsterData.DragSkill);
        if (dragSkillData != null)
        {
            var newDragSkillData = dragSkillData.CreateNewSkillData();
            var upgradeTable = DataTableManager.Get<UpgradeTable>(DataTableIds.Upgrade);
            var monsterGimmick = GameManager.instance.GameData.MonsterGimmickLevel;

            foreach (var gimmick in monsterGimmick)
            {
                if (gimmick.level <= 0)
                    continue;

                var upgradeData = upgradeTable.GetMonsterGimmickUpgrade(gimmick.monsterGimmick, gimmick.level);
                if (upgradeData == null)
                    continue;

                switch ((GameData.MonsterGimmick)gimmick.monsterGimmick)
                {
                    case GameData.MonsterGimmick.ATTACKRANGE:
                        newDragSkillData.Range += upgradeData.UpStatValue;
                        break;
                    case GameData.MonsterGimmick.ATTACKDAMAGE:
                        newDragSkillData.Damage += upgradeData.UpStatValue;
                        break;
                    case GameData.MonsterGimmick.ATTACKDURATION:
                        newDragSkillData.Duration += upgradeData.UpStatValue;
                        break;
                }
            }

            var dragDeathSkill = SkillFactory.CreateSkill(newDragSkillData, instantiatedMonster.gameObject);
            instantiatedMonster.dragDeathSkill = dragDeathSkill;
        }

        instantiatedMonster.pool = monsterPool[id];
        return instantiatedMonster;
    }

    private void OnTakeFromPool(MonsterController monster)
    {
        monster.gameObject.SetActive(true);
    }

    private void OnReturnToPool(MonsterController monster)
    {
        monster.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(MonsterController monster)
    {
        Object.Destroy(monster);
    }

    public MonsterController GetMonster(MonsterData data)
    {
        if (data == null)
            throw new NullReferenceException("해당 데이터에 대한 정보가 없습니다");

        if (!monsterPool.ContainsKey(data.Id))
            throw new InvalidOperationException($"몬스터 ID에 대한 풀을 찾을 수 없습니다: {data.Id}");

        return monsterPool[data.Id].Get();
    }
}