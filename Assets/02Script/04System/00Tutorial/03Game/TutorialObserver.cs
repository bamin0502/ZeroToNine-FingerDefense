using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class TutorialObserver : TutorialBase
{
    public List<TutorialGameTrigger> observedMonsters = new List<TutorialGameTrigger>();

    public TextMeshProUGUI tutorialText;
    public Image tutorialImage;
    private StringTable stringTable;

    public GameObject monsterSpawnParent;

    public bool isRemove = false;

    public bool isDragLock = false;
    public bool isState = false;
    public bool isDebuff = false;
    public bool isDrag = false;
    
    [SerializeField] private ItemDebuffMonster stop;

    public void Awake()
    {
        stringTable = DataTableManager.Get<StringTable>(DataTableIds.String);
    }

    private void Start()
    {
        CheckAndAddMonsters();
        // 주기적으로 자식 객체들을 확인하는 작업을 시작
        MonitorMonstersAsync().Forget();
    }

    private async UniTaskVoid MonitorMonstersAsync()
    {
        while (true) // 지속적으로 실행
        {
            if (monsterSpawnParent != null)
            {
                CheckAndAddMonsters();
                ControlMonsterStates(); // 항상 상태를 제어
            }
            else
            {
                Logger.LogWarning("monsterSpawnParent가 null입니다. MonitorMonstersAsync가 중지됩니다.");
                break; // monsterSpawnParent가 파괴되었으면 반복 중지
            }
            await UniTask.Delay(100); // 100ms마다 확인 (조정 가능)
        }
    }

    private void CheckAndAddMonsters()
    {
        if (monsterSpawnParent == null) return;

        var monsterTriggers = monsterSpawnParent.GetComponentsInChildren<TutorialGameTrigger>();
        foreach (var monsterTrigger in monsterTriggers)
        {
            if (!observedMonsters.Contains(monsterTrigger))
            {
                AddMonster(monsterTrigger);
            }
        }

        // 제거된 자식을 확인하여 리스트에서 제거
        RemoveDestroyedMonsters();
    }

    private void RemoveDestroyedMonsters()
    {
        observedMonsters.RemoveAll(monster => monster == null || !monster.gameObject.activeInHierarchy);
    }

    public void AddMonster(TutorialGameTrigger monsterTrigger)
    {
        if (monsterTrigger != null && !observedMonsters.Contains(monsterTrigger))
        {
            observedMonsters.Add(monsterTrigger);
            monsterTrigger.SetObserver(this);
            Debug.Log($"Monster added to observedMonsters: {monsterTrigger.name}");
        }
    }

    public override void Enter()
    {
        //시작할때 마찬가지로 찾는다 
        CheckAndAddMonsters();
    }

    public override void Execute(TutorialController controller)
    {
        if (isDragLock)
        {
            controller.SetNextTutorial();
        }

        if (isState)
        {
            controller.SetNextTutorial();
        }

        if (isDebuff)
        {
            bool hasBuffed = false; // 버프가 적용되었는지 확인하기 위한 플래그

            foreach (var monsterTrigger in observedMonsters)
            {
                var monsterController = monsterTrigger.GetComponent<MonsterController>();

                if (monsterController != null)
                {
                    Debug.Log($"Applying buff to Monster ID: {monsterController.Status.Data.Id}");
                    stop.GiveBuff(monsterController);
                    hasBuffed = true;
                }
                else
                {
                    Debug.LogWarning("MonsterController가 null이거나 유효하지 않습니다.");
                }
            }

            if (hasBuffed)
            {
                controller.SetNextTutorial();
            }
        }
        
        if (isDrag)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        // 종료 작업
    }

    public void OnTargetDisabled(TutorialGameTrigger monsterTrigger)
    {
        if (observedMonsters.Contains(monsterTrigger))
        {
            observedMonsters.Remove(monsterTrigger);
        }
    }

    public void NextTutorial()
    {
        var controller = GetComponentInParent<TutorialController>();
        controller.SetNextTutorial();
    }

    public void OnMonsterDragStarted(TutorialGameTrigger tutorialGameTrigger)
    {
        // 필요 시 구현
    }

    public void OnMonsterDropped(TutorialGameTrigger tutorialGameTrigger)
    {
        // 필요 시 구현
    }

    public void OnMonsterSurvived(TutorialGameTrigger tutorialGameTrigger)
    {
        // 필요 시 구현
    }

    private void ControlMonsterStates()
    {
        foreach (var monsterTrigger in observedMonsters)
        {
            var monsterController = monsterTrigger.gameObject.GetComponent<MonsterController>();
            if (monsterController != null)
            {
                var monsterID = monsterController.Status.Data.Id;

                // isDragLock가 true이면 드래그 유형을 보스로 설정
                if (isDragLock)
                {
                    monsterController.Status.Data.DragType = (int)MonsterData.DragTypes.BOSS;
                    if (monsterID == 12031)
                    {
                        monsterController.Status.Data.DragType = (int)MonsterData.DragTypes.SPECIAL;
                    }
                }

                // isState가 true이면 상태를 특정 상태로 전환 (예: PatrolState)
                if (isState)
                {
                    monsterController.IsTutorialMonster = false;
                }

                if (isDebuff)
                {
                    stop.GiveBuff(monsterController);
                }

                if (isDrag)
                {
                    monsterController.Status.Data.DragType = (int)MonsterData.DragTypes.NORMAL;
                }
            }    
        }
    }
}
