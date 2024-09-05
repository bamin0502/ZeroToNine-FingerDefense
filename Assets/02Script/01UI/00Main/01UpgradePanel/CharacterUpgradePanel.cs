using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUpgradePanel : MonoBehaviour
{
    private AssetListTable assetListTable;
    private UpgradeTable upgradeTable;
    private StringTable stringTable;
    private PlayerCharacterTable playerCharacterTable;
    
    public CharacterUpgradeSlotUI characterUpgradeSlot;
    public RectTransform characterUpgradeSlotContent; //캐릭터 업그레이드를 하기 전의 현재 있는 캐릭터를 담는 부모
    
    public RectTransform characterUpgradeSlotParent; //캐릭터 업그레이드를 하기 전의 슬롯
    public RectTransform characterUpgradeResultSlotContent; //캐릭터 업그레이드를 할때 그 결과를 보여줄 슬롯
    
    public TextMeshProUGUI characterUpgradeResultText; 
    public TextMeshProUGUI characterUpgradeGoldText;
    
    private List<int> obtainedGachaIds;
    
    public Button upgradeButton;

    private UpgradeData selectedUpgradeData;
    public static event Action<int, int> OnCharacterUpgraded;
    public void Awake()
    {
        assetListTable ??= DataTableManager.Get<AssetListTable>(DataTableIds.Asset);
        upgradeTable ??= DataTableManager.Get<UpgradeTable>(DataTableIds.Upgrade);
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
        playerCharacterTable ??= DataTableManager.Get<PlayerCharacterTable>(DataTableIds.PlayerCharacter);
    }
    
    private void OnEnable()
    {
        GachaSystem.OnCharacterSlotUpdated += RefreshPanel;
        RefreshPanel();
    }
    
    private void OnDisable()
    {
        GachaSystem.OnCharacterSlotUpdated -= RefreshPanel;
    }
    
    public void RefreshPanel()
    {
        obtainedGachaIds = GameManager.instance.GameData.characterIds;
        LoadUpgradableCharacters(obtainedGachaIds);
    }
    
    private void LoadUpgradableCharacters(List<int> obtainedCharacterIds)
    {
        ClearSlot(characterUpgradeSlotContent);

        foreach (var characterId in obtainedCharacterIds)
        {
            var characterData = playerCharacterTable.Get(characterId);
            if (characterData != null)
            {
                var upgradeData = upgradeTable.upgradeTable.Values.FirstOrDefault(up => up.NeedCharId == characterId);

                // 업그레이드가 가능한 캐릭터만 슬롯에 추가
                if (upgradeData != null)
                {
                    var characterSlot = Instantiate(characterUpgradeSlot, characterUpgradeSlotContent);
                    characterSlot.SetCharacterSlot(characterData);

                    characterSlot.OnSlotClick = (slot) => DisplayUpgradeOptions(upgradeData, characterData);
                }
            }
        }
    }


    private void DisplayUpgradeOptions(UpgradeData upgradeData, PlayerCharacterData characterData)
    {
        selectedUpgradeData = upgradeData; // 선택된 업그레이드 데이터를 저장

        // 선택된 캐릭터 정보를 characterUpgradeSlotParent에 표시
        ClearSlot(characterUpgradeSlotParent); // 기존 슬롯 비우기
        var selectedSlot = Instantiate(characterUpgradeSlot, characterUpgradeSlotParent);
        selectedSlot.SetCharacterSlot(characterData);
        selectedSlot.powerText.transform.SetAsLastSibling();
        selectedSlot.upgradeLevelText.transform.SetAsLastSibling();
        
        // 앵커 값을 중앙으로 설정하고, 스케일을 2로 조정
        RectTransform selectedSlotRect = selectedSlot.GetComponent<RectTransform>();
        selectedSlotRect.anchorMin = new Vector2(0.5f, 0.5f); // 중앙 앵커
        selectedSlotRect.anchorMax = new Vector2(0.5f, 0.5f); // 중앙 앵커
        selectedSlotRect.pivot = new Vector2(0.5f, 0.5f); // 피벗도 중앙으로 설정
        selectedSlotRect.localPosition = Vector3.zero; // 부모의 중앙으로 위치 설정
        selectedSlotRect.localScale = Vector3.one * 2; // 스케일 2배로 설정
        
        // UpgradeResultId를 기반으로 업그레이드 결과 표시
        ClearSlot(characterUpgradeResultSlotContent); 
        
        var resultCharacterData = playerCharacterTable.Get(upgradeData.UpgradeResultId);
        if (resultCharacterData != null)
        {
            var resultSlot = Instantiate(characterUpgradeSlot, characterUpgradeResultSlotContent);
            resultSlot.SetCharacterSlot(resultCharacterData);

            resultSlot.powerText.transform.SetAsLastSibling();
            resultSlot.upgradeLevelText.transform.SetAsLastSibling();
            
            // 앵커 값과 스케일을 동일하게 설정
            RectTransform resultSlotRect = resultSlot.GetComponent<RectTransform>();
            resultSlotRect.anchorMin = new Vector2(0.5f, 0.5f); 
            resultSlotRect.anchorMax = new Vector2(0.5f, 0.5f);
            resultSlotRect.pivot = new Vector2(0.5f, 0.5f);
            resultSlotRect.localPosition = Vector3.zero; // 부모의 중앙으로 위치 설정
            resultSlotRect.localScale = Vector3.one * 2; // 스케일 2배로 설정
            
            characterUpgradeResultText.text = stringTable.Get(upgradeData.Name.ToString()); // 업그레이드 설명 표시
        }

        // 업그레이드 가격 표시
        characterUpgradeGoldText.text = upgradeData.UpgradePrice.ToString();

        // 업그레이드 버튼 활성화 또는 비활성화
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(TryUpgradeCharacter);
    }

    private void TryUpgradeCharacter()
    {
        if (selectedUpgradeData == null) return;

        int playerGold = GameManager.instance.GameData.Gold;

        // 현재 버튼에 해당하는 업그레이드 가격과 스테이지 조건을 검사
        if (playerGold >= selectedUpgradeData.UpgradePrice)
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader("업그레이드 확인")
                    .SetBody("정말 업그레이드 하시겠습니까?")
                    .AddButton("확인", () => ApplyUpgrade(selectedUpgradeData))
                    .AddButton("취소", () => { });
            });
        }
        else
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader("업그레이드 실패")
                    .SetBody("골드가 부족합니다.")
                    .AddButton("확인", () => { });
            });
        }
    }

    private void ApplyUpgrade(UpgradeData upgradeData)
    {
        // 업그레이드 적용 로직 (예: 캐릭터의 등급을 업그레이드)
        var existingIndex = GameManager.instance.GameData.characterIds.IndexOf(upgradeData.NeedCharId);
        if (existingIndex >= 0)
        {
            GameManager.instance.GameData.characterIds[existingIndex] = upgradeData.UpgradeResultId;
        }
        
        // 업그레이드 이벤트 호출
        OnCharacterUpgraded?.Invoke(upgradeData.NeedCharId, upgradeData.UpgradeResultId);
        // 데이터 저장
        GameManager.instance.GameData.Gold -= upgradeData.UpgradePrice;
        DataManager.SaveFile(GameManager.instance.GameData);
        
        upgradeButton.onClick.RemoveAllListeners(); // 업그레이드 버튼 리스너 제거
        // UI 갱신
        ClearSlot(characterUpgradeSlotParent);
        ClearSlot(characterUpgradeResultSlotContent);
        characterUpgradeGoldText.text = "";
        characterUpgradeResultText.text = "";
        
        LoadUpgradableCharacters(obtainedGachaIds);  // 업그레이드 가능한 캐릭터 목록 다시 로드
        
    }


    private void ClearSlot(RectTransform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
