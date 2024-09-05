using UnityEngine;

[RequireComponent(typeof(DialogSystem))]
public class TutorialDialog : TutorialBase
{
    public DialogSystem dialogSystem;
    
    public override void Enter()
    {
        dialogSystem.DialogSetting();
        // 모든 UI 요소를 활성화
        dialogSystem.dialogCanvasGroup.gameObject.SetActive(true);
        
        foreach (var systemDialog in dialogSystem.systemDialog)
        {
            if (systemDialog.skeletonGraphic != null)
            {
                systemDialog.skeletonGraphic.gameObject.SetActive(true);    
            }
            
            systemDialog.nameText.gameObject.SetActive(true);
            systemDialog.dialogText.gameObject.SetActive(true);
        }

        dialogSystem.dialogTextPanel.gameObject.SetActive(true);  // dialogTextPanel 활성화
        dialogSystem.nextButton.gameObject.SetActive(true);  // nextButton 활성화
        dialogSystem.dialogCanvasGroup.alpha = 1;  // CanvasGroup의 alpha 값 설정
        
        dialogSystem.isFirstDialog = true;

        dialogSystem.dialogCanvasGroup.gameObject.transform.SetAsLastSibling();
        
    }

    public override void Execute(TutorialController controller)
    {
        bool isComplete = dialogSystem.UpdateDialog();
        
        if (isComplete)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}