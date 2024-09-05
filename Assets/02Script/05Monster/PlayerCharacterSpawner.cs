using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class PlayerCharacterSpawner : MonoBehaviour
{
    public Transform poolTransform;
    public GameObject[] spawnPositions;

    private PlayerCharacterTable playerCharacterTable;
    private SkillTable skillTable;
    private AssetListTable assetListTable;

    private PlayerCharacterController[] playerCharacters = new PlayerCharacterController[8];
    private PlayerCharacterController[] activePlayerCharacters = new PlayerCharacterController[6]; // 현재 활성화된 캐릭터 저장

    public GameObject playerUICharacterPrefab;
    public RectTransform playerUICharacterParent;
    private Button[] characterButtons = new Button[8];

    public UiButtonEffect uiButtonEffect;

    private int selectedCharacterIndex = -1;
    
    private InputManager inputManager;
    public TutorialController tutorial; 
    
    private void Awake()
    {
        // 데이터 테이블 로드
        playerCharacterTable = DataTableManager.Get<PlayerCharacterTable>(DataTableIds.PlayerCharacter);
        skillTable = DataTableManager.Get<SkillTable>(DataTableIds.Skill);
        assetListTable = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
        
        // InputManager 찾기
        inputManager = GameObject.FindWithTag("InputManager").GetComponent<InputManager>();
        
        // 캐릭터 슬롯 생성
        for (var i = 0; i < Variables.LoadTable.characterIds.Length; i++)
        {
            if (Variables.LoadTable.characterIds[i] != 0)
            {
                var data = playerCharacterTable.Get(Variables.LoadTable.characterIds[i]);
                var i1 = i;
                _ = CreatePlayerCharacter(data).ContinueWith(result => playerCharacters[i1] = result); // 비동기 생성 호출
                CreateUICharacter(data, i).Forget(); // 비동기 UI 생성 호출
            }
            else
            {
                CreateUICharacter(null, i).Forget(); // 비동기 UI 생성 호출
            }
        }
    }

    private void OnEnable()
    {
        inputManager.OnTouchStartedEvent += OnTouchStarted;
        inputManager.OnTouchEndedEvent += OnTouchEnded;
    }

    private void OnDisable()
    {
        inputManager.OnTouchStartedEvent -= OnTouchStarted;
        inputManager.OnTouchEndedEvent -= OnTouchEnded;
    }

    private void OnTouchStarted(Vector2 touchPosition)
    {
    }

    private void OnTouchEnded(Vector2 touchPosition)
    {
        if (selectedCharacterIndex == -1)
        {
            return;
        }

        Ray ray = Camera.main!.ScreenPointToRay(touchPosition);
        var layerMask = LayerMask.GetMask("PlayerSpawner");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            Debug.Log($"Raycast hit: {hit.point}");

            // 검출한 오브젝트의 위치로 캐릭터 소환
            for (var i = 0; i < spawnPositions.Length; i++)
            {
                if (Mathf.Abs(spawnPositions[i].transform.position.x - hit.point.x) 
                    <= 1f && Mathf.Abs(spawnPositions[i].transform.position.y - hit.point.y) <= 1f)
                {
                    Debug.Log($"Spawning character at position index {i}");
                    SpawnPlayerCharacter(i);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any collider.");
        }
    }

    private async UniTaskVoid CreateUICharacter(PlayerCharacterData data, int index)
    {
        var uiCharacter = Instantiate(playerUICharacterPrefab, playerUICharacterParent);
        if (data != null)
        {
            var assetName = assetListTable.Get(data.AssetNo);
            if (string.IsNullOrEmpty(assetName))
            {
                Logger.LogError($"Asset name not found for AssetNo {data.AssetNo}");
                return;
            }

            // Addressables를 통해 UI 캐릭터 프리팹 로드
            var uiAssetHandle = Addressables.LoadAssetAsync<GameObject>($"Prefab/00CharacterUI/{assetName}");
            var uiAsset = await uiAssetHandle.Task;
            if (uiAsset == null)
            {
                Logger.LogError($"UI Prefab not found for asset name {assetName}");
                return;
            }

            Instantiate(uiAsset, uiCharacter.transform).transform.SetAsFirstSibling();
        }
        else
        {
            Logger.LogWarning("Character data is null.");
        }

        var button = uiCharacter.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => SelectCharacterForSpawning(index));
            characterButtons[index] = button;
        }
    }

    private async UniTask<PlayerCharacterController> CreatePlayerCharacter(PlayerCharacterData data)
    {
        if (data == null)
        {
            Logger.LogError("Character data is null.");
            return null;
        }

        var assetName = assetListTable.Get(data.AssetNo);
        if (string.IsNullOrEmpty(assetName))
        {
            Logger.LogError($"Asset name not found for AssetNo {data.AssetNo}");
            return null;
        }

        // Addressables를 통해 캐릭터 프리팹 로드
        var prefabHandle = Addressables.LoadAssetAsync<GameObject>($"Prefab/02CharacterGame/{assetName}");
        var prefab = await prefabHandle.Task;
        if (prefab == null)
        {
            Logger.LogError($"Prefab not found for asset name {assetName}");
            return null;
        }

        var playerCharacter = Instantiate(prefab, poolTransform).GetComponent<PlayerCharacterController>();
        if (playerCharacter == null)
        {
            Logger.LogError($"PlayerCharacterController not found on prefab {assetName}");
            return null;
        }
        playerCharacter.Status.Data = data;
        
        if (playerCharacter.TryGetComponent<PlayerAttackBehavior>(out var attackBehavior))
        {
            var normalAttackData = skillTable.Get(data.Skill1);
            var skillAttackData = skillTable.Get(data.Skill2);

            var normalAttack = SkillFactory.CreateSkill(normalAttackData, playerCharacter.gameObject);
            var skillAttack = SkillFactory.CreateSkill(skillAttackData, playerCharacter.gameObject);

            attackBehavior.normalAttack = normalAttack;
            attackBehavior.SkillAttack = skillAttack;
        }

        playerCharacter.spawner = this;
        playerCharacter.gameObject.SetActive(false);

        return playerCharacter;
    }

    private void SelectCharacterForSpawning(int index)
    {
        if (index < 0 || index >= playerCharacters.Length)
        {
            Debug.LogWarning($"Index {index} is out of bounds for playerCharacters array.");
            return;
        }

        if (selectedCharacterIndex == index)
        {
            selectedCharacterIndex = -1;
            uiButtonEffect.ButtonRectTransform = null;
            foreach (var spawnPos in spawnPositions)
            {
                spawnPos.SetActive(false);
            }
            return;
        }

        if (playerCharacters[index] == null)
            return;

        selectedCharacterIndex = index;
        uiButtonEffect.ButtonRectTransform = characterButtons[selectedCharacterIndex].GetComponent<RectTransform>();
        uiButtonEffect.gameObject.SetActive(true);
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            spawnPositions[i].SetActive(activePlayerCharacters[i] == null);
        }
        Logger.Log($"Selected character at index {index} for spawning.");
    }
    
    public void SpawnPlayerCharacter(int positionIndex)
    {
        var playerCharacter = playerCharacters[selectedCharacterIndex];
        
        if (selectedCharacterIndex == -1 || playerCharacter == null || playerCharacter.gameObject.activeSelf)
        {
            return;
        }

        if (positionIndex < 0 || positionIndex >= activePlayerCharacters.Length || activePlayerCharacters[positionIndex] != null)
        {
            return;
        }

        playerCharacter.transform.position = spawnPositions[positionIndex].transform.position;

        activePlayerCharacters[positionIndex] = playerCharacter;
        playerCharacter.Status.Init();
        playerCharacter.gameObject.SetActive(true);
        
        if (characterButtons[selectedCharacterIndex] != null)
        {
            // characterButtons[selectedCharacterIndex].interactable = false;
            var slotButton = characterButtons[selectedCharacterIndex].GetComponent<UiSlotButton>();
            slotButton?.ActiveButton(false);
        }

        selectedCharacterIndex = -1;
        uiButtonEffect.ButtonRectTransform = null;
        foreach (var spawnPos in spawnPositions)
        {
            spawnPos.SetActive(false);
        }
    }

    public void RemoveActiveCharacter(PlayerCharacterController character)
    {
        if (!character)
            return;

        for (var i = 0; i < activePlayerCharacters.Length; i++)
        {
            if (activePlayerCharacters[i] == character)
            {
                activePlayerCharacters[i].gameObject.SetActive(false);
                activePlayerCharacters[i] = null;
            }

            for (var j = 0; j < playerCharacters.Length; j++)
            {
                if (playerCharacters[j] == character && characterButtons[j])
                {
                    // characterButtons[j].interactable = true;
                    UpdateRespawnTimer(characterButtons[j], character.Status.Data.RespawnCoolTime).Forget();
                }
            }
        }
    }

    private async UniTask UpdateRespawnTimer(Button button, float respawnTime)
    {
        float timer = 0f;
        var slotButton = button.GetComponent<UiSlotButton>();

        slotButton?.ActiveButton(false);
        while (timer <= respawnTime)
        {
            timer += Time.deltaTime;
            slotButton?.SetFillAmountBackground(1f - timer / respawnTime);
            await UniTask.Yield();
        }
        slotButton?.ActiveButton(true);
    }
}
