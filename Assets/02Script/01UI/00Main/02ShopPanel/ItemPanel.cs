using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    private StringTable stringTable;
    public Button[] ItemBuyButtons;

    public ItemSlotController itemSlotController;
    
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
        for (var i = 0; i < ItemBuyButtons.Length; i++)
        {
            var index = i;
            ItemBuyButtons[index].onClick.AddListener(() => OnClickItemBuyButton(index));
        }
    }

    private void OnClickItemBuyButton(int index)
    {
        int cost = 0;
        int itemId = 0;
        int stage = 0;

        switch (index)
        {
            case 0:
                itemId = 8005;
                cost = 5000;
                stage = 13005;
                break;
            case 1:
                itemId = 8006;
                cost = 5000;
                stage = 13005;
                break;
            case 2:
                itemId = 8007;
                cost = 10000;
                stage = 13010;
                break;
            case 3:
                itemId = 8008;
                cost = 10000;
                stage = 13010;
                break;
            case 4:
                itemId = 8009;
                cost = 10000;
                stage = 13010;
                break;
            case 5:
                itemId = 8010;
                cost = 20000;
                stage = 13015;
                break;
            case 6:
                itemId = 8011;
                cost = 20000;
                stage = 13015;
                break;
            case 7:
                itemId = 8012;
                cost = 20000;
                stage = 13015;
                break;
            case 8:
                itemId = 8013;
                cost = 20000;
                stage = 13015;
                break;
            case 9:
                itemId = 8014;
                cost = 20000;
                stage = 13015;
                break;
            case 10:
                itemId = 8015;
                cost = 20000;
                stage = 13015;
                break;
        }

        // 구매 확인 모달 창을 띄움
        CheckPurchase(cost, itemId,stage);
    }

    private void CheckPurchase(int costPerItem, int itemId, int stageId)
    {
        if(GameManager.instance.GameData.StageClearNum < stageId)
        {
            var stageNum = stageId - 13000;
            // 스테이지 클리어 조건을 만족하지 못한 경우
            ShowPurchaseResult("구매 실패", $"스테이지 {stageNum}를 클리어해야 구매할 수 있습니다.");
            return;
        }
        
        SliderModalWindow.Create(window =>
        {
            window.SetHeader("구매 확인")
                .SetBody(1)
                .SetCost(costPerItem)
                .SetDescription(90766)
                .AddButton("확인", () =>
                {
                    int itemCount = (int)SliderModalWindow.GetCurrentSliderValue(); // 현재 슬라이더 값을 가져옴
                    int totalCost = costPerItem * itemCount;

                    if (GameManager.instance.GameData.Gold >= totalCost)
                    {
                        // 골드가 충분한 경우
                        GameManager.instance.GameData.Gold -= totalCost;
                        GameManager.instance.GameData.AddItem(itemId, itemCount); // 슬라이더 값만큼 아이템 추가
                        DataManager.SaveFile(GameManager.instance.GameData);
                        itemSlotController.RefreshItemSlots(); // 아이템 슬롯 갱신
                        // 구매 성공 메시지 표시
                        ShowPurchaseResult("구매 성공", $"아이템 {itemCount}개를 {totalCost} 골드로 구매했습니다.");
                    }
                    else
                    {
                        // 골드가 부족한 경우
                        ShowPurchaseResult("구매 실패", "골드가 부족합니다.");
                    }
                })
                .Show();
        });
    }

    private void ShowPurchaseResult(string header, string message)
    {
        ModalWindow.Create(window =>
        {
            window.SetHeader(header)
                .SetBody(message)
                .AddButton("확인", () => { });
        });
    }
}
