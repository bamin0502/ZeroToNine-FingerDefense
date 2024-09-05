using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using DG.Tweening;

public class TutorialDisableCheck : TutorialBase
{
    public GameObject targetObject;
    public bool isDisable = false;
    public TextMeshProUGUI noticeText;
    private StringTable stringTable;
    
    public int firstMessageId = 95202;  // 첫 번째 메시지의 ID
    public int secondMessageId = 95212; // 두 번째 메시지의 ID
    public float messageDisplayDuration = 2f; // 각 메시지가 화면에 표시되는 시간
    public float fadeDuration = 1f; // 페이드 인/아웃 애니메이션 시간

    private void Awake()
    {
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
    }

    public override void Enter()
    {
        // Enter 메서드에서 비동기 작업을 시작
        RunTutorialSequence().Forget();    
    }

    private async UniTaskVoid RunTutorialSequence()
    {
        // 텍스트를 순차적으로 표시
        await ShowMessages();

        // 모든 텍스트 애니메이션이 끝난 후 대상 오브젝트 비활성화 여부를 체크
        await CheckDisable();

        // 메시지가 끝나고 대상 오브젝트가 비활성화되었으면 다음 단계로 넘어감
        if (isDisable)
        {
            TutorialController controller = GetComponentInParent<TutorialController>();
            if (controller != null)
            {
                controller.SetNextTutorial();
            }
        }
    }

    private async UniTask ShowMessages()
    {
        // 첫 번째 메시지 표시
        noticeText.gameObject.SetActive(true);
        noticeText.text = stringTable.Get(firstMessageId.ToString());
        await ShowTextWithDoTween(noticeText);

        // 지정된 시간 동안 대기
        await UniTask.Delay((int)(messageDisplayDuration * 1000));

        // 두 번째 메시지 표시
        noticeText.text = stringTable.Get(secondMessageId.ToString());
        await ShowTextWithDoTween(noticeText);

        // 지정된 시간 동안 대기
        await UniTask.Delay((int)(messageDisplayDuration * 1000));

        // 텍스트를 페이드 아웃하여 숨김
        await noticeText.DOFade(0, fadeDuration).AsyncWaitForCompletion();
        noticeText.gameObject.SetActive(false);
    }

    private async UniTask CheckDisable()
    {
        // targetObject의 자식 중 하나라도 비활성화될 때까지 대기
        await UniTask.WaitUntil(() => IsAnyChildDisabled(targetObject));

        // 자식이 비활성화되었으면 isDisable을 true로 설정
        isDisable = true;
    }

    private async UniTask ShowTextWithDoTween(TextMeshProUGUI textComponent)
    {
        // 텍스트를 페이드 인 애니메이션으로 나타내기
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0);
        await textComponent.DOFade(1, fadeDuration).AsyncWaitForCompletion();
    }

    private bool IsAnyChildDisabled(GameObject parent)
    {
        // 자식 중 하나라도 비활성화되었는지 확인
        foreach (Transform child in parent.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public override void Execute(TutorialController controller)
    {
        // 필요 시 추가 로직 구현
    }

    public override void Exit()
    {
        // 필요 시 Exit 로직 추가
    }
}
