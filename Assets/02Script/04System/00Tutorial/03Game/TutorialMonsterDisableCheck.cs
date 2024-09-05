using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TutorialMonsterDisableCheck : TutorialBase
{
    public GameObject monsterSpawnParent;
    public bool isDisable = false;
    public TextMeshProUGUI noticeText;
    private StringTable stringTable;
    private MonsterSpawner monsterSpawner;

    public float checkInterval = 1f; // 체크 주기
    private const int targetMonsterId = 12031; // 추적할 몬스터의 ID

    private void Awake()
    {
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
    }

    private void Start()
    {
        monsterSpawner = FindObjectOfType<MonsterSpawner>();
        MonitorMonstersAsync().Forget();
    }

    private async UniTaskVoid MonitorMonstersAsync()
    {
        while (!isDisable)
        {
            CheckMonsters();
            await UniTask.Delay((int)(checkInterval * 1000)); // 주기적으로 체크
        }
    }

    private void CheckMonsters()
    {
        if (!monsterSpawnParent) return;

        bool isTargetMonsterDisabled = true; // 타겟 몬스터(12031)가 비활성화되었는지 확인
        bool anyOtherMonsterActive = false;  // 타겟 몬스터 외 다른 몬스터가 활성화 상태인지 확인

        foreach (Transform child in monsterSpawnParent.transform)
        {
            var monsterController = child.GetComponent<MonsterController>();
            if (monsterController != null)
            {
                if (monsterController.Status.Data.Id == targetMonsterId)
                {
                    if (child.gameObject.activeSelf)
                    {
                        isTargetMonsterDisabled = false; // 타겟 몬스터가 활성화되어 있으면 false로 설정
                        
                        // 떨어졌을때 몬스터가 활성화되어 있는 경우, 그 몬스터를 움직이지 못하게 설정
                        
                    }
                }
                else if (child.gameObject.activeSelf)
                {
                    anyOtherMonsterActive = true; // 다른 몬스터가 활성화된 상태
                }
            }
            
            
        }

        if (isTargetMonsterDisabled && anyOtherMonsterActive)
        {
            // 타겟 몬스터가 비활성화되어 있고, 다른 몬스터가 활성화 상태일 때만 타겟 몬스터를 재생성
            monsterSpawner.RespawnSpecificMonster(targetMonsterId);
        }
        else if (!anyOtherMonsterActive)
        {
            // 모든 몬스터가 비활성화된 경우, 다음 튜토리얼 단계로 이동
            isDisable = true;
            var controller = GetComponentInParent<TutorialController>();
            if (controller != null)
            {
                controller.SetNextTutorial(); // 다음 튜토리얼 단계로 넘어감
            }
        }
        else
        {
            // 타겟 몬스터가 활성화되어 있는 상태에서 "드래그를 통해 모든 몬스터를 처치하세요" 메시지를 표시
            noticeText.text = "드래그를 통해 모든 몬스터를 처치하세요!";
        }
    }
    
    public override void Enter()
    {
        isDisable = false; // Enter에서 초기화
        noticeText.gameObject.SetActive(true); // noticeText를 활성화
        MonitorMonstersAsync().Forget(); // Enter 시 모니터링 시작
    }

    public override void Execute(TutorialController controller)
    {
        // 필요한 경우 구현
    }

    public override void Exit()
    {
        noticeText.gameObject.SetActive(false); // Exit 시 메시지 숨김
    }
}
