using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelManager : MonoBehaviour,IResourceObserver
{
    public MonsterDragPanel monsterUpgradePanelPrefab;
    public MonsterGimmickPanel monsterGimmickPanelPrefab;
    public CharacterUpgradePanel characterUpgradePanelPrefab;
    public CharacterFeaturePanel characterGimmickPanelPrefab;
    
    public Button[] upgradePanelButtons; // 버튼 배열 (0: 몬스터 업그레이드, 1: 몬스터 기믹, 2: 캐릭터 업그레이드, 3: 캐릭터 기믹)
    public TextMeshProUGUI[] costTexts; //재화 관련
    public TextMeshProUGUI[] shopTexts; //상점 관련
    public TextMeshProUGUI[] ChapterText;
    private StringTable stringTable;
    private GameManager gameManager;
    
    public TutorialController GimmickTutorialController;
    public TutorialController CharacterUpgradeTutorialController;
    public TutorialController PlayerUpgradeTutorialController;
    
    private void Awake()
    {
        stringTable ??= DataTableManager.Get<StringTable>(DataTableIds.String);
    }
    
    private void Start()
    {
        upgradePanelButtons[0].onClick.AddListener(() => ShowPanel(monsterUpgradePanelPrefab));
        upgradePanelButtons[1].onClick.AddListener(() => ShowPanel(monsterGimmickPanelPrefab));
        upgradePanelButtons[2].onClick.AddListener(() => ShowPanel(characterUpgradePanelPrefab));
        upgradePanelButtons[3].onClick.AddListener(() => ShowPanel(characterGimmickPanelPrefab));
        
        ShowPanel(monsterUpgradePanelPrefab);
        gameManager = GameManager.instance;
        gameManager.GameData.RegisterObserver(this);
        
        shopTexts[0].text = stringTable.Get("99802");
        shopTexts[1].text = stringTable.Get("99812");
        shopTexts[2].text = stringTable.Get("99822");
        shopTexts[3].text = stringTable.Get("99832");
        shopTexts[4].text = stringTable.Get("99842");
        
        ChapterText[0].text = stringTable.Get("99852");
        ChapterText[1].text = stringTable.Get("99862");
        ChapterText[2].text = stringTable.Get("99872");
        ChapterText[3].text = stringTable.Get("99882");
        
        costTexts[0].text = gameManager.GameData.Diamond.ToString();
        costTexts[1].text = gameManager.GameData.Gold.ToString();
        costTexts[2].text = gameManager.GameData.Ticket.ToString();
    }

    private void OnDestroy()
    {
        if (gameManager != null && gameManager.GameData != null)
        {
            gameManager.GameData.RemoveObserver(this);
        }
    }

    public void LateUpdate()
    {
        if(monsterGimmickPanelPrefab.gameObject.activeSelf && !GameManager.instance.GameData.ShopGimmickTutorialCheck)
        {
            GimmickTutorialController.gameObject.SetActive(true);   
        }
        
        if(characterUpgradePanelPrefab.gameObject.activeSelf && !GameManager.instance.GameData.ShopCharacterTutorialCheck)
        {
            CharacterUpgradeTutorialController.gameObject.SetActive(true);   
        }
        
        if(characterGimmickPanelPrefab.gameObject.activeSelf && !GameManager.instance.GameData.ShopFeatureTutorialCheck)
        {
            PlayerUpgradeTutorialController.gameObject.SetActive(true);   
        }
    }

    private void ShowPanel(MonoBehaviour selectedPanel)
    {
        monsterUpgradePanelPrefab.gameObject.SetActive(false);
        monsterGimmickPanelPrefab.gameObject.SetActive(false);
        characterUpgradePanelPrefab.gameObject.SetActive(false);
        characterGimmickPanelPrefab.gameObject.SetActive(false);
        selectedPanel.gameObject.SetActive(true);
    }

    public void OnResourceUpdate(ResourceType resourceType, int newValue)
    {
        switch (resourceType)
        {
            case ResourceType.Gold:
                costTexts[1].text = gameManager.GameData.Gold.ToString();
                break;
            case ResourceType.Diamond:
                costTexts[0].text = gameManager.GameData.Diamond.ToString();
                break;
            case ResourceType.Ticket:
                costTexts[2].text = gameManager.GameData.Ticket.ToString();
                break;
        }
    }
}