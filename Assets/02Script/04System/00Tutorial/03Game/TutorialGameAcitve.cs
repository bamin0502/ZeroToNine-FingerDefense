using UnityEngine;
using Cysharp.Threading.Tasks;

public class TutorialGameActive : TutorialBase
{
    private bool isGameActive = false;

    public GameObject parentObject;

    public override void Enter()
    {
        MonitorGameActiveAsync().Forget();
    }

    private async UniTaskVoid MonitorGameActiveAsync()
    {
        while (true)
        {
            Check();
            if (isGameActive)
            {
                break;
            }
            await UniTask.Delay(100);
        }
    }

    private void Check()
    {
        // parentObject의 자식 중 하나라도 활성화되어 있는지 확인
        foreach (Transform child in parentObject.transform)
        {
            if (child.gameObject.activeSelf)
            {
                isGameActive = true;
                break;
            }
        }
    }

    public override void Execute(TutorialController controller)
    {
        if (isGameActive)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        // 종료 작업
    }
}