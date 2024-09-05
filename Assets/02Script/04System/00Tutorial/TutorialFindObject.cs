using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TutorialFindObject : TutorialBase
{
    public GameObject findObject;
    private bool isClick = false;
    
    public UnmaskRaycastFilter unmaskRaycastFilter;
    public override void Enter()
    {
        FindModalAsync().Forget();
    }
    private async UniTask FindModalAsync()
    {
        await UniTask.Yield();  // 한 프레임 대기

        while (findObject == null)
        {
            findObject = GameObject.FindWithTag("Modal");
            await UniTask.Yield();  // 다음 프레임까지 대기
        }

        if (findObject != null)
        {
            unmaskRaycastFilter.transform.SetAsLastSibling();
            findObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                isClick = true;
            });
        }
    }
    public override void Execute(TutorialController controller)
    {
        if (isClick)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}

