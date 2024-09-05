using UnityEngine;
using Cysharp.Threading.Tasks;

public class TutorialTimeStop : TutorialBase
{
    [SerializeField]
    private int delayTimeInMilliseconds = 3000;
    
    public bool isPause = false;
    private bool isNext = false;
    public override void Enter()
    {
        //들어오고 3초뒤에 시간을 멈추고
        StopTimeAsync().Forget();
    }

    private async UniTaskVoid StopTimeAsync()
    {
        await UniTask.Delay(delayTimeInMilliseconds);
        TimeScaleController.SetTimeScale(isPause ? 0f : 1f);

        //다음으로 넘어가기
        isNext = true;
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


