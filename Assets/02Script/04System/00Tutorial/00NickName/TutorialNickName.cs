using UnityEngine;

public class TutorialNickName: TutorialBase
{
    [SerializeField]
    private NickSettingUI ActiveObject;
    
    public override void Enter()
    {
        if (ActiveObject.gameObject.activeSelf == false)
        {
            ActiveObject.gameObject.SetActive(true);
            ActiveObject.gameObject.transform.SetAsLastSibling();
        }
    }

    public override void Execute(TutorialController controller)
    {
        if (ActiveObject.isComplete == true)
        {
            controller.SetNextTutorial();
            
        }      
    }

    public override void Exit()
    {
    }
}
