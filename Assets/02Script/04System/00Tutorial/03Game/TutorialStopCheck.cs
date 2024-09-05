using UnityEngine;

public class TutorialStopCheck : TutorialBase
{
    public TutorialObserver tutorialObserver;
    [SerializeField]private MonsterController[] monster;
    
    private bool isNext = false;
    public override void Enter()
    {
    }

    public override void Execute(TutorialController controller)
    {
        if (isNext)
        {
            controller.SetNextTutorial();
        }   
    }

    public override void Exit()
    {
    }
}
