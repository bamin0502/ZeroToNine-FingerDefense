using System.Linq;
using Cysharp.Threading.Tasks;

public class TutorialSlotClick : TutorialBase
{
    public DeckSlotController deckSlotController;
    public TutorialController tutorialController;
    
    public override void Enter()
    {
        WatchCharacterSlots().Forget();        
    }
    public void Initialize(TutorialController controller)
    {
        tutorialController = controller;
    }
    private async UniTaskVoid WatchCharacterSlots()
    {
        while (true)
        {
            await UniTask.Yield();

            // characterSlots 리스트에 캐릭터가 추가되었는지 확인
            if (deckSlotController != null && deckSlotController.characterSlots.Any(slot => slot.characterData != null))
            {
                OnCharacterSlotAdded();
                break; // 슬롯이 추가되면 감시 종료
            }
        }
    }
    
    private void OnCharacterSlotAdded()
    {
        // 캐릭터 슬롯이 추가되면 다음 튜토리얼로 넘어감
        tutorialController.SetNextTutorial();
    }
    public override void Execute(TutorialController controller)
    {
    }

    public override void Exit()
    {
    }
}
