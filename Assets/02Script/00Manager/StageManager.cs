using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum StageState
{
    NONE,
    TUTORIAL,
    PLAYING,
    PAUSE,
    OPTION,
    MONSTER_INFO,
    GAME_OVER,
    GAME_CLEAR
}

[DefaultExecutionOrder(-1)]
public class StageManager : MonoBehaviour
{
    [Header("Castle")]
    public List<GameObject> castleImages;
    public Transform castleRightTopPos;
    public Transform castleLeftBottomPos;

    public SoundManager soundManager;
    public AudioClip castleDamageAudioClip;

    private float castleMaxHp;
    private float castleHp;
    private float CastleHp
    {
        get => castleHp;
        set
        {
            castleHp = value;
            gameUiManager.UpdateHpBar(castleHp, castleMaxHp);

            if (castleImages.Count != 0)
            {
                int castleIndex = Mathf.FloorToInt(castleHp / (castleMaxHp / castleImages.Count));
                castleIndex = Mathf.Clamp(castleIndex, 0, castleImages.Count - 1);
                for(int i = 0; i < castleImages.Count; i++)
                {
                    castleImages[i].SetActive(i == castleIndex);
                }
            }
        }
    }
    private float castleShield;
    private float CastleShield
    {
        get => castleShield;
        set
        {
            castleShield = value;
            gameUiManager.UpdateShieldBar(castleShield, castleMaxHp);
            if (castleShield > 0f)
                shieldEffect?.gameObject.SetActive(true);
            else
                shieldEffect?.gameObject.SetActive(false);
        }
    }
    [SerializeField] private EffectController shieldEffect;

    private int monsterCount;
    public int MonsterCount
    {
        get => monsterCount;
        set
        {
            monsterCount = value;
            gameUiManager.UpdateMonsterCount(monsterCount);
            if (monsterCount <= 0 && CastleHp > 0f)
                CurrentState = StageState.GAME_CLEAR;
        }
    }

    private int maxDragCount;
    private int dragCount;
    public int DragCount
    {
        get => dragCount;
        set
        {
            if (value < 0)
                return;

            dragCount = value;
            gameUiManager.UpdateMonsterDragCount(dragCount);
        }
    }

    [Header("골드")]
    [SerializeField] private EffectController goldEffect;
    private int earnedGold;
    public int EarnedGold
    {
        get => earnedGold;
        set
        {
            earnedGold = value;
            gameUiManager.UpdateEarnedGold(earnedGold);
        }
    }
    private float goldMultiplier = 1f;
    [HideInInspector] public float GoldMultiplier
    {
        get => goldMultiplier;
        set
        {
            goldMultiplier = value;
            if (goldMultiplier > 1f)
            {
                goldEffect?.gameObject?.SetActive(true);
            }
            else
            {
                goldEffect?.gameObject?.SetActive(false);
            }
        }
    }
    
    private StageState currentState;
    public StageState CurrentState
    {
        get => currentState;
        set => SetStageState((int)value);
    }

    public MonsterSpawner monsterSpawner;
    public PlayerCharacterSpawner playerCharacterSpawner;
    public GameUiManager gameUiManager;

    [HideInInspector] public bool isPlayerElementAdvantage = false;

    private void Awake()
    {
        var upgradesData = GameManager.instance.GameData.PlayerUpgradeLevel;
        var castleMaxHpLevel = 0;
        var monsterMaxDragCountLevel = 0;
        foreach (var upgradeData in upgradesData)
        {
            switch ((GameData.PlayerUpgrade)upgradeData.playerUpgrade)
            {
                case GameData.PlayerUpgrade.PLAYER_HEALTH:
                    castleMaxHpLevel = upgradeData.level;
                    break;
                case GameData.PlayerUpgrade.INCREASE_DRAG:
                    monsterMaxDragCountLevel = upgradeData.level;
                    break;
            }
        }

        var upgradeTable = DataTableManager.Get<UpgradeTable>(DataTableIds.Upgrade);

        castleMaxHp = upgradeTable.GetPlayerUpgrade((int)GameData.PlayerUpgrade.PLAYER_HEALTH, castleMaxHpLevel).UpStatValue;
        maxDragCount = (int)upgradeTable.GetPlayerUpgrade((int)GameData.PlayerUpgrade.INCREASE_DRAG, monsterMaxDragCountLevel).UpStatValue;
    }

