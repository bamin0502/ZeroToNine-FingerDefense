using UnityEngine;
using UnityEngine.UI;

public class GoldPanel : MonoBehaviour
{
    public Button[] GoldBuyButtons;
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
        for (var i = 0; i < GoldBuyButtons.Length; i++)
        {
            var index = i;
            GoldBuyButtons[index].onClick.AddListener(() => OnClickGoldBuyButton(index));
        }
    }
    private void OnClickGoldBuyButton(int index)
    {
        switch (index)
        {
            case 0:
                ShowPurchaseConfirmation(80, 1600, index, 90741);
                break;
            case 1:
                ShowPurchaseConfirmation(200, 4000, index, 90742);
                break;
            case 2:
                ShowPurchaseConfirmation(500, 10000, index, 90743);
                break;
            case 3:
                ShowPurchaseConfirmation(1000, 20000, index, 90744);
                break;
            case 4:
                ShowPurchaseConfirmation(3000, 60000, index, 90745);
                break;
            case 5:
                ShowPurchaseConfirmation(5000, 100000, index, 90746);
                break;
            default:
                Logger.LogWarning("Invalid GoldBuyButton Clicked");
                break;
        }
    }
    private void ShowPurchaseConfirmation(int diamond, int gold, int index, int bodyId)
    {
        ModalWindow.Create(window =>
        {
            window.SetHeader("구매 확인")
                .SetBody(bodyId)
                .AddButton("확인", () =>
                {
                    CheckPurchase(diamond, gold, index);
                })
                .AddButton("취소", () => { })
                .Show();
        });
    }
    
    private void CheckPurchase(int diamond, int gold, int index)
    {
        if (GameManager.instance.GameData.Diamond >= diamond)
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader("구매 성공")
                    .SetBody(GetSuccessBodyId(index))
                    .AddButton("확인", () =>
                    {
                        GameManager.instance.GameData.Diamond -= diamond;
                        GameManager.instance.GameData.Gold += gold;
                        DataManager.SaveFile(GameManager.instance.GameData);
                    })
                    .Show();
            });
        }
        else
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader("구매 실패")
                    .SetBody(90753)
                    .AddButton("확인", () => { })
                    .Show();
            });
        }
    }

    private int GetSuccessBodyId(int index)
    {
        switch (index)
        {
            case 0: return 90747;
            case 1: return 90748;
            case 2: return 90749;
            case 3: return 90750;
            case 4: return 90751;
            case 5: return 90752;
            default: return 0;
        }
    }
}
