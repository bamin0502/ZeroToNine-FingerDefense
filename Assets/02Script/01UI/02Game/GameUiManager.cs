using System.Collections;
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

    public TextMeshProUGUI eranedGoldText;
    public TextMeshProUGUI monsterCountText;

    public Image castleHpBar;

    private void Start()
    {
        stageManager = GameObject.FindWithTag("StageManager").GetComponent<StageManager>();

        stageStatesUi.Add(StageState.Playing, gameUi);
        stageStatesUi.Add(StageState.GameOver, gameOverUi);
        stageStatesUi.Add(StageState.GameClear, gameClearUi);
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
        castleHpBar.fillAmount = currentHp / maxHp;
    }

    public void UpdateMonsterCount(int monsterCount)
    {
        monsterCountText.text = $"Monster: {monsterCount}";
    }

    public void UpdateEarnedGold(int gold)
    {
        eranedGoldText.text = $"Gold: {gold}";
    }
}