    private void Start()
    {
        CastleHp = castleMaxHp;
        DragCount = maxDragCount;
        CurrentState = StageState.PLAYING;
        MonsterCount = monsterSpawner.MonsterCount;
        EarnedGold = 0;
        GoldMultiplier = 1f;
    }
    
    public void DamageCastle(float damage)
    {
        if (damage <= 0f)
            return;

        if (soundManager?.sfxAudioSource && castleDamageAudioClip)
            soundManager.sfxAudioSource.PlayOneShot(castleDamageAudioClip);

        if (CastleShield > 0f)
        {
            if (CastleShield > damage)
            {
                CastleShield -= damage;
                return;
            }
            else
            {
                damage = CastleShield - damage;
                CastleShield = 0f;
            }
        }

        CastleHp -= damage;

        if (CastleHp <= 0f)
            CurrentState = StageState.GAME_OVER;
    }

    public void RestoreCastle(float heal, bool isPercentage = false)
    {
        if (heal <= 0f)
            return;

        if (isPercentage)
            heal *= castleMaxHp;
        CastleHp += heal;

        if (CastleHp >= castleMaxHp)
            CastleHp = castleMaxHp;
    }

    public void GetShield(float shield, bool isPercentage = false)
    {
        if (shield <= 0f)
            return;

        if (isPercentage)
            shield *= castleMaxHp;
        CastleShield += shield;
    }

    public void GetGold(int gold)
    {
        EarnedGold += Mathf.CeilToInt(gold * GoldMultiplier);
        gameUiManager.UpdateEarnedGold(earnedGold);
    }

    [VisibleEnum(typeof(StageState))]
    public void SetStageState(int state)
    {
        if (currentState == (StageState)state || (StageState)state == StageState.NONE)
            return;

        currentState = (StageState)state;
        gameUiManager.SetStageStateUi(currentState);

        if (currentState == StageState.PLAYING)
        {
            TimeScaleController.SetTimeScale(1f);
        }
        else
        {
            TimeScaleController.SetTimeScale(0f);
            soundManager.sfxAudioSource.Stop();
        }

        switch (currentState)
        {
            case StageState.GAME_CLEAR:
                StageClear();
                break;
            case StageState.GAME_OVER:
                GetClearRewards();
                break;
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        TimeScaleController.SetTimeScale(1f);
    }
    
    public void LobbyScene(bool isNextStage)
    {
        Variables.LoadTable.isNextStage = isNextStage;
        SceneManager.LoadScene(1);
        TimeScaleController.SetTimeScale(1f);
        Variables.LoadTable.ItemId.Clear();
    }

    private void StageClear()
    {
        //클리어했을때 그 클리어 스테이지의 ID를 저장
        GameManager.instance.GameData.StageClearNum = Variables.LoadTable.StageId;

        var stageClear = GameManager.instance.GameData.StageClear;
        if (!stageClear[Variables.LoadTable.StageId]) // 최초 클리어
        {
            stageClear[Variables.LoadTable.StageId] = true;
            GetClearRewards(true);
        }
        else
            GetClearRewards();
    }

    private void GetClearRewards(bool isFirstClear = false)
    {
        // 게임 클리어 및 게임 오버 보상
        GameManager.instance.GameData.Gold += EarnedGold;

        // 최초 클리어 보상
        if (isFirstClear)
        {
            var stageTable = DataTableManager.Get<StageTable>(DataTableIds.Stage);
            var stageData = stageTable.Get(Variables.LoadTable.StageId);

            GameManager.instance.GameData.Ticket += stageData.Reward1Value;
            GameManager.instance.GameData.Diamond += stageData.Reward2Value;
        }

        if (gameUiManager.gameClearUi.TryGetComponent<UiGameClearPanel>(out var clearPanel))
        {
            clearPanel.ActiveRewardGetText(!isFirstClear);
        }
        DataManager.SaveFile(GameManager.instance.GameData);
    }
}
