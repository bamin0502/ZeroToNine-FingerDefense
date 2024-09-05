using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CharacterAttributeFilter : MonoBehaviour
{
    private PlayerCharacterTable playerCharacterTable;

    public Toggle[] ElementToggles; // 각 배열에 따라서 각 필터링 0,1,2 적용
    public Toggle[] ClassToggles; // 각 배열에 따라서 각 필터링 0,1,2 적용
    public Toggle[] GradeToggles; // 각 배열에 따라서 각 필터링 0,1,2 적용
    
    public Button ApplyButton;
    public Button ResetButton;
    
    private DeckSlotController deckSlotController;

    private void Start()
    {
        playerCharacterTable ??= DataTableManager.Get<PlayerCharacterTable>(DataTableIds.PlayerCharacter);

        deckSlotController = GameObject.FindWithTag("DeckUI").GetComponent<DeckSlotController>();
        
        ApplyButton.onClick.AddListener(ApplyFilter);
        ResetButton.onClick.AddListener(ResetFilter);
    }
    
    private void ApplyFilter()
    {
        var filteredCharacters = FilterCharacters();
        deckSlotController.UpdateFilteredSlots(filteredCharacters);
    }

    private List<PlayerCharacterData> FilterCharacters()
    {
        var characters = playerCharacterTable.GetAll();
        var filteredCharacters = new List<PlayerCharacterData>();

        bool isAnyElementChecked = ElementToggles.Any(t => t.isOn);
        bool isAnyClassChecked = ClassToggles.Any(t => t.isOn);
        bool isAnyGradeChecked = GradeToggles.Any(t => t.isOn);

        var playerCharacterData = characters.ToList();
        foreach (var character in playerCharacterData)
        {
            if (IsCharacterMatched(character, isAnyElementChecked, isAnyClassChecked, isAnyGradeChecked))
            {
                filteredCharacters.Add(character);
            }
        }

        // 필터 조건이 모두 꺼져있는 경우 모든 캐릭터를 반환
        if (!isAnyElementChecked && !isAnyClassChecked && !isAnyGradeChecked)
        {
            return playerCharacterData.ToList();
        }

        return filteredCharacters;
    }

    private bool IsCharacterMatched(PlayerCharacterData character, bool isAnyElementChecked, bool isAnyClassChecked, bool isAnyGradeChecked)
    {
        bool isElementMatched = !isAnyElementChecked || ElementToggles.Any(t => t.isOn && character.Element == System.Array.IndexOf(ElementToggles, t));
        bool isClassMatched = !isAnyClassChecked || ClassToggles.Any(t => t.isOn && character.Class == System.Array.IndexOf(ClassToggles, t));
        bool isGradeMatched = !isAnyGradeChecked || GradeToggles.Any(t => t.isOn && character.Grade == System.Array.IndexOf(GradeToggles, t));
        
        // 각각의 조건이 모두 맞아야 매칭되도록 변경
        return (!isAnyElementChecked || isElementMatched) 
               && (!isAnyClassChecked || isClassMatched) 
               && (!isAnyGradeChecked || isGradeMatched);
    }

    private void ResetFilter()
    {
        foreach (var toggle in ElementToggles)
        {
            toggle.isOn = false;
        }
        foreach (var toggle in ClassToggles)
        {
            toggle.isOn = false;
        }
        foreach (var toggle in GradeToggles)
        {
            toggle.isOn = false;
        }
    }
}
