
public class TutorialItemSlotClick : TutorialBase
{
    public ItemSlotController itemSlotParent;
    public TutorialController tutorialController;

    
    public override void Enter()
    {
        ItemSlotUI.ItemSlotClicked += OnItemSlotClicked;
    }
    private void OnItemSlotClicked(ItemSlotUI itemSlot)
    {
        // 아이템 슬롯이 클릭되면 다음 튜토리얼로 넘어감
        tutorialController.SetNextTutorial();

        // 필요한 경우 더 이상 감시하지 않도록 이벤트 해제
        ItemSlotUI.ItemSlotClicked -= OnItemSlotClicked;
    }
    
    public override void Execute(TutorialController controller)
    {
        
    }

    public override void Exit()
    {
        ItemSlotUI.ItemSlotClicked -= OnItemSlotClicked;
    }
}
