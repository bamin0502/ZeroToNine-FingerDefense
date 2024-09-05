using UnityEngine;

public class MainUiManager : MonoBehaviour
{
    public MainUI MainUI;
    public GameObject StageUI;
    public GameObject DeckUI;
    public GameObject NicknameUI;
    
    public QuitUI QuitUI;
    public ShopSettingUI ShopUI;
    public GachaSystem GachaSystem;
    private GameManager gameManager;
    public UpgradePanelManager upgradePanelManager;
    
    public TutorialController NicknameTutorialController;//닉네임 튜토리얼 
    public TutorialController stageTutorialController;//게임1 튜토리얼
    public TutorialController deckTutorialController;//덱UI 튜토리얼
    public TutorialController shopTutorialController;//상점 드래그 튜토리얼
    public TutorialController dragTutorialController;//드래그 튜토리얼
    
    private UpgradeTable upgradeTable;
    
    private void Awake()
    {
        upgradeTable ??= DataTableManager.Get<UpgradeTable>(DataTableIds.Upgrade);
        gameManager = GameManager.instance;
    }

    public void Start()
    {
        if (!gameManager.GameData.NicknameCheck)
        {
            NicknameTutorialController.gameObject.SetActive(true);
            MainUI.gameObject.SetActive(false);
            DeckUI.SetActive(false);
            StageUI.SetActive(false);
            NicknameUI.SetActive(false);
            ShopUI.gameObject.SetActive(false);
            GachaSystem.gameObject.SetActive(false);
            upgradePanelManager.gameObject.SetActive(false);
        }
        else
        {
            NicknameUI.SetActive(false);
            NicknameTutorialController.gameObject.SetActive(false);
            MainUI.gameObject.SetActive(true);
            DeckUI.SetActive(false);
            StageUI.SetActive(false);
            ShopUI.gameObject.SetActive(false);
            GachaSystem.gameObject.SetActive(false);
            upgradePanelManager.gameObject.SetActive(false);
        }
        
        if(Variables.LoadTable.isNextStage)
        {
            StageUI.SetActive(true);
            StageUI.transform.SetAsLastSibling();
        }
        
        
        foreach (var upgradeData in upgradeTable.upgradeTable.Values)
        {
            if (upgradeData.Type == 0)
            {
                if (GameManager.instance.GameData.MonsterDragLevel.TryAdd(upgradeData.UpgradeResultId, (int)GameData.MonsterDrag.LOCK))
                {
                    DataManager.SaveFile(GameManager.instance.GameData);
                }
            }
            else if (upgradeData.Type == 1)
            {
                // UpStatType에 따른 초기화
                switch (upgradeData.UpStatType)
                {
                    case 0: // MonsterGimmick ATTACKRANGE
                        if (!gameManager.GameData.MonsterGimmickLevel.Exists(x =>
                                x.monsterGimmick == (int)GameData.MonsterGimmick.ATTACKRANGE))
                        {
                            gameManager.GameData.MonsterGimmickLevel.Add(((int)GameData.MonsterGimmick.ATTACKRANGE, 0));
                        }

                        break;

                    case 1: // MonsterGimmick ATTACKDAMAG
                        if (!gameManager.GameData.MonsterGimmickLevel.Exists(x =>
                                x.monsterGimmick == (int)GameData.MonsterGimmick.ATTACKDAMAGE))
                        {
                            gameManager.GameData.MonsterGimmickLevel.Add(((int)GameData.MonsterGimmick.ATTACKDAMAGE, 0));
                        }

                        break;

                    case 2: // MonsterGimmick ATTACKDURATION
                        if (!gameManager.GameData.MonsterGimmickLevel.Exists(x =>
                                x.monsterGimmick == (int)GameData.MonsterGimmick.ATTACKDURATION))
                        {
                            gameManager.GameData.MonsterGimmickLevel.Add(((int)GameData.MonsterGimmick.ATTACKDURATION,
                                0));
                        }

                        break;
                }
            }
            else if (upgradeData.Type == 3)
            {
                // UpStatType에 따른 PlayerUpgrade 초기화
                switch (upgradeData.UpStatType)
                {
                    case 3: // PlayerUpgrade CHARACTER_ARRANGEMENT
                        if (!gameManager.GameData.PlayerUpgradeLevel.Exists(x =>
                                x.playerUpgrade == (int)GameData.PlayerUpgrade.CHARACTER_ARRANGEMENT))
                        {
                            gameManager.GameData.PlayerUpgradeLevel.Add((
                                (int)GameData.PlayerUpgrade.CHARACTER_ARRANGEMENT, 0));
                        }

                        break;

                    case 4: // PlayerUpgrade PLAYER_HEALTH
                        if (!gameManager.GameData.PlayerUpgradeLevel.Exists(x =>
                                x.playerUpgrade == (int)GameData.PlayerUpgrade.PLAYER_HEALTH))
                        {
                            gameManager.GameData.PlayerUpgradeLevel.Add(((int)GameData.PlayerUpgrade.PLAYER_HEALTH, 0));
                        }

                        break;

                    case 5: // PlayerUpgrade INCREASE_DRAG
                        if (!gameManager.GameData.PlayerUpgradeLevel.Exists(x =>
                                x.playerUpgrade == (int)GameData.PlayerUpgrade.INCREASE_DRAG))
                        {
                            gameManager.GameData.PlayerUpgradeLevel.Add(((int)GameData.PlayerUpgrade.INCREASE_DRAG, 0));
                        }

                        break;
                }
            }
        }
    }
    
    
    public void OnClickStartButton()
    {
        if (!StageUI.activeSelf)
        {
            StageUI.SetActive(true);
            
            if(!gameManager.GameData.StageChoiceTutorialCheck)
            {
                stageTutorialController.gameObject.SetActive(true);
            }
        }
    }

    private void LateUpdate()
    {
        //닉네임 튜토리얼은 했지만 게임1 튜토리얼은 안했을 때
        if(gameManager.GameData.NicknameCheck && !gameManager.GameData.Game1TutorialCheck)
        {
            stageTutorialController.gameObject.SetActive(true);
        }
        //게임1 튜토리얼은 했지만 덱UI 튜토리얼은 안했을 때
        if (gameManager.GameData.Game1TutorialCheck && !gameManager.GameData.DeckUITutorialCheck)
        {
            deckTutorialController.gameObject.SetActive(true);
        }
        //덱UI 튜토리얼은 했지만 상점 드래그 튜토리얼은 안했을 때
        if(gameManager.GameData.Game2TutorialCheck && !gameManager.GameData.ShopDragTutorialCheck)
        {
            shopTutorialController.gameObject.SetActive(true);
        }
        //상점 드래그 튜토리얼은 했지만 게임3 튜토리얼은 안했을 때
        if(gameManager.GameData.ShopDragTutorialCheck && !gameManager.GameData.Game3TutorialCheck)
        {
            dragTutorialController.gameObject.SetActive(true);
        }
    }
}
