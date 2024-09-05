using TMPro;
using UnityEngine;

public class TutorialMonsterDisable : TutorialBase
{
    public GameObject monsterSpawnParent;
    public bool isDisable = false;
    public TextMeshProUGUI noticeText;
    private StringTable stringTable;

    private void Awake()
    {
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
    }

    public override void Enter()
    {
        isDisable = false;    
    }

    public override void Execute(TutorialController controller)
    {
        if (monsterSpawnParent == null) return;

        bool allDisabled = true;

        foreach (Transform child in monsterSpawnParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                allDisabled = false; // 활성화된 자식이 하나라도 있으면 false로 설정
                break;
            }
        }

        if (allDisabled && !isDisable)
        {
            isDisable = true;
            controller.SetNextTutorial(); // 다음 튜토리얼 단계로 이동
        }    
    }

    public override void Exit()
    {
        
    }
}
