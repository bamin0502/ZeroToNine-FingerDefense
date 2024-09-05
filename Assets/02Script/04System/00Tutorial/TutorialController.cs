using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    public List<TutorialBase> tutorials;
    
    private TutorialBase currentTutorial=null;
    private int currentTutorialIndex = -1;
    private GameManager gameManager;
    
    public Button skipButton;
    public int skipIndex;
    
    private void Start()
    {     
        foreach (var tutorial in tutorials)
        {
            tutorial.gameObject.SetActive(false);
        }
        SetNextTutorial();
        
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);    
        }
        
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (currentTutorial)
        {
            currentTutorial.Execute(this);
        }
    }
    
    public void SetNextTutorial()
    {
        if (currentTutorial)
        {
            currentTutorial.Exit();
            currentTutorial.gameObject.SetActive(false);
        }    
        
        if(currentTutorialIndex >= tutorials.Count - 1)
        {
            CompletedAllTutorials();
            return;
        }
        
        currentTutorialIndex++;
        currentTutorial = tutorials[currentTutorialIndex];
        
        var dialogSystem = currentTutorial.GetComponent<DialogSystem>();
        if (!ReferenceEquals(dialogSystem, null))
        {
            dialogSystem.isFirstDialog = true;
            dialogSystem.DialogSetting();  // 대화 초기화
        }
        
        currentTutorial.gameObject.SetActive(true);
        currentTutorial.Enter();
    }

    private void CompletedAllTutorials()
    {
        currentTutorial = null;
        
        ResetAllTutorialData();
        
        DataManager.SaveFile(gameManager.GameData);
        //해당 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
        
    }

    private void ResetAllTutorialData()
    {
        var allMonsters = FindObjectsOfType<MonsterController>();
        foreach (var monster in allMonsters)
        {
            monster.ResetMonsterData();  // 몬스터 상태 초기화
        }    
    }

    private void SkipTutorial()
    {
        ModalWindow.Create(window =>
        {
            window.SetHeader("스킵 확인")
                .SetBody("튜토리얼을 스킵하시겠습니까?")
                .AddButton("확인", () =>
                {
                    currentTutorialIndex = skipIndex - 1;
                    SetNextTutorial();
                })
                .AddButton("취소", () => { })
                .Show();
        });
    }
}
