using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageSlotCreate : MonoBehaviour
{
    private StageTable stageTable;
    public StageSlot stageSlotPrefab;
    public RectTransform[] slotParents;
    public GameObject deckUI;
    private AssetListTable assetListTable;
    private StringTable stringTable;
    private GameManager gameManager;
    private bool slotsCreated = false;
    public GameObject stageMask;
    public GameObject deckMask;
    public GameObject dragMask;
    
    public Button[] stageButtons;
    
    private bool isModalOpen = false;
    
    private void Awake()
    {
        gameManager = GameManager.instance;
    }
    
    
    private void OnEnable()
    {
        if (stageTable == null)                     
        {
            stageTable = DataTableManager.Get<StageTable>(DataTableIds.Stage);
            assetListTable = DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
            stringTable = DataTableManager.Get<StringTable>(DataTableIds.String);
        }
        
        if (!slotsCreated)
        {
            CreateStageSlots();
            slotsCreated = true;
        }
        SetStartPositionBasedOnClearedStage();
        for (var i = 0; i < stageButtons.Length; i++)
        {
            var index = i; // 람다식 캡처 문제를 피하기 위해 지역 변수를 사용
            stageButtons[i].onClick.AddListener(() => OnStageButtonClicked(index));
        }
        
    }

    private void SetStartPositionBasedOnClearedStage()
    {
        // 마지막으로 클리어한 스테이지 ID를 가져옴
        int lastClearedStage = gameManager.GameData.StageClearNum;
    
        // 다음으로 도전할 스테이지 ID 결정
        int nextStageId = lastClearedStage + 1;

        // 모든 slotParents를 비활성화 (모두 보이지 않도록 함)
        foreach (var parent in slotParents)
        {
            parent.gameObject.SetActive(false);
        }

        // 다음으로 도전할 스테이지 ID에 따라 패널을 활성화
        if (nextStageId >= 13021)
        {
            ActivatePanel(4);  // 5번째 패널 (13021-13025)
        }
        else if (nextStageId >= 13016)
        {
            ActivatePanel(3);  // 4번째 패널 (13016-13020)
        }
        else if (nextStageId >= 13011)
        {
            ActivatePanel(2);  // 3번째 패널 (13011-13015)
        }
        else if (nextStageId >= 13006)
        {
            ActivatePanel(1);  // 2번째 패널 (13006-13010)
        }
        else if (nextStageId >= 13001)
        {
            ActivatePanel(0);  // 1번째 패널 (13001-13005)
        }
        else
        {
            ActivatePanel(0);  // 기본값으로 첫 번째 패널 활성화
        }
    }

    private void ActivatePanel(int panelIndex)
    {
        // 모든 slotParents를 비활성화
        foreach (var parent in slotParents)
        {
            parent.gameObject.SetActive(false);
        }

        // 해당 인덱스의 패널만 활성화
        if (panelIndex >= 0 && panelIndex < slotParents.Length)
        {
            slotParents[panelIndex].gameObject.SetActive(true);
        }

        // 해당 인덱스의 버튼을 활성화 (잠금 해제)
        if (panelIndex >= 0 && panelIndex < stageButtons.Length)
        {
            stageButtons[panelIndex].interactable = true;
        }
    }
    
    private void OnStageButtonClicked(int index)
    {
        // 버튼이 비활성화되었거나 모달 창이 이미 열려있다면 클릭 이벤트 무시
        if (!stageButtons[index].interactable || isModalOpen) return;

        // 스테이지 클리어 여부 체크
        var isStage5Cleared = gameManager.GameData.StageClear.TryGetValue(13005, out var stage5Cleared) && stage5Cleared;
        var isStage10Cleared = gameManager.GameData.StageClear.TryGetValue(13010, out var stage10Cleared) && stage10Cleared;
        var isStage15Cleared = gameManager.GameData.StageClear.TryGetValue(13015, out var stage15Cleared) && stage15Cleared;
        var isStage20Cleared = gameManager.GameData.StageClear.TryGetValue(13020, out var stage20Cleared) && stage20Cleared;

        // 각 버튼의 활성화 가능 여부를 명확하게 설정
        var canActivate = index switch
        {
            0 => true,
            1 => isStage5Cleared,
            2 => isStage10Cleared,
            3 => isStage15Cleared,
            4 => isStage20Cleared,
            _ => false
        };

        if (canActivate)
        {
            // 다른 모든 패널을 비활성화
            foreach (var parent in slotParents)
            {
                parent.gameObject.SetActive(false);
            }

            // 선택한 패널만 활성화
            if (index >= 0 && index < slotParents.Length)
            {
                slotParents[index].gameObject.SetActive(true);
            }
        }
        else
        {
            if(!isModalOpen)
            {
                isModalOpen = true;
                ModalWindow.Create(window =>
                {
                    window.SetHeader("잠금")
                        .SetBody("해당 스테이지를 모두 클리어해야 합니다.")
                        .AddButton("확인", () => 
                        { 
                            isModalOpen = false;  // 모달 창이 닫힐 때 플래그 리셋
                        })
                        .Show();
                });
            }
        }
    }


    
    private void CreateStageSlots()
    {
        List<StageData> batch = new List<StageData>();
        int parentIndex = 0;

        foreach (var stageData in stageTable.GetAll())
        {
            batch.Add(stageData);

            if (batch.Count == 5)
            {
                Logger.Log($"Creating batch for parent {parentIndex} with stages: {string.Join(", ", batch.Select(s => s.StageId))}");
                CreateBatch(batch, slotParents[parentIndex]);
                batch.Clear();
                parentIndex = (parentIndex + 1) % slotParents.Length;
            }
        }
        
        if (batch.Count > 0)
        {
            Logger.Log($"Creating final batch for parent {parentIndex} with stages: {string.Join(", ", batch.Select(s => s.StageId))}");
            CreateBatch(batch, slotParents[parentIndex]);
        }
    }
    
    

    private void CreateBatch(List<StageData> batch, RectTransform parent)
    {
        foreach (var stageData in batch)
        {
            StageSlot slot = Instantiate(stageSlotPrefab, parent);
            slot.SetAssetListTable(assetListTable);
            slot.Configure(stageData);
            slot.SetDeckUI(deckUI);
            slot.SetStageMask(stageMask);
            slot.GameManager(gameManager);
            slot.SetDragMask(dragMask);
            slot.SetDeckMask(deckMask);

            // 현재 스테이지가 클리어되었는지 확인
            bool isStageCleared = gameManager.GameData.StageClear.TryGetValue(stageData.StageId, out bool isCleared) && isCleared;

            // 슬롯 활성화 여부 설정
            slot.gameObject.SetActive(true);  // 슬롯은 모두 활성화

            // 클리어된 스테이지인 경우 firstRewardImage와 clearImage를 활성화
            if (isStageCleared)
            {
                slot.firstRewardImage.SetActive(true);
                slot.clearImage.SetActive(true);
            }
            
            

        }
    }
}