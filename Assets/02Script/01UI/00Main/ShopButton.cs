using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShopButtonType
{
    None = 0,
    Ticket = 1,
    Diamond = 2,
    Gold = 3,
    Item = 4,
    Mileage = 5,
}

public class ShopButton : MonoBehaviour
{
    public Button button;
    
    public GameObject ChoicePanel;
    public Button ChoiceButton1;
    public Button ChoiceButton2;
    
    public int ChoiceTitleText;
    public int ChoiceContentText;
    public string ChoiceButton1Text;
    public string ChoiceButton2Text;

    public ShopButtonType buttonType;
    public int buttonNumber;
    
    public TextMeshProUGUI choiceTitleText;
    public TextMeshProUGUI choiceContentText;
    public TextMeshProUGUI choiceButton1Text;
    public TextMeshProUGUI choiceButton2Text;

    private StringTable shopTable;
    public ShopPayCheckManager shopPayCheck;
    
    private void Start()
    {
        shopTable = DataTableManager.Get<StringTable>(DataTableIds.String);
        button.onClick.AddListener(OnButtonClicked);
        ChoicePanel.SetActive(false);
    }
    
    public void OnButtonClicked()
    {
        if (shopTable != null)
        {
            // 텍스트 설정
            choiceTitleText.text = shopTable.Get(ChoiceTitleText.ToString());
            choiceContentText.text = shopTable.Get(ChoiceContentText.ToString());
            SetButtonState(ChoiceButton1, choiceButton1Text, ChoiceButton1Text);
            SetButtonState(ChoiceButton2, choiceButton2Text, ChoiceButton2Text);
            
            // UI 창 활성화
            ChoicePanel.SetActive(true);
            
            shopPayCheck.HandleButtonClicked(buttonType, buttonNumber);
        }
    }
    
    private void SetButtonState(Button setbutton, TextMeshProUGUI buttonText, string text)
    {
        buttonText.text = text;
        setbutton.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }
    public void ResetButtonState()
    {
        ChoicePanel.SetActive(false);
    }
}


