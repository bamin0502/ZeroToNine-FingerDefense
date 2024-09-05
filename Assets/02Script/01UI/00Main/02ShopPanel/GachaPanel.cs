using UnityEngine;
using UnityEngine.UI;

public class GachaPanel : MonoBehaviour
{
    public Button[] gachaButtons;
    private GameManager gameManager;
    public GachaSystem gachaSystem; 

    public void Start()
    {
        gameManager = GameManager.instance;
        
        for (var i = 0; i < gachaButtons.Length; i++)
        {
            var index = i;
            
            gachaButtons[i].onClick.AddListener(() => OnClickGachaButton(index));
        }
    }

    private void OnClickGachaButton(int index)
    {
        switch (index)
        {
            case 0:
                ConfirmGachaPurchase(1, 160, index);
                break;
            case 1:
                ConfirmGachaPurchase(10, 1600, index);
                break;
        }
    }

    private void ConfirmGachaPurchase(int ticketCountRequired, int diamondCost, int index)
    {
        int currentTickets = gameManager.GameData.Ticket;
        int currentDiamonds = gameManager.GameData.Diamond;

        if (currentTickets >= ticketCountRequired)
        {
            // 티켓이 충분한 경우
            ModalWindow.Create(window =>
            {
                window.SetHeader("구매 확인")
                      .SetBody($"티켓 {ticketCountRequired}개로 {ticketCountRequired}회 뽑기를 진행하시겠습니까?")
                      .AddButton("확인", () =>
                      {
                          gameManager.GameData.Ticket -= ticketCountRequired;
                          gameManager.GameData.NotifyObservers(ResourceType.Ticket, gameManager.GameData.Ticket);
                          gachaSystem.PerformGacha(ticketCountRequired);
                      })
                      .AddButton("취소", () => { })
                      .Show();
            });
        }
        else if (currentDiamonds >= diamondCost)
        {
            // 티켓이 부족하지만 다이아몬드가 충분한 경우
            int ticketsNeeded = ticketCountRequired - currentTickets;
            int diamondNeeded = ticketsNeeded * 160; // 부족한 티켓 수만큼 다이아몬드 계산

            ModalWindow.Create(window =>
            {
                window.SetHeader("구매 확인")
                      .SetBody($"티켓이 부족합니다. {ticketsNeeded}개를 {diamondNeeded} 크리스탈로 구매해서 {ticketCountRequired}회 뽑기를 진행하시겠습니까?")
                      .AddButton("확인", () =>
                      {
                          if (currentDiamonds >= diamondNeeded)
                          {
                              gameManager.GameData.Diamond -= diamondNeeded;
                              gameManager.GameData.Ticket = 0; // 티켓을 모두 사용
                              gameManager.GameData.NotifyObservers(ResourceType.Ticket, gameManager.GameData.Ticket);
                              gameManager.GameData.NotifyObservers(ResourceType.Diamond, gameManager.GameData.Diamond);
                              gachaSystem.PerformGacha(ticketCountRequired);
                          }
                          else
                          {
                              ModalWindow.Create(innerWindow =>
                              {
                                  innerWindow.SetHeader("구매 실패")
                                             .SetBody("크리스탈이 부족합니다.")
                                             .AddButton("확인", () => { })
                                             .Show();
                              });
                          }
                      })
                      .AddButton("취소", () => { })
                      .Show();
            });
        }
        else
        {
            ModalWindow.Create(window =>
            {
                window.SetHeader("구매 실패")
                      .SetBody("티켓 또는 크리스탈이 부족합니다.")
                      .AddButton("확인", () => { })
                      .Show();
            });
        }
    }
}

