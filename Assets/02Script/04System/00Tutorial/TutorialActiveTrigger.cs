using UnityEngine;
using UnityEngine.UI;

public class TutorialActiveTrigger : TutorialBase
{
    public GameObject targetObject;
    public GameObject panelToShow;
    public Button targetButton;
    public bool includeChildren = false; // 자식 오브젝트만 검사할지 여부
    
    private Button[] childButtons;

    public override void Enter()
    {
        if (includeChildren)
        {
            // 자식 오브젝트의 버튼만 클릭 이벤트 등록
            childButtons = targetObject.GetComponentsInChildren<Button>();
            foreach (Button button in childButtons)
            {
                // 각 버튼에 대해 클릭 이벤트 개별 등록
                button.onClick.AddListener(() => OnChildButtonClicked(button));
            }
        }
        else if (targetButton != null)
        {
            // includeChildren이 꺼져있으면 부모 오브젝트의 버튼 클릭 이벤트 등록
            targetButton.onClick.AddListener(OnTargetObjectClicked);
        }
    }

    public override void Execute(TutorialController controller)
    {
        // Execute는 필요하지 않음, 모든 논리는 Enter와 Exit에서 처리됨
    }

    public override void Exit()
    {
        if (includeChildren && childButtons != null)
        {
            // 자식 오브젝트의 버튼 클릭 이벤트 제거
            foreach (Button button in childButtons)
            {
                button.onClick.RemoveListener(() => OnChildButtonClicked(button));
            }
        }
        else if (targetButton != null)
        {
            // 부모 오브젝트의 버튼 클릭 이벤트 제거
            targetButton.onClick.RemoveListener(OnTargetObjectClicked);
        }
    }

    // 자식 버튼이 클릭되었을 때 호출되는 메서드
    private void OnChildButtonClicked(Button clickedButton)
    {
        Debug.Log($"{clickedButton.name} 버튼이 클릭되었습니다.");
        TogglePanelAndProceed();
    }

    // 부모 오브젝트의 버튼이 클릭되었을 때 호출되는 메서드
    private void OnTargetObjectClicked()
    {
        TogglePanelAndProceed();
    }

    // 패널의 활성화 상태를 토글하고 튜토리얼을 진행하는 메서드
    private void TogglePanelAndProceed()
    {
        if (panelToShow != null)
        {
            // 패널이 활성화되어 있으면 비활성화, 비활성화되어 있으면 활성화
            panelToShow.SetActive(!panelToShow.activeSelf);

            // 패널 상태에 관계없이 다음 튜토리얼로 진행
            var controller = FindObjectOfType<TutorialController>();
            if (controller != null)
            {
                controller.SetNextTutorial();
            }
        }
    }
}
