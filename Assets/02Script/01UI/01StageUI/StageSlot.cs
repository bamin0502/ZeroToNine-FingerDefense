using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class StageSlot : MonoBehaviour
{
    public TextMeshProUGUI stageNameText;
    public TextMeshProUGUI monsterText;
    public TextMeshProUGUI rewardText;
    public RectTransform monsterSlotParent;
    public RectTransform rewardSlotParent;

    public GameObject monsterSlotPrefab;
    public GameObject rewardSlotPrefab;
    public Button DeckButton;

    private int StageId;
    private AssetListTable assetListTable;
    private GameObject stageMask; //스테이지 패널 마스크
    private GameObject deckMask;
    private GameObject dragMask;
    private GameManager gameManager;
    
    public GameObject firstRewardImage;
    public GameObject clearImage; 

    [SerializeField] private GameObject deckUI;
    private StagePanelController stagePanelController;

    public void Start()
    {
        stagePanelController = gameObject.GetComponentInParent<StagePanelController>();
        DeckButton.onClick.AddListener(OnClick);
    }

    public void SetDeckUI(GameObject deckUI)
    {
        this.deckUI = deckUI;
    }

    public void SetAssetListTable(AssetListTable assetListTable)
    {
        this.assetListTable = assetListTable;
    }

    public void SetStageMask(GameObject stageMask)
    {
        this.stageMask = stageMask;
    }

    public void SetDragMask(GameObject dragMask)
    {
        this.dragMask = dragMask;
    }

    public void SetDeckMask(GameObject deckMask)
    {
        this.deckMask = deckMask;
    }

    public void GameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void Configure(StageData stageData)
    {
        //해당 슬롯에 스테이지 이름 설정 => stageData.StageNameId을 토대로 StringTable에서 찾아서 가져오기 
        stageNameText.text = DataTableManager.Get<StringTable>(DataTableIds.String).Get(stageData.StageNameId.ToString());
        monsterText.text = DataTableManager.Get<StringTable>(DataTableIds.String).Get(98921.ToString());
        rewardText.text = DataTableManager.Get<StringTable>(DataTableIds.String).Get(98931.ToString());
        
        StageId = stageData.StageId;
        if (stageData.Monster1Id != 0) AddMonsterSlot(stageData.Monster1Id);
        if (stageData.Monster2Id != 0) AddMonsterSlot(stageData.Monster2Id);
        if (stageData.Monster3Id != 0) AddMonsterSlot(stageData.Monster3Id);
        if (stageData.Monster4Id != 0) AddMonsterSlot(stageData.Monster4Id);

        if (stageData.Reward1Id != 0 && stageData.Reward1Value != 0)
            AddRewardSlot(stageData.Reward1Id, stageData.Reward1Value);
        if (stageData.Reward2Id != 0 && stageData.Reward2Value != 0)
            AddRewardSlot(stageData.Reward2Id, stageData.Reward2Value);
        
    }

    private void AddMonsterSlot(int monsterId)
    {
        if (assetListTable == null)
        {
            Logger.LogError("AssetListTable is not set.");
            return;
        }

        // AssetListTable을 사용하여 프리팹 이름 가져오기
        string prefabName = assetListTable.Get(monsterId);
        if (!string.IsNullOrEmpty(prefabName))
        {
            // Addressables를 사용하여 프리팹 로드
            Addressables.LoadAssetAsync<GameObject>($"Prefab/01MonsterUI/{prefabName}").Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject monsterPrefab = handle.Result;
                    var monsterSlot = Instantiate(monsterSlotPrefab, monsterSlotParent);
                    var monsterInstance = Instantiate(monsterPrefab, monsterSlot.transform); // monsterSlot의 자식으로 추가
                    monsterInstance.transform.localPosition = new Vector3(0, 3, 0); // 필요한 경우 위치 조정
                    monsterInstance.transform.localScale = Vector3.one; // 필요한 경우 스케일 조정

                    var monsterText = monsterSlot.GetComponentInChildren<TextMeshProUGUI>();
                    //monsterText.text = monsterId.ToString();
                }
                else
                {
                    Logger.LogWarning($"Prefab not found for {prefabName}");
                }
            };
        }
        else
        {
            Logger.LogWarning($"Prefab name not found for Monster ID: {monsterId}");
        }
        
    }

    private void AddRewardSlot(int rewardId, int rewardValue)
    {
        GameObject rewardSlot = Instantiate(rewardSlotPrefab, rewardSlotParent);
        Image rewardImage = rewardSlot.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI rewardText = rewardSlot.GetComponentInChildren<TextMeshProUGUI>();

        // AssetListTable에서 리워드에 해당하는 프리팹 경로 또는 스프라이트 이름 가져오기
        string assetPath = assetListTable.Get(rewardId);

        if (rewardId == 409 || rewardId == 424 || rewardId == 416)
        {
            // 이미지 비활성화
            rewardImage.gameObject.SetActive(false);

            // AssetListTable에서 경로를 가져와 게임 오브젝트 스폰
            if (!string.IsNullOrEmpty(assetPath))
            {
                Addressables.LoadAssetAsync<GameObject>($"Prefab/00CharacterUI/{assetPath}").Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject rewardPrefab = handle.Result;
                        var rewardInstance = Instantiate(rewardPrefab, rewardSlot.transform);
                        var localPosition = Vector3.zero; // 부모의 중심에 배치
                        rewardInstance.transform.localScale = Vector3.one; // 스케일을 기본값으로 설정

                        // 필요하다면 추가적인 위치 조정
                        localPosition += new Vector3(0, 3, 0); // 필요한 경우 위치 조정
                        rewardInstance.transform.localPosition = localPosition;
                    }
                    else
                    {
                        Logger.LogWarning($"Prefab not found for Reward ID: {rewardId} at path {assetPath}");
                    }
                };
            }
            else
            {
                Logger.LogWarning($"Asset path not found for Reward ID: {rewardId} in AssetListTable");
            }
        }
        else if (!string.IsNullOrEmpty(assetPath))
        {
            // Addressables를 사용하여 스프라이트 로드
            Addressables.LoadAssetAsync<Sprite>($"Prefab/06ShopIcon/{assetPath}").Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Sprite loadedSprite = handle.Result;
                    rewardImage.sprite = loadedSprite; // 이미지 할당
                }
                else
                {
                    Logger.LogWarning($"Sprite not found for Reward ID: {rewardId} at path {assetPath}");
                }
            };
        }
        else
        {
            Logger.LogWarning($"Asset name not found for Reward ID: {rewardId}");
        }
        // 리워드 값 텍스트 설정
        rewardText.text = $"{rewardValue}";
    }


    public void OnClick()
    {
        // 첫 번째 스테이지인 경우 무조건 플레이 가능하게 처리
        if (StageId == 13001)
        {
            LoadStage();
            SetChapterId();
            return;
        }

        // 스테이지 클리어 여부 확인
        bool isStageCleared = gameManager.GameData.StageClear.TryGetValue(StageId, out bool isCleared) && isCleared;

        // 이전 스테이지가 클리어되었는지 확인 (첫 번째 스테이지는 예외 처리됨)
        bool isPreviousStageCleared =
            gameManager.GameData.StageClear.TryGetValue(StageId - 1, out bool previousCleared) && previousCleared;
        
        if (isStageCleared || isPreviousStageCleared)
        {
            LoadStage(); // 스테이지를 로드하는 공통 메서드 호출
            SetChapterId(); // 챕터 ID 설정
        }
        else
        {
            // 스테이지가 클리어되지 않았을 때 모달 창 띄우기
            ModalWindow.Create(window =>
            {
                window.SetHeader("스테이지 잠금")
                    .SetBody("이전 스테이지를 클리어해야 이용할 수 있습니다.")
                    .AddButton("확인", () => { })
                    .Show();
            });
        }
    }

    private void SetChapterId()
    {
        Variables.LoadTable.chapterId = (StageId - 13001) / 5;
        Logger.Log($"챕터 {Variables.LoadTable.chapterId} 선택");
    }

    private void LoadStage()
    {
        if (stageMask.gameObject.activeSelf)
        {
            Variables.LoadTable.StageId = StageId;
            SceneManager.LoadScene(2);
        }
        else if (dragMask.gameObject.activeSelf)
        {
            Variables.LoadTable.StageId = StageId;
            SceneManager.LoadScene(2);
        }
        else if (deckMask.gameObject.activeSelf)
        {
            deckUI.SetActive(true);
            Variables.LoadTable.StageId = StageId;
            deckUI.transform.SetAsLastSibling();
            Logger.Log($"스테이지 {StageId} 선택");
        }
        else
        {
            deckUI.SetActive(true);
            Variables.LoadTable.StageId = StageId;
            deckUI.transform.SetAsLastSibling();
            Logger.Log($"스테이지 {StageId} 선택");
        }
    }
}