using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System;

public class PlayerCharacterSpawnTest : MonoBehaviour
{
    public Transform playerSpawnPos;

    private PlayerCharacterTable playerCharacterTable;
    private AssetListTable assetListTable;
    private SkillTable skillTable;
    public TMP_InputField playerCharacterIdText;

    private PlayerCharacterController currentCharacter;

    private void Awake()
    {
        playerCharacterTable = DataTableManager.Get<PlayerCharacterTable>(DataTableIds.PlayerCharacter);
        assetListTable = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
        skillTable = DataTableManager.Get<SkillTable>(DataTableIds.Skill);
    }

    public void SpawnCharacter()
    {
        if (!int.TryParse(playerCharacterIdText.text, out var id))
            return;

        CreatePlayerCharacter(id).Forget();
    }

    private async UniTask<PlayerCharacterController> CreatePlayerCharacter(int id)
    {
        var playerCharacterData = playerCharacterTable.Get(id);

        if (playerCharacterData == null)
        {
            Logger.LogError($"해당 아이디에 해당하는 플레이어 캐릭터가 캐릭터 테이블에 존재하지 않습니다: {id}");
            return null;
        }

        var assetId = playerCharacterData.AssetNo;
        var assetFileName = assetListTable.Get(assetId);
        if (string.IsNullOrEmpty(assetFileName))
        {
            Logger.LogError($"해당 아이디에 해당하는 캐릭터 에셋이 에셋 리스트에 존재하지 않습니다: {id}");
            return null;
        }
        var assetPath = $"Prefab/02CharacterGame/{assetFileName}";

        // var playerCharacterPrefab = Resources.Load<PlayerCharacterController>(assetPath);
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(assetPath);
        await handle.Task; // 비동기 로드 완료를 기다림

        if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
        {
            throw new InvalidOperationException($"경로에서 GameObject를 로드하지 못했습니다.: {assetPath}");
        }

        GameObject characterGameObject = handle.Result;
        var playerCharacterPrefab = characterGameObject.GetComponent<PlayerCharacterController>();
        if (playerCharacterPrefab == null)
        {
            Logger.LogError($"해당 경로의 캐릭터 프리팹을 확인해주세요: {assetPath}");
            return null;
        }

        var spawnPos = playerSpawnPos.position;
        var instantiatedCharacter = Instantiate(playerCharacterPrefab, spawnPos, Quaternion.identity);
        instantiatedCharacter.Status.Data = playerCharacterData;
        
        if (instantiatedCharacter.TryGetComponent<PlayerAttackBehavior>(out var attackBehavior))
        {
            var normalAttackData = skillTable.Get(playerCharacterData.Skill1);
            var skillAttackData = skillTable.Get(playerCharacterData.Skill2);

            var normalAttack = SkillFactory.CreateSkill(normalAttackData, instantiatedCharacter.gameObject);
            var skillAttack = SkillFactory.CreateSkill(skillAttackData, instantiatedCharacter.gameObject);

            attackBehavior.normalAttack = normalAttack;
            attackBehavior.SkillAttack = skillAttack;
        }

        if (currentCharacter)
        {
            Destroy(currentCharacter.gameObject);
        }
        currentCharacter = instantiatedCharacter;

        return instantiatedCharacter;
    }
}
