using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckPanelController : MonoBehaviour
{
    public Button changeButton;

    public RectTransform CharacterPanel;
    public RectTransform itemPanel;
    public Button CharacterFilterButton;

    private bool isCharacterPanelActive = true;

    public void Start()
    {
        

        // 버튼 클릭 리스너 설정
        changeButton.onClick.AddListener(ChangePanel);
        changeButton.GetComponentInChildren<TextMeshProUGUI>().text = "<b>캐릭터</b>/아이템";
    }

    public void OnEnable()
    {
        // 무조건 CharacterPanel과 CharacterFilterButton을 활성화 상태로 설정
        CharacterPanel.gameObject.SetActive(true);
        CharacterFilterButton.gameObject.SetActive(true);
        itemPanel.gameObject.SetActive(false);    
    }

    private void ChangePanel()
    {
        isCharacterPanelActive = !isCharacterPanelActive;

        if (isCharacterPanelActive)
        {
            itemPanel.gameObject.SetActive(false);
            CharacterPanel.gameObject.SetActive(true);
            CharacterFilterButton.gameObject.SetActive(true);
            CharacterPanel.SetAsLastSibling();
            
            changeButton.GetComponentInChildren<TextMeshProUGUI>().text = "<b>캐릭터</b>/아이템";
        }
        else
        {
            CharacterPanel.gameObject.SetActive(false);
            itemPanel.gameObject.SetActive(true);
            CharacterFilterButton.gameObject.SetActive(false);
            itemPanel.SetAsLastSibling();
            
            changeButton.GetComponentInChildren<TextMeshProUGUI>().text = "캐릭터/<b>아이템</b>";
        }
    }
}