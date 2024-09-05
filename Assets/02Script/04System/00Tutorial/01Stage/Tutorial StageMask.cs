using Coffee.UIExtensions;

public class TutorialStageMask : TutorialBase
{
    public UnmaskRaycastFilter mask;
    public StagePanelController stagePanelController;
    
    public override void Enter()
    {
        mask.gameObject.SetActive(true);
        mask.transform.SetAsLastSibling();
        
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
        stagePanelController.enabled = false;
    }

    public override void Exit()
    {
           
    }
}
