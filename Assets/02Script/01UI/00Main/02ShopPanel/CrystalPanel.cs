using UnityEngine;
using UnityEngine.UI;

public class CrystalPanel : MonoBehaviour
{
    public Button[] crystalBuyButtons;
    private StringTable stringTable;
    
    private void Awake()
    {
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
    }
    
    private void Start()
    {
        AssignButtonEvents();
    }

    private void AssignButtonEvents()
    {
        for (var i = 0; i < crystalBuyButtons.Length; i++)
        {
            var index = i;
            crystalBuyButtons[index].onClick.AddListener(() => OnClickCrystalBuyButton(index));
        }
    }
    private void OnClickCrystalBuyButton(int index)
    {
        switch (index)
        {
            case 0:
                ShowPurchaseConfirmation(index, 90728, 60);
                break;
            case 1:
                ShowPurchaseConfirmation(index, 90729, 330);
                break;
            case 2:
                ShowPurchaseConfirmation(index, 90730, 1090);
                break;
            case 3:
                ShowPurchaseConfirmation(index, 90731, 2240);
                break;
            case 4:
                ShowPurchaseConfirmation(index, 90732, 3880);
                break;
            case 5:
                ShowPurchaseConfirmation(index, 90733, 8080);
                break;
            default:
                break;
        }
    }
    private void ShowPurchaseConfirmation(int index, int bodyId, int diamondsToAdd)
    {
        ModalWindow.Create(window =>
        {
            window.SetHeader("구매 확인")
                .SetBody(bodyId)
                .AddButton("확인", () =>
                {
                    GameManager.instance.GameData.Diamond += diamondsToAdd;
                    DataManager.SaveFile(GameManager.instance.GameData);
                    CheckPurchase(index);
                })
                .AddButton("취소", () => { })
                .Show();
        });
    }
    
    private void CheckPurchase(int index)
    {
        int bodyId = 0;
        switch (index)
        {
            case 0: bodyId = 90734; break;
            case 1: bodyId = 90735; break;
            case 2: bodyId = 90736; break;
            case 3: bodyId = 90737; break;
            case 4: bodyId = 90738; break;
            case 5: bodyId = 90739; break;
        }

        if (bodyId != 0)
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader("구매 성공")
                    .SetBody(bodyId)
                    .AddButton("확인", () => { })
                    .Show();
            });
        }
    }
}
