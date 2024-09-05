using UnityEngine.UI;

public class TutorialButtonClick : TutorialBase
{
    public Button ClickButton;
    private bool isChecked = false;
    
    public override void Enter()
    {
        ClickButton.onClick.AddListener(() => isChecked = true);
    }

    public override void Execute(TutorialController controller)
    {
        if (isChecked)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}
