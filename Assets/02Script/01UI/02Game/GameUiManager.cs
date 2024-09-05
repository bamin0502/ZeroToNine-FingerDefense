using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUiManager : MonoBehaviour
{
    private StageManager stageManager;

    private Dictionary<StageState, GameObject> stageStatesUi = new Dictionary<StageState, GameObject>();
    public GameObject gameUi;
    public GameObject gameOverUi;
    public GameObject gameClearUi;
    public GameObject pauseUi; // To-Do: Pause UI 추가(일단은 종료UI창이랑 같이 쓸거임)
    public GameObject monsterInfoUi;
    public GameObject optionUi;

    public TextMeshProUGUI eranedGoldText;
    public TextMeshProUGUI monsterCountText;
    public TextMeshProUGUI monsterDragCountText;

    public Slider castleHpBar;
    public Image castleShieldBar;

    private void Start()
    {
        stageManager = GameObject.FindWithTag("StageManager").GetComponent<StageManager>();

        stageStatesUi.Add(StageState.PLAYING, gameUi);
        stageStatesUi.Add(StageState.OPTION, optionUi);
        stageStatesUi.Add(StageState.MONSTER_INFO, monsterInfoUi);
        stageStatesUi.Add(StageState.GAME_OVER, gameOverUi);
        stageStatesUi.Add(StageState.GAME_CLEAR, gameClearUi);
    }

    public void SetStageStateUi(StageState state)
    {
        foreach (var stageStateUi in stageStatesUi)
        {
            stageStateUi.Value.SetActive(state == stageStateUi.Key);
        }
    }

    public void UpdateHpBar(float currentHp, float maxHp)
    {
        if (maxHp <= 0f)
            return;
        currentHp = Mathf.Clamp(currentHp, 0f, maxHp);
        castleHpBar.value = currentHp / maxHp;
    }

    public void UpdateShieldBar(float currentShield, float maxHp)
    {
        if (maxHp <= 0f)
            return;
        currentShield = Mathf.Clamp(currentShield, 0f, maxHp);
        castleShieldBar.fillAmount = currentShield / maxHp;
    }

    public void UpdateMonsterCount(int monsterCount)
    {
        monsterCountText.text = $"{monsterCount}";
    }

    public void UpdateEarnedGold(int gold)
    {
        eranedGoldText.text = $"{gold}";
    }

    public void UpdateMonsterDragCount(int dragCount)
    {
        monsterDragCountText.text = $"{dragCount}";
    }
}
