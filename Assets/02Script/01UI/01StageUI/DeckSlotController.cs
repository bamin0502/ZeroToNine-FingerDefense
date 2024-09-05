using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckSlotController : MonoBehaviour
{
    public RectTransform characterSlotParent;
    public RectTransform filterringSlotParent;

    private PlayerCharacterTable playerCharacterTable;
    public CharacterSlotUI characterSlotPrefab; // 캐릭터 UI를 생성하기 위한 프리팹

    [HideInInspector]public List<CharacterSlotUI> characterSlots = new List<CharacterSlotUI>();
    private List<CharacterSlotUI> filterSlots = new List<CharacterSlotUI>();
    private HashSet<int> addedCharacters = new HashSet<int>();
    private List<CharacterSlotUI> activeChoicePanelSlots = new List<CharacterSlotUI>();
    
    public Button startButton;
    public Button closeButton;
    
    private GameManager gameManager;
    public TutorialController DeckTutorialController;
    private int maxCharacterSlots = 3;
    
    public CharacterInfoSlot characterInfoSlot;
    
    private void Awake()
    {
        playerCharacterTable = DataTableManager.Get<PlayerCharacterTable>(DataTableIds.PlayerCharacter);
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        RefreshCharacterSlots();
        
        startButton.onClick.AddListener(() =>
        {
            SaveCharacterSelection(); // 캐릭터 선택 상태 저장
            SceneManager.LoadScene(2);
        });
        
        closeButton.onClick.AddListener(() =>
        {
    
        });
    }

    private void OnEnable()
    {
        if (filterringSlotParent != null)
        {
            RefreshCharacterSlots();
            LoadCharacterSelection();
        }
        foreach (var slot in characterSlots)
        {
            slot.ClearGradeImages();
        }
    }
    private void OnDisable()
    {
        SaveCharacterSelection();
    }

    public void RefreshCharacterSlots()
    {
        if (!characterSlotParent.gameObject.activeInHierarchy)
        {
            Logger.LogWarning("Character slot parent is inactive, skipping slot refresh.");
            return;
        }

        // 업그레이드 상태에 따라 최대 캐릭터 슬롯 개수 설정
        int arrangementLevel = GameManager.instance.GameData.PlayerUpgradeLevel
            .Find(x => x.playerUpgrade == (int)GameData.PlayerUpgrade.CHARACTER_ARRANGEMENT).level;
        maxCharacterSlots = 3 + arrangementLevel; // 기본 3개 + 업그레이드 레벨
        
        foreach (var slot in characterSlots)
        {
            slot.ClearSlot();
            Destroy(slot.gameObject);
        }
        characterSlots.Clear();
        
        foreach (var slot in filterSlots)
        {
            Destroy(slot.gameObject);
        }
        filterSlots.Clear();
        
        addedCharacters.Clear();
        activeChoicePanelSlots.Clear();

        // 새로운 슬롯 생성
        CreateCharacterSlots();
        CreateFilteringSlots();
        LoadCharacterSelection();
    }


    private void CreateCharacterSlots()
    {
        for (var i = 0; i < 8; i++)
        {
            var slot = Instantiate(characterSlotPrefab, characterSlotParent);

            if (slot != null)
            {
                if (i < maxCharacterSlots)
                {
                    slot.SetLocked(false);
                    slot.ChoicePanel.SetActive(false);
                    slot.OnSlotClick = HandleCharacterSlotClick;
                    slot.OnLongPress = HandleLongPressRelease;
                    slot.OnLongPressRelease = HandleLongPressReleaseComplete;
                }
                else
                {
                    slot.SetLocked(true);
                }

                characterSlots.Add(slot);
            }
            else
            {
                Logger.LogWarning("Failed to instantiate CharacterSlotUI.");
            }
        }
    }

    private void CreateFilteringSlots()
    {
        var characterIds = GameManager.instance.GameData.characterIds;

        // 이미 존재하는 슬롯을 재사용하거나 새로운 슬롯을 생성합니다.
        for (var i = 0; i < characterIds.Count; i++)
        {
            CharacterSlotUI slot;
            if (i < filterSlots.Count)
            {
                slot = filterSlots[i];
            }
            else
            {
                slot = Instantiate(characterSlotPrefab, filterringSlotParent);
                filterSlots.Add(slot);
            }

            var characterId = characterIds[i];
            var characterData = playerCharacterTable.Get(characterId);
            if (characterData != null)
            {
                slot.SetCharacterSlot(characterData);
                
                // 슬롯의 데이터를 다시 설정하여 이미지와 텍스트를 초기화합니다.
                //slot.ClearSlot();
                slot.UpdateUI().Forget();
                
                slot.OnSlotClick = HandleCharacterSlotClick;
                slot.OnLongPress = HandleLongPressRelease;
                slot.OnLongPressRelease = HandleLongPressReleaseComplete;
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    
    private void HandleCharacterSlotClick(CharacterSlotUI clickedSlot)
    {
        // 슬롯이 잠겨있는지 확인
        if (clickedSlot.LockImage.gameObject.activeSelf)
        {
            return; // 슬롯이 잠겨있다면 아무 동작도 하지 않음
        }
        
        if (!clickedSlot.isLongPress) 
        {
            if (clickedSlot.transform.parent == filterringSlotParent)
            {
                if (clickedSlot.characterData != null && addedCharacters.Contains(clickedSlot.characterData.Id))
                {
                    return;
                }

                if (addedCharacters.Count >= 8)
                {
                    return;
                }

                foreach (var slot in characterSlots)
                {
                    // 빈 슬롯에 캐릭터를 배치
                    if (slot.characterData == null && !slot.LockImage.gameObject.activeSelf)
                    {
                        slot.SetCharacterSlot(clickedSlot.characterData);
                        slot.ChoicePanel.SetActive(false);
                        if (clickedSlot.characterData != null)
                        {
                            addedCharacters.Add(clickedSlot.characterData.Id);
                        }
                        activeChoicePanelSlots.Add(clickedSlot);
                        UpdateChoicePanels();
                        break;
                    }
                }
            }
            else if (clickedSlot.transform.parent == characterSlotParent)
            {
                // 선택 해제 시, 해당 슬롯을 초기화하여 재사용
                var originalSlot = activeChoicePanelSlots.Find(slot => slot.characterData == clickedSlot.characterData);
                if (originalSlot != null)
                {
                    originalSlot.ChoicePanel.SetActive(true);
                    activeChoicePanelSlots.Remove(originalSlot);
                    addedCharacters.Remove(originalSlot.characterData.Id);
                    UpdateChoicePanels();
                }

                clickedSlot.ClearSlot();  // 데이터를 초기화하고 슬롯을 재사용 가능 상태로 만듭니다.
                characterInfoSlot.gameObject.SetActive(false);
                clickedSlot.SetLocked(false);
                
                characterInfoSlot.gameObject.SetActive(false);
            }
        }
        else
        {
            if (clickedSlot.characterData != null)
            {
                characterInfoSlot.gameObject.SetActive(true);
                characterInfoSlot.SetCharacterInfo(clickedSlot.characterData);    
            }
        }
        UpdateCharacterIds();
    }
    
    private void UpdateCharacterIds()
    {
        if (Variables.LoadTable.characterIds.Length < characterSlots.Count)
        {
            Array.Resize(ref Variables.LoadTable.characterIds, characterSlots.Count);
        }

        for (var i = 0; i < characterSlots.Count; i++)
        {
            if (characterSlots[i].characterData != null)
            {
                Variables.LoadTable.characterIds[i] = characterSlots[i].characterData.Id;
            }
            else
            {
                Variables.LoadTable.characterIds[i] = 0; // 빈 슬롯 처리
            }
        }
    }
                                          
    public void UpdateFilteredSlots(List<PlayerCharacterData> filteredCharacters)
    {
        foreach (var slot in filterSlots)
        {
            if (filteredCharacters.Contains(slot.characterData))
            {
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    private void SortCharacterSlots()
    {
        // filterringSlotParent의 자식인 CharacterSlotUI들을 가져옴
        var slots = filterringSlotParent.GetComponentsInChildren<CharacterSlotUI>();

        // 슬롯들을 Power 기준으로 오름차순으로 정렬
        var sortedSlots = slots
            .Where(slot => slot.characterData != null)  // 유효한 캐릭터 데이터가 있는 슬롯만 대상으로 함
            .OrderByDescending(slot => slot.characterData.Power)  // Power 오름차순
            .ThenBy(slot => slot.characterData.Class)  // 추가 정렬 기준: Class
            .ThenByDescending(slot => slot.characterData.Grade)  // 추가 정렬 기준: Grade 내림차순
            .ThenByDescending(slot => slot.characterData.Element)  // 추가 정렬 기준: Element 내림차순
            .ToList();

        // 정렬된 슬롯들의 순서를 UI에 반영
        for (int i = 0; i < sortedSlots.Count; i++)
        {
            sortedSlots[i].transform.SetSiblingIndex(i);
        }
    }

    private void UpdateChoicePanels()
    {
        foreach (var slot in filterSlots)
        {
            slot.ChoicePanel.SetActive(false);
        }

        foreach (var slot in activeChoicePanelSlots)
        {
            slot.ChoicePanel.SetActive(true);
        }
    }

    private void SaveCharacterSelection()
    {
        for (int i = 0; i < Variables.LoadTable.characterIds.Length; i++)
        {
            PlayerPrefs.SetInt($"CharacterId_{i}", Variables.LoadTable.characterIds[i]);
        }
        PlayerPrefs.Save();
    }

    private void LoadCharacterSelection()
    {
        // 캐릭터 ID를 불러와 슬롯에 설정
        for (int i = 0; i < Variables.LoadTable.characterIds.Length; i++)
        {
            int characterId = Variables.LoadTable.characterIds[i];
            if (characterId != 0)
            {
                var characterData = playerCharacterTable.Get(characterId);

                // 필터링 슬롯에 없는 캐릭터는 제거되게 함
                if (!filterSlots.Any(s => s.characterData != null && s.characterData.Id == characterId))
                {
                    Variables.LoadTable.characterIds[i] = 0; // 캐릭터 ID 초기화
                    continue;
                }

                // Index 체크 추가
                if (i < characterSlots.Count && characterData != null)
                {
                    var slot = characterSlots[i];
                    slot.SetCharacterSlot(characterData);
                    slot.ChoicePanel.SetActive(false);
                    addedCharacters.Add(characterId);
                    var originalSlot = filterSlots.FirstOrDefault(s => s.characterData.Id == characterId);
                    if (originalSlot != null)
                    {
                        activeChoicePanelSlots.Add(originalSlot);
                    }
                }
                else
                {
                }
            }
        }

        UpdateChoicePanels();
        SortCharacterSlots();
    }


    private void HandleLongPressRelease(CharacterSlotUI clickedSlot)
    {
        // 롱터치 후 상태창 활성화
        characterInfoSlot.gameObject.SetActive(true);
        characterInfoSlot.SetCharacterInfo(clickedSlot.characterData);
    }
    private void HandleLongPressReleaseComplete()
    {
        // 롱터치 해제 시 상태창 비활성화
        characterInfoSlot.gameObject.SetActive(false);
    }
}
