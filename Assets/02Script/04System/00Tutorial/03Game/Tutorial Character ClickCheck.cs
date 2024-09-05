using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class TutorialCharacterClickCheck : TutorialBase
{
    public TextMeshProUGUI noticeText;
    private StringTable stringTable;
    
    public GameObject slotClickCheck;
    public GameObject clickCheck;
    
    public UnmaskRaycastFilter slotClickCheckMask;
    public UnmaskRaycastFilter clickCheckMask;

    public bool isClick = false;
    private void Awake()
    {
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
    }

    public override void Enter()
    {
        if (slotClickCheck && slotClickCheck.transform.childCount > 0)
        {
            // 자식이 있으면 noticeText를 활성화하고 텍스트 설정, slotClickCheckMask 활성화
            noticeText.gameObject.SetActive(true);
            noticeText.text = stringTable.Get(95072.ToString());
            slotClickCheckMask.gameObject.SetActive(true);
            
            // slotClickCheckMask의 자식들 중 버튼을 찾아 클릭 이벤트를 추가
            foreach (Transform child in slotClickCheck.transform)
            {
                Button childButton = child.GetComponent<Button>();
                if (childButton != null)
                {
                    // 버튼이 클릭되었을 때 실행할 이벤트를 추가
                    childButton.onClick.AddListener(() =>
                    {
                        // clickCheckMask를 활성화하고 slotClickCheckMask를 비활성화
                        clickCheckMask.gameObject.SetActive(true);
                        slotClickCheckMask.gameObject.SetActive(false);
                        noticeText.text = stringTable.Get(95082.ToString());
                        
                        // clickCheck의 자식 중 하나라도 활성화될 때 다음 단계로 넘어가기
                        WaitForChildActivationAsync().Forget();
                    });
                }
            }
        }
    }

    private async UniTask WaitForChildActivationAsync()
    {
        // clickCheck의 자식 중 하나라도 활성화될 때까지 기다림
        await UniTask.WaitUntil(() => IsAnyChildActive(clickCheck));
        noticeText.gameObject.SetActive(false);
        clickCheckMask.gameObject.SetActive(false);
        isClick = true;
    }

    private bool IsAnyChildActive(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Logger.Log($"child.gameObject.activeSelf: {child.gameObject.activeSelf}");
            if (child.gameObject.activeSelf)
            {
                Logger.Log($"Child {child.name} is active.");
                return true;
            }
        }
        return false;
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
        // 필요 시 Exit 로직 추가
    }
}
